using Carter;
using Marten;
using medical_profile_service.Entities;
using medical_profile_service.Services;
using Microsoft.AspNetCore.Mvc;

namespace medical_profile_service.Modules;

public class PrescriptionsModule : ICarterModule
{
	private readonly IStorageService _storageService;
	private readonly IDatabaseService _databaseService;
	
	public PrescriptionsModule(IStorageService storageService, IDatabaseService databaseService)
	{
		_storageService = storageService;
		_databaseService = databaseService;
	}
	public void AddRoutes(IEndpointRouteBuilder app)
	{
		app.MapGet("/records/{patientId}/prescriptions",
			async (IDocumentSession session, string patientId, CancellationToken cancellationToken) =>
			{
				var record = await _storageService
					.GetLatestMedicalRecord(session, patientId, cancellationToken);
				return record is null
					? Results.NotFound()
					: Results.Ok(record.Prescriptions.IsEmpty() ? Array.Empty<MedicalHistory>() : record.Prescriptions);
			});
		app.MapPost("/records/{patientId}/prescriptions", async (IDocumentSession session, PrescriptionDto request, string patientId, CancellationToken cancellationToken) =>
		{
			var record = await _storageService
				.GetLatestMedicalRecord(session, patientId, cancellationToken);
			if (record is null)
			{
				var newRecord = new MedicalRecord();
				Prescription[] prescriptions = {request.ToPrescription()};
				newRecord.Prescriptions = prescriptions;
				var hash = await _storageService.SaveMedicalRecord(newRecord, cancellationToken);
				if (hash is null)
				{
					return Results.BadRequest();
				}
				await _databaseService
					.SaveMedicalRecordHashAsync(session, patientId, hash, cancellationToken);
				return Results.Ok();
			}
			var prev = record.Prescriptions ?? Array.Empty<Prescription>();
			var toAdd = new[] {request.ToPrescription()};
			record.Prescriptions = prev
				.Concat(toAdd)
				.Distinct()
				.ToArray();
			var newHash = await _storageService.SaveMedicalRecord(record, cancellationToken);
			if (newHash is null)
			{
				return Results.BadRequest();
			}
			await _databaseService
				.SaveMedicalRecordHashAsync(session, patientId, newHash, cancellationToken);
			await _storageService.SaveMedicalRecord(record, cancellationToken);
			return Results.Ok();
		});
		app.MapDelete("records/{patientId}/prescriptions",
		async (IDocumentSession session, string patientId, [FromQuery] string prescriptionId, CancellationToken cancellationToken) =>
		{
			var record = await _storageService
				.GetLatestMedicalRecord(session, patientId, cancellationToken);
			if (record is null)
			{
				return Results.NotFound();
			}
			record.Prescriptions = record.Prescriptions
				.Where(p => p.Id != prescriptionId)
				.ToArray();
			var newHash = await _storageService.SaveMedicalRecord(record, cancellationToken);
			if (newHash is null)
			{
				return Results.BadRequest();
			}
			await _databaseService
				.SaveMedicalRecordHashAsync(session, patientId, newHash, cancellationToken);
			return Results.Ok();
		});
	}
}
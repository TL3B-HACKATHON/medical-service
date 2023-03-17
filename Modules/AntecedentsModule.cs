using Carter;
using Marten;
using medical_profile_service.Entities;
using medical_profile_service.Services;
using Microsoft.AspNetCore.Mvc;

namespace medical_profile_service.Modules;

public class AntecedentsModule : ICarterModule
{
	private readonly IStorageService _storageService;
	private readonly IDatabaseService _databaseService;
	
	public AntecedentsModule(IStorageService storageService, IDatabaseService databaseService)
	{
		_storageService = storageService;
		_databaseService = databaseService;
	}
	public void AddRoutes(IEndpointRouteBuilder app)
	{
		app.MapGet("/records/{patientId}/antecedents",
			async (IDocumentSession session, string patientId, CancellationToken cancellationToken) =>
			{
				var record = await _storageService
					.GetLatestMedicalRecord(session, patientId, cancellationToken);
				return record is null
					? Results.NotFound()
					: Results.Ok(record.Antecedents.IsEmpty() ? Array.Empty<MedicalHistory>() : record.Antecedents);
			});
		app.MapPost("/records/{patientId}/antecedents", async (IDocumentSession session, MedicalHistoryDto request, string patientId, CancellationToken cancellationToken) =>
		{
			var record = await _storageService
				.GetLatestMedicalRecord(session, patientId, cancellationToken);
			if (record is null)
			{
				var newRecord = new MedicalRecord();
				MedicalHistory[] antecedents = {request.ToMedicalHistory()};
				newRecord.Antecedents = antecedents;
				var hash = await _storageService.SaveMedicalRecord(newRecord, cancellationToken);
				if (hash is null)
				{
					return Results.BadRequest();
				}
				await _databaseService
					.SaveMedicalRecordHashAsync(session, patientId, hash, cancellationToken);
				return Results.Ok();
			}

			var prev = record.Antecedents ?? Array.Empty<MedicalHistory>();
			var antecedentsToAdd = new[] {request.ToMedicalHistory()};
			record.Antecedents = prev
				.Concat(antecedentsToAdd)
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
		app.MapDelete("records/{patientId}/antecedents",
		async (IDocumentSession session, string patientId, [FromQuery] string antecedentId, CancellationToken cancellationToken) =>
		{
			var record = await _storageService
				.GetLatestMedicalRecord(session, patientId, cancellationToken);
			if (record is null)
			{
				return Results.NotFound();
			}
			record.Antecedents = record.Antecedents
				.Where(p => p.Id != antecedentId)
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
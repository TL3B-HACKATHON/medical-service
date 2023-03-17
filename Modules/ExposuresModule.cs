using Carter;
using Marten;
using medical_profile_service.Entities;
using medical_profile_service.Services;
using Microsoft.AspNetCore.Mvc;

namespace medical_profile_service.Modules;

public class ExposuresModule : ICarterModule
{
	private readonly IStorageService _storageService;
	private readonly IDatabaseService _databaseService;
	
	public ExposuresModule(IStorageService storageService, IDatabaseService databaseService)
	{
		_storageService = storageService;
		_databaseService = databaseService;
	}
	public void AddRoutes(IEndpointRouteBuilder app)
	{
		app.MapGet("/records/{patientId}/exposures",
			async (IDocumentSession session, string patientId, CancellationToken cancellationToken) =>
			{
				var record = await _storageService
					.GetLatestMedicalRecord(session, patientId, cancellationToken);
				return record is null
					? Results.NotFound()
					: Results.Ok(record.Exposures.IsEmpty() ? Array.Empty<Exposure>() : record.Exposures);
			});
		app.MapPost("/records/{patientId}/exposures", async (IDocumentSession session, ExposureDto request, string patientId, CancellationToken cancellationToken) =>
		{
			var record = await _storageService
				.GetLatestMedicalRecord(session, patientId, cancellationToken);
			if (record is null)
			{
				var newRecord = new MedicalRecord();
				Exposure[] exposures = {request.ToExposure()};
				newRecord.Exposures = exposures;
				var hash = await _storageService.SaveMedicalRecord(newRecord, cancellationToken);
				if (hash is null)
				{
					return Results.BadRequest();
				}
				await _databaseService
					.SaveMedicalRecordHashAsync(session, patientId, hash, cancellationToken);
				return Results.Ok();
			}
			var prev = record.Exposures ?? Array.Empty<Exposure>();
			var exposuresToAdd = new[] {request.ToExposure()};
			record.Exposures = prev
				.Concat(exposuresToAdd)
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
		app.MapDelete("records/{patientId}/exposures",
		async (IDocumentSession session, string patientId, [FromQuery] string exposureId, CancellationToken cancellationToken) =>
		{
			var record = await _storageService
				.GetLatestMedicalRecord(session, patientId, cancellationToken);
			if (record is null)
			{
				return Results.NotFound();
			}
			record.Exposures = record.Exposures
				.Where(p => p.Id != exposureId)
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
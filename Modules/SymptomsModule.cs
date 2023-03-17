using Carter;
using JasperFx.Core;
using Marten;
using medical_profile_service.Entities;
using medical_profile_service.Services;
using Microsoft.AspNetCore.Mvc;

namespace medical_profile_service.Modules;

public class SymptomsModule : ICarterModule
{
	private readonly IStorageService _storageService;
	private readonly IDatabaseService _databaseService;
	
	public SymptomsModule(IStorageService storageService, IDatabaseService databaseService)
	{
		_storageService = storageService;
		_databaseService = databaseService;
	}
	public void AddRoutes(IEndpointRouteBuilder app)
	{
		app.MapGet("/records/{patientId}/symptoms-analysis",
			async (IDocumentSession session, string patientId, CancellationToken cancellationToken) =>
			{
				var record = await _storageService
					.GetLatestMedicalRecord(session, patientId, cancellationToken);
				return record is null
					? Results.NotFound()
					: Results.Ok(record.PreviousSymptomsAnalysis.IsEmpty() ? Array.Empty<SymptomsAnalysis>() : record.PreviousSymptomsAnalysis);
			});
		app.MapPost("/records/{patientId}/symptoms-analysis", async 
			(IDocumentSession session, SymptomsAnalysisDto request, string patientId, CancellationToken cancellationToken) =>
		{
			var record = await _storageService
				.GetLatestMedicalRecord(session, patientId, cancellationToken);
			if (record is null)
			{
				var newRecord = new MedicalRecord();
				SymptomsAnalysis[] symptoms = {request.ToSymptomsAnalysis()};
				newRecord.PreviousSymptomsAnalysis = symptoms;
				var hash = await _storageService.SaveMedicalRecord(newRecord, cancellationToken);
				if (hash is null)
				{
					return Results.BadRequest();
				}
				await _databaseService
					.SaveMedicalRecordHashAsync(session, patientId, hash, cancellationToken);
				return Results.Ok();
			}
			var prev = record.PreviousSymptomsAnalysis ?? Array.Empty<SymptomsAnalysis>();
			var toAdd = new[] {request.ToSymptomsAnalysis()};
			record.PreviousSymptomsAnalysis = prev
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
		app.MapDelete("records/{patientId}/symptoms-analysis",
		async (IDocumentSession session, string patientId, [FromQuery] string analysisId, CancellationToken cancellationToken) =>
		{
			var record = await _storageService
				.GetLatestMedicalRecord(session, patientId, cancellationToken);
			if (record is null)
			{
				return Results.NotFound();
			}
			record.PreviousSymptomsAnalysis = record.PreviousSymptomsAnalysis
				.Where(s => s.Id != analysisId)
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
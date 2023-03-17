using Carter;
using Marten;
using medical_profile_service.Dtos;
using medical_profile_service.Entities;
using medical_profile_service.Services;
using Microsoft.AspNetCore.Mvc;

namespace medical_profile_service.Modules;

public class AllergiesModule : ICarterModule
{
	private readonly IStorageService _storageService;
	private readonly IDatabaseService _databaseService;
	
	public AllergiesModule(IStorageService storageService, IDatabaseService databaseService)
	{
		_storageService = storageService;
		_databaseService = databaseService;
	}

	public void AddRoutes(IEndpointRouteBuilder app)
	{
		app.MapGet("records/{patientId}/allergies/", async (IDocumentSession session, string patientId, CancellationToken cancellationToken) =>
		{
			var record = await _storageService
				.GetLatestMedicalRecord(session, patientId, cancellationToken);
			return record is null 
				? Results.NotFound() 
				: Results.Ok(record.Allergies.IsEmpty() ? Array.Empty<string>() : record.Allergies);
		})
		.WithName("Get Patient Allergies")
		.WithOpenApi();
		app.MapPost("records/{patientId}/allergies/", 
				async (IDocumentSession session, AddAllergiesRequest request, string patientId, CancellationToken cancellationToken) =>
		{
			var record = await _storageService
				.GetLatestMedicalRecord(session, patientId, cancellationToken);
			if (record is null)
			{
				var newRecord = new MedicalRecord
				{
					Allergies = request.Allergies
				};
				var hash = await _storageService.SaveMedicalRecord(newRecord, cancellationToken);
				if (hash is null)
				{
					return Results.BadRequest();
				}
				await _databaseService
					.SaveMedicalRecordHashAsync(session, patientId, hash, cancellationToken);
				return Results.Ok();
			}

			var prevAllergies = record.Allergies;
			var allergiesToAdd = request.Allergies;
			record.Allergies = prevAllergies
				.Concat(allergiesToAdd)
				.Distinct()
				.ToArray();
			record.Allergies = record.Allergies
				.Distinct()
				.ToArray();
			var newHash = await _storageService.SaveMedicalRecord(record, cancellationToken);
			if (newHash is null)
			{
				return Results.BadRequest();
			}
			await _databaseService
				.SaveMedicalRecordHashAsync(session, patientId, newHash, cancellationToken);
			return Results.Ok();
		})
		.WithName("Add Allergy to Patient")
		.WithOpenApi();
		app.MapDelete("records/{patientId}/allergies", 
				async (IDocumentSession session, string patientId, [FromQuery] string allergyName, CancellationToken cancellationToken) =>
		{
			var record = await _storageService
				.GetLatestMedicalRecord(session, patientId, cancellationToken);
			if (record is null)
			{
				return Results.NotFound();
			}
			record.Allergies = record.Allergies
				.Where(x => x != allergyName)
				.ToArray();
			var newHash = await _storageService.SaveMedicalRecord(record, cancellationToken);
			if (newHash is null)
			{
				return Results.BadRequest();
			}
			await _databaseService
				.SaveMedicalRecordHashAsync(session, patientId, newHash, cancellationToken);
			return Results.Ok();
		})
		.WithName("Remove Allergy from Patient")
		.WithOpenApi();
	}
}
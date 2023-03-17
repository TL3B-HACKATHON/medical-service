using System.Text;
using System.Text.Json;
using Marten;
using medical_profile_service.Dtos;
using medical_profile_service.Entities;

namespace medical_profile_service.Services;

public record MedicalRecordResponse(
	string Data
);
public class StorageService : IStorageService
{
	public async Task<MedicalRecord?> GetLatestMedicalRecord(IDocumentSession session, string patientId, CancellationToken cancellationToken = default)
	{
		MpsPatientMedicalRecord? hash = await session.Query<MpsPatientMedicalRecord>()
				.FirstOrDefaultAsync(x => x.PatientId == patientId, cancellationToken);
		var client = new HttpClient();
        var response = await client
        	.GetAsync($"{Constants.StorageServiceUrl}/{hash?.Hash}", cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
        	return null;
        }
        MedicalRecordResponse? res;
        try
        {
			res = await response.Content
				.ReadFromJsonAsync<MedicalRecordResponse>(cancellationToken: cancellationToken);
        }
        catch (Exception e)
		{
			return null;
		}
        if (res?.Data is null || string.IsNullOrWhiteSpace(res.Data))
        {
	        return null;
        }
        var record = JsonSerializer.Deserialize<MedicalRecord>(res.Data);
		return record;
	}
	public async Task<string?> SaveMedicalRecord(MedicalRecord record, CancellationToken cancellationToken = default)
	{
		var jsonString = JsonSerializer.Serialize(record);
		var req = new
		{
			data = jsonString
		};
		var client = new HttpClient();
		var response = await client
			.PostAsync($"{Constants.StorageServiceUrl}/",
				new StringContent(JsonSerializer.Serialize(req), Encoding.UTF8, "application/json"),
				cancellationToken);
		if (!response.IsSuccessStatusCode)
		{
			return null;
		}
		var res = await response.Content
			.ReadFromJsonAsync<PostMedicalRecordResponse>(cancellationToken: cancellationToken);
		return res?.data;
	}
}
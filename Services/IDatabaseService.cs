using Marten;

namespace medical_profile_service.Services;

public interface IDatabaseService
{
	Task SaveMedicalRecordHashAsync(IDocumentSession session, string patientId, string hash, CancellationToken cancellationToken = default);
	Task<string?> GetMedicalRecordHashAsync(IDocumentSession session, string patientId, CancellationToken cancellationToken = default);
}
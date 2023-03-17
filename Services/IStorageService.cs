using Marten;
using medical_profile_service.Entities;

namespace medical_profile_service.Services;

public interface IStorageService
{
	Task<MedicalRecord?> GetLatestMedicalRecord(IDocumentSession session, string patientId, CancellationToken cancellationToken = default);
	Task<string?> SaveMedicalRecord(MedicalRecord record, CancellationToken cancellationToken = default);
}
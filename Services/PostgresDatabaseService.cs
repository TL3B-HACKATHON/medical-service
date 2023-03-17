using Marten;

namespace medical_profile_service.Services;

public class PostgresDatabaseService : IDatabaseService
{
	public async Task SaveMedicalRecordHashAsync(IDocumentSession session, string patientId, string hash, CancellationToken cancellationToken = default)
	{
		var records = session.Query<MpsPatientMedicalRecord>()
			.Where(x => x.PatientId == patientId);
		foreach (var rec in records)
		{
			session.Delete(rec);
		}
		session.Store(
			new MpsPatientMedicalRecord(Guid.NewGuid().ToString(), patientId, hash)
		);
		await session.SaveChangesAsync(cancellationToken);
	}

	public async Task<string?> GetMedicalRecordHashAsync(IDocumentSession session, string patientId, CancellationToken cancellationToken = default)
	{
		var record = await session.Query<MpsPatientMedicalRecord>()
			.FirstOrDefaultAsync(x => x.PatientId == patientId, cancellationToken);
		return record?.Hash;
	}
}

public record MpsPatientMedicalRecord(
	string Id,
	string PatientId,
	string Hash
);
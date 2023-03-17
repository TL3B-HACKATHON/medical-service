namespace medical_profile_service.Entities;

public record MedicalHistory(
	string Id,
	string Date,
	string Type,
	string Name
);

public record MedicalHistoryDto(
	string Date,
	string Type,
	string Name
)
{
	public MedicalHistory ToMedicalHistory()
		=> new MedicalHistory(Guid.NewGuid().ToString(), Date, Type, Name);
}
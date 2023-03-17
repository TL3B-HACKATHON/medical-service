namespace medical_profile_service.Entities;

public record Surgery(
	string Id,
	string DoctorId,
	string Date,
	string Type,
	string Name
);

public record SurgeryDto(
	string DoctorId,
	string Date,
	string Type,
	string Name
)
{
	public Surgery ToSurgery() 
		=> new(Guid.NewGuid().ToString(), DoctorId, Date, Type, Name);
}
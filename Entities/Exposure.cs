namespace medical_profile_service.Entities;

public record Exposure(
	string Id,
	string DoctorId,
	string Duration,
	string Type,
	string[] Causes
);
public record ExposureDto(
	string DoctorId,
	string Duration,
	string Type,
	string[] Causes
)
{
	public Exposure ToExposure() 
		=> new(Guid.NewGuid().ToString(), DoctorId, Duration, Type, Causes);
}
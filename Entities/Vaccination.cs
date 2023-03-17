namespace medical_profile_service.Entities;

public record Vaccination(
	string Id,
	string DoctorId,
	string DateOfAdministration,
	string VaccineAdministratorId,
	string VaccineManufacturer,
	string VaccineLotNumber
);
public record VaccinationDto(
	string DoctorId,
	string DateOfAdministration,
	string VaccineAdministratorId,
	string VaccineManufacturer,
	string VaccineLotNumber
)
{
	public Vaccination ToVaccination() 
		=> new(Guid.NewGuid().ToString(), DoctorId, DateOfAdministration, VaccineAdministratorId, VaccineManufacturer, VaccineLotNumber);
}
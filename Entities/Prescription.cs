namespace medical_profile_service.Entities;

public record Prescription(
	string Id,
	string DoctorId,
	string Date,
	string[] Medications
);

public record PrescriptionDto(
	string DoctorId,
	string Date,
	string[] Medications
)
{
	public Prescription ToPrescription()
		=> new Prescription(Guid.NewGuid().ToString(), DoctorId, Date, Medications);
}
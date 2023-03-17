namespace medical_profile_service.Entities;

public record TestResult(
	string Id,
	string PatientId,
	string DoctorId,
	string TestType,
	string Result,
	string Notes,
	string Date
);
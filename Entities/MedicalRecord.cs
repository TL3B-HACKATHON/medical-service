namespace medical_profile_service.Entities;

public class MedicalRecord
{
	public string PatientId { get; set; }
	public string Date { get; set; }
	public string[] Allergies { get; set; }
	public Prescription[] Prescriptions { get; set; }
	public SymptomsAnalysis[] PreviousSymptomsAnalysis { get; set; }
	public MedicalHistory[] Antecedents { get; set; }
	public Surgery[] Surgeries { get; set; }
	public Exposure[] Exposures { get; set; }
	public Vaccination[] Vaccinations { get; set; }
	
}
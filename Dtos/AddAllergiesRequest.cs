namespace medical_profile_service.Dtos;

public record AddAllergiesRequest
{
	public string[] Allergies { get; set; }
}
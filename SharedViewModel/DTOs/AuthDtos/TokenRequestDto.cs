namespace SharedViewModel.DTOs;

public class TokenGenerationRequest
{
    public string UserId { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public IList<string> Roles { get; set; } = new List<string>();
}
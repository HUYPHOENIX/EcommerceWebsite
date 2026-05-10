namespace SharedViewModel.DTOs;
public class AuthResponseDto
{
    public bool IsSuccess { get; set; }
    public string Message {get; set;} = string.Empty;
    public List<string> Roles { get; set; } = new List<string>();
    public string AccessToken { get; set; } = string.Empty; 
    // public string RefreshToken {get;set;} = string.Empty;
    
}
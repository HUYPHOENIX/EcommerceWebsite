namespace SharedViewModel.DTOs;
public class AuthResponseDto
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string RefreshToken {get;set;} = string.Empty;
    public string Token { get; set; } = string.Empty; 
}
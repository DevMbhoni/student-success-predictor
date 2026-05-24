using StudentSuccess.Domain.Enums;

namespace StudentSuccess.Application.DTOs.Auth;

public class RegisterRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public UserRole Role { get; set; }

    public string? StudentNumber { get; set; }
    public string? Programme { get; set; }
    public int? YearOfStudy { get; set; }
}
namespace StudentSuccess.Application.DTOs.Students;

public class StudentDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string StudentNumber { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Programme { get; set; } = string.Empty;
    public int YearOfStudy { get; set; }
    public DateTime EnrolledAt { get; set; }
}
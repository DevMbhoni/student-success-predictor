namespace StudentSuccess.Application.DTOs.Students;

public class UpdateStudentRequest
{
    public string Programme { get; set; } = string.Empty;
    public int YearOfStudy { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}
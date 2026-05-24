namespace StudentSuccess.Application.DTOs.Enrollments;

public class CreateEnrollmentRequest
{
    public Guid StudentId { get; set; }
    public Guid ModuleId { get; set; }
    public int Year { get; set; }
    public string Semester { get; set; } = string.Empty;
}
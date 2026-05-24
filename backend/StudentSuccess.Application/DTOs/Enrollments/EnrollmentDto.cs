namespace StudentSuccess.Application.DTOs.Enrollments;

public class EnrollmentDto
{
    public Guid Id { get; set; }
    public Guid StudentId { get; set; }
    public string StudentNumber { get; set; } = string.Empty;
    public string StudentName { get; set; } = string.Empty;
    public Guid ModuleId { get; set; }
    public string ModuleCode { get; set; } = string.Empty;
    public string ModuleName { get; set; } = string.Empty;
    public int Year { get; set; }
    public string Semester { get; set; } = string.Empty;
    public double AttendancePercentage { get; set; }
    public double AssignmentAverage { get; set; }
    public double TestAverage { get; set; }
    public int LmsLoginCount { get; set; }
    public string Status { get; set; } = string.Empty;
}
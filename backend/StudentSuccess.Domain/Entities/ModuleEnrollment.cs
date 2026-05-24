using StudentSuccess.Domain.Enums;

namespace StudentSuccess.Domain.Entities;

public class ModuleEnrollment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid StudentId { get; set; }
    public Guid ModuleId { get; set; }
    public int Year { get; set; }
    public string Semester { get; set; } = string.Empty;

    public double AttendancePercentage { get; set; }
    public double AssignmentAverage { get; set; }
    public double TestAverage { get; set; }
    public int LmsLoginCount { get; set; }

    public EnrollmentStatus Status { get; set; } = EnrollmentStatus.Active;

    public Student Student { get; set; } = null!;
    public Module Module { get; set; } = null!;
}
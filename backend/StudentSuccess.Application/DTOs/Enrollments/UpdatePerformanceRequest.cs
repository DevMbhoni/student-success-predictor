namespace StudentSuccess.Application.DTOs.Enrollments;

public class UpdatePerformanceRequest
{
    public double AttendancePercentage { get; set; }
    public double AssignmentAverage { get; set; }
    public double TestAverage { get; set; }
    public int LmsLoginCount { get; set; }
}
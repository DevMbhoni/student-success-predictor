namespace StudentSuccess.Application.DTOs.Predictions;

public class MLServiceRequest
{
    public double AttendancePercentage { get; set; }
    public double AssignmentAverage { get; set; }
    public double TestAverage { get; set; }
    public int LmsLoginCount { get; set; }
}
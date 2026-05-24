namespace StudentSuccess.Application.DTOs.Predictions;

public class PredictionResultDto
{
    public Guid Id { get; set; }
    public Guid StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string StudentNumber { get; set; } = string.Empty;
    public string ModuleCode { get; set; } = string.Empty;
    public string ModuleName { get; set; } = string.Empty;
    public double PassProbability { get; set; }
    public double FailProbability { get; set; }
    public string RiskLevel { get; set; } = string.Empty;
    public string PredictedOutcome { get; set; } = string.Empty;
    public string Recommendation { get; set; } = string.Empty;
    public string ModelVersion { get; set; } = string.Empty;
    public DateTime PredictedAt { get; set; }

    public double AttendancePercentage { get; set; }
    public double AssignmentAverage { get; set; }
    public double TestAverage { get; set; }
    public int LmsLoginCount { get; set; }
}
namespace StudentSuccess.Application.DTOs.Predictions;

public class MLServiceResponse
{
    public double PassProbability { get; set; }
    public double FailProbability { get; set; }
    public string RiskLevel { get; set; } = string.Empty;
    public string PredictedOutcome { get; set; } = string.Empty;
    public string Recommendation { get; set; } = string.Empty;
    public string ModelVersion { get; set; } = string.Empty;
}
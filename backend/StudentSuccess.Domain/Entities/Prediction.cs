using StudentSuccess.Domain.Enums;

namespace StudentSuccess.Domain.Entities;

public class Prediction
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid StudentId { get; set; }
    public Guid ModuleEnrollmentId { get; set; }

    public double PassProbability { get; set; }   
    public double FailProbability { get; set; }
    public RiskLevel RiskLevel { get; set; }     

    public string RecommendedIntervention { get; set; } = string.Empty;
    public DateTime PredictedAt { get; set; } = DateTime.UtcNow;
    public string ModelVersion { get; set; } = string.Empty;

    public Student Student { get; set; } = null!;
}
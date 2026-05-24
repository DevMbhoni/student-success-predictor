using StudentSuccess.Application.DTOs.Predictions;

namespace StudentSuccess.Application.Interfaces;

public interface IPredictionService
{
    Task<PredictionResultDto> PredictForEnrollmentAsync(Guid enrollmentId);
    Task<IEnumerable<PredictionResultDto>> GetPredictionsByStudentAsync(Guid studentId);
    Task<IEnumerable<PredictionResultDto>> GetLatestPredictionsAsync();
}
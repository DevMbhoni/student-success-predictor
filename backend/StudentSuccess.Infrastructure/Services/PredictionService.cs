using System.Net.Http.Json;
using System.Text.Json;
using System.Net.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using StudentSuccess.Application.DTOs.Predictions;
using StudentSuccess.Application.Interfaces;
using StudentSuccess.Domain.Entities;
using StudentSuccess.Domain.Enums;
using StudentSuccess.Infrastructure.Data;

namespace StudentSuccess.Infrastructure.Services;

public class PredictionService : IPredictionService
{
    private readonly ApplicationDbContext _context;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _config;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
    };

    public PredictionService(
        ApplicationDbContext context,
        IHttpClientFactory httpClientFactory,
        IConfiguration config)
    {
        _context = context;
        _httpClientFactory = httpClientFactory;
        _config = config;
    }

    public async Task<PredictionResultDto> PredictForEnrollmentAsync(Guid enrollmentId)
    {
        var enrollment = await _context.ModuleEnrollments
            .Include(e => e.Student).ThenInclude(s => s.User)
            .Include(e => e.Module)
            .FirstOrDefaultAsync(e => e.Id == enrollmentId)
            ?? throw new KeyNotFoundException("Enrollment not found.");

        var mlRequest = new MLServiceRequest
        {
            AttendancePercentage = enrollment.AttendancePercentage,
            AssignmentAverage = enrollment.AssignmentAverage,
            TestAverage = enrollment.TestAverage,
            LmsLoginCount = enrollment.LmsLoginCount
        };

        var client = _httpClientFactory.CreateClient("MLService");
        var response = await client.PostAsJsonAsync(
            "/api/predict", mlRequest, JsonOptions);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"ML service error: {error}");
        }

        var mlResult = await response.Content
            .ReadFromJsonAsync<MLServiceResponse>(JsonOptions)
            ?? throw new Exception("Empty response from ML service.");

        var riskLevel = mlResult.RiskLevel switch
        {
            "High" => RiskLevel.High,
            "Medium" => RiskLevel.Medium,
            _ => RiskLevel.Low
        };

        var prediction = new Prediction
        {
            StudentId = enrollment.StudentId,
            ModuleEnrollmentId = enrollment.Id,
            PassProbability = mlResult.PassProbability,
            FailProbability = mlResult.FailProbability,
            RiskLevel = riskLevel,
            RecommendedIntervention = mlResult.Recommendation,
            ModelVersion = mlResult.ModelVersion,
            PredictedAt = DateTime.UtcNow
        };

        _context.Predictions.Add(prediction);
        await _context.SaveChangesAsync();

        return new PredictionResultDto
        {
            Id = prediction.Id,
            StudentId = enrollment.StudentId,
            StudentName = $"{enrollment.Student.User.FirstName} {enrollment.Student.User.LastName}",
            StudentNumber = enrollment.Student.StudentNumber,
            ModuleCode = enrollment.Module.Code,
            ModuleName = enrollment.Module.Name,
            PassProbability = mlResult.PassProbability,
            FailProbability = mlResult.FailProbability,
            RiskLevel = mlResult.RiskLevel,
            PredictedOutcome = mlResult.PredictedOutcome,
            Recommendation = mlResult.Recommendation,
            ModelVersion = mlResult.ModelVersion,
            PredictedAt = prediction.PredictedAt,
            AttendancePercentage = enrollment.AttendancePercentage,
            AssignmentAverage = enrollment.AssignmentAverage,
            TestAverage = enrollment.TestAverage,
            LmsLoginCount = enrollment.LmsLoginCount
        };
    }

    public async Task<IEnumerable<PredictionResultDto>> GetPredictionsByStudentAsync(Guid studentId)
    {
        return await _context.Predictions
            .Include(p => p.Student).ThenInclude(s => s.User)
            .Where(p => p.StudentId == studentId)
            .OrderByDescending(p => p.PredictedAt)
            .Select(p => new PredictionResultDto
            {
                Id = p.Id,
                StudentId = p.StudentId,
                StudentName = $"{p.Student.User.FirstName} {p.Student.User.LastName}",
                StudentNumber = p.Student.StudentNumber,
                PassProbability = p.PassProbability,
                FailProbability = p.FailProbability,
                RiskLevel = p.RiskLevel.ToString(),
                Recommendation = p.RecommendedIntervention,
                ModelVersion = p.ModelVersion,
                PredictedAt = p.PredictedAt
            })
            .ToListAsync();
    }
    public async Task<IEnumerable<PredictionResultDto>> GetLatestPredictionsAsync()
    {

        var predictions = await _context.Predictions
            .Include(p => p.Student).ThenInclude(s => s.User)
            .OrderByDescending(p => p.PredictedAt)
            .ToListAsync();

        var latest = predictions
            .GroupBy(p => p.StudentId)
            .Select(g => g.First()) 
            .ToList();

        return latest.Select(p => new PredictionResultDto
        {
            Id = p.Id,
            StudentId = p.StudentId,
            StudentName = $"{p.Student.User.FirstName} {p.Student.User.LastName}",
            StudentNumber = p.Student.StudentNumber,
            PassProbability = p.PassProbability,
            FailProbability = p.FailProbability,
            RiskLevel = p.RiskLevel.ToString(),
            Recommendation = p.RecommendedIntervention,
            PredictedAt = p.PredictedAt
        });
    }
}
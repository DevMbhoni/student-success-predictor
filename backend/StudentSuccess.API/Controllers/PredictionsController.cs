using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentSuccess.Application.DTOs.Predictions;
using StudentSuccess.Application.Interfaces;
using System.Security.Claims;

namespace StudentSuccess.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PredictionsController : ControllerBase
{
    private readonly IPredictionService _predictionService;

    public PredictionsController(IPredictionService predictionService)
    {
        _predictionService = predictionService;
    }
    [HttpPost]
    [Authorize(Roles = "Lecturer,Administrator,AcademicAdvisor")]
    public async Task<IActionResult> Predict([FromBody] PredictionRequestDto request)
    {
        try
        {
            var result = await _predictionService
                .PredictForEnrollmentAsync(request.EnrollmentId);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(503, new
            {
                message = "Prediction service unavailable. Please try again later.",
                detail = ex.Message
            });
        }
    }

    [HttpGet("student/{studentId:guid}")]
    public async Task<IActionResult> GetByStudent(Guid studentId)
    {
        if (User.IsInRole("Student"))
        {
            var userId = Guid.Parse(
                User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var isOwner = await VerifyStudentOwnership(userId, studentId);
            if (!isOwner) return Forbid();
        }

        var predictions = await _predictionService
            .GetPredictionsByStudentAsync(studentId);
        return Ok(predictions);
    }


    [HttpGet("latest")]
    [Authorize(Roles = "Administrator,AcademicAdvisor")]
    public async Task<IActionResult> GetLatest()
    {
        var predictions = await _predictionService.GetLatestPredictionsAsync();
        return Ok(predictions);
    }

    private async Task<bool> VerifyStudentOwnership(Guid userId, Guid studentId)
    {
        var db = HttpContext.RequestServices
            .GetRequiredService<Infrastructure.Data.ApplicationDbContext>();

        return await System.Threading.Tasks.Task.FromResult(
            db.Students.Any(s => s.UserId == userId && s.Id == studentId));
    }
}
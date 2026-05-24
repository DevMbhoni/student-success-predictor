using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentSuccess.Application.DTOs.Enrollments;
using StudentSuccess.Application.Interfaces;
using System.Security.Claims;

namespace StudentSuccess.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EnrollmentsController : ControllerBase
{
    private readonly IEnrollmentService _enrollmentService;

    public EnrollmentsController(IEnrollmentService enrollmentService)
    {
        _enrollmentService = enrollmentService;
    }

    [HttpPost]
    [Authorize(Roles = "Administrator,AcademicAdvisor")]
    public async Task<IActionResult> Enroll([FromBody] CreateEnrollmentRequest request)
    {
        try
        {
            var enrollment = await _enrollmentService.EnrollStudentAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = enrollment.Id }, enrollment);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var enrollment = await _enrollmentService.GetEnrollmentByIdAsync(id);
        return enrollment is null ? NotFound() : Ok(enrollment);
    }

    [HttpGet("student/{studentId:guid}")]
    public async Task<IActionResult> GetByStudent(Guid studentId)
    {
        if (User.IsInRole("Student"))
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var student = await GetStudentIdFromUserId(userId);
            if (student != studentId)
                return Forbid();
        }

        var enrollments = await _enrollmentService.GetEnrollmentsByStudentAsync(studentId);
        return Ok(enrollments);
    }

    [HttpGet("module/{moduleId:guid}")]
    [Authorize(Roles = "Lecturer,Administrator,AcademicAdvisor")]
    public async Task<IActionResult> GetByModule(Guid moduleId)
    {
        var enrollments = await _enrollmentService.GetEnrollmentsByModuleAsync(moduleId);
        return Ok(enrollments);
    }

    [HttpPut("{id:guid}/performance")]
    [Authorize(Roles = "Lecturer,Administrator")]
    public async Task<IActionResult> UpdatePerformance(
        Guid id, [FromBody] UpdatePerformanceRequest request)
    {
        try
        {
            var updated = await _enrollmentService.UpdatePerformanceAsync(id, request);
            return Ok(updated);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    private async Task<Guid> GetStudentIdFromUserId(Guid userId)
    {
        var student = await System.Threading.Tasks.Task.FromResult(
            HttpContext.RequestServices
                .GetRequiredService<Infrastructure.Data.ApplicationDbContext>()
                .Students
                .FirstOrDefault(s => s.UserId == userId));

        return student?.Id ?? Guid.Empty;
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentSuccess.Application.DTOs.Students;
using StudentSuccess.Application.Interfaces;
using System.Security.Claims;

namespace StudentSuccess.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] 
public class StudentsController : ControllerBase
{
    private readonly IStudentService _studentService;

    public StudentsController(IStudentService studentService)
    {
        _studentService = studentService;
    }

    [HttpGet]
    [Authorize(Roles = "Lecturer,Administrator,AcademicAdvisor")]
    public async Task<IActionResult> GetAll()
    {
        var students = await _studentService.GetAllStudentsAsync();
        return Ok(students);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        if (User.IsInRole("Student"))
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var own = await _studentService.GetStudentByUserIdAsync(userId);
            if (own is null || own.Id != id)
                return Forbid();
        }

        var student = await _studentService.GetStudentByIdAsync(id);
        return student is null ? NotFound() : Ok(student);
    }

    [HttpGet("me")]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> GetMyProfile()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var student = await _studentService.GetStudentByUserIdAsync(userId);
        return student is null ? NotFound() : Ok(student);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Administrator,AcademicAdvisor")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateStudentRequest request)
    {
        try
        {
            var updated = await _studentService.UpdateStudentAsync(id, request);
            return Ok(updated);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _studentService.DeleteStudentAsync(id);
            return NoContent(); 
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}
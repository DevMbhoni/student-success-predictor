using Microsoft.EntityFrameworkCore;
using StudentSuccess.Application.DTOs.Students;
using StudentSuccess.Application.Interfaces;
using StudentSuccess.Infrastructure.Data;

namespace StudentSuccess.Infrastructure.Services;

public class StudentService : IStudentService
{
    private readonly ApplicationDbContext _context;

    public StudentService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<StudentDto>> GetAllStudentsAsync()
    {
        return await _context.Students
            .Include(s => s.User)
            .Where(s => s.User.IsActive)
            .OrderBy(s => s.User.LastName)
            .Select(s => MapToDto(s))
            .ToListAsync();
    }

    public async Task<StudentDto?> GetStudentByIdAsync(Guid id)
    {
        var student = await _context.Students
            .Include(s => s.User)
            .FirstOrDefaultAsync(s => s.Id == id);

        return student is null ? null : MapToDto(student);
    }

    public async Task<StudentDto?> GetStudentByUserIdAsync(Guid userId)
    {
        var student = await _context.Students
            .Include(s => s.User)
            .FirstOrDefaultAsync(s => s.UserId == userId);

        return student is null ? null : MapToDto(student);
    }

    public async Task<StudentDto> UpdateStudentAsync(Guid id, UpdateStudentRequest request)
    {
        var student = await _context.Students
            .Include(s => s.User)
            .FirstOrDefaultAsync(s => s.Id == id)
            ?? throw new KeyNotFoundException($"Student with ID {id} not found.");

        student.Programme = request.Programme;
        student.YearOfStudy = request.YearOfStudy;

        student.User.FirstName = request.FirstName.Trim();
        student.User.LastName = request.LastName.Trim();

        await _context.SaveChangesAsync();
        return MapToDto(student);
    }

    public async Task DeleteStudentAsync(Guid id)
    {
        var student = await _context.Students.FindAsync(id)
            ?? throw new KeyNotFoundException($"Student with ID {id} not found.");


        student.User.IsActive = false;
        await _context.SaveChangesAsync();
    }


    private static StudentDto MapToDto(Domain.Entities.Student s) => new()
    {
        Id = s.Id,
        UserId = s.UserId,
        StudentNumber = s.StudentNumber,
        FullName = $"{s.User.FirstName} {s.User.LastName}",
        Email = s.User.Email,
        Programme = s.Programme,
        YearOfStudy = s.YearOfStudy,
        EnrolledAt = s.EnrolledAt
    };
}
using Microsoft.EntityFrameworkCore;
using StudentSuccess.Application.DTOs.Enrollments;
using StudentSuccess.Application.Interfaces;
using StudentSuccess.Domain.Entities;
using StudentSuccess.Domain.Enums;
using StudentSuccess.Infrastructure.Data;

namespace StudentSuccess.Infrastructure.Services;

public class EnrollmentService : IEnrollmentService
{
    private readonly ApplicationDbContext _context;

    public EnrollmentService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<EnrollmentDto> EnrollStudentAsync(CreateEnrollmentRequest request)
    {
        var student = await _context.Students
            .Include(s => s.User)
            .FirstOrDefaultAsync(s => s.Id == request.StudentId)
            ?? throw new KeyNotFoundException("Student not found.");

        var module = await _context.Modules
            .FirstOrDefaultAsync(m => m.Id == request.ModuleId)
            ?? throw new KeyNotFoundException("Module not found.");

        var alreadyEnrolled = await _context.ModuleEnrollments
            .AnyAsync(e =>
                e.StudentId == request.StudentId &&
                e.ModuleId == request.ModuleId &&
                e.Year == request.Year &&
                e.Semester == request.Semester);

        if (alreadyEnrolled)
            throw new InvalidOperationException(
                $"{student.User.FirstName} is already enrolled in {module.Code} for {request.Semester} {request.Year}.");

        var enrollment = new ModuleEnrollment
        {
            StudentId = request.StudentId,
            ModuleId = request.ModuleId,
            Year = request.Year,
            Semester = request.Semester,
            Status = EnrollmentStatus.Active,
            AttendancePercentage = 0,
            AssignmentAverage = 0,
            TestAverage = 0,
            LmsLoginCount = 0
        };

        _context.ModuleEnrollments.Add(enrollment);
        await _context.SaveChangesAsync();

        return MapToDto(enrollment, student, module);
    }

    public async Task<IEnumerable<EnrollmentDto>> GetEnrollmentsByStudentAsync(Guid studentId)
    {
        return await _context.ModuleEnrollments
            .Include(e => e.Student).ThenInclude(s => s.User)
            .Include(e => e.Module)
            .Where(e => e.StudentId == studentId)
            .OrderByDescending(e => e.Year)
            .ThenBy(e => e.Semester)
            .Select(e => MapToDto(e, e.Student, e.Module))
            .ToListAsync();
    }

    public async Task<IEnumerable<EnrollmentDto>> GetEnrollmentsByModuleAsync(Guid moduleId)
    {
        return await _context.ModuleEnrollments
            .Include(e => e.Student).ThenInclude(s => s.User)
            .Include(e => e.Module)
            .Where(e => e.ModuleId == moduleId)
            .OrderBy(e => e.Student.User.LastName)
            .Select(e => MapToDto(e, e.Student, e.Module))
            .ToListAsync();
    }

    public async Task<EnrollmentDto?> GetEnrollmentByIdAsync(Guid enrollmentId)
    {
        var enrollment = await _context.ModuleEnrollments
            .Include(e => e.Student).ThenInclude(s => s.User)
            .Include(e => e.Module)
            .FirstOrDefaultAsync(e => e.Id == enrollmentId);

        return enrollment is null
            ? null
            : MapToDto(enrollment, enrollment.Student, enrollment.Module);
    }

    public async Task<EnrollmentDto> UpdatePerformanceAsync(
        Guid enrollmentId, UpdatePerformanceRequest request)
    {
        var enrollment = await _context.ModuleEnrollments
            .Include(e => e.Student).ThenInclude(s => s.User)
            .Include(e => e.Module)
            .FirstOrDefaultAsync(e => e.Id == enrollmentId)
            ?? throw new KeyNotFoundException("Enrollment not found.");

        if (request.AttendancePercentage is < 0 or > 100)
            throw new ArgumentException("Attendance must be between 0 and 100.");

        if (request.AssignmentAverage is < 0 or > 100)
            throw new ArgumentException("Assignment average must be between 0 and 100.");

        if (request.TestAverage is < 0 or > 100)
            throw new ArgumentException("Test average must be between 0 and 100.");

        enrollment.AttendancePercentage = request.AttendancePercentage;
        enrollment.AssignmentAverage = request.AssignmentAverage;
        enrollment.TestAverage = request.TestAverage;
        enrollment.LmsLoginCount = request.LmsLoginCount;

        await _context.SaveChangesAsync();

        return MapToDto(enrollment, enrollment.Student, enrollment.Module);
    }


    private static EnrollmentDto MapToDto(
        ModuleEnrollment e,
        Domain.Entities.Student s,
        Domain.Entities.Module m) => new()
        {
            Id = e.Id,
            StudentId = e.StudentId,
            StudentNumber = s.StudentNumber,
            StudentName = $"{s.User.FirstName} {s.User.LastName}",
            ModuleId = e.ModuleId,
            ModuleCode = m.Code,
            ModuleName = m.Name,
            Year = e.Year,
            Semester = e.Semester,
            AttendancePercentage = e.AttendancePercentage,
            AssignmentAverage = e.AssignmentAverage,
            TestAverage = e.TestAverage,
            LmsLoginCount = e.LmsLoginCount,
            Status = e.Status.ToString()
        };
}
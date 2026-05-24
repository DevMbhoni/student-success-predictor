using StudentSuccess.Application.DTOs.Enrollments;

namespace StudentSuccess.Application.Interfaces;

public interface IEnrollmentService
{
    Task<EnrollmentDto> EnrollStudentAsync(CreateEnrollmentRequest request);
    Task<IEnumerable<EnrollmentDto>> GetEnrollmentsByStudentAsync(Guid studentId);
    Task<IEnumerable<EnrollmentDto>> GetEnrollmentsByModuleAsync(Guid moduleId);
    Task<EnrollmentDto> UpdatePerformanceAsync(Guid enrollmentId, UpdatePerformanceRequest request);
    Task<EnrollmentDto?> GetEnrollmentByIdAsync(Guid enrollmentId);
}
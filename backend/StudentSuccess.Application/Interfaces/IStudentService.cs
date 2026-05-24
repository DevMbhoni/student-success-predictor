using StudentSuccess.Application.DTOs.Students;

namespace StudentSuccess.Application.Interfaces;

public interface IStudentService
{
    Task<IEnumerable<StudentDto>> GetAllStudentsAsync();
    Task<StudentDto?> GetStudentByIdAsync(Guid id);
    Task<StudentDto?> GetStudentByUserIdAsync(Guid userId);
    Task<StudentDto> UpdateStudentAsync(Guid id, UpdateStudentRequest request);
    Task DeleteStudentAsync(Guid id);
}
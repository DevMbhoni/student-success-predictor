using StudentSuccess.Domain.Entities;

namespace StudentSuccess.Domain.Interfaces;

public interface IStudentRepository
{
    Task<Student?> GetByIdAsync(Guid id);
    Task<Student?> GetByUserIdAsync(Guid userId);
    Task<IEnumerable<Student>> GetAllAsync();
    Task<Student> CreateAsync(Student student);
    Task UpdateAsync(Student student);
    Task DeleteAsync(Guid id);
}
using Microsoft.EntityFrameworkCore;
using StudentSuccess.Domain.Entities;
using StudentSuccess.Domain.Interfaces;
using StudentSuccess.Infrastructure.Data;

namespace StudentSuccess.Infrastructure.Repositories;

public class StudentRepository : IStudentRepository
{
    private readonly ApplicationDbContext _context;

    public StudentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Student?> GetByIdAsync(Guid id)
    {
        return await _context.Students
            .Include(s => s.User)          
            .Include(s => s.Enrollments)   
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<Student?> GetByUserIdAsync(Guid userId)
    {
        return await _context.Students
            .Include(s => s.User)
            .FirstOrDefaultAsync(s => s.UserId == userId);
    }

    public async Task<IEnumerable<Student>> GetAllAsync()
    {
        return await _context.Students
            .Include(s => s.User)
            .OrderBy(s => s.User.LastName)
            .ToListAsync();
    }

    public async Task<Student> CreateAsync(Student student)
    {
        _context.Students.Add(student);
        await _context.SaveChangesAsync();
        return student;
    }

    public async Task UpdateAsync(Student student)
    {
        _context.Students.Update(student);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var student = await _context.Students.FindAsync(id);
        if (student is not null)
        {
            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
        }
    }
}
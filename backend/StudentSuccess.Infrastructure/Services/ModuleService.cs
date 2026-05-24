using Microsoft.EntityFrameworkCore;
using StudentSuccess.Application.DTOs.Modules;
using StudentSuccess.Application.Interfaces;
using StudentSuccess.Domain.Entities;
using StudentSuccess.Infrastructure.Data;

namespace StudentSuccess.Infrastructure.Services;

public class ModuleService : IModuleService
{
    private readonly ApplicationDbContext _context;

    public ModuleService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ModuleDto>> GetAllModulesAsync()
    {
        return await _context.Modules
            .Include(m => m.Enrollments)
            .OrderBy(m => m.Code)
            .Select(m => new ModuleDto
            {
                Id = m.Id,
                Code = m.Code,
                Name = m.Name,
                Credits = m.Credits,
                Department = m.Department,
                EnrolledStudentCount = m.Enrollments.Count
            })
            .ToListAsync();
    }

    public async Task<ModuleDto?> GetModuleByIdAsync(Guid id)
    {
        var module = await _context.Modules
            .Include(m => m.Enrollments)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (module is null) return null;

        return new ModuleDto
        {
            Id = module.Id,
            Code = module.Code,
            Name = module.Name,
            Credits = module.Credits,
            Department = module.Department,
            EnrolledStudentCount = module.Enrollments.Count
        };
    }

    public async Task<ModuleDto> CreateModuleAsync(CreateModuleRequest request)
    {
        var exists = await _context.Modules
            .AnyAsync(m => m.Code == request.Code.ToUpper());

        if (exists)
            throw new InvalidOperationException($"Module with code {request.Code} already exists.");

        var module = new Module
        {
            Code = request.Code.ToUpper().Trim(),
            Name = request.Name.Trim(),
            Credits = request.Credits,
            Department = request.Department.Trim()
        };

        _context.Modules.Add(module);
        await _context.SaveChangesAsync();

        return new ModuleDto
        {
            Id = module.Id,
            Code = module.Code,
            Name = module.Name,
            Credits = module.Credits,
            Department = module.Department,
            EnrolledStudentCount = 0
        };
    }

    public async Task<ModuleDto> UpdateModuleAsync(Guid id, CreateModuleRequest request)
    {
        var module = await _context.Modules
            .Include(m => m.Enrollments)
            .FirstOrDefaultAsync(m => m.Id == id)
            ?? throw new KeyNotFoundException($"Module with ID {id} not found.");

        module.Name = request.Name.Trim();
        module.Credits = request.Credits;
        module.Department = request.Department.Trim();

        await _context.SaveChangesAsync();

        return new ModuleDto
        {
            Id = module.Id,
            Code = module.Code,
            Name = module.Name,
            Credits = module.Credits,
            Department = module.Department,
            EnrolledStudentCount = module.Enrollments.Count
        };
    }

    public async Task DeleteModuleAsync(Guid id)
    {
        var module = await _context.Modules.FindAsync(id)
            ?? throw new KeyNotFoundException($"Module with ID {id} not found.");

        _context.Modules.Remove(module);
        await _context.SaveChangesAsync();
    }
}
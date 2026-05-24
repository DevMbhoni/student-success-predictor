using StudentSuccess.Application.DTOs.Modules; 

namespace StudentSuccess.Application.Interfaces;

public interface IModuleService
{
    Task<IEnumerable<ModuleDto>> GetAllModulesAsync();
    Task<ModuleDto?> GetModuleByIdAsync(Guid id);
    Task<ModuleDto> CreateModuleAsync(CreateModuleRequest request);
    Task<ModuleDto> UpdateModuleAsync(Guid id, CreateModuleRequest request);
    Task DeleteModuleAsync(Guid id);
}
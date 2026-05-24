using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentSuccess.Application.DTOs.Modules;
using StudentSuccess.Application.Interfaces;

namespace StudentSuccess.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ModulesController : ControllerBase
{
    private readonly IModuleService _moduleService;

    public ModulesController(IModuleService moduleService)
    {
        _moduleService = moduleService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var modules = await _moduleService.GetAllModulesAsync();
        return Ok(modules);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var module = await _moduleService.GetModuleByIdAsync(id);
        return module is null ? NotFound() : Ok(module);
    }

    [HttpPost]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> Create([FromBody] CreateModuleRequest request)
    {
        try
        {
            var created = await _moduleService.CreateModuleAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> Update(Guid id, [FromBody] CreateModuleRequest request)
    {
        try
        {
            var updated = await _moduleService.UpdateModuleAsync(id, request);
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
            await _moduleService.DeleteModuleAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}
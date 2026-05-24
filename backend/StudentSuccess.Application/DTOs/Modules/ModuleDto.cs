namespace StudentSuccess.Application.DTOs.Modules;

public class ModuleDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int Credits { get; set; }
    public string Department { get; set; } = string.Empty;
    public int EnrolledStudentCount { get; set; }
}
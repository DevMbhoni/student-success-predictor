namespace StudentSuccess.Domain.Entities;

public class Module
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code { get; set; } = string.Empty;   
    public string Name { get; set; } = string.Empty;   
    public int Credits { get; set; }
    public string Department { get; set; } = string.Empty;

    public ICollection<ModuleEnrollment> Enrollments { get; set; } = new List<ModuleEnrollment>();
}
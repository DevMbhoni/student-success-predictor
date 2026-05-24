namespace StudentSuccess.Domain.Entities
{
    public class Student
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }           
        public string StudentNumber { get; set; } = string.Empty;
        public string Programme { get; set; } = string.Empty;
        public int YearOfStudy { get; set; }
        public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;

        public User User { get; set; } = null!;
        public ICollection<ModuleEnrollment> Enrollments { get; set; } = new List<ModuleEnrollment>();
        public ICollection<Prediction> Predictions { get; set; } = new List<Prediction>();
    }
}

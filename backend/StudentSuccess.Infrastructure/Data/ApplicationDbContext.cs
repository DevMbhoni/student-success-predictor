using Microsoft.EntityFrameworkCore;
using StudentSuccess.Domain.Entities;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace StudentSuccess.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Student> Students => Set<Student>();
    public DbSet<Module> Modules => Set<Module>();
    public DbSet<ModuleEnrollment> ModuleEnrollments => Set<ModuleEnrollment>();
    public DbSet<Prediction> Predictions => Set<Prediction>();

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(e =>
        {
            e.HasKey(u => u.Id);
            e.HasIndex(u => u.Email).IsUnique();
            e.Property(u => u.Email).IsRequired().HasMaxLength(256);
            e.Property(u => u.FirstName).IsRequired().HasMaxLength(100);
            e.Property(u => u.LastName).IsRequired().HasMaxLength(100);
            e.Property(u => u.Role).HasConversion<int>();
        });

        modelBuilder.Entity<Student>(e =>
        {
            e.HasKey(s => s.Id);
            e.HasIndex(s => s.StudentNumber).IsUnique();
            e.Property(s => s.StudentNumber).IsRequired().HasMaxLength(20);

            // One User → one Student (one-to-one)
            e.HasOne(s => s.User)
             .WithOne(u => u.Student)
             .HasForeignKey<Student>(s => s.UserId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Module>(e =>
        {
            e.HasKey(m => m.Id);
            e.HasIndex(m => m.Code).IsUnique();
            e.Property(m => m.Code).IsRequired().HasMaxLength(20);
            e.Property(m => m.Name).IsRequired().HasMaxLength(200);
        });

        modelBuilder.Entity<ModuleEnrollment>(e =>
        {
            e.HasKey(me => me.Id);

            // One Student → many Enrollments
            e.HasOne(me => me.Student)
             .WithMany(s => s.Enrollments)
             .HasForeignKey(me => me.StudentId)
             .OnDelete(DeleteBehavior.Cascade);

            // One Module → many Enrollments
            e.HasOne(me => me.Module)
             .WithMany(m => m.Enrollments)
             .HasForeignKey(me => me.ModuleId)
             .OnDelete(DeleteBehavior.Restrict); 

            e.Property(me => me.AttendancePercentage)
             .HasPrecision(5, 2); 

            e.Property(me => me.AssignmentAverage)
             .HasPrecision(5, 2);

            e.Property(me => me.TestAverage)
             .HasPrecision(5, 2);
        });

        modelBuilder.Entity<Prediction>(e =>
        {
            e.HasKey(p => p.Id);

            e.HasOne(p => p.Student)
             .WithMany(s => s.Predictions)
             .HasForeignKey(p => p.StudentId)
             .OnDelete(DeleteBehavior.Cascade);

            e.Property(p => p.PassProbability).HasPrecision(5, 4);
            e.Property(p => p.FailProbability).HasPrecision(5, 4);
            e.Property(p => p.RiskLevel).HasConversion<int>();
            e.Property(p => p.RecommendedIntervention).HasMaxLength(500);
        });
    }
}
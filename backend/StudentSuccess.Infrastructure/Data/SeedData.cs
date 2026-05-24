using Microsoft.EntityFrameworkCore;
using StudentSuccess.Domain.Entities;
using StudentSuccess.Domain.Enums;

namespace StudentSuccess.Infrastructure.Data;

public static class SeedData
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        if (await context.Users.AnyAsync()) return;

        var random = new Random(42); 

        var modules = new List<Module>
        {
            new() { Code = "WRAV101", Name = "Introduction to Programming",     Credits = 16, Department = "Computer Science" },
            new() { Code = "WRAV102", Name = "Computing Fundamentals",          Credits = 16, Department = "Computer Science" },
            new() { Code = "WRAV201", Name = "Data Structures",                 Credits = 16, Department = "Computer Science" },
            new() { Code = "WRAV202", Name = "Algorithms & Complexity",         Credits = 16, Department = "Computer Science" },
            new() { Code = "WRAV203", Name = "Object Oriented Programming",     Credits = 16, Department = "Computer Science" },
            new() { Code = "WRAV204", Name = "Database Systems",                Credits = 16, Department = "Computer Science" },
            new() { Code = "WRAV301", Name = "Software Engineering",            Credits = 16, Department = "Computer Science" },
            new() { Code = "WRAV302", Name = "Operating Systems",               Credits = 16, Department = "Computer Science" },
            new() { Code = "WRAV303", Name = "Computer Networks",               Credits = 16, Department = "Computer Science" },
            new() { Code = "WRAV304", Name = "Human Computer Interaction",      Credits = 16, Department = "Computer Science" },
            new() { Code = "WRAV401", Name = "Machine Learning",                Credits = 16, Department = "Computer Science" },
            new() { Code = "WRAV402", Name = "Distributed Systems",             Credits = 16, Department = "Computer Science" },
            new() { Code = "STAT201", Name = "Statistical Inference",           Credits = 16, Department = "Statistics" },
            new() { Code = "STAT202", Name = "Probability Theory",              Credits = 16, Department = "Statistics" },
            new() { Code = "STAT301", Name = "Regression Analysis",             Credits = 16, Department = "Statistics" },
            new() { Code = "STAT302", Name = "Data Mining",                     Credits = 16, Department = "Statistics" },
            new() { Code = "STAT401", Name = "Predictive Analytics",            Credits = 16, Department = "Statistics" },
            new() { Code = "MATH101", Name = "Calculus I",                      Credits = 16, Department = "Mathematics" },
            new() { Code = "MATH102", Name = "Linear Algebra",                  Credits = 16, Department = "Mathematics" },
            new() { Code = "MATH201", Name = "Discrete Mathematics",            Credits = 16, Department = "Mathematics" },
            new() { Code = "MATH202", Name = "Numerical Methods",               Credits = 16, Department = "Mathematics" },
            new() { Code = "INFO101", Name = "Information Systems",             Credits = 16, Department = "Informatics" },
            new() { Code = "INFO201", Name = "Systems Analysis & Design",       Credits = 16, Department = "Informatics" },
            new() { Code = "INFO301", Name = "IT Project Management",           Credits = 16, Department = "Informatics" },
            new() { Code = "INFO302", Name = "Business Intelligence",           Credits = 16, Department = "Informatics" },
            new() { Code = "INFO401", Name = "Enterprise Architecture",         Credits = 16, Department = "Informatics" },
            new() { Code = "CYBR301", Name = "Cybersecurity Fundamentals",      Credits = 16, Department = "Cybersecurity" },
            new() { Code = "CYBR302", Name = "Network Security",                Credits = 16, Department = "Cybersecurity" },
            new() { Code = "CYBR401", Name = "Ethical Hacking",                 Credits = 16, Department = "Cybersecurity" },
            new() { Code = "DENG201", Name = "Data Engineering",                Credits = 16, Department = "Data Engineering" },
            new() { Code = "DENG301", Name = "Cloud Computing",                 Credits = 16, Department = "Data Engineering" },
            new() { Code = "DENG401", Name = "Big Data Technologies",           Credits = 16, Department = "Data Engineering" },
        };

        context.Modules.AddRange(modules);
        await context.SaveChangesAsync();

        var adminUser = new User
        {
            FirstName = "System",
            LastName = "Administrator",
            Email = "admin@university.ac.za",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@1234"),
            Role = UserRole.Administrator
        };
        context.Users.Add(adminUser);

        var advisorUser = new User
        {
            FirstName = "Sarah",
            LastName = "Dlamini",
            Email = "advisor@university.ac.za",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Advisor@1234"),
            Role = UserRole.AcademicAdvisor
        };
        context.Users.Add(advisorUser);

        var lecturerData = new[]
        {
            ("James",  "Mokoena",  "j.mokoena@university.ac.za"),
            ("Priya",  "Naidoo",   "p.naidoo@university.ac.za"),
            ("David",  "Van Wyk",  "d.vanwyk@university.ac.za"),
            ("Amara",  "Sithole",  "a.sithole@university.ac.za"),
            ("Thabo",  "Khumalo",  "t.khumalo@university.ac.za"),
        };

        foreach (var (first, last, email) in lecturerData)
        {
            context.Users.Add(new User
            {
                FirstName = first,
                LastName = last,
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Lecturer@1234"),
                Role = UserRole.Lecturer
            });
        }

        await context.SaveChangesAsync();

        var firstNames = new[]
        {
            "Lethabo", "Sipho", "Nomsa", "Andile", "Zanele",
            "Kagiso", "Thandeka", "Mpho", "Siyanda", "Lerato",
            "Bongani", "Nandi", "Lungelo", "Ayanda", "Thembi",
            "Tshepo", "Nokwanda", "Sibusiso", "Palesa", "Vusi",
            "Refilwe", "Mandla", "Lindiwe", "Sello", "Bongiwe",
            "Dineo", "Nthabiseng", "Khulekani", "Tebogo", "Zinhle",
            "Nkosi", "Mamosa", "Lusanda", "Buyani", "Phindile",
            "Sifiso", "Ntombi", "Lwazi", "Boitumelo", "Sandile",
            "Precious", "Tiyiselani", "Mulalo", "Rudzani", "Pfano",
            "Dimpho", "Keabetswe", "Tumelo", "Nolwazi", "Lungisa"
        };
        var lastNames = new[]
        {
            "Nkosi", "Dlamini", "Zulu", "Khumalo", "Ndlovu",
            "Mthembu", "Shabalala", "Cele", "Ngcobo", "Mkhize",
            "Sithole", "Hadebe", "Gumede", "Msweli", "Ntanzi",
            "Mokoena", "Molefe", "Kgositsile", "Radebe", "Mahlangu",
            "Nkoane", "Lekganyane", "Sefako", "Tau", "Moroka",
            "Tladi", "Seshibe", "Modise", "Monare", "Mohlala",
            "Tshivhase", "Mudau", "Mphephu", "Netshituni", "Ramudzuli",
            "Ramaphosa", "Makwela", "Mabunda", "Mashele", "Manganye",
            "Hlungwani", "Chauke", "Maluleke", "Baloyi", "Shivambu",
            "Nkuna", "Mathebula", "Rikhotso", "Mnisi", "Mahlalela"
        };
        var programmes = new[]
        {
            "BSc Computer Science",
            "BSc Computer Science & Statistics",
            "BSc Information Technology",
            "BSc Data Science",
            "BCom Informatics"
        };

        var students = new List<(User user, Student student)>();

        for (int i = 0; i < 50; i++)
        {
            var user = new User
            {
                FirstName = firstNames[i],
                LastName = lastNames[i],
                Email = $"{firstNames[i].ToLower()}.{lastNames[i].ToLower().Replace(" ", "")}@student.ac.za",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Student@1234"),
                Role = UserRole.Student
            };

            var student = new Student
            {
                UserId = user.Id,
                StudentNumber = $"S{220000 + i + 1}",
                Programme = programmes[i % programmes.Length],
                YearOfStudy = random.Next(1, 5)
            };

            context.Users.Add(user);
            context.Students.Add(student);
            students.Add((user, student));
        }

        await context.SaveChangesAsync();


        var enrollments = new List<ModuleEnrollment>();

        foreach (var (_, student) in students)
        {
            var profile = random.NextDouble() switch
            {
                < 0.50 => "low",
                < 0.80 => "medium",
                _ => "high"
            };
            var moduleCount = random.Next(4, 7);
            var shuffled = modules.OrderBy(_ => random.Next()).Take(moduleCount).ToList();

            foreach (var module in shuffled)
            {
                double attendance, assignAvg, testAvg;
                int lmsLogins;

                if (profile == "low")
                {
                    attendance = Clamp(GaussianRandom(random, 82, 8), 60, 100);
                    assignAvg = Clamp(GaussianRandom(random, 74, 8), 50, 100);
                    testAvg = Clamp(GaussianRandom(random, 72, 9), 50, 100);
                    lmsLogins = (int)Clamp(GaussianRandom(random, 65, 15), 30, 120);
                }
                else if (profile == "medium")
                {
                    attendance = Clamp(GaussianRandom(random, 65, 10), 40, 85);
                    assignAvg = Clamp(GaussianRandom(random, 58, 10), 35, 75);
                    testAvg = Clamp(GaussianRandom(random, 55, 11), 30, 72);
                    lmsLogins = (int)Clamp(GaussianRandom(random, 35, 12), 10, 70);
                }
                else 
                {
                    attendance = Clamp(GaussianRandom(random, 42, 12), 0, 65);
                    assignAvg = Clamp(GaussianRandom(random, 40, 12), 0, 60);
                    testAvg = Clamp(GaussianRandom(random, 37, 13), 0, 58);
                    lmsLogins = (int)Clamp(GaussianRandom(random, 14, 8), 0, 40);
                }

                enrollments.Add(new ModuleEnrollment
                {
                    StudentId = student.Id,
                    ModuleId = module.Id,
                    Year = 2026,
                    Semester = random.Next(2) == 0 ? "Semester 1" : "Semester 2",
                    AttendancePercentage = Math.Round(attendance, 2),
                    AssignmentAverage = Math.Round(assignAvg, 2),
                    TestAverage = Math.Round(testAvg, 2),
                    LmsLoginCount = lmsLogins,
                    Status = EnrollmentStatus.Active
                });
            }
        }

        context.ModuleEnrollments.AddRange(enrollments);
        await context.SaveChangesAsync();

        Console.WriteLine($"✓ Seed complete: 50 students, 32 modules, {enrollments.Count} enrollments");
    }


    private static double GaussianRandom(Random rng, double mean, double stdDev)
    {
        double u1 = 1.0 - rng.NextDouble();
        double u2 = 1.0 - rng.NextDouble();
        double z = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
        return mean + stdDev * z;
    }

    private static double Clamp(double value, double min, double max)
        => Math.Max(min, Math.Min(max, value));
}
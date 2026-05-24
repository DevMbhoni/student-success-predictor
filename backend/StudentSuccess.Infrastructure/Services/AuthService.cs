using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using StudentSuccess.Application.DTOs.Auth;
using StudentSuccess.Application.Interfaces;
using StudentSuccess.Domain.Entities;
using StudentSuccess.Domain.Enums;
using StudentSuccess.Infrastructure.Data;

namespace StudentSuccess.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _config;

    public AuthService(ApplicationDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email.ToLower());

        if (existingUser is not null)
            throw new InvalidOperationException("A user with this email already exists.");

        var user = new User
        {
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            Email = request.Email.ToLower().Trim(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = request.Role
        };

        _context.Users.Add(user);

        if (request.Role == UserRole.Student)
        {
            if (string.IsNullOrWhiteSpace(request.StudentNumber))
                throw new InvalidOperationException("Student number is required for student registration.");

            var student = new Student
            {
                UserId = user.Id,
                StudentNumber = request.StudentNumber.Trim().ToUpper(),
                Programme = request.Programme ?? string.Empty,
                YearOfStudy = request.YearOfStudy ?? 1
            };

            _context.Students.Add(student);
        }

        await _context.SaveChangesAsync();

        return BuildAuthResponse(user);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email.ToLower());


        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid email or password.");

        if (!user.IsActive)
            throw new UnauthorizedAccessException("This account has been deactivated.");

        return BuildAuthResponse(user);
    }

    private AuthResponse BuildAuthResponse(User user)
    {
        var jwt = _config.GetSection("JwtSettings");
        var secretKey = jwt["SecretKey"]!;
        var issuer = jwt["Issuer"]!;
        var audience = jwt["Audience"]!;
        var expiryHours = int.Parse(jwt["ExpiryHours"]!);
        var expiresAt = DateTime.UtcNow.AddHours(expiryHours);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email,          user.Email),
            new Claim(ClaimTypes.Role,           user.Role.ToString()),
            new Claim(ClaimTypes.GivenName,      user.FirstName),
            new Claim(ClaimTypes.Surname,        user.LastName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) 
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: creds
        );

        return new AuthResponse
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            Email = user.Email,
            FullName = $"{user.FirstName} {user.LastName}",
            Role = user.Role.ToString(),
            UserId = user.Id,
            ExpiresAt = expiresAt
        };
    }
}
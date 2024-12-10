using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using API_Modul295.Models;

namespace API_Modul295.Services
{
    public class AuthService
    {
        private readonly IConfiguration _configuration;

        public AuthService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public bool VerifyPassword(string inputPassword, string storedPassword)
        {
            // Vergleiche das eingegebene Passwort direkt mit dem gespeicherten Passwort
            return inputPassword == storedPassword;
        }

        public string GenerateJwtToken(Employee employee)
        {
            // Temporäres Logging zur Überprüfung der Konfiguration
            Console.WriteLine($"JWT Key: {_configuration["Jwt:Key"]}");
            Console.WriteLine($"JWT Issuer: {_configuration["Jwt:Issuer"]}");
            Console.WriteLine($"JWT Audience: {_configuration["Jwt:Audience"]}");

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, employee.Username),
                new Claim(ClaimTypes.Role, employee.IsAdmin ? "Admin" : "Employee"),
                new Claim("EmployeeID", employee.EmployeeID.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
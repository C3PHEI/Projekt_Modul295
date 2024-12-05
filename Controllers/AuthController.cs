using API_Modul295.Data;
using API_Modul295.Services;
using API_Modul295.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace API_Modul295.Controllers
{
    /// <summary>
    /// Controller for employee authentication and login functionality.
    /// </summary>
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly ApiDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(ApiDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        /// <summary>
        /// Authenticates an employee and returns a JWT token.
        /// </summary>
        /// <param name="request">The login request containing username and password.</param>
        /// <returns>A JWT token upon successful login.</returns>
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var employee = _context.Employees.SingleOrDefault(e => e.Username == request.Username);

            // Check if the employee exists and the password is correct
            if (employee == null || !VerifyPassword(request.Password, employee.PasswordHash))
            {
                return Unauthorized("Invalid username or password.");
            }

            // Check if the account is locked
            if (employee.IsLocked)
            {
                return Unauthorized("Account is locked.");
            }

            // Generate JWT token for the employee
            var token = GenerateJwtToken(employee);
            return Ok(new { Token = token });
        }

        /// <summary>
        /// Verifies if the provided password matches the stored hash.
        /// </summary>
        /// <param name="password">The password to verify.</param>
        /// <param name="storedHash">The stored hash of the password.</param>
        /// <returns>True if the password is correct, false otherwise.</returns>
        private bool VerifyPassword(string password, byte[] storedHash)
        {
            var hash = SHA256.HashData(Encoding.UTF8.GetBytes(password));
            return hash.SequenceEqual(storedHash);
        }

        /// <summary>
        /// Generates a JWT token for the authenticated employee.
        /// </summary>
        /// <param name="employee">The authenticated employee.</param>
        /// <returns>The generated JWT token.</returns>
        private string GenerateJwtToken(Employee employee)
        {
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
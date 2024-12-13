using API_Modul295.Data;
using API_Modul295.Services;
using API_Modul295.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Collections.Generic;
using System.Security.Claims;

namespace API_Modul295.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly ApiDbContext _context;
        private readonly AuthService _authService;

        public AuthController(ApiDbContext context, AuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var employee = _context.Employees.SingleOrDefault(e => e.Username == request.Username);

            if (employee == null || !_authService.VerifyPassword(request.Password, employee.Password))
            {
                return Unauthorized("Invalid username or password.");
            }

            if (employee.IsLocked)
            {
                return Unauthorized("Account is locked.");
            }

            var token = _authService.GenerateJwtToken(employee);
            return Ok(new { Token = token });
        }

        [HttpGet("me")]
        [Authorize]
        public IActionResult GetCurrentUser()
        {
            var username = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            var role = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            var employeeId = User.Claims.FirstOrDefault(c => c.Type == "EmployeeID")?.Value;

            return Ok(new {
                userName = username,
                role = role,
                employeeID = employeeId
            });
        }

        [HttpGet("employees")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetAllEmployees()
        {
            // Alle Mitarbeiter laden
            var employees = _context.Employees.ToList();

            var result = employees.Select(e => new {
                employeeID = e.EmployeeID,
                username = e.Username,
                isAdmin = e.IsAdmin,
                isLocked = e.IsLocked
            });

            return Ok(result);
        }

        [HttpPut("unlock/{employeeId}")]
        [Authorize(Roles = "Admin")]
        public IActionResult UnlockEmployee(int employeeId)
        {
            var employee = _context.Employees.SingleOrDefault(e => e.EmployeeID == employeeId);
            if (employee == null)
            {
                return NotFound("Mitarbeiter nicht gefunden.");
            }

            if (!employee.IsLocked)
            {
                return BadRequest("Mitarbeiter ist bereits entsperrt.");
            }

            // Mitarbeiter entsperren
            employee.IsLocked = false;
            _context.SaveChanges();

            return Ok(new { message = "Mitarbeiter wurde entsperrt." });
        }
    }
}
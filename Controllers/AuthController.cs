using API_Modul295.Data;
using API_Modul295.Services;
using API_Modul295.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

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
    }
}
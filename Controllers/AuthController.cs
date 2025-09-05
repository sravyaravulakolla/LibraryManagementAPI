using LibraryManagementAPI.Models;
using LibraryManagementAPI.Models.DTOs;
using LibraryManagementAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LibraryManagementAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] Register model)
        {
            var result = await _authService.RegisterAsync(model);
            if (result == null) return BadRequest("Registration failed");
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] Login model)
        {
            var (token, expiration) = await _authService.LoginAsync(model);
            if (token == null) return Unauthorized("Invalid credentials");

            return Ok(new { token, expiration });
        }

        [HttpPost]
        [Authorize(Roles = "LibraryManager")] // ✅ Only LibraryManager can assign roles
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleDto model)
        {
            // Get current logged-in user's role
            var currentUserRole = User.FindFirstValue(ClaimTypes.Role);

            var result = await _authService.AssignRoleAsync(model, currentUserRole);
            if (result == null) return BadRequest("Failed to assign role.");

            return Ok(result);
        }
    }
}

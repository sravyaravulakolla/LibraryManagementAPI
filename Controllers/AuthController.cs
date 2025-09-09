using LibraryManagementAPI.Models;
using LibraryManagementAPI.Models.DTOs;
using LibraryManagementAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LibraryManagementAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [EnableCors]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly UserManager<ApplicationUser> _userManager;

        public AuthController(IAuthService authService, UserManager<ApplicationUser> userManager)
        {
            _authService = authService;
            _userManager = userManager;
        }

        [HttpPost]
        
            public async Task<IActionResult> Register([FromBody] Register model)
            {
                var result = await _authService.RegisterAsync(model);
                if (result == null)
                    return BadRequest(new { message = "Registration failed" });

                return Ok(new { message = "Registration successful", user = result });
            }

        

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] Login model)
        {
            var (token, expiration) = await _authService.LoginAsync(model);

            if (token == null)
                return Unauthorized("Invalid credentials");

            var user = await _userManager.FindByEmailAsync(model.Email);
            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault() ?? "Borrower";  // fallback if no role

            return Ok(new
            {
                token,
                expiration,
                role,
                userName = user.FullName
            });
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

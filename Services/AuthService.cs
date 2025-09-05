using LibraryManagementAPI.Controllers;
using LibraryManagementAPI.Models;
using LibraryManagementAPI.Models.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LibraryManagementAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        public async Task<string?> RegisterAsync(Register model)
        {
            var userExists = await _userManager.FindByEmailAsync(model.Email);
            if (userExists != null) return "User already exists!";

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return string.Join(", ", result.Errors.Select(e => e.Description));

            // default to "Customer" if role not passed
            var role = string.IsNullOrEmpty(model.Role) ? "Customer" : model.Role;

            if (!await _roleManager.RoleExistsAsync(role))
                await _roleManager.CreateAsync(new IdentityRole(role));

            await _userManager.AddToRoleAsync(user, role);

            return "User registered successfully!";
        }

        public async Task<(string? token, DateTime? expiration)> LoginAsync(Login model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
                return (null, null);

            var roles = await _userManager.GetRolesAsync(user);

            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            authClaims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var token = GetToken(authClaims);
            return (new JwtSecurityTokenHandler().WriteToken(token), token.ValidTo);
        }

        public async Task<string?> AssignRoleAsync(AssignRoleDto model, string currentUserRole)
        {
            if (currentUserRole != "LibraryManager")
                return "Only LibraryManager can assign roles.";

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null) return "User not found";

            if (!await _roleManager.RoleExistsAsync(model.Role))
                await _roleManager.CreateAsync(new IdentityRole(model.Role));

            await _userManager.AddToRoleAsync(user, model.Role);
            return $"Role '{model.Role}' assigned to {user.Email}";
        }

        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]));

            return new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.UtcNow.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );
        }
    }
}

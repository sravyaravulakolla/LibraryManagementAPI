using LibraryManagementAPI.Models;
using LibraryManagementAPI.Models.DTOs;
using LibraryManagementAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LibraryManagementAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(Roles = "LibraryManager,Admin")]  // Only Admin and LibraryManager can manage users
    public class UsersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly LibMgmtDbContext _context;

        public UsersController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            LibMgmtDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> AddUser([FromBody] Register model)
        {
            var userExists = await _userManager.FindByEmailAsync(model.Email);
            if (userExists != null)
                return BadRequest(new { success = false, message = "User already exists." });

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return BadRequest(new { success = false, message = string.Join(", ", result.Errors.Select(e => e.Description)) });


            var role = string.IsNullOrEmpty(model.Role) ? "Borrower" : model.Role;
            if (!await _roleManager.RoleExistsAsync(role))
                await _roleManager.CreateAsync(new IdentityRole(role));

            await _userManager.AddToRoleAsync(user, role);

            return Ok(new { success = true, message = "User added successfully." });
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userManager.Users.ToListAsync();
            var result = new List<object>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var isBorrower = roles.Contains("Borrower");

                var borrowedBooks = new List<object>();
                if (isBorrower)
                {
                    borrowedBooks = await _context.BorrowingRecords
                        .Include(r => r.Book)
                        .Where(r => r.UserId == user.Id && r.ReturnedDate == null)
                        .Select(r => new { r.Book.BookId, r.Book.Title, r.Book.Author })
                        .ToListAsync<object>();
                }

                result.Add(new
                {
                    user.Id,
                    user.FullName,
                    user.Email,
                    user.UserName,
                    user.CreatedAt,
                    roles = roles,
                    borrowedBooks = borrowedBooks
                });
            }

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound("User not found.");

            var roles = await _userManager.GetRolesAsync(user);
            var isBorrower = roles.Contains("Borrower");

            var borrowedBooks = new List<object>();
            if (isBorrower)
            {
                borrowedBooks = await _context.BorrowingRecords
                    .Include(r => r.Book)
                    .Where(r => r.UserId == user.Id && r.ReturnedDate == null)
                    .Select(r => new { r.Book.BookId, r.Book.Title, r.Book.Author })
                    .ToListAsync<object>();
            }

            return Ok(new
            {
                user.Id,
                user.FullName,
                user.Email,
                user.UserName,
                user.CreatedAt,
                roles = roles,
                borrowedBooks = borrowedBooks
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditUser(string id, [FromBody] EditUserDto model)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound(new { success = false, message = "User not found." });


            user.FullName = model.FullName ?? user.FullName;
            user.Email = model.Email ?? user.Email;
            user.UserName = model.Email ?? user.UserName;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
                return BadRequest(new { success = false, message = string.Join(", ", result.Errors.Select(e => e.Description)) });


            return Ok(new { success = true, message = "User updated successfully." });

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound(new { success = false, message = "User not found." });


            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
                return BadRequest(new { success = false, message = string.Join(", ", result.Errors.Select(e => e.Description)) });

            return Ok(new { success = true, message = "User deleted successfully." });
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> ChangeUserRole(string id, [FromBody] ChangeRoleDto model)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound("User not found.");

            var currentRoles = await _userManager.GetRolesAsync(user);

            await _userManager.RemoveFromRolesAsync(user, currentRoles);

            if (!await _roleManager.RoleExistsAsync(model.NewRole))
                await _roleManager.CreateAsync(new IdentityRole(model.NewRole));

            await _userManager.AddToRoleAsync(user, model.NewRole);

            return Ok($"User role changed to '{model.NewRole}'.");
        }
    }
}

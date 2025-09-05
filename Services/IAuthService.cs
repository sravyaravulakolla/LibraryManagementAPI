using LibraryManagementAPI.Controllers;
using LibraryManagementAPI.Models;
using LibraryManagementAPI.Models.DTOs;

namespace LibraryManagementAPI.Services
{
    public interface IAuthService
    {
        Task<string?> RegisterAsync(Register model);
        Task<(string? token, DateTime? expiration)> LoginAsync(Login model);
        Task<string?> AssignRoleAsync(AssignRoleDto model, string currentUserRole);
    }
}
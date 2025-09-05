using Microsoft.AspNetCore.Identity;

namespace LibraryManagementAPI.Models
{
    public class ApplicationUser : IdentityUser
    {
        // Extra fields you want for all users
        public string FullName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

         }
}

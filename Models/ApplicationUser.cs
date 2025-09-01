using Microsoft.AspNetCore.Identity;

namespace LibraryManagementAPI.Models
{
    public class ApplicationUser : IdentityUser
    {
        // Extra fields you want for all users
        public string FullName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Role-based separation (optional, since IdentityRole handles this too)
        // But useful if you want to directly identify type
        public string UserType { get; set; } = string.Empty;  // e.g., "LibraryManager", "Donor", "Customer"
    }
}

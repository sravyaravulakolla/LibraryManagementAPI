using LibraryManagementAPI.Models;
using Microsoft.AspNetCore.Identity;

namespace LibraryManagementAPI.Services
{
    public class RoleSeeder
    {
        public static async Task SeedRolesAndAdminAsync(RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager)
        {
            string[] roleNames = { "Admin", "Librarian", "Samaritan", "Borrower" };

            foreach (var role in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            var adminEmail = "admin@library.com";
            var adminPassword = "Admin@123"; // strong password

            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "Default Admin",
                    CreatedAt = DateTime.UtcNow,
                    EmailConfirmed = true
                };

                var createAdmin = await userManager.CreateAsync(adminUser, adminPassword);
                if (createAdmin.Succeeded)
                {
                    // Add admin to the "Admin" role
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }
    }
}

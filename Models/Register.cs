using System.ComponentModel.DataAnnotations;

namespace LibraryManagementAPI.Models
{
    public class Register
    {
        [Required(ErrorMessage = "FullName is required")]
        public string? FullName { get; set; }
        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string? Email { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }
        public string? Role { get; set; } = "Customer";
    }
}

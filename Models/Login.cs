using System.ComponentModel.DataAnnotations;

namespace LibraryManagementAPI.Models
{
    public class Login
    {
        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string? Email { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace LibraryManagementAPI.Models.DTOs
{
    public class BookDTO
    {
        public int BookId { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters.")]
        public string Title { get; set; } = null!;

        [Required(ErrorMessage = "Author is required.")]
        [StringLength(100, ErrorMessage = "Author name cannot exceed 100 characters.")]
        public string Author { get; set; } = null!;

        [Required(ErrorMessage = "CategoryId is required.")]
        public int CategoryId {  get; set; }
        [StringLength(100, ErrorMessage = "CategoryName cannot exceed 100 characters.")]
        public string? CategoryName { get; set; } = null!;

        [Required(ErrorMessage = "Price is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
        public decimal Price { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "AvailableCopies cannot be negative.")]
        public int AvailableCopies { get; set; } = 1;

        [Url(ErrorMessage = "ImageUrl must be a valid URL.")]
        public string? ImageUrl { get; set; }

        // Optional: You can include library info if you want to show it in frontend
        //public decimal Rating { get; set; } = 2.5m;
        public string? LibraryName { get; set; }
    }
}

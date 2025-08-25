namespace LibraryManagementAPI.Models.DTOs
{
    public class BookDTO
    {
        public int BookId { get; set; }
        public string Title { get; set; } = null!;
        public string Author { get; set; } = null!;
        public string CategoryName { get; set; } = null!;
        public decimal Price {  get; set; } 
        public List<LibraryAvailabilityDto> Libraries { get; set; } = new();
    }
}

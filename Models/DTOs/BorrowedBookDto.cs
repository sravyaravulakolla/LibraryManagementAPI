namespace LibraryManagementAPI.Models.DTOs
{
    public class BorrowedBookDto
    {
        public int BorrowingRecordId { get; set; } // <-- add this
        public int BookId { get; set; }
        public string Title { get; set; } = null!;
        public string Author { get; set; } = null!;
        public string? CategoryName { get; set; } // optional
        public DateTime BorrowedDate { get; set; }
        public DateTime? ReturnedDate { get; set; }
    }


}

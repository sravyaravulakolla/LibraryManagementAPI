using LibraryManagementAPI.Models;

public class BorrowingRecord
{
    public int BorrowingRecordId { get; set; }
    public string UserId { get; set; }
    public int BookId { get; set; }
    public DateTime BorrowedDate { get; set; }
    public DateTime? ReturnedDate { get; set; }  // Null if not returned yet

    public virtual ApplicationUser User { get; set; } = null!;
    public virtual Book Book { get; set; } = null!;
}

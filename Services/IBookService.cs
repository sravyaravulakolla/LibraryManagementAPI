using LibraryManagementAPI.Models.DTOs;

public interface IBookService
{
    Task<IEnumerable<BookDTO>> GetAllBooksAsync();
    Task<BookDTO?> GetBookByIdAsync(int bookId);
    Task<BookDTO> AddBookAsync(BookDTO book);
    Task<IEnumerable<BookDTO>> AddBooksBulkAsync(IEnumerable<BookDTO> books);  // Bulk Insert
    Task<BookDTO> UpdateBookAsync(int id, BookDTO book);
    Task DeleteBookAsync(int bookId);
}

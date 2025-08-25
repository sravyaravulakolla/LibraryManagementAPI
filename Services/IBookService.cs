using LibraryManagementAPI.Models;
using LibraryManagementAPI.Models.DTOs;

namespace LibraryManagementAPI.Services
{

    public interface IBookService
    {
        Task<IEnumerable<BookDTO>> GetAllBooksAsync();
        Task<BookDTO?> GetBookByIdAsync(int bookId);
        Task<Book> AddBookAsync(BookDTO book);
        Task<Book> UpdateBookAsync(BookDTO book);
        Task DeleteBookAsync(int bookId);
    }

}


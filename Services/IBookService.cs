using LibraryManagementAPI.Models;
using LibraryManagementAPI.Models.DTOs;

namespace LibraryManagementAPI.Services
{

    public interface IBookService
    {
        Task<IEnumerable<BookDTO>> GetAllBooksAsync();
        Task<BookDTO?> GetBookByIdAsync(int bookId);
        Task<BookDTO> AddBookAsync(BookDTO book);
        Task<BookDTO> UpdateBookAsync(BookDTO book);
        Task DeleteBookAsync(int bookId);
    }

}


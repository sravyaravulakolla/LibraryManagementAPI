using LibraryManagementAPI.Models.DTOs;
using LibraryManagementAPI.Models;

namespace LibraryManagementAPI.Services
{
    public interface ILibraryService
    {
        Task<IEnumerable<LibraryDTO>> GetAllLibrariesAsync();
        Task<LibraryDTO?> GetLibraryByIdAsync(int id);
        Task<Library> AddLibraryAsync(Library library);
        Task<Library?> UpdateLibraryAsync(int id, Library library);
        Task<bool> DeleteLibraryAsync(int id);
        Task<IEnumerable<BookDTO>> GetBooksInLibraryAsync(int libraryId);


        // Extra functionality
        //Task<IEnumerable<LibraryAvailabilityDto>> GetBookAvailabilityAsync(int bookId);

    }
}

using LibraryManagementAPI.Models.DTOs;
using LibraryManagementAPI.Models;

namespace LibraryManagementAPI.Services
{
    public interface ILibraryService
    {
        Task<IEnumerable<Library>> GetAllLibrariesAsync();
        Task<LibraryDTO?> GetLibraryByIdAsync(int id);
        Task<Library> AddLibraryAsync(Library library);
        Task<Library?> UpdateLibraryAsync(int id, Library library);
        Task<bool> DeleteLibraryAsync(int id);

        // Extra functionality
        Task<IEnumerable<LibraryAvailabilityDto>> GetBookAvailabilityAsync(int bookId);

    }
}

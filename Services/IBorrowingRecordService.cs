using LibraryManagementAPI.Models.DTOs;

namespace LibraryManagementAPI.Services
{
    public interface IBorrowingRecordService
    {
        Task<IEnumerable<BorrowedBookDto>> GetAllBorrowedBooksAsync(string userId);
        Task<IEnumerable<BorrowedBookDto>> GetCurrentlyBorrowedBooksAsync(string userId);
        public Task<bool> ReturnBookAsync(int borrowingRecordId, string userId);
    }

}

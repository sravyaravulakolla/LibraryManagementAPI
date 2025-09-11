using Microsoft.EntityFrameworkCore;
using LibraryManagementAPI.Models;
using LibraryManagementAPI.Models.DTOs;

namespace LibraryManagementAPI.Services
{
    public class BorrowingRecordService : IBorrowingRecordService
    {
        private readonly LibMgmtDbContext _context;

        public BorrowingRecordService(LibMgmtDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BorrowedBookDto>> GetAllBorrowedBooksAsync(string userId)
        {
            return await _context.BorrowingRecords
                .Where(r => r.UserId == userId)
                .Include(r => r.Book)
                .Select(r => new BorrowedBookDto
                {
                    BorrowingRecordId = r.BorrowingRecordId, // <-- add this
                    BookId = r.BookId,
                    Title = r.Book.Title,
                    Author = r.Book.Author,
                    CategoryName = r.Book.Category.Name,
                    BorrowedDate = r.BorrowedDate,
                    ReturnedDate = r.ReturnedDate
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<BorrowedBookDto>> GetCurrentlyBorrowedBooksAsync(string userId)
        {
            return await _context.BorrowingRecords
                .Where(r => r.UserId == userId && r.ReturnedDate == null)
                .Include(r => r.Book)
                .Select(r => new BorrowedBookDto
                {
                    BorrowingRecordId = r.BorrowingRecordId, // <-- add this
                    BookId = r.BookId,
                    Title = r.Book.Title,
                    Author = r.Book.Author,
                    CategoryName = r.Book.Category.Name, // optional but consistent
                    BorrowedDate = r.BorrowedDate,
                    ReturnedDate = r.ReturnedDate
                })
                .ToListAsync();
        }

        public async Task<bool> ReturnBookAsync(int borrowingRecordId, string userId)
        {
            var record = await _context.BorrowingRecords
                .FirstOrDefaultAsync(r => r.BorrowingRecordId == borrowingRecordId && r.UserId == userId && r.ReturnedDate == null);

            if (record == null) return false;

            record.ReturnedDate = DateTime.UtcNow;

            var book = await _context.Books.FindAsync(record.BookId);
            if (book != null)
            {
                book.AvailableCopies += 1;
            }

            await _context.SaveChangesAsync();
            return true;
        }

    }
}

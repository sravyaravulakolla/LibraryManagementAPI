using LibraryManagementAPI.Models.DTOs;
using LibraryManagementAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementAPI.Services
{
    public class LibraryService:ILibraryService
    {
        private readonly LibMgmtDbContext _context;

        public LibraryService(LibMgmtDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Library>> GetAllLibrariesAsync()
        {
            return await _context.Libraries.ToListAsync();
        }

        public async Task<LibraryDTO?> GetLibraryByIdAsync(int id)
        {
            var library = await _context.Libraries
                .Include(l => l.LibraryBooks)
                    .ThenInclude(lb => lb.Book)
                .FirstOrDefaultAsync(l => l.LibraryId == id);

            if (library == null) return null;

            return new LibraryDTO
            {
                LibraryId = library.LibraryId,
                Name = library.Name,
                Address = library.Address,
                MaximumCapacity = library.MaximumCapacity,
                Books = library.LibraryBooks.Select(lb => new LibraryAvailabilityDto
                {
                    LibraryId = lb.LibraryId,
                    LibraryName = library.Name,
                    AvailableCopies = lb.AvailableCopies
                }).ToList()
            };
        }

        public async Task<Library> AddLibraryAsync(Library library)
        {
            library.CreatedDate = DateTime.UtcNow;
            library.UpdatedDate = DateTime.UtcNow;

            _context.Libraries.Add(library);
            await _context.SaveChangesAsync();
            return library;
        }

        public async Task<Library?> UpdateLibraryAsync(int id, Library library)
        {
            var existing = await _context.Libraries.FindAsync(id);
            if (existing == null)
                return null;

            existing.Name = library.Name;
            existing.Address = library.Address;
            existing.MaximumCapacity = library.MaximumCapacity;
            existing.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteLibraryAsync(int id)
        {
            var existing = await _context.Libraries.FindAsync(id);
            if (existing == null)
                return false;

            _context.Libraries.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<LibraryAvailabilityDto>> GetBookAvailabilityAsync(int bookId)
        {
            return await _context.LibraryBooks
                .Where(lb => lb.BookId == bookId)
                .Select(lb => new LibraryAvailabilityDto
                {
                    LibraryId = lb.LibraryId,
                    LibraryName = lb.Library.Name,
                    AvailableCopies = lb.AvailableCopies
                })
                .ToListAsync();
        }
    }
}

using LibraryManagementAPI.Models.DTOs;
using LibraryManagementAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementAPI.Services
{
    public class LibraryService : ILibraryService
    {
        private readonly LibMgmtDbContext _context;

        public LibraryService(LibMgmtDbContext context)
        {
            _context = context;
        }

        // Get all libraries
        public async Task<IEnumerable<LibraryDTO>> GetAllLibrariesAsync()
        {
            var libraries = await _context.Libraries
                .Include(l => l.Categories)
                    .ThenInclude(c => c.Books)
                .ToListAsync();

            return libraries.Select(library => new LibraryDTO
            {
                LibraryId = library.LibraryId,
                Name = library.Name,
                Address = library.Address,
                MaximumCapacity = library.MaximumCapacity,
                Categories = library.Categories.Select(c => new CategoryDto
                {
                    CategoryId = c.CategoryId,
                    Name = c.Name
                }).ToList(),
                Books = library.Categories
                    .SelectMany(c => c.Books)
                    .Select(b => new BookDTO
                    {
                        BookId = b.BookId,
                        Title = b.Title,
                        Author = b.Author,
                        CategoryName = b.Category.Name,  // Will work because of fix-up
                        Price = b.Price
                    }).ToList()
            }).ToList();
        }



        // Get library by ID along with its books (via categories)
        public async Task<LibraryDTO?> GetLibraryByIdAsync(int id)
        {
            var library = await _context.Libraries
                .Include(l => l.Categories)
                    .ThenInclude(c => c.Books)
                .FirstOrDefaultAsync(l => l.LibraryId == id);

            if (library == null) return null;

            var categories = library.Categories
                .Select(c => new CategoryDto
                {
                    CategoryId = c.CategoryId,
                    Name = c.Name,
                    LibraryId = library.LibraryId,
                    LibraryName = library.Name
                }).ToList();

            var books = library.Categories
                .SelectMany(c => c.Books)
                .Select(b => new BookDTO
                {
                    BookId = b.BookId,
                    Title = b.Title,
                    Author = b.Author,
                    CategoryName = b.Category.Name,
                    Price = b.Price,
                    AvailableCopies = b.AvailableCopies,
                    ImageUrl = b.ImageUrl,
                    LibraryName = library.Name
                })
                .ToList();

            return new LibraryDTO
            {
                LibraryId = library.LibraryId,
                Name = library.Name,
                Address = library.Address,
                MaximumCapacity = library.MaximumCapacity,
                Categories = categories,
                Books = books
            };
        }


        // Add a new library
        public async Task<Library> AddLibraryAsync(Library library)
        {
            library.CreatedDate = DateTime.UtcNow;
            library.UpdatedDate = DateTime.UtcNow;

            _context.Libraries.Add(library);
            await _context.SaveChangesAsync();
            return library;
        }

        // Update existing library
        public async Task<Library?> UpdateLibraryAsync(int id, Library library)
        {
            var existing = await _context.Libraries.FindAsync(id);
            if (existing == null) return null;

            existing.Name = library.Name;
            existing.Address = library.Address;
            existing.MaximumCapacity = library.MaximumCapacity;
            existing.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return existing;
        }

        // Delete a library
        public async Task<bool> DeleteLibraryAsync(int id)
        {
            var existing = await _context.Libraries.FindAsync(id);
            if (existing == null) return false;

            _context.Libraries.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }

        // Get all books in a specific library
        public async Task<IEnumerable<BookDTO>> GetBooksInLibraryAsync(int libraryId)
        {
            var library = await _context.Libraries
                .Include(l => l.Categories)
                    .ThenInclude(c => c.Books)
                        .ThenInclude(b => b.Category) // Ensure Category is loaded
                .FirstOrDefaultAsync(l => l.LibraryId == libraryId);

            if (library == null) return new List<BookDTO>();

            return library.Categories
                .SelectMany(c => c.Books)
                .Select(b => new BookDTO
                {
                    BookId = b.BookId,
                    Title = b.Title,
                    Author = b.Author,
                    CategoryName = b.Category.Name,
                    Price = b.Price
                }).ToList();
        }
    }
}

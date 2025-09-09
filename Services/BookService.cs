using LibraryManagementAPI.Models;
using LibraryManagementAPI.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementAPI.Services
{
    public class BookService : IBookService
    {
        private readonly LibMgmtDbContext _context;

        public BookService(LibMgmtDbContext context)
        {
            _context = context;
        }

        // Bulk insert
       

        // Get all books
        public async Task<IEnumerable<BookDTO>> GetAllBooksAsync()
        {
            return await _context.Books
                .Include(b => b.Category)
                .ThenInclude(c => c.Library)
                .Select(b => new BookDTO
                {
                    BookId = b.BookId,
                    Title = b.Title,
                    Author = b.Author,
                    ImageUrl = b.ImageUrl,
                    CategoryName = b.Category.Name,
                    Price = b.Price,
                    LibraryName = b.Category.Library.Name,
                    AvailableCopies= b.AvailableCopies
                })
                .ToListAsync();
        }

        // Get book by ID
        public async Task<BookDTO?> GetBookByIdAsync(int bookId)
        {
            var book = await _context.Books
                .Include(b => b.Category)
                .ThenInclude(c => c.Library)
                .FirstOrDefaultAsync(b => b.BookId == bookId);

            if (book == null) return null;

            return new BookDTO
            {
                BookId = book.BookId,
                Title = book.Title,
                Author = book.Author,
                CategoryName = book.Category.Name,
                ImageUrl=book.ImageUrl,
                Price = book.Price,
                LibraryName = book.Category.Library.Name,
                AvailableCopies = book.AvailableCopies
            };
        }

        // Add single book
        public async Task<BookDTO> AddBookAsync(BookDTO bookDto)
        {
            var category = await _context.Categories
                .Include(c => c.Library)
                .FirstOrDefaultAsync(c => c.CategoryId == bookDto.CategoryId);

            if (category == null)
                throw new Exception("Category does not exist. Please create the category first.");

            var existingBook = await _context.Books
                .FirstOrDefaultAsync(b => b.Title == bookDto.Title && b.Author == bookDto.Author
                    && b.CategoryId == category.CategoryId);

            Book book;

            if (existingBook != null)
            {
                existingBook.AvailableCopies += bookDto.AvailableCopies > 0 ? bookDto.AvailableCopies : 1;
                existingBook.UpdatedDate = DateTime.Now;
                book = existingBook;
            }
            else
            {
                book = new Book
                {
                    Title = bookDto.Title,
                    Author = bookDto.Author,
                    CategoryId = category.CategoryId,
                    Price = bookDto.Price,
                    AvailableCopies = bookDto.AvailableCopies > 0 ? bookDto.AvailableCopies : 1,
                    ImageUrl= bookDto.ImageUrl,
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now
                };

                _context.Books.Add(book);
            }

            await _context.SaveChangesAsync();

            return new BookDTO
            {
                BookId = book.BookId,
                Title = book.Title,
                Author = book.Author,
                CategoryName = category.Name,
                Price = book.Price,
                ImageUrl = book.ImageUrl,
                AvailableCopies = bookDto.AvailableCopies,
                LibraryName = category.Library?.Name
            };
        }
        public async Task<IEnumerable<BookDTO>> AddBooksBulkAsync(IEnumerable<BookDTO> bookDtos)
        {
            var addedBooks = new List<BookDTO>();

            foreach (var bookDto in bookDtos)
            {
                // Reuse single book add logic
                var addedBook = await AddBookAsync(bookDto);
                addedBooks.Add(addedBook);
            }

            return addedBooks;
        }


        // Update book
        public async Task<BookDTO> UpdateBookAsync(int id, BookDTO bookDto)
        {
            var book = await _context.Books.FirstOrDefaultAsync(b => b.BookId == id);
            if (book == null)
                throw new Exception($"Book with Id {id} not found.");

            var category = await _context.Categories
                .Include(c => c.Library)
                .FirstOrDefaultAsync(c => c.CategoryId == bookDto.CategoryId);

            if (category == null)
                throw new Exception($"Category  does not exist.");

            book.Title = bookDto.Title;
            book.Author = bookDto.Author;
            book.Price = bookDto.Price;
            book.ImageUrl = bookDto.ImageUrl;
            book.CategoryId = category.CategoryId;
            book.UpdatedDate = DateTime.Now;

            await _context.SaveChangesAsync();

            return new BookDTO
            {
                BookId = book.BookId,
                Title = book.Title,
                Author = book.Author,
                CategoryId= category.CategoryId,
                CategoryName = category.Name,
                ImageUrl= book.ImageUrl,
                Price = book.Price,
                AvailableCopies= book.AvailableCopies,
                LibraryName = category.Library?.Name
            };
        }

        // Delete book
        public async Task DeleteBookAsync(int bookId)
        {
            var book = await _context.Books
                .Include(b => b.Category)
                .FirstOrDefaultAsync(b => b.BookId == bookId);

            if (book == null)
                throw new Exception($"Book with ID {bookId} not found.");

            var category = book.Category;

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();

            if (category != null)
            {
                bool hasOtherBooks = await _context.Books.AnyAsync(b => b.CategoryId == category.CategoryId);
                if (!hasOtherBooks)
                {
                    _context.Categories.Remove(category);
                    await _context.SaveChangesAsync();
                }
            }
        }

    }
}

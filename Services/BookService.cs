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

        public async Task<IEnumerable<BookDTO>> GetAllBooksAsync()
        {
            return await _context.Books
                .Include(b => b.Category)
                .Include(b => b.LibraryBooks)
                    .ThenInclude(lb => lb.Library)
                .Select(b => new BookDTO
                {
                    BookId = b.BookId,
                    Title = b.Title,
                    Author = b.Author,
                    CategoryName = b.Category.Name,
                    Price = b.Price,
                    Libraries = b.LibraryBooks
                        .Select(lb => new LibraryAvailabilityDto
                        {
                            LibraryId = lb.Library.LibraryId,
                            LibraryName = lb.Library.Name,
                            AvailableCopies = lb.AvailableCopies
                        }).ToList()
                })
                .ToListAsync();
        }

        public async Task<BookDTO?> GetBookByIdAsync(int bookId)
        {
            return await _context.Books
             .Include(b => b.Category)
             .Include(b => b.LibraryBooks)
                 .ThenInclude(lb => lb.Library)
             .Where(b => b.BookId == bookId)
             .Select(b => new BookDTO
             {
                 BookId = b.BookId,
                 Title = b.Title,
                 Author = b.Author,
                 CategoryName = b.Category.Name,
                 Price = b.Price,
                 Libraries = b.LibraryBooks
                     .Select(lb => new LibraryAvailabilityDto
                     {
                         LibraryId = lb.Library.LibraryId,
                         LibraryName = lb.Library.Name,
                         AvailableCopies = lb.AvailableCopies
                     }).ToList()
             })
             .FirstOrDefaultAsync();
        }
        public async Task<Book> AddBookAsync(BookDTO bookDto)
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Name == bookDto.CategoryName);

            if (category == null)
            {
                category = new Category { Name = bookDto.CategoryName };
                _context.Categories.Add(category);
                await _context.SaveChangesAsync();
            }

            Book book;

            var existingBook = await _context.Books
                .FirstOrDefaultAsync(b => b.Title == bookDto.Title && b.Author == bookDto.Author && b.CategoryId == category.CategoryId);

            if (existingBook != null)
            {
                existingBook.AvailableCopies++;
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
                    AvailableCopies = 1,
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now
                };

                _context.Books.Add(book);
            }

            await _context.SaveChangesAsync();
            return book; 
        }

        public async Task<Book> UpdateBookAsync(BookDTO bookDto)
        {
            var book = await _context.Books
                .FirstOrDefaultAsync(b => b.BookId == bookDto.BookId);

            if (book == null)
                throw new Exception($"Book with Id {bookDto.BookId} not found.");

            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Name == bookDto.CategoryName);

            if (category == null)
            {
                category = new Category { Name = bookDto.CategoryName };
                _context.Categories.Add(category);
                await _context.SaveChangesAsync();
            }


            book.Title = bookDto.Title;
            book.Author = bookDto.Author;
            book.Price = bookDto.Price;
            book.CategoryId = category.CategoryId;
            book.UpdatedDate = DateTime.Now;

            if (bookDto.Libraries != null && bookDto.Libraries.Any())
            {
                foreach (var libDto in bookDto.Libraries)
                {
                    var libraryBook = await _context.LibraryBooks
                        .FirstOrDefaultAsync(lb => lb.BookId == book.BookId && lb.LibraryId == libDto.LibraryId);

                    if (libraryBook != null)
                    {
                        libraryBook.AvailableCopies = libDto.AvailableCopies;
                    }
                    else
                    {
                        _context.LibraryBooks.Add(new LibraryBook
                        {
                            BookId = book.BookId,
                            LibraryId = libDto.LibraryId,
                            AvailableCopies = libDto.AvailableCopies
                        });
                    }
                }
            }

            await _context.SaveChangesAsync();
            return book;
        }


        public async Task DeleteBookAsync(int bookId)
        {
            var book = await _context.Books
                .Include(b => b.Category)
                .Include(b => b.LibraryBooks)
                .FirstOrDefaultAsync(b => b.BookId == bookId);

            if (book == null)
                throw new Exception($"Book with ID {bookId} not found.");

            if (book.LibraryBooks != null && book.LibraryBooks.Any())
            {
                _context.LibraryBooks.RemoveRange(book.LibraryBooks);
            }

            var category = book.Category;

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();

            if (category != null)
            {
                bool hasOtherBooks = await _context.Books
                    .AnyAsync(b => b.CategoryId == category.CategoryId);

                if (!hasOtherBooks)
                {
                    _context.Categories.Remove(category);
                    await _context.SaveChangesAsync();
                }
            }
        }

    }
}


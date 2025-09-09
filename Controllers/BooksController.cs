using LibraryManagementAPI.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Web.Http.ModelBinding;

[ApiController]
[Route("api/[controller]")]
[EnableCors]
//[Authorize]
public class BooksController : ControllerBase
{
    private readonly IBookService _bookService;

    public BooksController(IBookService bookService)
    {
        _bookService = bookService;
    }

    [HttpGet]
    //[Authorize(Roles = "Admin,Librarian,Samaritan,Borrower")]
    public async Task<IActionResult> GetAllBooks()
    {
        var books = await _bookService.GetAllBooksAsync();
        return Ok(books);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Librarian,Samaritan,Borrower")]
    public async Task<IActionResult> GetBookById(int id)
    {
        var book = await _bookService.GetBookByIdAsync(id);
        if (book == null)
            return NotFound($"Book with ID {id} not found.");

        return Ok(book);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Librarian,Samaritan")]
    public async Task<IActionResult> AddBook([FromBody] BookDTO bookDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var newBook = await _bookService.AddBookAsync(bookDto);
        return CreatedAtAction(nameof(GetBookById), new { id = newBook.BookId }, newBook);
    }

    [HttpPost("bulk")]
    [Authorize(Roles = "Admin,Librarian,Samaritan")]
    public async Task<IActionResult> BulkAddBooks([FromBody] List<BookDTO> booksDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var addedBooks = await _bookService.AddBooksBulkAsync(booksDto);
        return Ok(addedBooks);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Librarian")]
    public async Task<IActionResult> UpdateBook(int id, [FromBody] BookDTO bookDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var updatedBook = await _bookService.UpdateBookAsync(id, bookDto);
            return Ok(updatedBook);
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteBook(int id)
    {
        try
        {
            await _bookService.DeleteBookAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }
    //[HttpPost("{bookId}/rate")]
    //public async Task<IActionResult> RateBook(int bookId, [FromBody] decimal newRating)
    //{
    //    var book = await _context.Books.FindAsync(bookId);
    //    if (book == null) return NotFound();

    //    // Simple average logic:
    //    book.Rating = (book.Rating + newRating) / 2;
    //    book.UpdatedDate = DateTime.UtcNow;
    //    await _context.SaveChangesAsync();

    //    return Ok(new { message = "Rating updated successfully", updatedRating = book.Rating });
    //}

}

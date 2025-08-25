using LibraryManagementAPI.Models.DTOs;
using LibraryManagementAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly IBookService _bookService;

    public BooksController(IBookService bookService)
    {
        _bookService = bookService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllBooks()
    {
        var books = await _bookService.GetAllBooksAsync();
        return Ok(books);
    }

    // GET: api/books/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetBookById(int id)
    {
        var book = await _bookService.GetBookByIdAsync(id);
        if (book == null)
            return NotFound($"Book with ID {id} not found.");

        return Ok(book);
    }

    // POST: api/books
    [HttpPost]
    public async Task<IActionResult> AddBook([FromBody] BookDTO bookDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var newBook = await _bookService.AddBookAsync(bookDto);
        return CreatedAtAction(nameof(GetBookById), new { id = newBook.BookId }, newBook);
    }

    // PUT: api/books/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateBook([FromBody] BookDTO bookDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var updatedBook = await _bookService.UpdateBookAsync(bookDto);
            return Ok(updatedBook);
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }

    // DELETE: api/books/{id}
    [HttpDelete("{id}")]
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
}

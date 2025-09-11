using LibraryManagementAPI.Models.DTOs;
using LibraryManagementAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LibraryManagementAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]  // Require user to be logged in
    public class BorrowingRecordController : ControllerBase
    {
        private readonly IBorrowingRecordService _borrowingRecordService;

        public BorrowingRecordController(IBorrowingRecordService borrowingRecordService)
        {
            _borrowingRecordService = borrowingRecordService;
        }

        [HttpGet]
        [Authorize(Roles = "Borrower, Samaritan, Admin, Librarian")]
        public async Task<IActionResult> GetAllBorrowedBooks()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var borrowedBooks = await _borrowingRecordService.GetAllBorrowedBooksAsync(userId);
            return Ok(borrowedBooks);
        }

        [HttpGet]
        [Authorize(Roles = "Borrower, Samaritan, Admin, Librarian")]
        public async Task<IActionResult> GetCurrentlyBorrowedBooks()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var borrowedBooks = await _borrowingRecordService.GetCurrentlyBorrowedBooksAsync(userId);
            return Ok(borrowedBooks);
        }

        [HttpPut("{borrowingRecordId}")]
        [Authorize(Roles = "Borrower")]
        public async Task<IActionResult> ReturnBook(int borrowingRecordId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _borrowingRecordService.ReturnBookAsync(borrowingRecordId, userId);

            if (!result)
                return BadRequest("Failed to return the book or invalid record.");

            return NoContent();
        }

    }
}

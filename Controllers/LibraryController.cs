using LibraryManagementAPI.Models.DTOs;
using LibraryManagementAPI.Models;
using LibraryManagementAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace LibraryManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LibraryController : ControllerBase
    {
        private readonly ILibraryService _libraryService;

        public LibraryController(ILibraryService libraryService)
        {
            _libraryService = libraryService;
        }

        // GET: api/libraries
        [HttpGet]
        [Authorize(Roles = "Admin,Librarian,User")]
        public async Task<ActionResult<IEnumerable<Library>>> GetLibraries()
        {
            var libraries = await _libraryService.GetAllLibrariesAsync();
            return Ok(libraries);
        }

        // GET: api/libraries/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Librarian,User")]
        public async Task<ActionResult<LibraryDTO>> GetLibrary(int id)
        {
            var library = await _libraryService.GetLibraryByIdAsync(id);
            if (library == null)
            {
                return NotFound();
            }
            return Ok(library);
        }

        // POST: api/libraries
        [HttpPost]
        [Authorize(Roles = "Admin,Librarian")]
        public async Task<ActionResult<Library>> CreateLibrary(Library library)
        {
            var createdLibrary = await _libraryService.AddLibraryAsync(library);
            return CreatedAtAction(nameof(GetLibrary), new { id = createdLibrary.LibraryId }, createdLibrary);
        }

        // PUT: api/libraries/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Librarian")]
        public async Task<IActionResult> UpdateLibrary(int id, Library library)
        {
            //if (id != library.LibraryId)
            //{
            //    return BadRequest("Library ID mismatch");
            //}

            var updatedLibrary = await _libraryService.UpdateLibraryAsync(id, library);

            if (updatedLibrary == null)
            {
                return NotFound("Library not found");
            }

            return Ok(updatedLibrary); // send back updated entity
        }

        // DELETE: api/libraries/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteLibrary(int id)
        {
            var deleted = await _libraryService.DeleteLibraryAsync(id);
            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }

        // GET: api/libraries/5/availability
        [HttpGet("{id}/availability")]
        [Authorize(Roles = "Admin,Librarian,User")]
        public async Task<ActionResult<IEnumerable<LibraryAvailabilityDto>>> GetLibraryAvailability(int id)
        {
            var availability = await _libraryService.GetBookAvailabilityAsync(id);
            if (availability == null || !availability.Any())
            {
                return NotFound();
            }
            return Ok(availability);
        }
    }
}


using LibraryManagementAPI.Models;
using LibraryManagementAPI.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementAPI.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly LibMgmtDbContext _context;

        public CategoryService(LibMgmtDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
        {
            return await _context.Categories
                .Include(c => c.Library)
                .Select(c => new CategoryDto
                {
                    CategoryId = c.CategoryId,
                    Name = c.Name,
                    LibraryId = c.LibraryId,
                    LibraryName = c.Library.Name
                })
                .ToListAsync();
        }


        public async Task<CategoryDto?> GetCategoryByIdAsync(int id)
        {
            var category = await _context.Categories
                .Include(c => c.Library)
                .FirstOrDefaultAsync(c => c.CategoryId == id);

            if (category == null)
                return null;

            return new CategoryDto
            {
                CategoryId = category.CategoryId,
                Name = category.Name,
                LibraryId = category.LibraryId,
                LibraryName = category.Library.Name
            };
        }


        public async Task<Category> AddCategoryAsync(CategoryDto categoryDto)
        {
            // Check if a category with the same name already exists in this library
            bool exists = await _context.Categories
                .AnyAsync(c => c.LibraryId == categoryDto.LibraryId && c.Name.ToLower() == categoryDto.Name.ToLower());

            if (exists)
            {
                throw new InvalidOperationException("A category with this name already exists in the library.");
            }

            var category = new Category
            {
                Name = categoryDto.Name,
                LibraryId = categoryDto.LibraryId
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return category;
        }


        public async Task<Category?> UpdateCategoryAsync(int id, CategoryDto categoryDto)
        {
            var existing = await _context.Categories.FindAsync(id);
            if (existing == null) return null;

            existing.Name = categoryDto.Name;
            existing.LibraryId = categoryDto.LibraryId;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var existing = await _context.Categories
                .Include(c => c.Books)   // Load related books
                .FirstOrDefaultAsync(c => c.CategoryId == id);

            if (existing == null)
                return false;

            if (existing.Books.Any())
            {
                throw new InvalidOperationException("Cannot delete category because it has associated books.");
            }

            _context.Categories.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }

    }
}

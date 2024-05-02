using Microsoft.EntityFrameworkCore;
using NewsAggregationPlatform.Data;
using NewsAggregationPlatform.Models.Entities;
using NewsAggregationPlatform.Services.Abstraction;

namespace NewsAggregationPlatform.Services.Implementation
{
    public class CategoryService : ICategoryService
    {
        private readonly AppDbContext _dbContext;

        public CategoryService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<IEnumerable<Category>> GetCategoriesAsync()
        {
            return await _dbContext.Categories.ToListAsync();
        }
        public async Task<Category> GetCategoryByIdAsync(Guid id)
        {
            return await _dbContext.Categories.Include(c => c.Articles).FirstOrDefaultAsync(c => c.Id == id);
        }
        public async Task<Category> GetCategoryByNameAsync(String name)
        {
            return await _dbContext.Categories.Include(c => c.Articles).FirstOrDefaultAsync(c => c.Name == name);
        }
        public bool AddCategory(Category category)
        {
            _dbContext.Categories.Add(category);
            return Save();
        }
        public bool UpdateCategory(Category category)
        {
            _dbContext.Categories.Update(category);
            return Save();
        }
        public bool DeleteCategory(Category category)
        {
            _dbContext.Remove(category);
            return Save();
        }
        public bool Save()
        {
            var changes = _dbContext.SaveChanges();
            return changes > 0;
        }
    }
}

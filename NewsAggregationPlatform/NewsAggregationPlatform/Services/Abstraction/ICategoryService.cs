using NewsAggregationPlatform.Models.DTOs.Category;
using NewsAggregationPlatform.Models.Entities;

namespace NewsAggregationPlatform.Services.Abstraction
{
    public interface ICategoryService
    {
        Task<IEnumerable<Category>> GetCategoriesAsync();
        Task<Category> GetCategoryByIdAsync(Guid id);
        Task<Category> GetCategoryByNameAsync(String name);
        bool AddCategory(Category category);
        bool UpdateCategory(Category category);
        bool UpdateCategory(Guid id, UpdateCategoryRequestDto dto);
        bool DeleteCategory(Category category);
        bool Save();
    }
}

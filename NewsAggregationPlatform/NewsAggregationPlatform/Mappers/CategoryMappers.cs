using NewsAggregationPlatform.Models.DTOs.Category;
using NewsAggregationPlatform.Models.Entities;

namespace NewsAggregationPlatform.Mappers
{
    public static class CategoryMappers
    {
        public static CategoryDto ToCategoryDto(this Category category)
        {
            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name
            };
        }

        public static Category ToCategoryFromCreateDto(this CreateCategoryRequestDto categoryDto)
        {
            return new Category
            {
                Id = Guid.NewGuid(),
                Name = categoryDto.Name

            };
        }
    }
}

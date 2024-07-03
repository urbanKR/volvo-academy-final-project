using System.ComponentModel.DataAnnotations;

namespace NewsAggregationPlatform.Models.DTOs.Category
{
    public class CreateCategoryRequestDto
    {
        [Required]
        [MaxLength(30)]
        public string Name { get; set; }
    }
}

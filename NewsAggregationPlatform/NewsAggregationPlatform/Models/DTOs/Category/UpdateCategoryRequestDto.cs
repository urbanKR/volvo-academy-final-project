using System.ComponentModel.DataAnnotations;

namespace NewsAggregationPlatform.Models.DTOs.Category
{
    public class UpdateCategoryRequestDto
    {
        [Required]
        [MaxLength(30)]
        public string Name { get; set; }
    }
}

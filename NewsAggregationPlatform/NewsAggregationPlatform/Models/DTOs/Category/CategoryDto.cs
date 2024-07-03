using System.ComponentModel.DataAnnotations;

namespace NewsAggregationPlatform.Models.DTOs.Category
{
    public class CategoryDto
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        [MaxLength(30)]
        public string Name { get; set; }
    }
}

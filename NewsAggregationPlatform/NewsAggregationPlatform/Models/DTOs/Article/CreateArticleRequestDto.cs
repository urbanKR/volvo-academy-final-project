using System.ComponentModel.DataAnnotations;

namespace NewsAggregationPlatform.Models.DTOs.Article
{
    public class CreateArticleRequestDto
    {
        [Required]
        [MaxLength(150)]
        public string Title { get; set; }
        [Required]
        [MaxLength(1200)]
        public string Description { get; set; }
        [MaxLength(10000)]
        public string? Content { get; set; }
        [Required]
        [Url]
        public string Url { get; set; }
        [Required]
        [Range(0, 100, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public int BasePositivityLevel { get; set; }
        [Url]
        public string? Thumbnail { get; set; }
        public Guid CategoryId { get; set; }
        public Guid? SourceId { get; set; }
    }
}

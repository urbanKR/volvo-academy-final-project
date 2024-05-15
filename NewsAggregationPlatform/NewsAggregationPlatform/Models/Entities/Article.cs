using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewsAggregationPlatform.Models.Entities
{
    public class Article
    {
        [Key]
        public Guid Id { get; set; }
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
        [Required]
        [DataType(DataType.Date)]
        public DateTime PublishedDate { get; set; }
        [ForeignKey("User")]
        public Guid? CreatedByUserId { get; set; }
        [ForeignKey("Category")]
        public Guid CategoryId { get; set; }
        [ForeignKey("Source")]
        public Guid? SourceId { get; set; }
        public User? User { get; set; }
        public Category? Category { get; set; }
        public Source? Source { get; set; }
        public ICollection<Comment>? Comments { get; set; }
        public ICollection<Rating>? Ratings { get; set; }
        public ICollection<UserPositivityLevel>? PositivityLevels { get; set; }
    }
}

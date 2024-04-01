using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace NewsAggregationPlatform.Models.Entities
{
    public class Article
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string Title { get; set; }
        [Required]
        public string Content { get; set; }
        [Required]
        [Url]
        public string Url { get; set; }
        public int BasePositivityLevel { get; set; }
        public string Thumbnail { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime PublishedDate { get; set; }
        public Guid? CreatedByUserId { get; set; }
        [Required]
        public Guid CategoryId { get; set; }
        public User User { get; set; }
        public Category Category { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public ICollection<Rating> Ratings { get; set; }
        public ICollection<UserPositivityLevel> PositivityLevels { get; set; }
    }
}

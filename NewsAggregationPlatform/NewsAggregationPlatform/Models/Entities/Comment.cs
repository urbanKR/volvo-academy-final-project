using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewsAggregationPlatform.Models.Entities
{
    public class Comment
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        [MaxLength(300)]
        public string Content { get; set; }
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; }
        [ForeignKey("ArticleId")]
        public Guid ArticleId { get; set; }
        [ForeignKey("UserId")]
        public Guid UserId { get; set; }
        public Article Article { get; set; }
        public User User { get; set; }
    }
}

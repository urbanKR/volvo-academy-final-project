using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace NewsAggregationPlatform.Models.Entities
{
    [PrimaryKey(nameof(UserId), nameof(ArticleId))]
    public class UserPositivityLevel
    {
        public Guid UserId { get; set; }
        public Guid ArticleId { get; set; }
        [Required]
        [Range(0, 100, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public int Level { get; set; }
        public User User { get; set; }
        public Article Article { get; set; }
    }
}

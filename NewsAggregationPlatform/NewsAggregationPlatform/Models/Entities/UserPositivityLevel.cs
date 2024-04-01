using Microsoft.EntityFrameworkCore;

namespace NewsAggregationPlatform.Models.Entities
{
    [PrimaryKey(nameof(UserId), nameof(ArticleId))]
    public class UserPositivityLevel
    {
        public Guid UserId { get; set; }
        public Guid ArticleId { get; set; }
        public int Level { get; set; }
        public User User { get; set; }
        public Article Article { get; set; }
    }
}

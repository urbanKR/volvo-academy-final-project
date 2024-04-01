using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace NewsAggregationPlatform.Models.Entities
{
    [PrimaryKey(nameof(UserId), nameof(ArticleId))]
    public class Rating
    {
        public Guid UserId { get; set; }
        public Guid ArticleId { get; set; }
        public bool Vote { get; set; }
        public User User { get; set; }
        public Article Article { get; set; }
    }
}

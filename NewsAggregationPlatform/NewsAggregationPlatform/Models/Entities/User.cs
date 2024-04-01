using Microsoft.AspNetCore.Identity;

namespace NewsAggregationPlatform.Models.Entities
{
    public class User : IdentityUser<Guid>
    {
        public ICollection<Article> Articles { get; set; }
        public ICollection<UserPositivityLevel> UserPositivityLevels { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public ICollection<Rating> Ratings { get; set; }
    }
}

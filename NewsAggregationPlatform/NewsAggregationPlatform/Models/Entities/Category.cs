using System.ComponentModel.DataAnnotations;

namespace NewsAggregationPlatform.Models.Entities
{
    public class Category
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }
        public ICollection<Article> Articles { get; set; }
    }
}

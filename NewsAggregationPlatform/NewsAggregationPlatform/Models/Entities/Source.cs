using System.ComponentModel.DataAnnotations;

namespace NewsAggregationPlatform.Models.Entities
{
    public class Source
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        [MaxLength(30)]
        public string Name { get; set; }
        [Required]
        [Url]
        public string RssUrl { get; set; }
        [Required]
        [Url]
        public string OriginUrl { get; set; }
        public ICollection<Article>? Articles { get; set; }
    }
}

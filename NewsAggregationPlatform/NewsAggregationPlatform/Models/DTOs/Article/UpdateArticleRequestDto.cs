namespace NewsAggregationPlatform.Models.DTOs.Article
{
    public class UpdateArticleRequestDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string? Content { get; set; }
        public string Url { get; set; }
        public int BasePositivityLevel { get; set; }
        public string? Thumbnail { get; set; }
        public Guid CategoryId { get; set; }
        public Guid? SourceId { get; set; }
    }
}

using NewsAggregationPlatform.Models.DTOs.Article;
using NewsAggregationPlatform.Models.Entities;

namespace NewsAggregationPlatform.Mappers
{
    public static class ArticleMappers
    {
        public static ArticleDto ToArticleDto(this Article article)
        {
            return new ArticleDto
            {
                Id = article.Id,
                Title = article.Title,
                Description = article.Description,
                Content = article.Content,
                Url = article.Url,
                BasePositivityLevel = article.BasePositivityLevel,
                Thumbnail = article.Thumbnail,
                PublishedDate = article.PublishedDate,
                CategoryId = article.CategoryId,
                SourceId = article.SourceId
            };
        }
        public static Article ToArticleFromCreateDto(this CreateArticleRequestDto articleDto)
        {
            return new Article
            {
                Id = Guid.NewGuid(),
                Title = articleDto.Title,
                Description = articleDto.Description,
                Content = articleDto.Content,
                Url = articleDto.Url,
                BasePositivityLevel = articleDto.BasePositivityLevel,
                Thumbnail = articleDto.Thumbnail,
                PublishedDate = DateTime.Now,
                CategoryId = articleDto.CategoryId,
                SourceId = articleDto.SourceId,

            };
        }
    }
}

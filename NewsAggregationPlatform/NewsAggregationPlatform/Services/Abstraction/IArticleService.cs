using NewsAggregationPlatform.Models.DTOs.Article;
using NewsAggregationPlatform.Models.Entities;

namespace NewsAggregationPlatform.Services.Abstraction
{
    public interface IArticleService
    {
        Task<IEnumerable<Article>> GetArticlesAsync();
        Task<Article> GetArticleByIdAsync(Guid id);
        bool AddArticle(Article article);
        bool UpdateArticle(Article article);
        bool UpdateArticle(Guid id, UpdateArticleRequestDto dto);
        bool DeleteArticle(Article article);
        bool Save();
        Task AggregateFromSourcesAsync(CancellationToken cancellationToken);
        Task AnalyzeAndUpdateArticlePositivityAsync(CancellationToken cancellationToken);
    }
}

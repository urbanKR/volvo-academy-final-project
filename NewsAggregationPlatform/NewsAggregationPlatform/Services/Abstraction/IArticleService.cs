using NewsAggregationPlatform.Models.Entities;

namespace NewsAggregationPlatform.Services.Abstraction
{
    public interface IArticleService
    {
        Task<IEnumerable<Article>> GetArticlesAsync();
        Task<Article> GetArticleByIdAsync(Guid id);
        bool AddArticle(Article article);
        bool UpdateArticle(Article article);
        bool DeleteArticle(Article article);
        bool Save();
        Task AggregateFromSourceAsync(string rssLink, CancellationToken cancellationToken);
    }
}

using NewsAggregationPlatform.Models.Entities;

namespace NewsAggregationPlatform.Interfaces
{
    public interface IArticleSource
    {
        Task FetchArticlesAsync(CancellationToken cancellationToken);
    }
}

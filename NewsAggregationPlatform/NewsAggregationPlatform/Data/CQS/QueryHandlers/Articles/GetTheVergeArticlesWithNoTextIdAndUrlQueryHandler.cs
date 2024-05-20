using MediatR;
using Microsoft.EntityFrameworkCore;
using NewsAggregationPlatform.Data.CQS.Queries.Articles;

namespace NewsAggregationPlatform.Data.CQS.QueryHandlers.Articles
{
    public class GetTheVergeArticlesWithNoTextIdAndUrlQueryHandler : IRequestHandler<GetTheVergeArticlesWithNoTextIdAndUrlQuery, Dictionary<Guid, string>>
    {
        private readonly AppDbContext _dbContext;

        public GetTheVergeArticlesWithNoTextIdAndUrlQueryHandler(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Dictionary<Guid, string>> Handle(GetTheVergeArticlesWithNoTextIdAndUrlQuery request, CancellationToken cancellationToken)
        {
            var result = await _dbContext.Articles
                .AsNoTracking()
                .Where(a => string.IsNullOrWhiteSpace(a.Content) && a.Category.Name == "Technology")
                .ToDictionaryAsync(a => a.Id, a => a.Url, cancellationToken: cancellationToken);

            return result;
        }
    }
}

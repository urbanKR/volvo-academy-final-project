
using MediatR;
using Microsoft.EntityFrameworkCore;
using NewsAggregationPlatform.Data.CQS.Queries.Articles;

namespace NewsAggregationPlatform.Data.CQS.QueryHandlers.Articles
{
    public class GetArticlesWithNoTextIdAndUrlQueryHandler : IRequestHandler<GetArticlesWithNoTextIdAndUrlQuery,Dictionary<Guid, string>>
    {
        private readonly AppDbContext _dbContext;

        public GetArticlesWithNoTextIdAndUrlQueryHandler(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Dictionary<Guid, string>> Handle(GetArticlesWithNoTextIdAndUrlQuery idAndUrlLinkQuery, CancellationToken cancellationToken)
        {
            var result = await _dbContext.Articles
                .AsNoTracking()
                .Where(a => string.IsNullOrWhiteSpace(a.Content))
                .ToDictionaryAsync(a => a.Id, a => a.Url, cancellationToken: cancellationToken);

            return result;
        }
    }
}

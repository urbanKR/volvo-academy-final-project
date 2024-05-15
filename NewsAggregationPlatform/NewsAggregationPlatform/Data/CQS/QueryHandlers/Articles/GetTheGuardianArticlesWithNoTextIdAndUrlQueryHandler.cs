using MediatR;
using Microsoft.EntityFrameworkCore;
using NewsAggregationPlatform.Data.CQS.Queries.Articles;

namespace NewsAggregationPlatform.Data.CQS.QueryHandlers.Articles
{
    public class GetTheGuardianArticlesWithNoTextIdAndUrlQueryHandler : IRequestHandler<GetTheGuardianArticlesWithNoTextIdAndUrlQuery, Dictionary<Guid, string>>
    {
        private readonly AppDbContext _dbContext;

        public GetTheGuardianArticlesWithNoTextIdAndUrlQueryHandler(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Dictionary<Guid, string>> Handle(GetTheGuardianArticlesWithNoTextIdAndUrlQuery idAndUrlLinkQuery, CancellationToken cancellationToken)
        {
            var result = await _dbContext.Articles
                .AsNoTracking()
                .Where(a => string.IsNullOrWhiteSpace(a.Content) && a.Category.Name == "World News")
                .ToDictionaryAsync(a => a.Id, a => a.Url, cancellationToken: cancellationToken);

            return result;
        }
    }
}

using MediatR;

namespace NewsAggregationPlatform.Data.CQS.Queries.Articles
{
    public class GetTheVergeArticlesWithNoTextIdAndUrlQuery : IRequest<Dictionary<Guid, string>>
    {
    }
}

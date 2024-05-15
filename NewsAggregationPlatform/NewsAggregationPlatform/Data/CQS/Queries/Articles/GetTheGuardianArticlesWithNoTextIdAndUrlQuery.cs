using MediatR;

namespace NewsAggregationPlatform.Data.CQS.Queries.Articles
{
    public class GetTheGuardianArticlesWithNoTextIdAndUrlQuery : IRequest<Dictionary<Guid, string>>
    {
    }
}

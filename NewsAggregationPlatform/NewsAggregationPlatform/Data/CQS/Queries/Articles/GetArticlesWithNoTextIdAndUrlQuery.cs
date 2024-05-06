using MediatR;

namespace NewsAggregationPlatform.Data.CQS.Queries.Articles
{
    public class GetArticlesWithNoTextIdAndUrlQuery : IRequest<Dictionary<Guid, string>>
    {
    }
}

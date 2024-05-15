using MediatR;
using System.ServiceModel.Syndication;

namespace NewsAggregationPlatform.Data.CQS.Commands.Articles
{
    public class InitializeArticlesByTheGuardianRssDataCommand : IRequest
    {
        public IEnumerable<SyndicationItem> RssData { get; set; }
    }
}

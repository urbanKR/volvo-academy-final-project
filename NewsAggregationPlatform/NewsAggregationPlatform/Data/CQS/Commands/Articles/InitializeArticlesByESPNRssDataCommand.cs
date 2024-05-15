using MediatR;
using System.ServiceModel.Syndication;

namespace NewsAggregationPlatform.Data.CQS.Commands.Articles
{
    public class InitializeArticlesByESPNRssDataCommand : IRequest
    {
        public IEnumerable<SyndicationItem> RssData { get; set; }
    }
}

using MediatR;
using System.ServiceModel.Syndication;

namespace NewsAggregationPlatform.Data.CQS.Commands.Articles
{
    public class InitializeArticlesByTheVergeRssDataCommand : IRequest
    {
        public IEnumerable<SyndicationItem> RssData { get; set; }
    }
}

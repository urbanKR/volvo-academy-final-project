using MediatR;

namespace NewsAggregationPlatform.Data.CQS.Commands.Articles
{
    public class AddTextToArticlesCommand : IRequest
    {
        public Dictionary<Guid, string> ArticleTexts { get; set; }
    }
}

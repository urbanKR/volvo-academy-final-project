using HtmlAgilityPack;
using MediatR;
using NewsAggregationPlatform.Data.CQS.Commands.Articles;
using NewsAggregationPlatform.Data.CQS.Queries.Articles;
using NewsAggregationPlatform.Interfaces;
using System.ServiceModel.Syndication;
using System.Text;
using System.Xml;

namespace NewsAggregationPlatform.Sources
{
    public class TheGuardianRssArticleSource : IArticleSource
    {
        private readonly IMediator _mediator;
        private readonly string rssLink = "https://www.theguardian.com/world/rss";

        public TheGuardianRssArticleSource(IMediator mediator)
        {
            _mediator = mediator;
        }
        public async Task FetchArticlesAsync(CancellationToken cancellationToken)
        {
            try
            {
                var reader = XmlReader.Create(rssLink);
                var feed = SyndicationFeed.Load(reader);

                await _mediator.Send(new InitializeArticlesByTheGuardianRssDataCommand()
                {
                    RssData = feed.Items
                }, cancellationToken);

                var articlesWithNoText = await _mediator.Send(
                 new GetTheGuardianArticlesWithNoTextIdAndUrlQuery(),
                 cancellationToken);

                var data = new Dictionary<Guid, string>();
                foreach (var article in articlesWithNoText)
                {
                    var text = await GetArticleTextByUrl(article.Value);
                    data.Add(article.Key, text);
                }

                await _mediator.Send(new AddTextToArticlesCommand()
                {
                    ArticleTexts = data
                }, cancellationToken);

            }
            catch (Exception e)
            {
                throw;
            }
        }
        private async Task<string> GetArticleTextByUrl(string url)
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_3) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/99.0.4844.84 Safari/537.36");
            var response = await httpClient.GetStringAsync(url);

            var doc = new HtmlDocument();
            doc.LoadHtml(response);

            var paragraphBuilder = new StringBuilder();
            var paragraphs = doc.DocumentNode.SelectNodes("//p");

            if (paragraphs != null)
            {
                foreach (var paragraph in paragraphs)
                {
                    paragraphBuilder.AppendLine(paragraph.InnerText);
                }
            }

            return paragraphBuilder.ToString();
        }
    }
}

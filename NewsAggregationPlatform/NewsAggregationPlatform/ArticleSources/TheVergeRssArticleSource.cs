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
    public class TheVergeRssArticleSource : IArticleSource
    {
        private readonly IMediator _mediator;
        private readonly ILogger<TheVergeRssArticleSource> _logger;
        private readonly string rssLink = "https://www.theverge.com/rss/index.xml";

        public TheVergeRssArticleSource(IMediator mediator, ILogger<TheVergeRssArticleSource> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }
        public async Task FetchArticlesAsync(CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Fetching articles from The Verge RSS feed");
                var reader = XmlReader.Create(rssLink);
                var feed = SyndicationFeed.Load(reader);

                await _mediator.Send(new InitializeArticlesByTheVergeRssDataCommand()
                {
                    RssData = feed.Items
                }, cancellationToken);

                var articlesWithNoText = await _mediator.Send(
                                    new GetTheVergeArticlesWithNoTextIdAndUrlQuery(),
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
                _logger.LogInformation("Successfully fetched and processed articles from The Verge RSS feed");

            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred while fetching or processing articles from The Verge RSS feed");
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

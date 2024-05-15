using HtmlAgilityPack;
using MediatR;
using NewsAggregationPlatform.Data.CQS.Commands.Articles;
using NewsAggregationPlatform.Data.CQS.Queries.Articles;
using NewsAggregationPlatform.Interfaces;
using NewsAggregationPlatform.Models.Entities;
using System.ServiceModel.Syndication;
using System.Text;
using System.Xml;

namespace NewsAggregationPlatform.Sources
{
    public class ESPNRssArticleSource : IArticleSource
    {
        private readonly IMediator _mediator;
        private readonly string rssLink =  "https://www.espn.com/espn/rss/soccer/news";

        public ESPNRssArticleSource(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task FetchArticlesAsync(CancellationToken cancellationToken)
        {
            try
            {
                var reader = XmlReader.Create(rssLink);
                var feed = SyndicationFeed.Load(reader);

                await _mediator.Send(new InitializeArticlesByESPNRssDataCommand()
                {
                    RssData = feed.Items
                }, cancellationToken);

                var articlesWithNoText = await _mediator.Send(
                 new GetArticlesWithNoTextIdAndUrlQuery(),
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
            var web = new HtmlWeb();
            var doc = await web.LoadFromWebAsync(url);
            var articleBody = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'article-body')]");
            if (articleBody != null)
            {
                var paragraphBuilder = new StringBuilder();
                var paragraphs = articleBody.SelectNodes(".//p");

                if (paragraphs != null)
                {
                    foreach (var paragraph in paragraphs)
                    {
                        paragraphBuilder.AppendLine(paragraph.InnerText);
                    }
                }

                return paragraphBuilder.ToString();
            }
            else
            {
                return "";
            }
        }
    }
}

using HtmlAgilityPack;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NewsAggregationPlatform.Data;
using NewsAggregationPlatform.Data.CQS.CommandHandlers.Articles;
using NewsAggregationPlatform.Data.CQS.Commands.Articles;
using NewsAggregationPlatform.Data.CQS.Queries.Articles;
using NewsAggregationPlatform.Data.CQS.QueryHandlers.Articles;
using NewsAggregationPlatform.Models.Entities;
using NewsAggregationPlatform.Services.Abstraction;
using System.ServiceModel.Syndication;
using System.Text;
using System.Xml;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace NewsAggregationPlatform.Services.Implementation
{
    public class ArticleService : IArticleService
    {
        private readonly AppDbContext _dbContext;
        private readonly IMediator _mediator;

        public ArticleService(AppDbContext dbContext, IMediator mediator)
        {
            _dbContext = dbContext;
            _mediator = mediator;
        }
        public async Task<IEnumerable<Article>> GetArticlesAsync()
        {
            return await _dbContext.Articles.Include(a => a.Category).Include(a => a.Source).ToListAsync();
        }
        public async Task<Article> GetArticleByIdAsync(Guid id)
        {
            return await _dbContext.Articles.Include(a => a.Category).Include(a => a.Source).FirstOrDefaultAsync(a => a.Id == id);
        }
        public bool AddArticle(Article article)
        {
            _dbContext.Articles.Add(article);
            return Save();
        }
        public bool UpdateArticle(Article article)
        {
            _dbContext.Articles.Update(article);
            return Save();
        }
        public bool DeleteArticle(Article article)
        {
            _dbContext.Remove(article);
            return Save();
        }
        public bool Save()
        {
            var changes = _dbContext.SaveChanges();
            return changes > 0;
        }

        public async Task AggregateFromSourceAsync(string rssLink, CancellationToken cancellationToken)
        {
            try
            {
                var reader = XmlReader.Create(rssLink);
                var feed = SyndicationFeed.Load(reader);

                await _mediator.Send(new InitializeArticlesByRssDataCommand()
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

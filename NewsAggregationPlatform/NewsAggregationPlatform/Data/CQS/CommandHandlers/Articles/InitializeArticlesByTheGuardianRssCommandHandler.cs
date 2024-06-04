using HtmlAgilityPack;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NewsAggregationPlatform.Data.CQS.Commands.Articles;
using NewsAggregationPlatform.Models.Entities;
using System.Xml;
using System.Xml.Linq;

namespace NewsAggregationPlatform.Data.CQS.CommandHandlers.Articles
{
    public class InitializeArticlesByTheGuardianRssCommandHandler : IRequestHandler<InitializeArticlesByTheGuardianRssDataCommand>
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<InitializeArticlesByTheGuardianRssCommandHandler> _logger;

        public InitializeArticlesByTheGuardianRssCommandHandler(AppDbContext dbContext, ILogger<InitializeArticlesByTheGuardianRssCommandHandler> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }
        public async Task Handle(InitializeArticlesByTheGuardianRssDataCommand command, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Initializing articles by The Guardian RSS data");
                var existedArticleLinks = await _dbContext.Articles.Select(a => a.Url).ToListAsync(cancellationToken);
                var category = await _dbContext.Categories.FirstOrDefaultAsync(c => c.Name == "World News");
                var source = await _dbContext.Sources.FirstOrDefaultAsync(s => s.Name == "The Guardian");

                var articles = command.RssData
                       .Select(item =>
                       {
                           var htmlDoc = new HtmlDocument();
                           htmlDoc.LoadHtml(item.Summary.Text);
                           var description = htmlDoc.DocumentNode.InnerText;
                           int maxDescriptionLength = 1200;
                           if (description.Length > maxDescriptionLength)
                           {
                               description = description.Substring(0, maxDescriptionLength - 3);
                               description += "...";
                           }
                           DateTime publishDate;
                           try
                           {
                               publishDate = item.PublishDate.UtcDateTime;
                           }
                           catch (XmlException)
                           {
                               publishDate = DateTime.UtcNow;
                           }
                           var mediaContent = item.ElementExtensions
                            .Where(e => e.OuterName == "content" && e.OuterNamespace == "http://search.yahoo.com/mrss/")
                            .Select(e => XElement.Parse(e.GetReader().ReadOuterXml()))
                            .FirstOrDefault();
                           var thumbnail = mediaContent?.Attribute("url")?.Value;

                           return new Article
                           {
                               Id = Guid.NewGuid(),
                               Title = item.Title.Text,
                               Description = description,
                               Url = item.Id,
                               PublishedDate = publishDate,
                               BasePositivityLevel = 50,
                               Thumbnail = thumbnail,
                               SourceId = source.Id,
                               CategoryId = category.Id
                           };
                       })
                       .Where(a => !existedArticleLinks.Contains(a.Url) && !a.Url.IsNullOrEmpty()).ToList();

                await _dbContext.Articles.AddRangeAsync(articles, cancellationToken);
                await _dbContext.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("Successfully initialized articles by The Guardian RSS data");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred while initializing articles by The Guardian RSS data");
                throw;
            }
        }
    }
}

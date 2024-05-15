using MediatR;
using Microsoft.EntityFrameworkCore;
using NewsAggregationPlatform.Data.CQS.Commands.Articles;
using NewsAggregationPlatform.Models.Entities;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System.Xml;
using NewsAggregationPlatform.Migrations;
using System.Security.Policy;
using Microsoft.IdentityModel.Tokens;
using System.Xml.Linq;
using HtmlAgilityPack;

namespace NewsAggregationPlatform.Data.CQS.CommandHandlers.Articles
{
    public class InitializeArticlesByTheGuardianRssCommandHandler : IRequestHandler<InitializeArticlesByTheGuardianRssDataCommand>
    {
        private readonly AppDbContext _dbContext;

        public InitializeArticlesByTheGuardianRssCommandHandler(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task Handle(InitializeArticlesByTheGuardianRssDataCommand command, CancellationToken cancellationToken)
        {
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
                       if(description.Length > maxDescriptionLength)
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
        }
    }
}

using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NewsAggregationPlatform.Data.CQS.Commands.Articles;
using NewsAggregationPlatform.Models.Entities;
using System.Xml;

namespace NewsAggregationPlatform.Data.CQS.CommandHandlers.Articles
{
    public class InitializeArticlesByESPNRssCommandHandler : IRequestHandler<InitializeArticlesByESPNRssDataCommand>
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<InitializeArticlesByESPNRssCommandHandler> _logger;

        public InitializeArticlesByESPNRssCommandHandler(AppDbContext dbContext, ILogger<InitializeArticlesByESPNRssCommandHandler> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task Handle(InitializeArticlesByESPNRssDataCommand command, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Initializing articles by ESPN RSS data");
                var existedArticleLinks = await _dbContext.Articles.Select(a => a.Url).ToListAsync(cancellationToken);
                var category = await _dbContext.Categories.FirstOrDefaultAsync(c => c.Name == "Sport");
                var source = await _dbContext.Sources.FirstOrDefaultAsync(s => s.Name == "ESPN");

                var articles = command.RssData
                       .Skip(1)
                       .Select(item =>
                       {
                           var linkElement = item.ElementExtensions.ReadElementExtensions<XmlElement>("link", "http://www.w3.org/2005/Atom").FirstOrDefault();
                           DateTime publishDate;
                           try
                           {
                               publishDate = item.PublishDate.UtcDateTime;
                           }
                           catch (XmlException)
                           {
                               publishDate = DateTime.UtcNow;
                           }

                           return new Article
                           {
                               Id = Guid.NewGuid(),
                               Title = item.Title.Text,
                               Description = item.Summary.Text,
                               Url = item.Links.Skip(1).FirstOrDefault()?.Uri.AbsoluteUri,
                               PublishedDate = publishDate,
                               BasePositivityLevel = 50,
                               Thumbnail = item.Links.FirstOrDefault()?.Uri.AbsoluteUri,
                               SourceId = source.Id,
                               CategoryId = category.Id
                           };
                       })
                       .Where(a => !existedArticleLinks.Contains(a.Url) && !a.Url.IsNullOrEmpty()).ToList();

                await _dbContext.Articles.AddRangeAsync(articles, cancellationToken);
                await _dbContext.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("Successfully initialized articles by ESPN RSS data");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred while initializing articles by ESPN RSS data");
                throw;
            }
        }
    }
}

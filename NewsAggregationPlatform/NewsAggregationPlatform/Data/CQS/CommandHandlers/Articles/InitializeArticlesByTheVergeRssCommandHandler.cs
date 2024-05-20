using HtmlAgilityPack;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NewsAggregationPlatform.Data.CQS.Commands.Articles;
using NewsAggregationPlatform.Models.Entities;
using System.ServiceModel.Syndication;

namespace NewsAggregationPlatform.Data.CQS.CommandHandlers.Articles
{
    public class InitializeArticlesByTheVergeRssCommandHandler : IRequestHandler<InitializeArticlesByTheVergeRssDataCommand>
    {
        private readonly AppDbContext _dbContext;

        public InitializeArticlesByTheVergeRssCommandHandler(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Handle(InitializeArticlesByTheVergeRssDataCommand command, CancellationToken cancellationToken)
        {
            var existedArticleLinks = await _dbContext.Articles.Select(a => a.Url).ToListAsync(cancellationToken);
            var category = await _dbContext.Categories.FirstOrDefaultAsync(c => c.Name == "Technology");
            var source = await _dbContext.Sources.FirstOrDefaultAsync(s => s.Name == "The Verge");
            var articles = command.RssData
                .Select(item =>
                {

                    var htmlDoc = new HtmlDocument();
                    var content = item.Content as TextSyndicationContent;
                    if (content == null)
                    {
                        throw new InvalidCastException("Item content is not text.");
                    }
                    htmlDoc.LoadHtml(content.Text);

                    var descriptionNode = htmlDoc.DocumentNode.SelectSingleNode("//p");
                    var description = descriptionNode != null ? descriptionNode.InnerText : string.Empty;
                    var maxDescriptionLength = 1200;

                    if (description.Length > maxDescriptionLength)
                    {
                        description = description.Substring(0, maxDescriptionLength - 3);
                        description += "...";
                    }

                    DateTime publishDate;
                    try
                    {
                        publishDate = DateTime.Parse(item.PublishDate.ToString());
                    }
                    catch (Exception)
                    {
                        publishDate = DateTime.UtcNow;
                    }

                    var thumbnailNode = htmlDoc.DocumentNode.SelectSingleNode("//img");
                    var thumbnail = thumbnailNode != null ? thumbnailNode.GetAttributeValue("src", null) : string.Empty;


                    return new Article
                    {
                        Id = Guid.NewGuid(),
                        Title = item.Title.Text,
                        Description = description,
                        Url = item.Id,
                        PublishedDate = publishDate,
                        BasePositivityLevel = 50,
                        Thumbnail = null,
                        SourceId = source.Id,
                        CategoryId = category.Id
                    };
                })
                .Where(a => !existedArticleLinks.Contains(a.Url) && !string.IsNullOrEmpty(a.Url)).ToList();

            await _dbContext.Articles.AddRangeAsync(articles, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}

using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using NewsAggregationPlatform.Data;
using NewsAggregationPlatform.Models.Entities;
using NewsAggregationPlatform.Services.Abstraction;
using System.ServiceModel.Syndication;
using System.Text;
using System.Xml;

namespace NewsAggregationPlatform.Services.Implementation
{
    public class ArticleService : IArticleService
    {
        private readonly AppDbContext _dbContext;

        public ArticleService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
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

        public async Task AggregateFromSourceAsync(string rssLink, Guid categoryId, Guid sourceId)
        {
            try
            {
                var reader = XmlReader.Create(rssLink);
                var feed = SyndicationFeed.Load(reader);
                var existedArticles = await _dbContext.Articles.Select(a => a.Url).ToListAsync();

                var articles = feed.Items
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
                            Url = item.Links.Skip(1).First().Uri.AbsoluteUri,
                            PublishedDate = publishDate,
                            BasePositivityLevel = 50,
                            Thumbnail = item.Links.First().Uri.AbsoluteUri,
                            SourceId = sourceId,
                            CategoryId = categoryId
                        };
                    })
                    .Where(a => !existedArticles.Contains(a.Url)).ToDictionary(a => a.Url, a => a);

                foreach (var a in articles)
                {
                    var articleText = await GetArticleTextByUrl(a.Key);
                    a.Value.Content = articleText;
                }

                await _dbContext.Articles.AddRangeAsync(articles.Values);
                await _dbContext.SaveChangesAsync();


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

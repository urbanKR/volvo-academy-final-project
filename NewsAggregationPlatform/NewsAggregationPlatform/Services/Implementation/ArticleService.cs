using Microsoft.EntityFrameworkCore;
using NewsAggregationPlatform.Data;
using NewsAggregationPlatform.Models.Entities;
using NewsAggregationPlatform.Services.Abstraction;

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
            return await _dbContext.Articles.ToListAsync();
        }
        public async Task<Article> GetArticleByIdAsync(Guid id)
        {
            return await _dbContext.Articles.FirstOrDefaultAsync(a => a.Id == id);
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
    }
}

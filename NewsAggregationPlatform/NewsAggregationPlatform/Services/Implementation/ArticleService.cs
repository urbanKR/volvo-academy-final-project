using MediatR;
using Microsoft.EntityFrameworkCore;
using NewsAggregationPlatform.Data;
using NewsAggregationPlatform.Interfaces;
using NewsAggregationPlatform.Models.Entities;
using NewsAggregationPlatform.Services.Abstraction;

namespace NewsAggregationPlatform.Services.Implementation
{
    public class ArticleService : IArticleService
    {
        private readonly AppDbContext _dbContext;
        private readonly IMediator _mediator;
        private readonly IEnumerable<IArticleSource> _articleSources;

        public ArticleService(AppDbContext dbContext, IMediator mediator, IEnumerable<IArticleSource> articleSources)
        {
            _dbContext = dbContext;
            _mediator = mediator;
            _articleSources = articleSources;
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

        public async Task AggregateFromSourcesAsync(CancellationToken cancellationToken)
        {
            foreach (var source in _articleSources)
            {
                await source.FetchArticlesAsync(cancellationToken);
            }
        }
    }
}

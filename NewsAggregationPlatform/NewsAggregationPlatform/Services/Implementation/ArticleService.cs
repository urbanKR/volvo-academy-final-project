using MediatR;
using Microsoft.EntityFrameworkCore;
using NewsAggregationPlatform.Data;
using NewsAggregationPlatform.Data.CQS.Commands.Articles;
using NewsAggregationPlatform.Interfaces;
using NewsAggregationPlatform.Models.DTOs.Article;
using NewsAggregationPlatform.Models.Entities;
using NewsAggregationPlatform.Services.Abstraction;

namespace NewsAggregationPlatform.Services.Implementation
{
    public class ArticleService : IArticleService
    {
        private readonly AppDbContext _dbContext;
        private readonly IMediator _mediator;
        private readonly IEnumerable<IArticleSource> _articleSources;
        private readonly IPositivityAnalysisService _positivityAnalysisService;

        public ArticleService(AppDbContext dbContext, IMediator mediator, IEnumerable<IArticleSource> articleSources, IPositivityAnalysisService positivityAnalysisService)
        {
            _dbContext = dbContext;
            _mediator = mediator;
            _articleSources = articleSources;
            _positivityAnalysisService = positivityAnalysisService;
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
        public bool UpdateArticle(Guid id, UpdateArticleRequestDto dto)
        {
            var existingArticle = _dbContext.Articles.FirstOrDefault(a => a.Id == id);

            if (existingArticle == null)
            {
                return false;
            }

            existingArticle.Title = dto.Title;
            existingArticle.Description = dto.Description;
            existingArticle.Content = dto.Content;
            existingArticle.Url = dto.Url;
            existingArticle.BasePositivityLevel = dto.BasePositivityLevel;
            existingArticle.Thumbnail = dto.Thumbnail;
            existingArticle.CategoryId = dto.CategoryId;
            existingArticle.SourceId = dto.SourceId;

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
        public async Task AnalyzeAndUpdateArticlePositivityAsync(CancellationToken cancellationToken)
        {
            await _mediator.Send(new AnalyzeAndUpdateArticlePositivityCommand(), cancellationToken);
        }
    }
}

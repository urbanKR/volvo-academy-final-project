using MediatR;
using Microsoft.EntityFrameworkCore;
using NewsAggregationPlatform.Data.CQS.Commands.Articles;
using NewsAggregationPlatform.Services.Abstraction;

namespace NewsAggregationPlatform.Data.CQS.CommandHandlers.Articles
{
    public class AnalyzeAndUpdateArticlePositivityCommandHandler : IRequestHandler<AnalyzeAndUpdateArticlePositivityCommand>
    {
        private readonly AppDbContext _dbContext;
        private readonly IPositivityAnalysisService _positivityAnalysisService;

        public AnalyzeAndUpdateArticlePositivityCommandHandler(AppDbContext dbContext, IPositivityAnalysisService positivityAnalysisService)
        {
            _dbContext = dbContext;
            _positivityAnalysisService = positivityAnalysisService;
        }

        public async Task Handle(AnalyzeAndUpdateArticlePositivityCommand request, CancellationToken cancellationToken)
        {
            var articles = await _dbContext.Articles.ToListAsync(cancellationToken);

            foreach (var article in articles)
            {
                if (article.Content != null)
                {
                    var prediction = _positivityAnalysisService.AnalyzePositivity(article.Content);
                    article.BasePositivityLevel = (int)prediction.Score + 15 > 100 ? 100 : (int)prediction.Score + 15;
                }
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}

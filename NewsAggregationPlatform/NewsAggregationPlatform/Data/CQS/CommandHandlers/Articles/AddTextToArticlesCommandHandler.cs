using MediatR;
using Microsoft.EntityFrameworkCore;
using NewsAggregationPlatform.Data.CQS.Commands.Articles;

namespace NewsAggregationPlatform.Data.CQS.CommandHandlers.Articles
{
    public class AddTextToArticlesCommandHandler : IRequestHandler<AddTextToArticlesCommand>
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<AddTextToArticlesCommandHandler> _logger;

        public AddTextToArticlesCommandHandler(AppDbContext dbContext, ILogger<AddTextToArticlesCommandHandler> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task Handle(AddTextToArticlesCommand command, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Adding text to articles");
                var articles = await _dbContext.Articles.ToListAsync(cancellationToken);

                foreach (var a in articles)
                {
                    if (command.ArticleTexts.TryGetValue(a.Id, out var text))
                    {
                        a.Content = text;
                    }
                }
                await _dbContext.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("Successfully added text to articles");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred while adding text to articles");
                throw;
            }
        }
    }
}

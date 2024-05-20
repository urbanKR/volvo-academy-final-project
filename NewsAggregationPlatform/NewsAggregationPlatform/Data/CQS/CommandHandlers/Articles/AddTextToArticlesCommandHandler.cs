using MediatR;
using Microsoft.EntityFrameworkCore;
using NewsAggregationPlatform.Data.CQS.Commands.Articles;

namespace NewsAggregationPlatform.Data.CQS.CommandHandlers.Articles
{
    public class AddTextToArticlesCommandHandler : IRequestHandler<AddTextToArticlesCommand>
    {
        private readonly AppDbContext _dbContext;

        public AddTextToArticlesCommandHandler(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Handle(AddTextToArticlesCommand command, CancellationToken cancellationToken)
        {
            var articles = await _dbContext.Articles.ToListAsync(cancellationToken);

            foreach (var a in articles)
            {
                if (command.ArticleTexts.TryGetValue(a.Id, out var text))
                {
                    a.Content = text;
                }
            }
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}

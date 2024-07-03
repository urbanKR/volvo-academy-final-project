using FluentAssertions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NewsAggregationPlatform.Data;
using NewsAggregationPlatform.Interfaces;
using NewsAggregationPlatform.Models.Entities;
using NewsAggregationPlatform.Services.Abstraction;
using NewsAggregationPlatform.Services.Implementation;
using NSubstitute;

namespace NewsAggregationPlatform.Tests
{
    public class ArticleServiceTests
    {
        private readonly AppDbContext _dbContext;
        private readonly ArticleService _articleService;
        private readonly IMediator _mediator = Substitute.For<IMediator>();
        private readonly IEnumerable<IArticleSource> _articleSources = Substitute.For<IEnumerable<IArticleSource>>();
        private readonly IPositivityAnalysisService _positivityAnalysisService = Substitute.For<IPositivityAnalysisService>();

        public ArticleServiceTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;
            _dbContext = new AppDbContext(options);
            _articleService = new ArticleService(_dbContext, _mediator, _articleSources, _positivityAnalysisService);
        }

        [Fact]
        public async Task ArticleService_GetArticlesAsync_ReturnsAllArticles()
        {
            //Arrange
            _dbContext.Database.EnsureDeleted();
            var category = new Category { Id = Guid.NewGuid(), Name = "Test Category" };
            var source = new Source { Id = Guid.NewGuid(), Name = "Test Source", OriginUrl = "http://test.com", RssUrl = "http://test.com" };
            var article1 = new Article
            {
                Id = Guid.NewGuid(),
                Title = "Test Article 1",
                Description = "Test Description 1",
                Category = category,
                Source = source,
                Url = "http://test.com"
            };
            var article2 = new Article
            {
                Id = Guid.NewGuid(),
                Title = "Test Article 2",
                Description = "Test Description 2",
                Category = category,
                Source = source,
                Url = "http://test.com"
            };
            _articleService.AddArticle(article1);
            _articleService.AddArticle(article2);

            //Act
            var result = await _articleService.GetArticlesAsync();

            //Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.Should().Contain(a => a.Id == article1.Id);
            result.Should().Contain(a => a.Id == article2.Id);
        }

        [Fact]
        public async Task ArticleService_GetArticleByIdAsync_ReturnsArticleWhenArticleExists()
        {
            //Arrange
            _dbContext.Database.EnsureDeleted();
            var category = new Category { Id = Guid.NewGuid(), Name = "Test Category" };
            var source = new Source { Id = Guid.NewGuid(), Name = "Test Source", OriginUrl = "http://test.com", RssUrl = "http://test.com" };
            var article = new Article
            {
                Id = Guid.NewGuid(),
                Title = "Test Article",
                Description = "Test Description",
                Category = category,
                Source = source,
                Url = "http://test.com"
            };
            _articleService.AddArticle(article);

            //Act
            var result = await _articleService.GetArticleByIdAsync(article.Id);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(article);
        }

        [Fact]
        public void ArticleService_AddArticle_AddsArticleSuccessfully()
        {
            //Arrange
            var article = new Article
            {
                Id = Guid.NewGuid(),
                Title = "Test Article",
                Description = "Test Description",
                Url = "http://test.com"
            };

            //Act
            var result = _articleService.AddArticle(article);

            //Assert
            result.Should().BeTrue();
            _dbContext.Articles.Should().ContainSingle(a => a.Id == article.Id);
        }

        [Fact]
        public async Task UpdateArticle_UpdatesArticleSuccessfully()
        {
            //Arrange
            var category = new Category { Id = Guid.NewGuid(), Name = "Test Category" };
            var source = new Source { Id = Guid.NewGuid(), Name = "Test Source", OriginUrl = "http://test.com", RssUrl = "http://test.com" };
            var article = new Article
            {
                Id = Guid.NewGuid(),
                Title = "Original Title",
                Description = "Original Description",
                Category = category,
                Source = source,
                Url = "http://test.com"
            };
            _articleService.AddArticle(article);

            //Act
            article.Title = "Updated Title";
            article.Description = "Updated Description";
            var result = _articleService.UpdateArticle(article);

            //Assert
            result.Should().BeTrue();
            var updatedArticle = await _articleService.GetArticleByIdAsync(article.Id);
            updatedArticle.Title.Should().Be("Updated Title");
            updatedArticle.Description.Should().Be("Updated Description");
        }

        [Fact]
        public void ArticleService_DeleteArticle_DeletesArticleSuccessfully()
        {
            //Arrange
            var article = new Article
            {
                Id = Guid.NewGuid(),
                Title = "Test Article",
                Description = "Test Description",
                Url = "http://test.com"
            };
            _articleService.AddArticle(article);

            //Act
            var result = _articleService.DeleteArticle(article);

            //Assert
            result.Should().BeTrue();
            _dbContext.Articles.Should().NotContain(article);
        }
    }
}

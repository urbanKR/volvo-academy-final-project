using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NewsAggregationPlatform.Models.Entities;
using NewsAggregationPlatform.Services.Abstraction;

namespace NewsAggregationPlatform.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ArticlesController : Controller
    {
        private readonly IArticleService _articleService;
        private readonly ICategoryService _categoryService;
        private readonly ISourceService _sourceService;
        private readonly ILogger<ArticlesController> _logger;
        public ArticlesController(IArticleService articleService, ICategoryService categoryService, ISourceService sourceService, ILogger<ArticlesController> logger, IPositivityAnalysisService positivityAnalysisService)
        {
            _articleService = articleService;
            _categoryService = categoryService;
            _sourceService = sourceService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<Article> articles = await _articleService.GetArticlesAsync();
            return View(articles);
        }

        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                _logger.LogError("Article id is null in Details method");
                return NotFound();
            }

            Article article = await _articleService.GetArticleByIdAsync(id.Value);

            if (article == null)
            {
                _logger.LogError("Article not found with id: {Id} in Details method", id.Value);
                return NotFound();
            }

            return View(article);
        }

        public async Task<IActionResult> Create()
        {
            var categories = await _categoryService.GetCategoriesAsync();
            var sources = await _sourceService.GetSourcesAsync();
            var categoryList = categories.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            });
            var sourceList = sources.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            });

            ViewData["Categories"] = new SelectList(categoryList, "Value", "Text");
            ViewData["Sources"] = new SelectList(sourceList, "Value", "Text");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Article article)
        {
            if (ModelState.IsValid)
            {
                article.Id = Guid.NewGuid();
                article.PublishedDate = DateTime.Now;
                if (string.IsNullOrWhiteSpace(article.Thumbnail))
                {
                    article.Thumbnail = null;
                }
                _articleService.AddArticle(article);
                return RedirectToAction(nameof(Index));
            }
            _logger.LogWarning("Invalid model state while creating article");
            return View(article);
        }

        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                _logger.LogError("Article id is null in Edit method");
                return NotFound();
            }

            Article article = await _articleService.GetArticleByIdAsync(id.Value);
            if (article == null)
            {
                _logger.LogError("Article not found with id: {Id} in Edit method", id.Value);
                return NotFound();
            }
            var categories = await _categoryService.GetCategoriesAsync();
            var sources = await _sourceService.GetSourcesAsync();
            var categoryList = categories.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            });
            var sourceList = sources.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            });

            ViewData["Categories"] = new SelectList(categoryList, "Value", "Text");
            ViewData["Sources"] = new SelectList(sourceList, "Value", "Text");
            return View(article);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, Article article)
        {
            if (id != article.Id)
            {
                _logger.LogError("Mismatch between route id: {Id} and article id: {ArticleId} in Edit method", id, article.Id);
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    article.PublishedDate = DateTime.Now;
                    _articleService.UpdateArticle(article);
                }
                catch (DbUpdateConcurrencyException e)
                {
                    if (!ArticleExists(article.Id))
                    {
                        _logger.LogError("Article not found with id: {Id} in Edit method", id);
                        return NotFound();
                    }
                    else
                    {
                        _logger.LogError(e, "DbUpdateConcurrencyException for article with id: {Id} in Edit method", id);
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            _logger.LogWarning("Invalid model state while updating article with id: {Id}", id);
            return View(article);
        }

        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                _logger.LogError("Article id is null in Delete method");
                return NotFound();
            }

            Article article = await _articleService.GetArticleByIdAsync(id.Value);
            if (article == null)
            {
                _logger.LogError("Article not found with id: {Id} in Delete method", id.Value);
                return NotFound();
            }

            return View(article);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var article = await _articleService.GetArticleByIdAsync(id);
            if (article != null)
            {
                _articleService.DeleteArticle(article);
            }
            else
            {
                _logger.LogError("Article not found with id: {Id} in DeleteConfirmed method", id);
                return NotFound();
            }
            return RedirectToAction(nameof(Index));
        }
        private bool ArticleExists(Guid id)
        {
            if (_articleService.GetArticleByIdAsync(id).Result != null)
            {
                return true;
            }
            return false;
        }

        public async Task<IActionResult> Aggregate()
        {
            await _articleService.AggregateFromSourcesAsync(new CancellationToken());
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> AnalyzePositivity()
        {
            await _articleService.AnalyzeAndUpdateArticlePositivityAsync(new CancellationToken());
            return RedirectToAction("Index");
        }
    }
}

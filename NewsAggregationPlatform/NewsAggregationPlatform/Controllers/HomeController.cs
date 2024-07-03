using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NewsAggregationPlatform.Models;
using NewsAggregationPlatform.Models.Entities;
using NewsAggregationPlatform.Services.Abstraction;
using System.Diagnostics;

namespace NewsAggregationPlatform.Controllers
{
    public class HomeController : Controller
    {
        private readonly IArticleService _articleService;
        private readonly ICategoryService _categoryService;
        private readonly ISourceService _sourceService;
        private readonly ILogger<ArticlesController> _logger;
        public HomeController(IArticleService articleService, ICategoryService categoryService, ISourceService sourceService, ILogger<ArticlesController> logger)
        {
            _articleService = articleService;
            _categoryService = categoryService;
            _sourceService = sourceService;
            _logger = logger;
        }
        public async Task<IActionResult> Index(Guid? categoryId, Guid? sourceId, double? positivityLevel)
        {
            int? minPositivityLevelVal = TempData["PositivityLevel"] as int?;
            if (minPositivityLevelVal == null)
            {
                minPositivityLevelVal = 0;
            }
            IEnumerable<Article> articles = await _articleService.GetArticlesAsync();
            articles = articles.Where(a => a.BasePositivityLevel >= minPositivityLevelVal.Value);

            if (categoryId.HasValue)
            {
                articles = articles.Where(a => a.CategoryId == categoryId.Value);
            }

            if (sourceId.HasValue)
            {
                articles = articles.Where(a => a.SourceId == sourceId.Value);
            }

            if (positivityLevel.HasValue)
            {
                articles = articles.Where(a => a.BasePositivityLevel >= positivityLevel.Value);
            }

            ViewData["Categories"] = new SelectList(await _categoryService.GetCategoriesAsync(), "Id", "Name");
            ViewData["Sources"] = new SelectList(await _sourceService.GetSourcesAsync(), "Id", "Name");

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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public IActionResult SetPositivityLevel(int positivityLevel)
        {
            TempData["PositivityLevel"] = positivityLevel;
            return RedirectToAction("Index");
        }
    }
}

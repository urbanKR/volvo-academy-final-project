using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> Index()
        {
            IEnumerable<Article> articles = await _articleService.GetArticlesAsync();
            return View(articles);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

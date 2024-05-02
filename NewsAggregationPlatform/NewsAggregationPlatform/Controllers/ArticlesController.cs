using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NewsAggregationPlatform.Data;
using NewsAggregationPlatform.Models.Entities;
using NewsAggregationPlatform.Services.Abstraction;
using NewsAggregationPlatform.Services.Implementation;

namespace NewsAggregationPlatform.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ArticlesController : Controller
    {
        private readonly IArticleService _articleService;
        private readonly ICategoryService _categoryService;
        private readonly ISourceService _sourceService;
        public ArticlesController(IArticleService articleService, ICategoryService categoryService, ISourceService sourceService)
        {
            _articleService = articleService;
            _categoryService = categoryService;
            _sourceService = sourceService;
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
                return NotFound();
            }

            Article article = await _articleService.GetArticleByIdAsync(id.Value);

            if (article == null)
            {
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
            return View(article);
        }

        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Article article = await _articleService.GetArticleByIdAsync(id.Value);
            if (article == null)
            {
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
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    article.PublishedDate = DateTime.Now;
                    _articleService.UpdateArticle(article);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ArticleExists(article.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(article);

        }

        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Article article = await _articleService.GetArticleByIdAsync(id.Value);
            if (article == null)
            {
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
            var rssLink = "https://www.espn.com/espn/rss/soccer/news";
            var category = await _categoryService.GetCategoryByNameAsync("Sport");
            var source = await _sourceService.GetSourceByNameAsync("ESPN");
            await _articleService.AggregateFromSourceAsync(rssLink, category.Id, source.Id);  
            
            return RedirectToAction("Index");
        }
    }
}

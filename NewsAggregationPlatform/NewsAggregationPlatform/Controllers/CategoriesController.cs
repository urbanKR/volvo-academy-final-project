using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewsAggregationPlatform.Models.Entities;
using NewsAggregationPlatform.Services.Abstraction;

namespace NewsAggregationPlatform.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CategoriesController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly ILogger<CategoriesController> _logger;

        public CategoriesController(ICategoryService categoryService, ILogger<CategoriesController> logger)
        {
            _categoryService = categoryService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<Category> categories = await _categoryService.GetCategoriesAsync();
            return View(categories);
        }

        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                _logger.LogError("Category id is null in Details method");
                return NotFound();
            }

            Category category = await _categoryService.GetCategoryByIdAsync(id.Value);

            if (category == null)
            {
                _logger.LogError("Category not found with id: {Id} in Details method", id.Value);
                return NotFound();
            }

            return View(category);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            category.Id = Guid.NewGuid();
            if (ModelState.IsValid)
            {
                _categoryService.AddCategory(category);
                return RedirectToAction(nameof(Index));
            }
            _logger.LogWarning("Invalid model state while creating category");
            return View(category);
        }

        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                _logger.LogError("Category id is null in Edit method");
                return NotFound();
            }

            var category = await _categoryService.GetCategoryByIdAsync(id.Value);
            if (category == null)
            {
                _logger.LogError("Category not found with id: {Id} in Edit method", id.Value);
                return NotFound();
            }
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, Category category)
        {
            if (id != category.Id)
            {
                _logger.LogError("Mismatch between route id: {Id} and category id: {CategoryId} in Edit method", id, category.Id);
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _categoryService.UpdateCategory(category);
                }
                catch (DbUpdateConcurrencyException e)
                {
                    if (_categoryService.GetCategoryByIdAsync(id) == null)
                    {
                        _logger.LogError("Category not found with id: {Id} in Edit method", id);
                        return NotFound();
                    }
                    else
                    {
                        _logger.LogError(e, "DbUpdateConcurrencyException for category with id: {Id} in Edit method", id);
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            _logger.LogWarning("Invalid model state while updating category with id: {Id}", id);
            return View(category);
        }
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                _logger.LogError("Category id is null in Delete method");
                return NotFound();
            }

            var category = await _categoryService.GetCategoryByIdAsync(id.Value);
            if (category == null)
            {
                _logger.LogError("Category not found with id: {Id} in Delete method", id.Value);
                return NotFound();
            }

            return View(category);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category != null)
            {
                _categoryService.DeleteCategory(category);
            }
            else
            {
                _logger.LogError("Category not found with id: {Id} in DeleteConfirmed method", id);
                return NotFound();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}

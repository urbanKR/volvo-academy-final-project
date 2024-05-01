using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewsAggregationPlatform.Models.Entities;
using NewsAggregationPlatform.Services.Abstraction;

namespace NewsAggregationPlatform.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SourcesController : Controller
    {
        private readonly ISourceService _sourceService;

        public SourcesController(ISourceService sourceService)
        {
            _sourceService = sourceService;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<Source> sources = await _sourceService.GetSourcesAsync();
            return View(sources);
        }

        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Source source = await _sourceService.GetSourceByIdAsync(id.Value);

            if (source == null)
            {
                return NotFound();
            }

            return View(source);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Source source)
        {
            source.Id = Guid.NewGuid();
            if (ModelState.IsValid)
            {
                _sourceService.AddSource(source);
                return RedirectToAction(nameof(Index));
            }
            return View(source);
        }

        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var source = await _sourceService.GetSourceByIdAsync(id.Value);
            if (source == null)
            {
                return NotFound();
            }
            return View(source);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, Source source)
        {
            if (id != source.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _sourceService.UpdateSource(source);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (_sourceService.GetSourceByIdAsync(id) == null)
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
            return View(source);
        }
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var source = await _sourceService.GetSourceByIdAsync(id.Value);
            if (source == null)
            {
                return NotFound();
            }

            return View(source);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var source = await _sourceService.GetSourceByIdAsync(id);
            if (source != null)
            {
                _sourceService.DeleteSource(source);
            }
            return RedirectToAction(nameof(Index));
        }

    }
}

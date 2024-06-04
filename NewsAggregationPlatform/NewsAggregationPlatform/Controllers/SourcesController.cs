using Microsoft.AspNetCore.Authorization;
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
        private readonly ILogger<SourcesController> _logger;

        public SourcesController(ISourceService sourceService, ILogger<SourcesController> logger)
        {
            _sourceService = sourceService;
            _logger = logger;
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
                _logger.LogError("Source id is null in Details method");
                return NotFound();
            }

            Source source = await _sourceService.GetSourceByIdAsync(id.Value);

            if (source == null)
            {
                _logger.LogError("Source not found with id: {Id} in Details method", id.Value);
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
            _logger.LogWarning("Invalid model state while creating source");
            return View(source);
        }

        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                _logger.LogError("Source id is null in Edit method");
                return NotFound();
            }

            var source = await _sourceService.GetSourceByIdAsync(id.Value);
            if (source == null)
            {
                _logger.LogError("Source not found with id: {Id} in Edit method", id.Value);
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
                _logger.LogError("Mismatch between route id: {Id} and source id: {SourceId} in Edit method", id, source.Id);
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _sourceService.UpdateSource(source);
                }
                catch (DbUpdateConcurrencyException e)
                {
                    if (_sourceService.GetSourceByIdAsync(id) == null)
                    {
                        _logger.LogError("Source not found with id: {Id} in Edit method", id);
                        return NotFound();
                    }
                    else
                    {
                        _logger.LogError(e, "DbUpdateConcurrencyException for source with id: {Id} in Edit method", id);
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            _logger.LogWarning("Invalid model state while updating source with id: {Id}", id);
            return View(source);
        }
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                _logger.LogError("Source id is null in Delete method");
                return NotFound();
            }

            var source = await _sourceService.GetSourceByIdAsync(id.Value);
            if (source == null)
            {
                _logger.LogError("Source not found with id: {Id} in Delete method", id.Value);
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
            else
            {
                _logger.LogError("Source not found with id: {Id} in DeleteConfirmed method", id);
                return NotFound();
            }
            return RedirectToAction(nameof(Index));
        }

    }
}

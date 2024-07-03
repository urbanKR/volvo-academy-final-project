using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewsAggregationPlatform.Mappers;
using NewsAggregationPlatform.Models.DTOs.Category;
using NewsAggregationPlatform.Services.Abstraction;

namespace NewsAggregationPlatform.WebApi.Controllers
{
    /// <summary>
    /// Controller to work with categoiries categories
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        /// <summary>
        /// Constructor
        /// </summary>
        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        /// <summary>
        /// Get all categories
        /// </summary>
        /// <returns>Categories or not found</returns>
        /// <response code="200">Returns the categories</response>
        /// <response code="404">Categories are not found</response>
        /// <response code="400"></response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CategoryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get()
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var categories = await _categoryService.GetCategoriesAsync();
            var categoryDtos = categories.Select(c => c.ToCategoryDto());

            return Ok(categoryDtos);
        }
        /// <summary>
        /// Get Category by id
        /// </summary>
        /// <param name="id">Unique Category Identifier</param>
        /// <returns>Category with specified Id or not found</returns>
        /// <response code="200">Returns the category</response>
        /// <response code="404">If the category is not found</response>
        /// <response code="400"></response>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            return Ok(category.ToCategoryDto());
        }

        /// <summary>
        /// Create new category
        /// </summary>
        /// <param name="categoryDto"></param>
        /// <returns></returns>
        /// /// <response code="201">Return an 201 with link to created category</response>
        /// <response code="400">Incorrect Request</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateCategoryRequestDto categoryDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var categoryModel = categoryDto.ToCategoryFromCreateDto();
            _categoryService.AddCategory(categoryModel);

            return CreatedAtAction(nameof(Get), new { id = categoryModel.Id }, categoryModel.ToCategoryDto());
        }

        /// <summary>
        /// Update category by id
        /// </summary>
        /// <param name="id">Category Id</param>
        /// <returns></returns>
        /// <response code="200"></response>
        /// <response code="400"></response>
        /// <response code="404"></response>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateCategoryRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var categoryModel = await _categoryService.GetCategoryByIdAsync(id);

            if (categoryModel == null)
            {
                return NotFound();
            }

            _categoryService.UpdateCategory(categoryModel.Id, dto);

            return Ok(categoryModel.ToCategoryDto());
        }
        /// <summary>
        /// Remove category by id
        /// </summary>
        /// <param name="id">Category Id</param>
        /// <returns></returns>
        /// <response code="204"></response>
        /// <response code="404"></response>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var categoryModel = await _categoryService.GetCategoryByIdAsync(id);
            if (categoryModel == null)
            {
                return NotFound();
            }
            _categoryService.DeleteCategory(categoryModel);

            return NoContent();
        }
    }
}

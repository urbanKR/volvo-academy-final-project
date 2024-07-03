using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewsAggregationPlatform.Mappers;
using NewsAggregationPlatform.Models.DTOs.Article;
using NewsAggregationPlatform.Services.Abstraction;

namespace NewsAggregationPlatform.WebApi.Controllers
{
    /// <summary>
    /// Controller to work with articles
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ArticlesController : ControllerBase
    {
        private readonly IArticleService _articleService;
        private readonly ISourceService _sourceService;
        private readonly ICategoryService _categoryService;

        /// <summary>
        /// Constructor
        /// </summary>
        public ArticlesController(IArticleService articleService, ISourceService sourceService, ICategoryService categoryService)
        {
            _articleService = articleService;
            _sourceService = sourceService;
            _categoryService = categoryService;
        }
        /// <summary>
        /// Get all articles
        /// </summary>
        /// <returns>Articles or not found</returns>
        /// <response code="200">Returns the articles</response>
        /// <response code="404">Articles are not found</response>
        /// <response code="400"></response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ArticleDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get()
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var articles = await _articleService.GetArticlesAsync();
            var articleDtos = articles.Select(a => a.ToArticleDto());
            return Ok(articleDtos);
        }
        /// <summary>
        /// Get Article by id
        /// </summary>
        /// <param name="id">Unique Article Identifier</param>
        /// <returns>Article with specified Id or not found</returns>
        /// <response code="200">Returns the article</response>
        /// <response code="404">If the article is not found</response>
        /// <response code="400"></response>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ArticleDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var article = await _articleService.GetArticleByIdAsync(id);
            if (article == null)
            {
                return NotFound();
            }

            return Ok(article.ToArticleDto());
        }

        /// <summary>
        /// Create new article
        /// </summary>
        /// <param name="articleDto"></param>
        /// <returns></returns>
        /// /// <response code="201">Return an 201 with link to created article</response>
        /// <response code="400">Incorrect Request</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateArticleRequestDto articleDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var sourceExists = await _sourceService.GetSourceByIdAsync((Guid)articleDto.SourceId);
            var categoryExists = await _categoryService.GetCategoryByIdAsync(articleDto.CategoryId);

            if (sourceExists == null)
            {
                return BadRequest("The provided SourceId does not exist.");
            }

            if (categoryExists == null)
            {
                return BadRequest("The provided CategoryId does not exist.");
            }
            var articleModel = articleDto.ToArticleFromCreateDto();
            _articleService.AddArticle(articleModel);

            return CreatedAtAction(nameof(Get), new { id = articleModel.Id }, articleModel.ToArticleDto());
        }

        /// <summary>
        /// Update article by id
        /// </summary>
        /// <param name="id">Article Id</param>
        /// <returns></returns>
        /// <response code="200"></response>
        /// /// <response code="400"></response>
        /// <response code="404"></response>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateArticleRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var articleModel = await _articleService.GetArticleByIdAsync(id);

            if (articleModel == null)
            {
                return NotFound();
            }

            var source = await _sourceService.GetSourceByIdAsync((Guid)dto.SourceId);
            var category = await _categoryService.GetCategoryByIdAsync(dto.CategoryId);

            if (source == null)
            {
                return BadRequest("The provided SourceId does not exist.");
            }

            if (category == null)
            {
                return BadRequest("The provided CategoryId does not exist.");
            }

            _articleService.UpdateArticle(articleModel.Id, dto);

            return Ok(articleModel.ToArticleDto());
        }

        /// <summary>
        /// Remove article by id
        /// </summary>
        /// <param name="id">Article Id</param>
        /// <returns></returns>
        /// <response code="204"></response>
        /// <response code="404"></response>
        /// <response code="400"></response>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var articleModel = await _articleService.GetArticleByIdAsync(id);
            if (articleModel == null)
            {
                return NotFound();
            }
            _articleService.DeleteArticle(articleModel);

            return NoContent();
        }
    }
}

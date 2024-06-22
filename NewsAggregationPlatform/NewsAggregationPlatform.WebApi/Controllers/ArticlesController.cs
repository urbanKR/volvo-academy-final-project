using Microsoft.AspNetCore.Mvc;
using NewsAggregationPlatform.Data;
using NewsAggregationPlatform.Mappers;
using NewsAggregationPlatform.Models.DTOs.Article;

namespace NewsAggregationPlatform.WebApi.Controllers
{
    /// <summary>
    /// Controller to work with articles
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ArticlesController : ControllerBase
    {
        private readonly AppDbContext _context;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context"></param>
        public ArticlesController(AppDbContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Get all articles
        /// </summary>
        /// <returns>Articles or not found</returns>
        /// <response code="200">Returns the articles</response>
        /// <response code="404">Articles are not found</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ArticleDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Get()
        {
            var _articles = _context.Articles.ToList().Select(a => a.ToArticleDto());
            return Ok(_articles);
        }
        /// <summary>
        /// Get Article by id
        /// </summary>
        /// <param name="id">Unique Article Identifier</param>
        /// <returns>Article with specified Id or not found</returns>
        /// <response code="200">Returns the article</response>
        /// <response code="404">If the article is not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ArticleDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Get([FromRoute] Guid id)
        {
            var article = _context.Articles.Find(id);
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
        public IActionResult Create([FromBody] CreateArticleRequestDto articleDto)
        {
            var sourceExists = _context.Sources.Any(s => s.Id == articleDto.SourceId);
            var categoryExists = _context.Categories.Any(c => c.Id == articleDto.CategoryId);

            if (!sourceExists)
            {
                return BadRequest("The provided SourceId does not exist.");
            }

            if (!categoryExists)
            {
                return BadRequest("The provided CategoryId does not exist.");
            }
            var articleModel = articleDto.ToArticleFromCreateDto();
            _context.Articles.Add(articleModel);
            _context.SaveChanges();
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
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Update([FromRoute] Guid id, [FromBody] UpdateArticleRequestDto dto)
        {
            var articleModel = _context.Articles.FirstOrDefault(a => a.Id == id);

            if (articleModel == null)
            {
                return NotFound();
            }

            var sourceExists = _context.Sources.Any(s => s.Id == dto.SourceId);
            var categoryExists = _context.Categories.Any(c => c.Id == dto.CategoryId);

            if (!sourceExists)
            {
                return BadRequest("The provided SourceId does not exist.");
            }

            if (!categoryExists)
            {
                return BadRequest("The provided CategoryId does not exist.");
            }
            articleModel.Title = dto.Title;
            articleModel.Description = dto.Description;
            articleModel.Content = dto.Content;
            articleModel.Url = dto.Url;
            articleModel.BasePositivityLevel = dto.BasePositivityLevel;
            articleModel.Thumbnail = dto.Thumbnail;
            articleModel.CategoryId = dto.CategoryId;
            articleModel.SourceId = dto.SourceId;

            _context.SaveChanges();

            return Ok(articleModel.ToArticleDto());
        }

        /// <summary>
        /// Remove article by id
        /// </summary>
        /// <param name="id">Article Id</param>
        /// <returns></returns>
        /// <response code="204"></response>
        /// <response code="404"></response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Delete([FromRoute] Guid id)
        {
            var articleModel = _context.Articles.FirstOrDefault(a => a.Id.Equals(id));
            if (articleModel == null)
            {
                return NotFound();
            }
            _context.Articles.Remove(articleModel);
            _context.SaveChanges();

            return NoContent();
        }
    }
}

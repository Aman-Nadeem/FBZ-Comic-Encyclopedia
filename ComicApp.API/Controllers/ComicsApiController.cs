using ComicApp.Core.Interfaces;
using ComicApp.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace ComicApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ComicsApiController : ControllerBase
    {
        private readonly IComicRepository _repository;
        private readonly IWebHostEnvironment _env;
        private readonly IMemoryCache _cache;
        private const string CacheKey = "AllComics";

        public ComicsApiController(IComicRepository repository, IWebHostEnvironment env, IMemoryCache cache)
        {
            _repository = repository;
            _env = env;
            _cache = cache;
        }

        private List<Comic> GetAllComics()
        {
            if (!_cache.TryGetValue(CacheKey, out List<Comic>? comics))
            {
                string dataPath = Path.Combine(
                    Directory.GetParent(_env.ContentRootPath)!.FullName,
                    "ComicApp.Web", "wwwroot", "FBZData"
                );
                comics = _repository.LoadComics(dataPath);
                _cache.Set(CacheKey, comics, TimeSpan.FromHours(24));
            }
            return comics!;
        }

        // GET: api/comicsapi?page=1&pageSize=100 — public
        [HttpGet]
        public ActionResult<IEnumerable<Comic>> GetComics([FromQuery] int page = 1, [FromQuery] int pageSize = 100)
        {
            if (pageSize > 100) pageSize = 100;
            var comics = GetAllComics()
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            return Ok(comics);
        }

        // GET: api/comicsapi/{id} — public
        [HttpGet("{id}")]
        public ActionResult<Comic> GetComic(string id)
        {
            var comic = GetAllComics().FirstOrDefault(c => c.BlRecordId == id);
            if (comic == null) return NotFound();
            return Ok(comic);
        }

        // GET: api/comicsapi/search?title=batman — public
        [HttpGet("search")]
        public ActionResult<IEnumerable<Comic>> Search([FromQuery] string title)
        {
            var results = GetAllComics()
                .Where(c => c.Title != null && c.Title.Contains(title, StringComparison.OrdinalIgnoreCase))
                .Take(100)
                .ToList();
            return Ok(results);
        }

        // GET: api/comicsapi/all — admin only
        [Authorize]
        [HttpGet("all")]
        public ActionResult<IEnumerable<Comic>> GetAll()
        {
            return Ok(GetAllComics());
        }

        // DELETE: api/comicsapi/{id} — admin only
        [Authorize]
        [HttpDelete("{id}")]
        public ActionResult DeleteComic(string id)
        {
            var comic = GetAllComics().FirstOrDefault(c => c.BlRecordId == id);
            if (comic == null) return NotFound();
            return Ok(new { message = $"Comic {id} would be deleted in a real DB implementation." });
        }
    }
}
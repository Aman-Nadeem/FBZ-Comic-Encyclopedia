using Microsoft.AspNetCore.Mvc;
using ComicApp.Core.Interfaces;
using ComicApp.Core.DataAccess;
using ComicApp.Core.Services;
using Microsoft.Extensions.Caching.Memory;

namespace ComicApp.Web.Controllers
{
    public class ComicsController : Controller
    {
        private readonly IComicRepository _comicRepository;
        private readonly IComicSearchService _comicSearchService;
        private readonly ISavedSearchService _savedSearchService;
        private readonly AppDbContext _context;
        private readonly IMemoryCache _cache;
        private readonly EmailService _emailService;
        private const string ComicCacheKey = "AllComics";
        private const int PageSize = 100;

        public ComicsController(
            IComicRepository comicRepository,
            IComicSearchService comicSearchService,
            ISavedSearchService savedSearchService,
            AppDbContext context,
            IMemoryCache cache,
            EmailService emailService)
        {
            _comicRepository = comicRepository;
            _comicSearchService = comicSearchService;
            _savedSearchService = savedSearchService;
            _context = context;
            _cache = cache;
            _emailService = emailService;
        }

        private string GetDataPath()
        {
            return Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "data");
        }

        private List<ComicApp.Core.Models.Comic> GetCachedComics()
        {
            if (!_cache.TryGetValue(ComicCacheKey, out List<ComicApp.Core.Models.Comic> comics))
            {
                comics = _comicRepository.LoadComics(GetDataPath());
                _cache.Set(ComicCacheKey, comics, TimeSpan.FromHours(24));
            }
            return comics;
        }

        private void SetCommonViewBag(List<ComicApp.Core.Models.Comic> comics)
        {
            ViewBag.FlaggedIds = _context.FlaggedComics
                .Select(f => f.ComicId)
                .ToList();
        }

        public IActionResult Index(int page = 1)
        {
            var comics = GetCachedComics();
            SetCommonViewBag(comics);

            var totalComics = comics.Count;
            var totalPages = (int)Math.Ceiling(totalComics / (double)PageSize);
            var paged = comics
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalComics = totalComics;

            return View(paged);
        }

        // BASIC SEARCH
        [HttpPost]
        public async Task<IActionResult> Search(string title, int page = 1)
        {
            var username = HttpContext.Session.GetString("Username");
            var comics = GetCachedComics();
            var results = _comicSearchService.BasicTitleSearch(comics, title).ToList();

            _comicSearchService.RegisterSearch($"Title:{title}", results);
            _savedSearchService.SaveSearch($"Title:{title}", results, username);

            // Send email if user is logged in
            if (!string.IsNullOrEmpty(username))
            {
                try
                {
                    await _emailService.SendSavedSearchEmailAsync(
                        "kennyaltona@aol.com", username, title);
                }
                catch { /* Don't break the app if email fails */ }
            }

            SetCommonViewBag(comics);

            var totalPages = (int)Math.Ceiling(results.Count / (double)PageSize);
            var paged = results
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalComics = results.Count;

            return View("Index", paged);
        }

        // GENRE FILTER
        [HttpPost]
        public IActionResult FilterByGenre(string genre, int page = 1)
        {
            var username = HttpContext.Session.GetString("Username");
            var comics = GetCachedComics();

            var results = comics
                .Where(c => c.Genres.Any(g =>
                    g.Contains(genre, StringComparison.OrdinalIgnoreCase)))
                .ToList();

            _comicSearchService.RegisterSearch($"Genre:{genre}", results);
            _savedSearchService.SaveSearch($"Genre:{genre}", results, username);

            SetCommonViewBag(comics);

            var totalPages = (int)Math.Ceiling(results.Count / (double)PageSize);
            var paged = results
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalComics = results.Count;

            return View("Index", paged);
        }

        // SORTING
        [HttpPost]
        public IActionResult Sort(string order, int page = 1)
        {
            var comics = GetCachedComics();

            var sorted = order == "desc"
                ? comics.OrderByDescending(c => c.Title).ToList()
                : comics.OrderBy(c => c.Title).ToList();

            SetCommonViewBag(comics);

            var totalPages = (int)Math.Ceiling(sorted.Count / (double)PageSize);
            var paged = sorted
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalComics = sorted.Count;

            return View("Index", paged);
        }

        // GROUPING
        [HttpPost]
        public IActionResult GroupByAuthor()
        {
            var comics = GetCachedComics();
            var grouped = _comicSearchService.GroupByAuthor(comics);
            return View("GroupByAuthor", grouped);
        }

        [HttpPost]
        public IActionResult GroupByYear()
        {
            var comics = GetCachedComics();
            var grouped = _comicSearchService.GroupByYear(comics);
            return View("GroupByYear", grouped);
        }

        // ADVANCED SEARCH
        [HttpPost]
        public IActionResult AdvancedSearch(
            string? title,
            string? author,
            int? year,
            string? genre,
            string? language,
            string? nameType,
            int page = 1)
        {
            var username = HttpContext.Session.GetString("Username");
            var comics = GetCachedComics();

            var results = _comicSearchService.AdvancedSearch(
                comics,
                title,
                author,
                year,
                genre,
                language,
                nameType).ToList();

            _comicSearchService.RegisterSearch(
                $"Advanced:{title}-{author}-{year}-{genre}-{language}-{nameType}",
                results);

            _savedSearchService.SaveSearch("Advanced Search", results, username);

            SetCommonViewBag(comics);

            var totalPages = (int)Math.Ceiling(results.Count / (double)PageSize);
            var paged = results
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalComics = results.Count;

            return View("Index", paged);
        }

        // SAVE INDIVIDUAL COMIC
        [HttpPost]
        public IActionResult SaveComic(string id)
        {
            var username = HttpContext.Session.GetString("Username");
            var comics = GetCachedComics();
            var comic = comics.FirstOrDefault(c => c.BlRecordId == id);

            if (comic != null)
            {
                _savedSearchService.SaveSearch(
                    $"SavedComic:{comic.Title}", new[] { comic }, username);
            }

            return RedirectToAction("Index");
        }

        // SAVED SEARCHES PAGE
        public IActionResult SavedSearches()
        {
            var username = HttpContext.Session.GetString("Username");
            var searches = _savedSearchService.GetSavedSearches(username);
            return View(searches);
        }

        // REPORTS — STAFF ONLY
        public IActionResult Reports()
        {
            var role = HttpContext.Session.GetString("Role");

            if (role != "Staff")
            {
                return RedirectToAction("Index");
            }

            ViewBag.TopQueries = _comicSearchService.GetTopQueries(10);
            ViewBag.TopResults = _comicSearchService.GetTopResults(10);
            ViewBag.Frequent = _comicSearchService.GetComicIdsWithMoreThan(100);

            return View("~/Views/Reports/Index.cshtml");
        }
    }
}
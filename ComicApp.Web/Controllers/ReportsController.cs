using Microsoft.AspNetCore.Mvc;
using ComicApp.Core.Interfaces;
using ComicApp.Core.DataAccess;

namespace ComicApp.Web.Controllers
{
    public class ReportsController : Controller
    {
        private readonly IComicSearchService _searchService;
        private readonly AppDbContext _context;


    public ReportsController(IComicSearchService searchService, AppDbContext context)
        {
            _searchService = searchService;
            _context = context;
        }

        public IActionResult Index()
        {
            ViewBag.TopQueries = _searchService.GetTop10Queries();
            ViewBag.TopResults = _searchService.GetTop10Results();
            ViewBag.FrequentComics = _searchService.GetComicsAppearingInMoreThan100Searches();

            return View();
        }

        public IActionResult Flagged()
        {
            var flags = _context.FlaggedComics
                .OrderByDescending(f => f.FlaggedAt)
                .ToList();

            return View(flags);
        }
    }


}

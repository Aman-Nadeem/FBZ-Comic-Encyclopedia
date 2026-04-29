using ComicApp.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace ComicApp.Web.Controllers
{
    public class OpenLibraryController : Controller
    {
        private readonly OpenLibraryService _openLibraryService;

        public OpenLibraryController(OpenLibraryService openLibraryService)
        {
            _openLibraryService = openLibraryService;
        }

        public async Task<IActionResult> Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return View(new List<OpenLibraryBook>());

            var results = await _openLibraryService.SearchAsync(query);
            ViewBag.Query = query;
            return View(results);
        }
    }
}
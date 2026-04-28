using ComicApp.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace ComicApp.Web.Controllers
{
    public class ComicVineController : Controller
    {
        private readonly ComicVineService _comicVineService;

        public ComicVineController(ComicVineService comicVineService)
        {
            _comicVineService = comicVineService;
        }

        public async Task<IActionResult> Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return View(new List<ComicVineResult>());

            var results = await _comicVineService.SearchAsync(query);
            ViewBag.Query = query;
            return View(results);
        }
    }
}
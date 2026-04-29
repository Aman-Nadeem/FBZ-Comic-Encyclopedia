using Microsoft.AspNetCore.Mvc;
using ComicApp.Core.DataAccess;
using ComicApp.Core.Models;

namespace ComicApp.Web.Controllers
{
    public class FlagController : Controller
    {
        private readonly AppDbContext _context;


    public FlagController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult Flag(string comicId)
        {
            var username = HttpContext.Session.GetString("Username");

            var flagged = new FlaggedComic
            {
                ComicId = comicId,
                FlaggedBy = username,
                FlaggedAt = DateTime.UtcNow
            };

            _context.FlaggedComics.Add(flagged);
            _context.SaveChanges();

            return RedirectToAction("Index", "Comics");
        }
    }


}

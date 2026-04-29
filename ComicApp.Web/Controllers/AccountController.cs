using Microsoft.AspNetCore.Mvc;
using ComicApp.Core.Interfaces;

namespace ComicApp.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;

        public AccountController(IUserService userService)
        {
            _userService = userService;
        }

        // SHOW LOGIN PAGE
        public IActionResult Login()
        {
            return View();
        }

        // HANDLE LOGIN
        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            var user = _userService.Login(username, password);

            if (user == null)
            {
                ViewBag.Error = "Invalid username or password";
                return View();
            }

            HttpContext.Session.SetString("Username", user.Username);
            HttpContext.Session.SetString("Role", user.Role);

            return RedirectToAction("Index", "Comics");
        }

        // SHOW REGISTER PAGE
        public IActionResult Register()
        {
            return View();
        }

        // HANDLE REGISTER
        [HttpPost]
        public IActionResult Register(string username, string password, string role)
        {
            var success = _userService.Register(username, password, role);

            if (!success)
            {
                ViewBag.Error = "Username already exists";
                return View();
            }

            return RedirectToAction("Login");
        }

        // LOGOUT
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Comics");
        }
    }
}

using Microsoft.AspNetCore.Mvc;

namespace SchoolLibrary.Controllers;

public class HomeController : Controller
{
    // GET: /  — страница авторизации
    public IActionResult Index() => View();

    // POST: /Home/Login
    [HttpPost]
    public IActionResult Login(string login, string password, string role)
    {
        HttpContext.Session.SetString("UserName", string.IsNullOrWhiteSpace(login) ? "Гость" : login);
        HttpContext.Session.SetString("UserRole", role ?? "Student");
        return RedirectToAction("Index", "Books");
    }

    // GET: /Home/NotFound  — страница 404
    [Route("Home/NotFound")]
    public IActionResult NotFoundPage()
    {
        Response.StatusCode = 404;
        return View("NotFound");
    }
}
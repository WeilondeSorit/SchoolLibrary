using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolLibrary.Data;
using SchoolLibrary.Services;

namespace SchoolLibrary.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _db;
    public HomeController(AppDbContext db) => _db = db;

    public IActionResult Index() => View();

[HttpGet]
public IActionResult Login() => RedirectToAction(nameof(Index));

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(string login, string password)
    {
        if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
        {
            ViewBag.Error = "Введите логин и пароль";
            return View("Index");
        }

        try
        {
            var user = await _db.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Login == login);

            if (user is null || !PasswordHasher.Verify(password, user.PasswordHash))
            {
                ViewBag.Error = "Неверный логин или пароль";
                return View("Index");
            }

            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("UserName", user.DisplayName);
            HttpContext.Session.SetString("UserRole", user.Role?.Name ?? "Student");

            return RedirectToAction("Index", "Books");
        }
        catch (Exception ex)
        {
            ViewBag.Error = $"Ошибка подключения к БД: {ex.Message}";
            return View("Index");
        }
    }

    [Route("Home/NotFound")]
    public IActionResult NotFoundPage()
    {
        Response.StatusCode = 404;
        return View("NotFound");
    }
}
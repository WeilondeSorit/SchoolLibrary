using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SchoolLibrary.Models;

namespace SchoolLibrary.Controllers;

public class HomeController : Controller
{
    // GET: /  — страница авторизации
    public IActionResult Index() => View();

    // POST: /Home/Login
    [HttpPost]
    public IActionResult Login(string login, string password, string role)
    {
        // TODO: заменить на реальную проверку через Identity / БД
        HttpContext.Session.SetString("UserName", string.IsNullOrWhiteSpace(login) ? "Гость" : login);
        HttpContext.Session.SetString("UserRole", role ?? "Student");
        return RedirectToAction("Index", "Books");
    }
}
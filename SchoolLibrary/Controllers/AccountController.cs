using Microsoft.AspNetCore.Mvc;
using SchoolLibrary.Models;

namespace SchoolLibrary.Controllers;

public class AccountController : Controller
{
    public IActionResult Profile()
    {
        var role = HttpContext.Session.GetString("UserRole") ?? "Student";
        var name = HttpContext.Session.GetString("UserName") ?? "Гость";
        ViewBag.UserName = name;
        ViewBag.UserRole = role;

        if (role == "Student")
        {
            var my = MockData.Borrowings.Where(b => b.StudentId == 1).ToList();
            return View(my);
        }
        return View(new List<Borrowing>());
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Home");
    }
}
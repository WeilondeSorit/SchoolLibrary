using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolLibrary.Data;
using SchoolLibrary.Models;
using SchoolLibrary.Services;
using SchoolLibrary.ViewModels;

namespace SchoolLibrary.Controllers;

public class AccountController : Controller
{
    private readonly AppDbContext _db;
    public AccountController(AppDbContext db) => _db = db;

    public async Task<IActionResult> Profile()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId is null) return RedirectToAction("Index", "Home");

        var user = await _db.Users
            .Include(u => u.Role)
            .Include(u => u.Student)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user is null) return RedirectToAction("Index", "Home");

        ViewBag.UserName = user.DisplayName;
        ViewBag.UserRole = user.Role?.Name ?? "Student";

        if (user.Role?.Name == "Student" && user.Student is not null)
        {
            var my = await _db.Borrowings
                .Include(b => b.Book)!.ThenInclude(b => b!.Authors)
                .Where(b => b.StudentId == user.Student.Id)
                .OrderByDescending(b => b.BorrowDate)
                .ToListAsync();
            return View(my);
        }

        return View(new List<Borrowing>());
    }

    // ---------- Регистрация ----------

    [HttpGet]
    public IActionResult Register()
    {
        if (HttpContext.Session.GetInt32("UserId") is not null)
            return RedirectToAction(nameof(Profile));

        return View(new RegisterViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var loginExists = await _db.Users.AnyAsync(u => u.Login == model.Login);
        if (loginExists)
        {
            ModelState.AddModelError(nameof(model.Login), "Такой логин уже занят");
            return View(model);
        }

        var roleName = model.Role == "Librarian" ? "Librarian" : "Student";
        var role = await _db.Roles.FirstOrDefaultAsync(r => r.Name == roleName);
        if (role is null)
        {
            ModelState.AddModelError("", "Не найдена роль в базе. Проверьте init.sql");
            return View(model);
        }

        var user = new User
        {
            Login = model.Login,
            PasswordHash = PasswordHasher.Hash(model.Password),
            DisplayName = model.DisplayName,
            RoleId = role.Id,
            CreatedAt = DateTime.UtcNow
        };
        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        // Если ученик — создаём карточку ученика и связываем с аккаунтом
        if (roleName == "Student")
        {
            _db.Students.Add(new Student
            {
                UserId = user.Id,
                FullName = model.DisplayName,
                ClassName = model.ClassName ?? "",
                Email = model.Email ?? ""
            });
            await _db.SaveChangesAsync();
        }

        // Автовход после регистрации
        HttpContext.Session.SetInt32("UserId", user.Id);
        HttpContext.Session.SetString("UserName", user.DisplayName);
        HttpContext.Session.SetString("UserRole", roleName);

        return RedirectToAction("Index", "Books");
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Home");
    }
}
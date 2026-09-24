using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolLibrary.Data;

namespace SchoolLibrary.Controllers;

public class StudentsController : Controller
{
    private readonly AppDbContext _db;
    public StudentsController(AppDbContext db) => _db = db;

    public async Task<IActionResult> Index() =>
        View(await _db.Students.OrderBy(s => s.FullName).ToListAsync());

    public async Task<IActionResult> Details(int id)
    {
        var student = await _db.Students
            .Include(s => s.User)
            .FirstOrDefaultAsync(s => s.Id == id);
        if (student is null) return NotFound();

        ViewBag.Borrowings = await _db.Borrowings
            .Include(b => b.Book).ThenInclude(b => b.Authors)
            .Where(b => b.StudentId == id)
            .OrderByDescending(b => b.BorrowDate)
            .ToListAsync();

        return View(student);
    }
    // TODO: Create / Edit / Delete
}
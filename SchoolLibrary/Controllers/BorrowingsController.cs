using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolLibrary.Data;

namespace SchoolLibrary.Controllers;

public class BorrowingsController : Controller
{
    private readonly AppDbContext _db;
    public BorrowingsController(AppDbContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        var list = await _db.Borrowings
            .Include(b => b.Book)!.ThenInclude(b => b!.Authors)
            .Include(b => b.Student)
            .OrderByDescending(b => b.BorrowDate)
            .ToListAsync();

        return View(list);
    }

    public async Task<IActionResult> Details(int id)
    {
        var b = await _db.Borrowings
            .Include(x => x.Book)!.ThenInclude(x => x!.Authors)
            .Include(x => x.Student)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (b is null) return NotFound();
        return View(b);
    }
    // TODO: Create / Edit / Delete
}
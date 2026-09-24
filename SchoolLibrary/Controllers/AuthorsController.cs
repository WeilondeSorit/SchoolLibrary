using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolLibrary.Data;

namespace SchoolLibrary.Controllers;

public class AuthorsController : Controller
{
    private readonly AppDbContext _db;
    public AuthorsController(AppDbContext db) => _db = db;

    public async Task<IActionResult> Index() =>
        View(await _db.Authors.OrderBy(a => a.FullName).ToListAsync());

    public async Task<IActionResult> Details(int id)
    {
        var author = await _db.Authors
            .Include(a => a.Books)
            .FirstOrDefaultAsync(a => a.Id == id);
        if (author is null) return NotFound();
        return View(author);
    }
    // TODO: Create / Edit / Delete
}
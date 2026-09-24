using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolLibrary.Data;
using SchoolLibrary.Models;

namespace SchoolLibrary.Controllers;

public class BooksController : Controller
{
    private readonly AppDbContext _db;
    public BooksController(AppDbContext db) => _db = db;

    // GET: /books
    public async Task<IActionResult> Index(string? search, string? sort, string? genre, bool? onlyAvailable)
    {
        var books = _db.Books.Include(b => b.Authors).AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            books = books.Where(b => EF.Functions.ILike(b.Title, $"%{search}%")
                                  || b.Authors.Any(a => EF.Functions.ILike(a.FullName, $"%{search}%")));

        if (!string.IsNullOrWhiteSpace(genre))
            books = books.Where(b => b.Genre == genre);

        if (onlyAvailable == true)
            books = books.Where(b => b.AvailableCopies > 0);

        books = sort switch
        {
            "year_asc"  => books.OrderBy(b => b.Year),
            "year_desc" => books.OrderByDescending(b => b.Year),
            "title"     => books.OrderBy(b => b.Title),
            "author"    => books.OrderBy(b => b.Authors.Min(a => a.FullName)),
            _           => books.OrderBy(b => b.Id)
        };

        ViewBag.Search = search;
        ViewBag.Sort = sort;
        ViewBag.Genre = genre;
        ViewBag.OnlyAvailable = onlyAvailable;
        ViewBag.Genres = await _db.Books.Select(b => b.Genre).Distinct().OrderBy(g => g).ToListAsync();
        ViewBag.Role = HttpContext.Session.GetString("UserRole");

        return View(await books.ToListAsync());
    }

    // GET: /books/{id}
    public async Task<IActionResult> Details(int id)
    {
        var book = await _db.Books.Include(b => b.Authors).FirstOrDefaultAsync(b => b.Id == id);
        if (book is null) return NotFound();
        return View(book);
    }

    // GET: /books/create
    public IActionResult Create() => View(new Book());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Book book)
    {
        if (!ModelState.IsValid) return View(book);
        _db.Books.Add(book);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // GET: /books/edit/{id}
    public async Task<IActionResult> Edit(int id)
    {
        var book = await _db.Books.Include(b => b.Authors).FirstOrDefaultAsync(b => b.Id == id);
        if (book is null) return NotFound();
        return View(book);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Book form)
    {
        var book = await _db.Books.FindAsync(id);
        if (book is null) return NotFound();

        book.Title = form.Title;
        book.Genre = form.Genre;
        book.Year = form.Year;
        book.Isbn = form.Isbn;
        book.TotalCopies = form.TotalCopies;
        book.AvailableCopies = form.AvailableCopies;
        book.CoverUrl = form.CoverUrl;

        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // GET: /books/delete/{id}
    public async Task<IActionResult> Delete(int id)
    {
        var book = await _db.Books.Include(b => b.Authors).FirstOrDefaultAsync(b => b.Id == id);
        if (book is null) return NotFound();
        return View(book);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var book = await _db.Books.FindAsync(id);
        if (book is null) return NotFound();
        _db.Books.Remove(book);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // POST: /books/borrow/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Borrow(int id)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId is null) return RedirectToAction("Index", "Home");

        var student = await _db.Students.FirstOrDefaultAsync(s => s.UserId == userId);
        if (student is null) return Forbid();

        var book = await _db.Books.FindAsync(id);
        if (book is null) return NotFound();
        if (book.AvailableCopies <= 0) return RedirectToAction(nameof(Index));

        var today = DateTime.UtcNow.Date;
        _db.Borrowings.Add(new Borrowing
        {
            BookId = book.Id,
            StudentId = student.Id,
            BorrowDate = today,
            DueDate = today.AddMonths(1)
        });
        book.AvailableCopies--;

        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // POST: /books/return/{borrowingId}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Return(int id)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId is null) return RedirectToAction("Index", "Home");

        var student = await _db.Students.FirstOrDefaultAsync(s => s.UserId == userId);
        if (student is null) return Forbid();

        var borrowing = await _db.Borrowings
            .Include(b => b.Book)
            .FirstOrDefaultAsync(b => b.Id == id && b.StudentId == student.Id);

        if (borrowing is null) return NotFound();
        if (!borrowing.IsReturned)
        {
            borrowing.ReturnDate = DateTime.UtcNow.Date;
            if (borrowing.Book is not null &&
                borrowing.Book.AvailableCopies < borrowing.Book.TotalCopies)
            {
                borrowing.Book.AvailableCopies++;
            }
            await _db.SaveChangesAsync();
        }

        return RedirectToAction("Profile", "Account");
    }
}
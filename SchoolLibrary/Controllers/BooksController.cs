using Microsoft.AspNetCore.Mvc;
using SchoolLibrary.Models;

namespace SchoolLibrary.Controllers;

public class BooksController : Controller
{
    // GET: /books
    public IActionResult Index(string? search, string? sort, string? genre, bool? onlyAvailable)
    {
        var books = MockData.Books.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(search))
            books = books.Where(b => b.Title.Contains(search, StringComparison.OrdinalIgnoreCase)
                                  || b.AuthorName.Contains(search, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrWhiteSpace(genre))
            books = books.Where(b => b.Genre == genre);

        if (onlyAvailable == true)
            books = books.Where(b => b.AvailableCopies > 0);

        books = sort switch
        {
            "year_asc"   => books.OrderBy(b => b.Year),
            "year_desc"  => books.OrderByDescending(b => b.Year),
            "title"      => books.OrderBy(b => b.Title),
            "author"     => books.OrderBy(b => b.AuthorName),
            _            => books.OrderBy(b => b.Id)
        };

        ViewBag.Search = search;
        ViewBag.Sort = sort;
        ViewBag.Genre = genre;
        ViewBag.OnlyAvailable = onlyAvailable;
        ViewBag.Genres = MockData.Books.Select(b => b.Genre).Distinct().OrderBy(g => g).ToList();
        ViewBag.Role = HttpContext.Session.GetString("UserRole");

        return View(books.ToList());
    }

    // GET: /books/{id}
    public IActionResult Details(int id)
    {
        var book = MockData.Books.FirstOrDefault(b => b.Id == id);
        if (book is null) return NotFound();
        return View(book);
    }

    // GET: /books/create
    public IActionResult Create() => View(new Book());

    [HttpPost]
    public IActionResult Create(Book book) => RedirectToAction(nameof(Index)); // TODO: сохранение в БД

    // GET: /books/edit/{id}
    public IActionResult Edit(int id)
    {
        var book = MockData.Books.FirstOrDefault(b => b.Id == id);
        if (book is null) return NotFound();
        return View(book);
    }

    [HttpPost]
    public IActionResult Edit(Book book) => RedirectToAction(nameof(Index));

    // GET: /books/delete/{id}
    public IActionResult Delete(int id)
    {
        var book = MockData.Books.FirstOrDefault(b => b.Id == id);
        if (book is null) return NotFound();
        return View(book);
    }

    [HttpPost, ActionName("Delete")]
    public IActionResult DeleteConfirmed(int id) => RedirectToAction(nameof(Index));

    // POST: /books/borrow/{id}
    [HttpPost]
    public IActionResult Borrow(int id)
    {
        // TODO: реальное оформление выдачи
        return RedirectToAction(nameof(Index));
    }

    // POST: /books/return/{id}
    [HttpPost]
    public IActionResult Return(int id)
    {
        // TODO: реальное возвращение книги
        return RedirectToAction("Profile", "Account");
    }
}
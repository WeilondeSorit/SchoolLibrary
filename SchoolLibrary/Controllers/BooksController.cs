using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolLibrary.Data;
using SchoolLibrary.Models;
using SchoolLibrary.ViewModels;
using ClosedXML.Excel;

namespace SchoolLibrary.Controllers;

public class BooksController : Controller
{
    private readonly AppDbContext _db;
    private readonly IWebHostEnvironment _env;

    public BooksController(AppDbContext db, IWebHostEnvironment env)
    {
        _db = db;
        _env = env;
    }

    // ---------- Список ----------
    public async Task<IActionResult> Index(string? search, string? sort, string? genre, bool? onlyAvailable)
    {
        var role = HttpContext.Session.GetString("UserRole");
        var isLibrarian = role == "Librarian";

        var books = _db.Books.Include(b => b.Authors).AsQueryable();

        // Ученики не видят скрытые книги
        if (!isLibrarian)
            books = books.Where(b => !b.IsHidden);

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
        ViewBag.Genres = await _db.Books
            .Where(b => b.Genre != null && b.Genre != "")
            .Select(b => b.Genre!)
            .Distinct()
            .OrderBy(g => g)
            .ToListAsync();
        ViewBag.Role = role;

        return View(await books.ToListAsync());
    }

    // ---------- Детали ----------
    public async Task<IActionResult> Details(int id)
    {
        var book = await _db.Books.Include(b => b.Authors).FirstOrDefaultAsync(b => b.Id == id);
        if (book is null) return NotFound();

        ViewBag.Role = HttpContext.Session.GetString("UserRole");
        return View(book);
    }

    // ---------- Create ----------
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        ViewBag.Authors = await _db.Authors.OrderBy(a => a.FullName).ToListAsync();
        return View(new BookFormViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BookFormViewModel model)
    {
        if (model.AvailableCopies > model.TotalCopies)
            ModelState.AddModelError(nameof(model.AvailableCopies),
                "Доступных не может быть больше, чем всего экземпляров");

        if (!ModelState.IsValid)
        {
            ViewBag.Authors = await _db.Authors.OrderBy(a => a.FullName).ToListAsync();
            return View(model);
        }

        var book = new Book
        {
            Title = model.Title,
            Genre = model.Genre,
            Year = model.Year,
            Isbn = model.Isbn,
            TotalCopies = model.TotalCopies,
            AvailableCopies = model.AvailableCopies,
            CoverUrl = model.CoverUrl,
            IsHidden = model.IsHidden
        };

        // Загрузка файла (приоритет над URL)
        if (model.ImageFile is { Length: > 0 })
        {
            var fileName = Guid.NewGuid().ToString("N") + Path.GetExtension(model.ImageFile.FileName);
            var dir = Path.Combine(_env.WebRootPath, "uploads", "covers");
            Directory.CreateDirectory(dir);
            var fullPath = Path.Combine(dir, fileName);
            await using var stream = System.IO.File.Create(fullPath);
            await model.ImageFile.CopyToAsync(stream);
            book.CoverUrl = "/uploads/covers/" + fileName;
        }

        if (model.AuthorIds.Count > 0)
        {
            var authors = await _db.Authors.Where(a => model.AuthorIds.Contains(a.Id)).ToListAsync();
            foreach (var a in authors) book.Authors.Add(a);
        }

        _db.Books.Add(book);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // ---------- Edit ----------
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var book = await _db.Books.Include(b => b.Authors).FirstOrDefaultAsync(b => b.Id == id);
        if (book is null) return NotFound();

        ViewBag.Authors = await _db.Authors.OrderBy(a => a.FullName).ToListAsync();

        return View(new BookFormViewModel
        {
            Id = book.Id,
            Title = book.Title,
            Genre = book.Genre,
            Year = book.Year,
            Isbn = book.Isbn,
            TotalCopies = book.TotalCopies,
            AvailableCopies = book.AvailableCopies,
            CoverUrl = book.CoverUrl,
            IsHidden = book.IsHidden,
            AuthorIds = book.Authors.Select(a => a.Id).ToList()
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, BookFormViewModel model)
    {
        if (id != model.Id) return BadRequest();

        if (model.AvailableCopies > model.TotalCopies)
            ModelState.AddModelError(nameof(model.AvailableCopies),
                "Доступных не может быть больше, чем всего экземпляров");

        if (!ModelState.IsValid)
        {
            ViewBag.Authors = await _db.Authors.OrderBy(a => a.FullName).ToListAsync();
            return View(model);
        }

        var book = await _db.Books.Include(b => b.Authors).FirstOrDefaultAsync(b => b.Id == id);
        if (book is null) return NotFound();

        book.Title = model.Title;
        book.Genre = model.Genre;
        book.Year = model.Year;
        book.Isbn = model.Isbn;
        book.TotalCopies = model.TotalCopies;
        book.AvailableCopies = model.AvailableCopies;
        book.IsHidden = model.IsHidden;

        if (model.ImageFile is { Length: > 0 })
        {
            var fileName = Guid.NewGuid().ToString("N") + Path.GetExtension(model.ImageFile.FileName);
            var dir = Path.Combine(_env.WebRootPath, "uploads", "covers");
            Directory.CreateDirectory(dir);
            var fullPath = Path.Combine(dir, fileName);
            await using var stream = System.IO.File.Create(fullPath);
            await model.ImageFile.CopyToAsync(stream);
            book.CoverUrl = "/uploads/covers/" + fileName;
        }
        else if (!string.IsNullOrWhiteSpace(model.CoverUrl))
        {
            book.CoverUrl = model.CoverUrl;
        }

        book.Authors.Clear();
        if (model.AuthorIds.Count > 0)
        {
            var authors = await _db.Authors.Where(a => model.AuthorIds.Contains(a.Id)).ToListAsync();
            foreach (var a in authors) book.Authors.Add(a);
        }

        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Details), new { id = book.Id });
    }

    // ---------- Hide / Unhide ----------
    [HttpGet]
    public async Task<IActionResult> Hide(int id)
    {
        var book = await _db.Books.Include(b => b.Authors).FirstOrDefaultAsync(b => b.Id == id);
        if (book is null) return NotFound();

        ViewBag.ActiveBorrowings = await _db.Borrowings
            .CountAsync(b => b.BookId == id && b.ReturnDate == null);
        return View(book);
    }

    [HttpPost, ActionName("Hide")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> HideConfirmed(int id)
    {
        var book = await _db.Books.FindAsync(id);
        if (book is null) return NotFound();
        book.IsHidden = true;
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Unhide(int id)
    {
        var book = await _db.Books.FindAsync(id);
        if (book is null) return NotFound();
        book.IsHidden = false;
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // ---------- Взять / вернуть ----------
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Borrow(int id)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId is null) return RedirectToAction("Index", "Home");

        var student = await _db.Students.FirstOrDefaultAsync(s => s.UserId == userId);
        if (student is null) return Forbid();

        var book = await _db.Books.FindAsync(id);
        if (book is null || book.IsHidden) return NotFound();
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
        return RedirectToAction("Profile", "Account");
    }
// ---------- Импорт из Excel ----------
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Import(IFormFile? excelFile)
{
    if (excelFile is null || excelFile.Length == 0)
    {
        TempData["ImportError"] = "Файл не выбран";
        return RedirectToAction(nameof(Create));
    }

    if (!excelFile.FileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
    {
        TempData["ImportError"] = "Поддерживается только формат .xlsx";
        return RedirectToAction(nameof(Create));
    }

    int imported = 0;
    int skipped = 0;
    var errors = new List<string>();

    // Все существующие ISBN из БД (в нижнем регистре для надёжного сравнения)
    var existingIsbns = (await _db.Books
            .Where(b => b.Isbn != null && b.Isbn != "")
            .Select(b => b.Isbn!)
            .ToListAsync())
        .ToHashSet(StringComparer.OrdinalIgnoreCase);

    var seenInFile = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

    var toInsert = new List<Book>();

    try
    {
        using var stream = excelFile.OpenReadStream();
        using var wb = new XLWorkbook(stream);
        var ws = wb.Worksheets.First();

        var lastRow = ws.LastRowUsed()?.RowNumber() ?? 1;

        for (int r = 2; r <= lastRow; r++)
        {
            var row = ws.Row(r);
            var title = row.Cell(1).GetString().Trim();

            if (string.IsNullOrWhiteSpace(title))
                continue;

            try
            {
                var isbn = NullIfEmpty(row.Cell(4).GetString());

                if (isbn != null)
                {
                    if (existingIsbns.Contains(isbn))
                    {
                        skipped++;
                        errors.Add($"строка {r}: ISBN {isbn} уже есть в базе — пропущено");
                        continue;
                    }
                    if (seenInFile.Contains(isbn))
                    {
                        skipped++;
                        errors.Add($"строка {r}: ISBN {isbn} дублируется в файле — пропущено");
                        continue;
                    }
                    seenInFile.Add(isbn);
                }

                var total = row.Cell(5).IsEmpty() ? 1 : (int)row.Cell(5).GetDouble();
                var avail = row.Cell(6).IsEmpty() ? total : (int)row.Cell(6).GetDouble();

                if (total < 0) total = 0;
                if (avail < 0) avail = 0;
                if (avail > total) avail = total;

                toInsert.Add(new Book
                {
                    Title = title,
                    Genre = NullIfEmpty(row.Cell(2).GetString()),
                    Year = row.Cell(3).IsEmpty() ? null : (int?)row.Cell(3).GetDouble(),
                    Isbn = isbn,
                    TotalCopies = total,
                    AvailableCopies = avail,
                    CoverUrl = NullIfEmpty(row.Cell(7).GetString()),
                    IsHidden = ParseBool(row.Cell(8).GetString())
                });
                imported++;
            }
            catch (Exception ex)
            {
                errors.Add($"строка {r}: {ex.Message}");
            }
        }
    }
    catch (Exception ex)
    {
        TempData["ImportError"] = "Не удалось прочитать файл: " + ex.Message;
        return RedirectToAction(nameof(Create));
    }

    if (toInsert.Count > 0)
    {
        try
        {
            _db.Books.AddRange(toInsert);
            await _db.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            TempData["ImportError"] = "Ошибка при сохранении: " + ex.Message;
            return RedirectToAction(nameof(Create));
        }
    }

    var summary = $"Импортировано книг: {imported}";
    if (skipped > 0) summary += $", пропущено из-за дублей: {skipped}";
    TempData["ImportSuccess"] = summary;

    if (errors.Count > 0)
        TempData["ImportError"] = "Детали: " + string.Join("; ", errors.Take(10));

    return RedirectToAction(nameof(Create));
}

// ---------- Шаблон для скачивания ----------
[HttpGet]
public IActionResult DownloadTemplate()
{
    using var wb = new XLWorkbook();
    var ws = wb.Worksheets.Add("Книги");

    var headers = new[]
    {
        "Название", "Жанр", "Год", "ISBN",
        "Всего экземпляров", "Доступно", "Ссылка на обложку", "Скрыта"
    };
    for (int i = 0; i < headers.Length; i++)
        ws.Cell(1, i + 1).Value = headers[i];

    ws.Row(1).Style.Font.Bold = true;
    ws.Row(1).Style.Fill.BackgroundColor = XLColor.LightGray;

    // Пример БЕЗ ISBN — чтобы не конфликтовать с существующими книгами
    ws.Cell(2, 1).Value = "Пример: Название книги";
    ws.Cell(2, 2).Value = "Роман";
    ws.Cell(2, 3).Value = 2000;
    // ISBN (D) намеренно оставлен пустым
    ws.Cell(2, 5).Value = 3;
    ws.Cell(2, 6).Value = 3;
    ws.Cell(2, 7).Value = "https://example.com/cover.jpg";
    ws.Cell(2, 8).Value = "нет";

    ws.Columns().AdjustToContents();

    using var ms = new MemoryStream();
    wb.SaveAs(ms);

    return File(
        ms.ToArray(),
        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        "books-template.xlsx");
}
// ---------- Хелперы ----------
private static string? NullIfEmpty(string s)
    => string.IsNullOrWhiteSpace(s) ? null : s.Trim();

private static bool ParseBool(string s)
{
    s = (s ?? "").Trim().ToLowerInvariant();
    return s is "да" or "true" or "1" or "yes" or "y" or "истина" or "скрыта";
}
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
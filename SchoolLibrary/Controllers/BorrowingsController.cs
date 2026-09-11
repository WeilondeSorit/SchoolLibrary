using Microsoft.AspNetCore.Mvc;

namespace SchoolLibrary.Controllers;

public class BorrowingsController : Controller
{
    public IActionResult Index() => View(MockData.Borrowings);

    public IActionResult Details(int id)
    {
        var b = MockData.Borrowings.FirstOrDefault(x => x.Id == id);
        if (b is null) return NotFound();
        return View(b);
    }
    // TODO: Create / Edit / Delete
}
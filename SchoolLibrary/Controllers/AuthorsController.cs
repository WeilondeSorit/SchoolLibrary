using Microsoft.AspNetCore.Mvc;

namespace SchoolLibrary.Controllers;

public class AuthorsController : Controller
{
    public IActionResult Index() => View(MockData.Authors);

    public IActionResult Details(int id)
    {
        var author = MockData.Authors.FirstOrDefault(a => a.Id == id);
        if (author is null) return NotFound();
        return View(author);
    }
    // TODO: Create / Edit / Delete
}
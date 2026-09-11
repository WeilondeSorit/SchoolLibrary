using Microsoft.AspNetCore.Mvc;

namespace SchoolLibrary.Controllers;

public class StudentsController : Controller
{
    public IActionResult Index() => View(MockData.Students);

    public IActionResult Details(int id)
    {
        var student = MockData.Students.FirstOrDefault(s => s.Id == id);
        if (student is null) return NotFound();
        return View(student);
    }
    // TODO: Create / Edit / Delete
}
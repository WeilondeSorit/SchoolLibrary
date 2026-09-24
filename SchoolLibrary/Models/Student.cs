namespace SchoolLibrary.Models;

public class Student
{
    public int Id { get; set; }
    public int? UserId { get; set; }
    public User? User { get; set; }

    public string FullName { get; set; } = "";
    public string? ClassName { get; set; }
    public string? Email { get; set; }
}
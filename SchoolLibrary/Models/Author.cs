namespace SchoolLibrary.Models;

public class Author
{
    public int Id { get; set; }
    public string FullName { get; set; } = "";
    public string? Country { get; set; }
    public string? Bio { get; set; }

    public ICollection<Book> Books { get; set; } = new List<Book>();
}
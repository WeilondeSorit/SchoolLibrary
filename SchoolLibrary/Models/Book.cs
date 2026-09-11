namespace SchoolLibrary.Models;

public class Book
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public int AuthorId { get; set; }
    public string AuthorName { get; set; } = "";
    public string Genre { get; set; } = "";
    public int Year { get; set; }
    public string Isbn { get; set; } = "";
    public int TotalCopies { get; set; }
    public int AvailableCopies { get; set; }
    public string CoverUrl { get; set; } = "";
}
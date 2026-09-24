using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolLibrary.Models;

public class Book
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string? Genre { get; set; }
    public int? Year { get; set; }
    public string? Isbn { get; set; }
    public int TotalCopies { get; set; }
    public int AvailableCopies { get; set; }
    public string? CoverUrl { get; set; }

    public ICollection<Author> Authors { get; set; } = new List<Author>();

    [NotMapped]
    public string AuthorName => Authors.FirstOrDefault()?.FullName ?? "—";
}
namespace SchoolLibrary.Models;

public class Borrowing
{
    public int Id { get; set; }
    public int BookId { get; set; }
    public string BookTitle { get; set; } = "";
    public string AuthorName { get; set; } = "";
    public int StudentId { get; set; }
    public string StudentName { get; set; } = "";
    public DateTime BorrowDate { get; set; }
    public DateTime DueDate { get; set; }   // BorrowDate + 1 месяц
    public DateTime? ReturnDate { get; set; }

    public bool IsReturned => ReturnDate.HasValue;
    public bool IsOverdue => !IsReturned && DateTime.Now > DueDate;
}
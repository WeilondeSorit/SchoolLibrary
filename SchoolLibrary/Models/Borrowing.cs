namespace SchoolLibrary.Models;

public class Borrowing
{
    public int Id { get; set; }
    public int BookId { get; set; }
    public Book? Book { get; set; }

    public int StudentId { get; set; }
    public Student? Student { get; set; }

    public DateTime BorrowDate { get; set; }
    public DateTime DueDate { get; set; }   // BorrowDate + 1 месяц
    public DateTime? ReturnDate { get; set; }

    public bool IsReturned => ReturnDate.HasValue;
    public bool IsOverdue => !IsReturned && DateTime.Now > DueDate;
}
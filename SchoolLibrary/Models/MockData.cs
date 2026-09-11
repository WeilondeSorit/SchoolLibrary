using SchoolLibrary.Models;

public static class MockData
{
    public static List<Author> Authors { get; } = new()
    {
        new Author { Id = 1, FullName = "Лев Толстой",       Country = "Россия",   Bio = "Русский писатель, классик мировой литературы." },
        new Author { Id = 2, FullName = "Фёдор Достоевский", Country = "Россия",   Bio = "Русский писатель и мыслитель." },
        new Author { Id = 3, FullName = "Джоан Роулинг",     Country = "Великобритания", Bio = "Автор серии книг о Гарри Поттере." },
        new Author { Id = 4, FullName = "Антуан де Сент-Экзюпери", Country = "Франция", Bio = "Писатель и лётчик." }
    };

    public static List<Book> Books { get; } = new()
    {
        new Book { Id = 1, Title = "Война и мир",   AuthorId = 1, AuthorName = "Лев Толстой",       Genre = "Роман",     Year = 1869, Isbn = "978-5-04-116000-1", TotalCopies = 5, AvailableCopies = 3 },
        new Book { Id = 2, Title = "Преступление и наказание", AuthorId = 2, AuthorName = "Фёдор Достоевский", Genre = "Роман", Year = 1866, Isbn = "978-5-04-116001-8", TotalCopies = 4, AvailableCopies = 1 },
        new Book { Id = 3, Title = "Гарри Поттер и философский камень", AuthorId = 3, AuthorName = "Джоан Роулинг", Genre = "Фэнтези", Year = 1997, Isbn = "978-5-389-07435-4", TotalCopies = 8, AvailableCopies = 5 },
        new Book { Id = 4, Title = "Маленький принц", AuthorId = 4, AuthorName = "Антуан де Сент-Экзюпери", Genre = "Сказка", Year = 1943, Isbn = "978-5-699-66295-1", TotalCopies = 6, AvailableCopies = 6 },
        new Book { Id = 5, Title = "Анна Каренина", AuthorId = 1, AuthorName = "Лев Толстой", Genre = "Роман", Year = 1877, Isbn = "978-5-04-116002-5", TotalCopies = 3, AvailableCopies = 0 },
    };

    public static List<Student> Students { get; } = new()
    {
        new Student { Id = 1, FullName = "Иванов Иван",  ClassName = "10А", Email = "ivanov@school.ru" },
        new Student { Id = 2, FullName = "Петрова Анна", ClassName = "9Б",  Email = "petrova@school.ru" },
    };

    public static List<Borrowing> Borrowings { get; } = new()
    {
        new Borrowing
        {
            Id = 1, BookId = 2, BookTitle = "Преступление и наказание", AuthorName = "Фёдор Достоевский",
            StudentId = 1, StudentName = "Иванов Иван",
            BorrowDate = DateTime.Today.AddDays(-10),
            DueDate = DateTime.Today.AddDays(-10).AddMonths(1)
        },
        new Borrowing
        {
            Id = 2, BookId = 3, BookTitle = "Гарри Поттер и философский камень", AuthorName = "Джоан Роулинг",
            StudentId = 1, StudentName = "Иванов Иван",
            BorrowDate = DateTime.Today.AddDays(-35),
            DueDate = DateTime.Today.AddDays(-35).AddMonths(1)
        }
    };
}
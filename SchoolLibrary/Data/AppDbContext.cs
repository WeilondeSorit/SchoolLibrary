using Microsoft.EntityFrameworkCore;
using SchoolLibrary.Models;

namespace SchoolLibrary.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Role> Roles => Set<Role>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Author> Authors => Set<Author>();
    public DbSet<Book> Books => Set<Book>();
    public DbSet<Student> Students => Set<Student>();
    public DbSet<Borrowing> Borrowings => Set<Borrowing>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        // ---------- roles ----------
        mb.Entity<Role>(e =>
        {
            e.ToTable("roles");
            e.Property(r => r.Id).HasColumnName("id");
            e.Property(r => r.Name).HasColumnName("name");
        });

        // ---------- users ----------
        mb.Entity<User>(e =>
        {
            e.ToTable("users");
            e.Property(u => u.Id).HasColumnName("id");
            e.Property(u => u.Login).HasColumnName("login");
            e.Property(u => u.PasswordHash).HasColumnName("password_hash");
            e.Property(u => u.DisplayName).HasColumnName("display_name");
            e.Property(u => u.RoleId).HasColumnName("role_id");
            e.Property(u => u.CreatedAt).HasColumnName("created_at");
        });

        // ---------- authors ----------
        mb.Entity<Author>(e =>
        {
            e.ToTable("authors");
            e.Property(a => a.Id).HasColumnName("id");
            e.Property(a => a.FullName).HasColumnName("full_name");
            e.Property(a => a.Country).HasColumnName("country");
            e.Property(a => a.Bio).HasColumnName("bio");
        });

        // ---------- books ----------
        mb.Entity<Book>(e =>
        {
            e.ToTable("books");
            e.Property(b => b.Id).HasColumnName("id");
            e.Property(b => b.Title).HasColumnName("title");
            e.Property(b => b.Genre).HasColumnName("genre");
            e.Property(b => b.Year).HasColumnName("year");
            e.Property(b => b.Isbn).HasColumnName("isbn");
            e.Property(b => b.TotalCopies).HasColumnName("total_copies");
            e.Property(b => b.AvailableCopies).HasColumnName("available_copies");
            e.Property(b => b.CoverUrl).HasColumnName("cover_url");
        });

        // ---------- students ----------
        mb.Entity<Student>(e =>
        {
            e.ToTable("students");
            e.Property(s => s.Id).HasColumnName("id");
            e.Property(s => s.UserId).HasColumnName("user_id");
            e.Property(s => s.FullName).HasColumnName("full_name");
            e.Property(s => s.ClassName).HasColumnName("class_name");
            e.Property(s => s.Email).HasColumnName("email");
        });

        // ---------- borrowings ----------
        mb.Entity<Borrowing>(e =>
        {
            e.ToTable("borrowings");
            e.Property(b => b.Id).HasColumnName("id");
            e.Property(b => b.BookId).HasColumnName("book_id");
            e.Property(b => b.StudentId).HasColumnName("student_id");
            e.Property(b => b.BorrowDate).HasColumnName("borrow_date");
            e.Property(b => b.DueDate).HasColumnName("due_date");
            e.Property(b => b.ReturnDate).HasColumnName("return_date");
        });

        // ---------- связи ----------

        // M:N книга ↔ автор
        mb.Entity<Book>()
          .HasMany(b => b.Authors)
          .WithMany(a => a.Books)
          .UsingEntity<Dictionary<string, object>>(
              "book_authors",
              j => j.HasOne<Author>().WithMany().HasForeignKey("author_id"),
              j => j.HasOne<Book>().WithMany().HasForeignKey("book_id"),
              j => { j.ToTable("book_authors"); j.HasKey("book_id", "author_id"); });

        // User → Role
        mb.Entity<User>()
          .HasOne(u => u.Role)
          .WithMany(r => r.Users)
          .HasForeignKey(u => u.RoleId);

        // User ↔ Student (1:1)
        mb.Entity<Student>()
          .HasOne(s => s.User)
          .WithOne(u => u.Student)
          .HasForeignKey<Student>(s => s.UserId);

        // Borrowing → Book / Student
        mb.Entity<Borrowing>()
          .HasOne(b => b.Book).WithMany().HasForeignKey(b => b.BookId);
        mb.Entity<Borrowing>()
          .HasOne(b => b.Student).WithMany().HasForeignKey(b => b.StudentId);
    }
}
# The School Libray Site

for course work


A web application for managing a school library. It supports two roles — Librarian and Student — with role‑based access to books, authors, students, and borrowings. The project is built with ASP.NET Core MVC, Entity Framework Core, and PostgreSQL, and is fully containerised with Docker.

# Features

- Authentication & Registration — Login with BCrypt‑hashed passwords, role‑based access, and self‑registration for students and librarians.
- Books Catalogue - View all books with search, sorting (by title, author, year), and filtering by genre/availability.
- Librarian Actions - Add, edit, and delete books, authors, students, and borrowings. <-- in progress
- Student Actions - Borrow available books (due date is automatically set to one month from the borrow date), return books early, and view personal borrowed books with due dates.
- Profile Page - Shows user information and, for students, a list of currently borrowed books with the option to return them.
- Error Pages - Custom 404.
- Database - PostgreSQL schema normalised to Third Normal Form (3NF).

# Tech Stack

- Backend: ASP.NET Core 8.0 MVC, C#
- ORM: Entity Framework Core 8 with Npgsql provider
- Database: PostgreSQL 15
- Authentication: Session‑based, BCrypt for password hashing
- Frontend: Razor Views, HTML5, CSS3, vanilla JavaScript
- Containerisation: Docker, Docker Compose
- Other: EFCore.NamingConventions for snake_case mapping, Data Protection keys persisted in a volume.

# Project Structure

```
tree -L 4
.
├── README.md
└── SchoolLibrary
    ├── appsettings.Development.json
    ├── appsettings.json
    ├── bin
    │   └── Debug
    │       └── net8.0
    ├── Controllers
    │   ├── AccountController.cs
    │   ├── AuthorsController.cs
    │   ├── BooksController.cs
    │   ├── BorrowingsController.cs
    │   ├── HomeController.cs
    │   └── StudentsController.cs
    ├── Data
    │   └── AppDbContext.cs
    ├── docker-compose.yml
    ├── Dockerfile
    ├── init.sql
    ├── Models
    │   ├── Author.cs
    │   ├── Book.cs
    │   ├── Borrowing.cs
    │   ├── ErrorViewModel.cs
    │   ├── Role.cs
    │   ├── Student.cs
    │   └── User.cs
    ├── obj
    │   ├── Debug
    │   │   └── net8.0
    │   ├── project.assets.json
    │   ├── project.nuget.cache
    │   ├── SchoolLibrary.csproj.nuget.dgspec.json
    │   ├── SchoolLibrary.csproj.nuget.g.props
    │   └── SchoolLibrary.csproj.nuget.g.targets
    ├── Program.cs
    ├── Properties
    │   └── launchSettings.json
    ├── SchoolLibrary.csproj
    ├── Services
    │   └── PasswordHasher.cs
    ├── ViewModels
    │   └── RegisterViewModel.cs
    ├── Views
    │   ├── Account
    │   │   ├── Profile.cshtml
    │   │   └── Register.cshtml
    │   ├── Authors
    │   │   ├── Details.cshtml
    │   │   └── Index.cshtml
    │   ├── Books
    │   │   ├── Create.cshtml
    │   │   ├── Delete.cshtml
    │   │   ├── Details.cshtml
    │   │   ├── Edit.cshtml
    │   │   └── Index.cshtml
    │   ├── Borrowings
    │   │   ├── Details.cshtml
    │   │   └── Index.cshtml
    │   ├── Home
    │   │   ├── Index.cshtml
    │   │   ├── NotFound.cshtml
    │   │   └── Privacy.cshtml
    │   ├── Shared
    │   │   ├── Error.cshtml
    │   │   ├── _Layout.cshtml
    │   │   ├── _Layout.cshtml.css
    │   │   ├── _LayoutLogin.cshtml
    │   │   └── _ValidationScriptsPartial.cshtml
    │   ├── Students
    │   │   ├── Details.cshtml
    │   │   └── Index.cshtml
    │   ├── _ViewImports.cshtml
    │   └── _ViewStart.cshtml
    └── wwwroot
        ├── css
        │   ├── books.css
        │   ├── forms.css
        │   ├── layout.css
        │   ├── login.css
        │   ├── notfound.css
        │   ├── profile.css
        │   └── site.css
        ├── favicon.ico
        ├── images
        │   ├── bin.png
        │   ├── login-bg.jpg
        │   ├── logo.png
        │   ├── pencil.png
        ├── js
        │   └── site.js
        └── lib
            ├── bootstrap
            ├── jquery
            ├── jquery-validation
            └── jquery-validation-unobtrusive

31 directories, 70 files
```
# How to run

- Install Docker Compose
- Run
  ```bash
  git clone https://github.com/WeilondeSorit/SchoolLibrary.git && cd SchoolLibrary/SchoolLibrary
  ```
- Run
  ```bash
  docker compose up --build
  ```
- Open in your browser
  ```
  http://localhost:5000
  ```
- Register or log in

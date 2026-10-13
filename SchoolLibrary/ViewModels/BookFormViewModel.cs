using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace SchoolLibrary.ViewModels;

public class BookFormViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Введите название книги")]
    [StringLength(300)]
    [Display(Name = "Название")]
    public string Title { get; set; } = "";

    [StringLength(100)]
    [Display(Name = "Жанр")]
    public string? Genre { get; set; }

    [Range(0, 3000, ErrorMessage = "Год от 0 до 3000")]
    [Display(Name = "Год издания")]
    public int? Year { get; set; }

    [StringLength(20)]
    [Display(Name = "ISBN")]
    public string? Isbn { get; set; }

    [Range(0, 10000)]
    [Display(Name = "Всего экземпляров")]
    public int TotalCopies { get; set; } = 1;

    [Range(0, 10000)]
    [Display(Name = "Доступно сейчас")]
    public int AvailableCopies { get; set; } = 1;

    [StringLength(500)]
    [Display(Name = "Ссылка на обложку (URL)")]
    public string? CoverUrl { get; set; }

    [Display(Name = "…или загрузите файл")]
    public IFormFile? ImageFile { get; set; }

    [Display(Name = "Скрыть от учеников")]
    public bool IsHidden { get; set; }

    [Display(Name = "Авторы")]
    public List<int> AuthorIds { get; set; } = new();
}
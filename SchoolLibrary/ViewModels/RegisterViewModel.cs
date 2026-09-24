using System.ComponentModel.DataAnnotations;

namespace SchoolLibrary.ViewModels;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Введите логин")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Логин: от 3 до 100 символов")]
    public string Login { get; set; } = "";

    [Required(ErrorMessage = "Введите пароль")]
    [StringLength(100, MinimumLength = 5, ErrorMessage = "Пароль: минимум 5 символов")]
    public string Password { get; set; } = "";

    [Compare(nameof(Password), ErrorMessage = "Пароли не совпадают")]
    public string PasswordConfirm { get; set; } = "";

    [Required(ErrorMessage = "Введите ФИО")]
    public string DisplayName { get; set; } = "";

    [Required(ErrorMessage = "Выберите роль")]
    public string Role { get; set; } = "Student";

    public string? ClassName { get; set; }
    public string? Email { get; set; }
}
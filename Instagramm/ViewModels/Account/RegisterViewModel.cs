using System.ComponentModel.DataAnnotations;
using Instagramm.Validation;

namespace Instagramm.ViewModels.Account;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Поле обязательно для заполнения")]
    [StringLength(20, MinimumLength = 3, ErrorMessage = "Длина должна быть от 3 до 20 символов")]
    [Display(Name = "Login")]
    [RegularExpression(@"^[a-zA-Z0-9!@#$%^&*]+$", ErrorMessage = "Логин должен содержать только латинские буквы, цифры и символы: !@#$%^&*")]
    public string UserName { get; set; }
    
    [Required(ErrorMessage = "Поле обязательно для заполнения")]
    [Display(Name = "Email")]
    [RegularExpression(@"^[\w\.-]+@[\w\.-]+\.\w+$", ErrorMessage = "НЕ валидный адрес электронной почты")]
    public string Email { get; set; }

   
    // [FileExtensions(Extensions = "jpg,jpeg,png", ErrorMessage = "Пожалуйста, выберите файл с расширением .jpg, .jpeg или .png")]
    [MaxFileSize(10 * 1024 * 1024, ErrorMessage = "Максимальный размер файла - 10 МБ")]
    [Display(Name = "Загрузить изображение")]
    public IFormFile? AvatarFile { get; set; }
    
    public string? AvatarFileName { get; set; }
    
    [Required(ErrorMessage = "Поле обязательно для заполнения")]
    [DataType(DataType.Password)]
    [Display(Name = "Пароль")]
    public string Password { get; set; }
    
    [Required(ErrorMessage = "Поле обязательно для заполнения")]
    [DataType(DataType.Password)]
    [Display(Name = "Подтвердить пароль")]
    [Compare("Password", ErrorMessage = "Пароли не совпадают")]
    public string PasswordConfirm { get; set; }
}
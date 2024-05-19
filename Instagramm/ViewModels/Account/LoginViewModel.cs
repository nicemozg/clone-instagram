using System.ComponentModel.DataAnnotations;

namespace Instagramm.ViewModels.Account;

public class LoginViewModel
{
    [Required(ErrorMessage = "Поле обязательно для заполнения")]
    [Display(Name = "Электронный адресс или логин")]
    public string EmailOrUserName { get; set; }

    [Required(ErrorMessage = "Поле обязательно для заполнения")]
    [DataType(DataType.Password)]
    [Display(Name = "Пароль")]
    public string Password { get; set; }

    [Display(Name = "Запомнить")]
    public bool RememberMe { get; set; }

    public string? ReturnUrl { get; set; }
}
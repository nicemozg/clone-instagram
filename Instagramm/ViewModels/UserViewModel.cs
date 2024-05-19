using System.ComponentModel.DataAnnotations;
using Instagramm.Enum;
using Instagramm.Models;
using Instagramm.Validation;

namespace Instagramm.ViewModels;

public class UserViewModel
{
    public string Id { get; set; }
    [MaxFileSize(10 * 1024 * 1024, ErrorMessage = "Максимальный размер файла - 10 МБ")]
    [Display(Name = "Загрузить изображение")]
    public IFormFile? AvatarFile { get; set; }
    public string? AvatarFileName { get; set; }
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public string? GenderInfo { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public Gender? Gender { get; set; }
    
    
    

    public UserViewModel()
    {
    }
}
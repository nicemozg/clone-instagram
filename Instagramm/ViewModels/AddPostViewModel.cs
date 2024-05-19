using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Instagramm.Models;
using Instagramm.Validation;

namespace Instagramm.ViewModels;

public class AddPostViewModel
{
    [Display(Name = "Тут можно добавить описание")]
    public string? Description { get; set; }

    public string? UserId { get; set; }
    
    [Required]
    // [FileExtensions(Extensions = ".jpg,.jpeg,.png", ErrorMessage = "Пожалуйста, выберите файл с расширением .jpg, .jpeg или .png")]
    [MaxFileSize(10 * 1024 * 1024, ErrorMessage = "Максимальный размер файла - 10 МБ")]
    [Display(Name = "Загрузить изображение")]
    public IFormFile PostPictureFile { get; set; }
    
    [Display(Name = "Изображение")]
    public string? PostFileName { get; set; }
    
}
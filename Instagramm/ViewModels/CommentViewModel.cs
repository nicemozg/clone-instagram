using System.ComponentModel.DataAnnotations.Schema;
using Instagramm.Models;

namespace Instagramm.ViewModels;

public class CommentViewModel
{
    public int Id { get; set; }
    public string? AvatarFileName { get; set; }
    
    public string? UserName { get; set; }
    
    public string? Description { get; set; }
    
    public DateTime? Creation { get; set; }

    public string? UserId { get; set; }
    
    public string CurrentUserId { get; set; }
    public CommentViewModel()
    {
    }
}
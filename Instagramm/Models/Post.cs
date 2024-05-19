using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Instagramm.Models;

public class Post
{
    public int Id { get; set; }
    
    public string? Description { get; set; }

    public int LikesCount { get; set; }
    public int CommentsCount { get; set; }
    
    [Display(Name = "Изображение")]
    public string? PostFileName { get; set; }
    
    public DateTime Creation { get; set; }

    public string UserId { get; set; }
    [ForeignKey("UserId")]
    public User User { get; set; }
}
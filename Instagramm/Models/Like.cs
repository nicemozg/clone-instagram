using System.ComponentModel.DataAnnotations.Schema;

namespace Instagramm.Models;

public class Like
{
    public int Id { get; set; }
    
    public DateTime Creation { get; set; }
    
    public string UserId { get; set; }
    [ForeignKey("UserId")]
    public User User { get; set; }

    public int PostId { get; set; }
    [ForeignKey("PostId")]
    public Post Post { get; set; }
}
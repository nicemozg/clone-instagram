using System.ComponentModel.DataAnnotations.Schema;

namespace Instagramm.Models;

public class Follower
{
    public int Id { get; set; }
    public string FollowerId { get; set; } //ID того кто на меня подписывается
    public string FollowedId { get; set; } //ID на кого я подписываюсь
    
    public DateTime Creation { get; set; }
    
    [ForeignKey("FollowerId")]
    public User FollowerUser { get; set; } // Пользователь, который подписывается на вас

    [ForeignKey("FollowedId")]
    public User FollowedUser { get; set; } // Пользователь, на которого вы подписаны
}
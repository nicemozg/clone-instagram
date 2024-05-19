using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Instagramm.Enum;
using Microsoft.AspNetCore.Identity;

namespace Instagramm.Models;

public class User : IdentityUser
{
    
    public string AvatarFileName { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public Gender? Gender { get; set; }
    public DateTime Creation { get; set; }

    public User()
    {
    }
}
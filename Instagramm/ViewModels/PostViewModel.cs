using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Instagramm.Models;

namespace Instagramm.ViewModels;

public class PostViewModel
{
    public int Id { get; set; }
    public string? UserId { get; set; }
    public string? UserName { get; set; }
    public int LikesCount { get; set; }
    public int CommentsCount { get; set; }
    public string AvatarFileName { get; set; }
    public string? PostFileName { get; set; }
    public string? DescriptionForComment { get; set; }
    public string? DescriptionForPost { get; set; }
    public DateTime Creation { get; set; }

    public PostViewModel()
    {
    }

    public PostViewModel(Post post)
    {
        Id = post.Id;
        LikesCount = post.LikesCount;
        CommentsCount = post.CommentsCount;
        PostFileName = post.PostFileName;
    }
}
namespace Instagramm.ViewModels;

public class DeletePostViewModel
{
    public int PostId { get; set; }
    public string PostFileName { get; set; }
    
    public int LikesCount { get; set; }
    public int CommentsCount { get; set; }
    public string? DescriptionForPost { get; set; }
    
}
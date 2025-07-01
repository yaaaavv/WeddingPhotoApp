public class Comment
{
    public int CommentId { get; set; }
    public int PhotoId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public DateTime PostDate { get; set; }
    public Photo? Photo { get; set; }
}
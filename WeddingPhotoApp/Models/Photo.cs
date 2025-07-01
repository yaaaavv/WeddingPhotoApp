public class Photo
{
    public int PhotoId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public DateTime UploadDate { get; set; }
    public ICollection<Comment>? Comments { get; set; }
}
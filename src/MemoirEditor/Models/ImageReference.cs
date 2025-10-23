namespace MemoirEditor.Models;

/// <summary>
/// Reference to an embedded image in a block
/// </summary>
public class ImageReference
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public byte[]? ImageData { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public string Caption { get; set; } = string.Empty;
}

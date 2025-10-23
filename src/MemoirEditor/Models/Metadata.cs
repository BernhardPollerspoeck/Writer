namespace MemoirEditor.Models;

/// <summary>
/// Project metadata
/// </summary>
public class Metadata
{
    public string Title { get; set; } = "Mein Memoir";
    public string Author { get; set; } = string.Empty;
    public PageFormat PageFormat { get; set; } = new();
    public Margins Margins { get; set; } = new();
    public DateTime LastSaved { get; set; } = DateTime.Now;
    public DateTime Created { get; set; } = DateTime.Now;
}

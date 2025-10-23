using System.Collections.ObjectModel;

namespace MemoirEditor.Models;

/// <summary>
/// A chapter in the memoir
/// </summary>
public class Chapter
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Title { get; set; } = "Neues Kapitel";
    public ChapterSettings Settings { get; set; } = new();
    public ObservableCollection<Block> Blocks { get; set; } = new();
    public double EstimatedPages { get; set; }
}

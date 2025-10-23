using System.Collections.ObjectModel;

namespace MemoirEditor.Models;

/// <summary>
/// Main data structure for a memoir project
/// </summary>
public class MemoirProject
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public Metadata Metadata { get; set; } = new();
    public ObservableCollection<Chapter> Chapters { get; set; } = new();
    public GlobalSettings GlobalSettings { get; set; } = new();
    public string FilePath { get; set; } = string.Empty;

    /// <summary>
    /// Total word count across all chapters and blocks
    /// </summary>
    public int TotalWordCount
    {
        get
        {
            return Chapters.Sum(c => c.Blocks.Sum(b => b.WordCount));
        }
    }

    /// <summary>
    /// Total character count across all chapters and blocks
    /// </summary>
    public int TotalCharacterCount
    {
        get
        {
            return Chapters.Sum(c => c.Blocks.Sum(b => b.Content.Length));
        }
    }

    /// <summary>
    /// Total image count across all chapters and blocks
    /// </summary>
    public int TotalImageCount
    {
        get
        {
            return Chapters.Sum(c => c.Blocks.Sum(b => b.Images.Count));
        }
    }
}

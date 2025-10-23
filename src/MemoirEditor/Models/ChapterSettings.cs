namespace MemoirEditor.Models;

/// <summary>
/// Settings for chapter page positioning
/// </summary>
public class ChapterSettings
{
    public bool StartsOnNewPage { get; set; } = true;
    public bool AlwaysLeft { get; set; } = false;
    public bool AlwaysRight { get; set; } = false;
    public int EmptyLinesFromPrevious { get; set; } = 2;
}

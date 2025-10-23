using System.Collections.ObjectModel;

namespace MemoirEditor.Models;

/// <summary>
/// A content block within a chapter (like a sub-section)
/// </summary>
public class Block
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Title { get; set; } = "Neuer Block";
    public string Content { get; set; } = string.Empty; // Rich text content
    public ObservableCollection<ImageReference> Images { get; set; } = new();
    public int WordCount => CountWords(Content);

    private int CountWords(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return 0;

        var words = text.Split(new[] { ' ', '\r', '\n', '\t' },
            StringSplitOptions.RemoveEmptyEntries);
        return words.Length;
    }
}

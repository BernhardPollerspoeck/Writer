using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace MemoirEditor.Models;

/// <summary>
/// A content block within a chapter (like a sub-section)
/// </summary>
public partial class Block : ObservableObject
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [ObservableProperty]
    private string _title = "Neuer Block";

    [ObservableProperty]
    private bool _showTitle = true;

    /// <summary>
    /// Formatting for the block title
    /// </summary>
    public TextFormatting TitleFormatting { get; set; } = new()
    {
        Font = "Georgia",
        FontSize = 14,
        Color = "#000000"
    };

    [ObservableProperty]
    private string _content = string.Empty; // Rich text content (XAML FlowDocument with per-character formatting)

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

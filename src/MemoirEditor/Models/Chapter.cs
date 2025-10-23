using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace MemoirEditor.Models;

/// <summary>
/// A chapter in the memoir
/// </summary>
public partial class Chapter : ObservableObject
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ShowTitle))]
    private string _title = "Neues Kapitel";

    [ObservableProperty]
    private bool _showTitle = true;

    /// <summary>
    /// Formatting for the chapter title
    /// </summary>
    public TextFormatting TitleFormatting { get; set; } = new()
    {
        Font = "Georgia",
        FontSize = 16,
        Color = "#000000"
    };

    public ChapterSettings Settings { get; set; } = new();
    public ObservableCollection<Block> Blocks { get; set; } = new();

    [ObservableProperty]
    private double _estimatedPages;
}

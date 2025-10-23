using CommunityToolkit.Mvvm.ComponentModel;

namespace MemoirEditor.Models;

/// <summary>
/// Text formatting settings (font, size, color)
/// </summary>
public partial class TextFormatting : ObservableObject
{
    [ObservableProperty]
    private string _font = "Georgia";

    [ObservableProperty]
    private double _fontSize = 12;

    [ObservableProperty]
    private string _color = "#000000"; // Hex color code
}

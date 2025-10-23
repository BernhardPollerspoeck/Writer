namespace MemoirEditor.Models;

/// <summary>
/// Global formatting settings for the memoir
/// </summary>
public class GlobalSettings
{
    public string Font { get; set; } = "Georgia";
    public double FontSize { get; set; } = 12;
    public double LineHeight { get; set; } = 1.5;
    public bool AutoSave { get; set; } = true;
}

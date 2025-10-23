using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MemoirEditor.Models;
using MemoirEditor.Interfaces;
using MemoirEditor.Helpers;

namespace MemoirEditor.ViewModels;

/// <summary>
/// ViewModel for the text editor area
/// </summary>
public partial class EditorViewModel : ObservableObject
{
    private readonly IImageService _imageService;

    public event EventHandler? ContentChanged;

    [ObservableProperty]
    private Block? _currentBlock;

    [ObservableProperty]
    private string _blockTitle = string.Empty;

    [ObservableProperty]
    private string _blockContent = string.Empty;

    [ObservableProperty]
    private int _wordCount;

    [ObservableProperty]
    private int _characterCount;

    [ObservableProperty]
    private string _selectedFont = "Georgia";

    [ObservableProperty]
    private double _selectedFontSize = 12;

    [ObservableProperty]
    private bool _isBold;

    [ObservableProperty]
    private bool _isItalic;

    [ObservableProperty]
    private bool _isUnderline;

    public EditorViewModel(IImageService imageService)
    {
        _imageService = imageService;
    }

    public void LoadBlock(Block block)
    {
        CurrentBlock = block;
        BlockTitle = block.Title;
        BlockContent = block.Content;
        UpdateStatistics();
    }

    partial void OnBlockContentChanged(string value)
    {
        if (CurrentBlock != null)
        {
            CurrentBlock.Content = value;
            UpdateStatistics();
            ContentChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    partial void OnBlockTitleChanged(string value)
    {
        if (CurrentBlock != null)
        {
            CurrentBlock.Title = value;
        }
    }

    private void UpdateStatistics()
    {
        WordCount = CurrentBlock?.WordCount ?? 0;
        CharacterCount = BlockContent.Length;
    }

    [RelayCommand]
    private async Task InsertImageAsync()
    {
        var dialog = new Microsoft.Win32.OpenFileDialog
        {
            Filter = "Image Files (*.jpg;*.jpeg;*.png;*.gif;*.bmp)|*.jpg;*.jpeg;*.png;*.gif;*.bmp|All Files (*.*)|*.*",
            DefaultExt = ".jpg",
            Multiselect = false
        };

        if (dialog.ShowDialog() == true && CurrentBlock != null)
        {
            try
            {
                var imageRef = await _imageService.ImportImageAsync(dialog.FileName);
                CurrentBlock.Images.Add(imageRef);

                // Add image placeholder to content
                BlockContent += $"\r\n[📷 Bild: {imageRef.FileName}]\r\n";
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Fehler beim Einfügen des Bildes");
            }
        }
    }

    [RelayCommand]
    private void InsertPageBreak()
    {
        BlockContent += "\r\n<!-- PAGE_BREAK -->\r\n";
    }

    [RelayCommand]
    private void ToggleBold()
    {
        IsBold = !IsBold;
        // Note: Actual formatting is applied by RichTextBox edit commands in the view
    }

    [RelayCommand]
    private void ToggleItalic()
    {
        IsItalic = !IsItalic;
        // Note: Actual formatting is applied by RichTextBox edit commands in the view
    }

    [RelayCommand]
    private void ToggleUnderline()
    {
        IsUnderline = !IsUnderline;
        // Note: Actual formatting is applied by RichTextBox edit commands in the view
    }

    partial void OnSelectedFontChanged(string value)
    {
        // Font change will be applied in the view
    }

    partial void OnSelectedFontSizeChanged(double value)
    {
        // Font size change will be applied in the view
    }
}

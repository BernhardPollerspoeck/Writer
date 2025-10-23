using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MemoirEditor.Models;
using System.Collections.ObjectModel;

namespace MemoirEditor.ViewModels;

/// <summary>
/// ViewModel for the chapter and block tree view
/// </summary>
public partial class ChapterTreeViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<Chapter> _chapters = new();

    [ObservableProperty]
    private Chapter? _selectedChapter;

    [ObservableProperty]
    private Block? _selectedBlock;

    public ChapterTreeViewModel()
    {
    }

    public void LoadChapters(ObservableCollection<Chapter> chapters)
    {
        Chapters = chapters;
    }

    [RelayCommand]
    private void AddChapter()
    {
        var newChapter = new Chapter
        {
            Title = $"Kapitel {Chapters.Count + 1}"
        };

        // Add initial block
        var initialBlock = new Block { Title = "Neuer Abschnitt" };
        newChapter.Blocks.Add(initialBlock);

        Chapters.Add(newChapter);
        SelectedChapter = newChapter;
    }

    [RelayCommand]
    private void AddBlock()
    {
        if (SelectedChapter != null)
        {
            var newBlock = new Block
            {
                Title = $"Block {SelectedChapter.Blocks.Count + 1}"
            };

            SelectedChapter.Blocks.Add(newBlock);
            SelectedBlock = newBlock;
        }
    }

    [RelayCommand]
    private void DeleteChapter()
    {
        if (SelectedChapter != null)
        {
            Chapters.Remove(SelectedChapter);
            SelectedChapter = Chapters.FirstOrDefault();
        }
    }

    [RelayCommand]
    private void DeleteBlock()
    {
        if (SelectedBlock != null && SelectedChapter != null)
        {
            SelectedChapter.Blocks.Remove(SelectedBlock);
            SelectedBlock = SelectedChapter.Blocks.FirstOrDefault();
        }
    }
}

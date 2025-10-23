using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MemoirEditor.Models;
using MemoirEditor.Interfaces;

namespace MemoirEditor.ViewModels;

/// <summary>
/// Main ViewModel orchestrating the entire application
/// </summary>
public partial class MainViewModel : ObservableObject
{
    private readonly IProjectRepository _projectRepository;
    private readonly IAutoSaveService _autoSaveService;

    [ObservableProperty]
    private MemoirProject _currentProject;

    [ObservableProperty]
    private Chapter? _selectedChapter;

    [ObservableProperty]
    private Block? _selectedBlock;

    [ObservableProperty]
    private bool _isModified;

    public EditorViewModel EditorViewModel { get; }
    public ChapterTreeViewModel ChapterTreeViewModel { get; }
    public PreviewViewModel PreviewViewModel { get; }
    public ProjectManagementViewModel ProjectManagementViewModel { get; }

    public MainViewModel(
        IProjectRepository projectRepository,
        IAutoSaveService autoSaveService,
        EditorViewModel editorViewModel,
        ChapterTreeViewModel chapterTreeViewModel,
        PreviewViewModel previewViewModel,
        ProjectManagementViewModel projectManagementViewModel)
    {
        _projectRepository = projectRepository;
        _autoSaveService = autoSaveService;

        EditorViewModel = editorViewModel;
        ChapterTreeViewModel = chapterTreeViewModel;
        PreviewViewModel = previewViewModel;
        ProjectManagementViewModel = projectManagementViewModel;

        // Subscribe to content changes for live preview
        EditorViewModel.ContentChanged += (s, e) =>
        {
            IsModified = true;
            PreviewViewModel.RequestRender(CurrentProject);
        };

        // Subscribe to auto-save completion
        _autoSaveService.AutoSaveCompleted += (s, e) =>
        {
            ProjectManagementViewModel.LastSaveTime = _autoSaveService.LastSaveTime;
        };

        // Initialize with new project
        _currentProject = _projectRepository.CreateNew();

        // Setup initial chapter
        var initialChapter = new Chapter { Title = "Kapitel 1: Der Anfang" };
        var initialBlock = new Block { Title = "Einleitung" };
        initialChapter.Blocks.Add(initialBlock);
        _currentProject.Chapters.Add(initialChapter);

        SelectedChapter = initialChapter;
        SelectedBlock = initialBlock;

        // Load chapters into tree view
        ChapterTreeViewModel.LoadChapters(_currentProject.Chapters);

        // Subscribe to chapter tree changes
        ChapterTreeViewModel.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(ChapterTreeViewModel.SelectedChapter))
            {
                SelectedChapter = ChapterTreeViewModel.SelectedChapter;
            }
            else if (e.PropertyName == nameof(ChapterTreeViewModel.SelectedBlock))
            {
                SelectedBlock = ChapterTreeViewModel.SelectedBlock;
            }
        };

        // Note: Initial preview render will be triggered once WebView2 is initialized
    }

    partial void OnSelectedChapterChanged(Chapter? oldValue, Chapter? newValue)
    {
        // Unsubscribe from old chapter
        if (oldValue != null)
        {
            oldValue.PropertyChanged -= OnChapterPropertyChanged;
            if (oldValue.TitleFormatting != null)
            {
                oldValue.TitleFormatting.PropertyChanged -= OnFormattingChanged;
            }
        }

        // Subscribe to new chapter
        if (newValue != null)
        {
            newValue.PropertyChanged += OnChapterPropertyChanged;
            if (newValue.TitleFormatting != null)
            {
                newValue.TitleFormatting.PropertyChanged += OnFormattingChanged;
            }

            if (newValue.Blocks.Count > 0)
            {
                SelectedBlock = newValue.Blocks[0];
            }
        }
    }

    partial void OnSelectedBlockChanged(Block? oldValue, Block? newValue)
    {
        // Unsubscribe from old block
        if (oldValue != null)
        {
            oldValue.PropertyChanged -= OnBlockPropertyChanged;
            if (oldValue.TitleFormatting != null)
            {
                oldValue.TitleFormatting.PropertyChanged -= OnFormattingChanged;
            }
        }

        // Subscribe to new block
        if (newValue != null)
        {
            newValue.PropertyChanged += OnBlockPropertyChanged;
            if (newValue.TitleFormatting != null)
            {
                newValue.TitleFormatting.PropertyChanged += OnFormattingChanged;
            }

            EditorViewModel.LoadBlock(newValue);
        }
    }

    private void OnChapterPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        // Trigger preview update when chapter properties change (like ShowTitle)
        if (e.PropertyName == nameof(Chapter.ShowTitle) || e.PropertyName == nameof(Chapter.Title))
        {
            PreviewViewModel.RequestRender(CurrentProject);
        }
    }

    private void OnBlockPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        // Trigger preview update when block properties change (like ShowTitle)
        if (e.PropertyName == nameof(Block.ShowTitle) || e.PropertyName == nameof(Block.Title))
        {
            PreviewViewModel.RequestRender(CurrentProject);
        }
    }

    private void OnFormattingChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        // Trigger preview update when formatting changes
        PreviewViewModel.RequestRender(CurrentProject);
    }

    [RelayCommand]
    private void NewProject()
    {
        CurrentProject = _projectRepository.CreateNew();
        IsModified = false;
    }

    [RelayCommand]
    private async Task LoadProjectAsync(string filePath)
    {
        CurrentProject = await _projectRepository.LoadAsync(filePath);
        IsModified = false;
    }

    [RelayCommand]
    private async Task SaveProjectAsync()
    {
        if (!string.IsNullOrEmpty(CurrentProject.FilePath))
        {
            await _projectRepository.SaveAsync(CurrentProject, CurrentProject.FilePath);
            IsModified = false;

            // Start auto-save if enabled and not already running
            if (ProjectManagementViewModel.AutoSaveEnabled)
            {
                _autoSaveService.Start(CurrentProject, CurrentProject.FilePath, TimeSpan.FromMinutes(2));
            }
        }
    }

    partial void OnCurrentProjectChanged(MemoirProject value)
    {
        // Stop auto-save for old project
        _autoSaveService.Stop();

        // Load project into sub-viewmodels
        if (value != null)
        {
            ProjectManagementViewModel.LoadProject(value);
            ChapterTreeViewModel.LoadChapters(value.Chapters);

            // Start auto-save if project has a file path and auto-save is enabled
            if (!string.IsNullOrEmpty(value.FilePath) && ProjectManagementViewModel.AutoSaveEnabled)
            {
                _autoSaveService.Start(value, value.FilePath, TimeSpan.FromMinutes(2));
            }
        }
    }
}

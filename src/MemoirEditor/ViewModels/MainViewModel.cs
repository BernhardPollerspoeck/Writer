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

        // Trigger initial preview render
        PreviewViewModel.RequestRender(_currentProject);
    }

    partial void OnSelectedChapterChanged(Chapter? value)
    {
        if (value != null && value.Blocks.Count > 0)
        {
            SelectedBlock = value.Blocks[0];
        }
    }

    partial void OnSelectedBlockChanged(Block? value)
    {
        if (value != null)
        {
            EditorViewModel.LoadBlock(value);
        }
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

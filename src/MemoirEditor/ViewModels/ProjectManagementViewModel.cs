using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MemoirEditor.Models;
using MemoirEditor.Interfaces;
using MemoirEditor.Helpers;

namespace MemoirEditor.ViewModels;

/// <summary>
/// ViewModel for project management (file operations, settings)
/// </summary>
public partial class ProjectManagementViewModel : ObservableObject
{
    private readonly IProjectRepository _projectRepository;
    private readonly IExportService _exportService;
    private readonly IAutoSaveService _autoSaveService;

    [ObservableProperty]
    private MemoirProject? _currentProject;

    [ObservableProperty]
    private double _pageWidth = 210;

    [ObservableProperty]
    private double _pageHeight = 297;

    [ObservableProperty]
    private double _marginLeft = 25;

    [ObservableProperty]
    private double _marginRight = 25;

    [ObservableProperty]
    private double _marginTop = 25;

    [ObservableProperty]
    private double _marginBottom = 25;

    [ObservableProperty]
    private bool _autoSaveEnabled = true;

    [ObservableProperty]
    private DateTime? _lastSaveTime;

    public ProjectManagementViewModel(
        IProjectRepository projectRepository,
        IExportService exportService,
        IAutoSaveService autoSaveService)
    {
        _projectRepository = projectRepository;
        _exportService = exportService;
        _autoSaveService = autoSaveService;

        _autoSaveService.AutoSaveCompleted += (s, e) =>
        {
            LastSaveTime = _autoSaveService.LastSaveTime;
        };
    }

    public void LoadProject(MemoirProject project)
    {
        CurrentProject = project;

        // Load settings from project
        PageWidth = project.Metadata.PageFormat.Width;
        PageHeight = project.Metadata.PageFormat.Height;
        MarginLeft = project.Metadata.Margins.Left;
        MarginRight = project.Metadata.Margins.Right;
        MarginTop = project.Metadata.Margins.Top;
        MarginBottom = project.Metadata.Margins.Bottom;
        AutoSaveEnabled = project.GlobalSettings.AutoSave;
    }

    partial void OnPageWidthChanged(double value)
    {
        if (CurrentProject != null)
            CurrentProject.Metadata.PageFormat.Width = value;
    }

    partial void OnPageHeightChanged(double value)
    {
        if (CurrentProject != null)
            CurrentProject.Metadata.PageFormat.Height = value;
    }

    partial void OnMarginLeftChanged(double value)
    {
        if (CurrentProject != null)
            CurrentProject.Metadata.Margins.Left = value;
    }

    partial void OnMarginRightChanged(double value)
    {
        if (CurrentProject != null)
            CurrentProject.Metadata.Margins.Right = value;
    }

    partial void OnMarginTopChanged(double value)
    {
        if (CurrentProject != null)
            CurrentProject.Metadata.Margins.Top = value;
    }

    partial void OnMarginBottomChanged(double value)
    {
        if (CurrentProject != null)
            CurrentProject.Metadata.Margins.Bottom = value;
    }

    partial void OnAutoSaveEnabledChanged(bool value)
    {
        if (CurrentProject != null)
        {
            CurrentProject.GlobalSettings.AutoSave = value;

            // Start or stop auto-save based on new value
            if (value && !string.IsNullOrEmpty(CurrentProject.FilePath))
            {
                _autoSaveService.Start(CurrentProject, CurrentProject.FilePath, TimeSpan.FromMinutes(2));
            }
            else
            {
                _autoSaveService.Stop();
            }
        }
    }

    [RelayCommand]
    private Task NewProjectAsync()
    {
        var newProject = _projectRepository.CreateNew();
        LoadProject(newProject);
        return Task.CompletedTask;
    }

    [RelayCommand]
    private async Task LoadProjectAsync()
    {
        try
        {
            var dialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Memoir Files (*.memoir)|*.memoir|All Files (*.*)|*.*",
                DefaultExt = ".memoir"
            };

            if (dialog.ShowDialog() == true)
            {
                var project = await _projectRepository.LoadAsync(dialog.FileName);
                LoadProject(project);
                ErrorHandler.ShowInfo($"Projekt '{project.Metadata.Title}' erfolgreich geladen.", "Projekt geladen");
            }
        }
        catch (Exception ex)
        {
            ErrorHandler.HandleException(ex, "Fehler beim Laden des Projekts");
        }
    }

    [RelayCommand]
    private async Task SaveProjectAsync()
    {
        if (CurrentProject == null)
            return;

        try
        {
            if (string.IsNullOrEmpty(CurrentProject.FilePath))
            {
                // No file path, use Save As
                await SaveAsProjectAsync();
            }
            else
            {
                await _projectRepository.SaveAsync(CurrentProject, CurrentProject.FilePath);
                LastSaveTime = DateTime.Now;
            }
        }
        catch (Exception ex)
        {
            ErrorHandler.HandleException(ex, "Fehler beim Speichern des Projekts");
        }
    }

    [RelayCommand]
    private async Task SaveAsProjectAsync()
    {
        if (CurrentProject == null)
            return;

        try
        {
            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "Memoir Files (*.memoir)|*.memoir|All Files (*.*)|*.*",
                DefaultExt = ".memoir",
                FileName = string.IsNullOrEmpty(CurrentProject.FilePath)
                    ? "Neues Memoir.memoir"
                    : System.IO.Path.GetFileName(CurrentProject.FilePath)
            };

            if (dialog.ShowDialog() == true)
            {
                await _projectRepository.SaveAsync(CurrentProject, dialog.FileName);
                LastSaveTime = DateTime.Now;
                ErrorHandler.ShowInfo("Projekt erfolgreich gespeichert.", "Speichern");
            }
        }
        catch (Exception ex)
        {
            ErrorHandler.HandleException(ex, "Fehler beim Speichern des Projekts");
        }
    }

    [RelayCommand]
    private async Task ExportToPdfAsync()
    {
        if (CurrentProject == null)
            return;

        try
        {
            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "PDF Files (*.pdf)|*.pdf|All Files (*.*)|*.*",
                DefaultExt = ".pdf",
                FileName = $"{CurrentProject.Metadata.Title}.pdf"
            };

            if (dialog.ShowDialog() == true)
            {
                await _exportService.ExportToPdfAsync(CurrentProject, dialog.FileName);
                ErrorHandler.ShowInfo($"PDF erfolgreich exportiert nach:\n{dialog.FileName}", "Export erfolgreich");
            }
        }
        catch (Exception ex)
        {
            ErrorHandler.HandleException(ex, "Fehler beim PDF-Export");
        }
    }
}

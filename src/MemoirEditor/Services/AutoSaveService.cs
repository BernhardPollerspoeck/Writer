using MemoirEditor.Interfaces;
using MemoirEditor.Models;
using MemoirEditor.Helpers;

namespace MemoirEditor.Services;

/// <summary>
/// Service for automatic project saving
/// </summary>
public class AutoSaveService : IAutoSaveService
{
    private readonly IProjectRepository _projectRepository;
    private System.Threading.Timer? _timer;
    private MemoirProject? _currentProject;
    private string? _currentFilePath;

    public event EventHandler? AutoSaveCompleted;
    public DateTime? LastSaveTime { get; private set; }

    public AutoSaveService(IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    public void Start(MemoirProject project, string filePath, TimeSpan interval)
    {
        Stop();

        _currentProject = project;
        _currentFilePath = filePath;

        _timer = new System.Threading.Timer(
            async _ => await SaveNowAsync(),
            null,
            interval,
            interval
        );
    }

    public void Stop()
    {
        _timer?.Dispose();
        _timer = null;
    }

    public async Task SaveNowAsync()
    {
        if (_currentProject != null && !string.IsNullOrEmpty(_currentFilePath))
        {
            try
            {
                await _projectRepository.SaveAsync(_currentProject, _currentFilePath);
                LastSaveTime = DateTime.Now;
                AutoSaveCompleted?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                // Log error for debugging
                System.Diagnostics.Debug.WriteLine($"[AutoSave Error] Failed to auto-save project: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[AutoSave Error] Stack trace: {ex.StackTrace}");

                // Show error to user - auto-save failure is important
                ErrorHandler.ShowError(
                    $"Automatisches Speichern fehlgeschlagen!\n\n" +
                    $"Datei: {_currentFilePath}\n" +
                    $"Fehler: {ex.Message}\n\n" +
                    $"Bitte speichern Sie manuell, um Datenverlust zu vermeiden.",
                    "Auto-Speichern fehlgeschlagen");
            }
        }
    }
}

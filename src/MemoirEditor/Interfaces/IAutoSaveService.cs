using MemoirEditor.Models;

namespace MemoirEditor.Interfaces;

/// <summary>
/// Service for automatic project saving
/// </summary>
public interface IAutoSaveService
{
    /// <summary>
    /// Starts auto-save timer
    /// </summary>
    void Start(MemoirProject project, string filePath, TimeSpan interval);

    /// <summary>
    /// Stops auto-save timer
    /// </summary>
    void Stop();

    /// <summary>
    /// Manually triggers a save
    /// </summary>
    Task SaveNowAsync();

    /// <summary>
    /// Event raised when auto-save completes
    /// </summary>
    event EventHandler? AutoSaveCompleted;

    /// <summary>
    /// Gets the last save time
    /// </summary>
    DateTime? LastSaveTime { get; }
}

using MemoirEditor.Models;

namespace MemoirEditor.Interfaces;

/// <summary>
/// Repository for loading and saving memoir projects
/// </summary>
public interface IProjectRepository
{
    /// <summary>
    /// Loads a project from a file
    /// </summary>
    Task<MemoirProject> LoadAsync(string filePath, CancellationToken cancellationToken = default);

    /// <summary>
    /// Saves a project to a file
    /// </summary>
    Task SaveAsync(MemoirProject project, string filePath, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new empty project
    /// </summary>
    MemoirProject CreateNew();

    /// <summary>
    /// Creates a backup of the project
    /// </summary>
    Task CreateBackupAsync(MemoirProject project, CancellationToken cancellationToken = default);
}

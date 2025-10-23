using MemoirEditor.Interfaces;
using MemoirEditor.Models;
using Newtonsoft.Json;
using System.IO;

namespace MemoirEditor.Services;

/// <summary>
/// File-based implementation of project repository using JSON
/// </summary>
public class FileProjectRepository : IProjectRepository
{
    public async Task<MemoirProject> LoadAsync(string filePath, CancellationToken cancellationToken = default)
    {
        var json = await File.ReadAllTextAsync(filePath, cancellationToken);
        var project = JsonConvert.DeserializeObject<MemoirProject>(json);

        if (project == null)
            throw new InvalidOperationException("Failed to deserialize project");

        project.FilePath = filePath;
        return project;
    }

    public async Task SaveAsync(MemoirProject project, string filePath, CancellationToken cancellationToken = default)
    {
        project.FilePath = filePath;
        project.Metadata.LastSaved = DateTime.Now;

        var json = JsonConvert.SerializeObject(project, Formatting.Indented);
        await File.WriteAllTextAsync(filePath, json, cancellationToken);
    }

    public MemoirProject CreateNew()
    {
        return new MemoirProject
        {
            Metadata = new Metadata
            {
                Title = "Neues Memoir",
                Author = Environment.UserName,
                Created = DateTime.Now,
                LastSaved = DateTime.Now
            }
        };
    }

    public async Task CreateBackupAsync(MemoirProject project, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(project.FilePath))
            return;

        var backupPath = $"{project.FilePath}.backup_{DateTime.Now:yyyyMMdd_HHmmss}";
        await SaveAsync(project, backupPath, cancellationToken);
    }
}

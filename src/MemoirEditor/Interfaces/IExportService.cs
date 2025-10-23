using MemoirEditor.Models;

namespace MemoirEditor.Interfaces;

/// <summary>
/// Service for exporting memoir to PDF
/// </summary>
public interface IExportService
{
    /// <summary>
    /// Exports the memoir to a PDF file
    /// </summary>
    Task ExportToPdfAsync(MemoirProject project, string outputPath, IProgress<int>? progress = null, CancellationToken cancellationToken = default);
}

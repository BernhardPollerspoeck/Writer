using MemoirEditor.Interfaces;
using MemoirEditor.Models;
using System.IO;

namespace MemoirEditor.Services;

/// <summary>
/// Service for exporting memoir to PDF
/// </summary>
public class ExportService : IExportService
{
    private readonly IQuestPdfRenderer _renderer;

    public ExportService(IQuestPdfRenderer renderer)
    {
        _renderer = renderer;
    }

    public async Task ExportToPdfAsync(MemoirProject project, string outputPath, IProgress<int>? progress = null, CancellationToken cancellationToken = default)
    {
        progress?.Report(0);

        var pdfData = await _renderer.GeneratePdfAsync(project, cancellationToken);

        progress?.Report(50);

        await File.WriteAllBytesAsync(outputPath, pdfData, cancellationToken);

        progress?.Report(100);
    }
}

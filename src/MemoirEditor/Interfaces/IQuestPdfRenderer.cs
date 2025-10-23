using MemoirEditor.Models;

namespace MemoirEditor.Interfaces;

/// <summary>
/// Service for rendering memoir content using QuestPDF
/// </summary>
public interface IQuestPdfRenderer
{
    /// <summary>
    /// Generates a PDF from the memoir project
    /// </summary>
    Task<byte[]> GeneratePdfAsync(MemoirProject project, CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates a preview PDF (optimized for quick rendering)
    /// </summary>
    Task<byte[]> GeneratePreviewAsync(MemoirProject project, int startPage, int pageCount, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the page count from a generated PDF
    /// </summary>
    int GetPageCount(byte[] pdfData);
}

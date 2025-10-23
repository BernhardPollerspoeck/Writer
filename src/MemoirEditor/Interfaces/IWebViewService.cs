using Microsoft.Web.WebView2.Wpf;

namespace MemoirEditor.Interfaces;

/// <summary>
/// Service for managing WebView2 PDF preview
/// </summary>
public interface IWebViewService
{
    /// <summary>
    /// Sets the WebView2 control reference
    /// </summary>
    void SetWebView(WebView2 webView);

    /// <summary>
    /// Loads a PDF into the WebView
    /// </summary>
    Task LoadPdfAsync(byte[] pdfData, CancellationToken cancellationToken = default);

    /// <summary>
    /// Navigates to a specific page in the PDF
    /// </summary>
    Task NavigateToPageAsync(int pageNumber);

    /// <summary>
    /// Cleans up temporary files
    /// </summary>
    Task CleanupAsync();
}

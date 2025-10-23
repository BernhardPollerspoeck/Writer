using MemoirEditor.Interfaces;
using Microsoft.Web.WebView2.Wpf;
using System.IO;
using System.Windows;

namespace MemoirEditor.Services;

/// <summary>
/// Service for managing WebView2 PDF preview
/// </summary>
public class WebViewService : IWebViewService
{
    private readonly List<string> _tempFiles = new();
    private WebView2? _webView;

    public void SetWebView(WebView2 webView)
    {
        _webView = webView;
    }

    public async Task LoadPdfAsync(byte[] pdfData, CancellationToken cancellationToken = default)
    {
        if (_webView == null)
        {
            throw new InvalidOperationException("WebView2 control has not been set. Call SetWebView first.");
        }

        // Create temporary file
        var tempPath = Path.Combine(Path.GetTempPath(), $"memoir_preview_{Guid.NewGuid()}.pdf");
        await File.WriteAllBytesAsync(tempPath, pdfData, cancellationToken);
        _tempFiles.Add(tempPath);

        // Navigate to PDF on UI thread
        await Application.Current.Dispatcher.InvokeAsync(() =>
        {
            _webView.CoreWebView2.Navigate(new Uri(tempPath).AbsoluteUri);
        });
    }

    public async Task NavigateToPageAsync(int pageNumber)
    {
        if (_webView == null || _webView.CoreWebView2 == null)
            return;

        try
        {
            // Navigate to specific page using PDF fragment identifier
            // Format: file.pdf#page=N
            var currentSource = _webView.CoreWebView2.Source;
            if (!string.IsNullOrEmpty(currentSource))
            {
                // Remove existing fragment if any
                var baseUrl = currentSource.Split('#')[0];

                // Add page fragment
                var newUrl = $"{baseUrl}#page={pageNumber}";

                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    _webView.CoreWebView2.Navigate(newUrl);
                });
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to navigate to page {pageNumber}: {ex.Message}");
        }
    }

    public async Task CleanupAsync()
    {
        foreach (var file in _tempFiles)
        {
            try
            {
                if (File.Exists(file))
                {
                    File.Delete(file);
                }
            }
            catch
            {
                // Ignore cleanup errors
            }
        }
        _tempFiles.Clear();
        await Task.CompletedTask;
    }
}

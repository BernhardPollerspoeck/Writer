using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MemoirEditor.Models;
using MemoirEditor.Interfaces;
using MemoirEditor.Helpers;

namespace MemoirEditor.ViewModels;

/// <summary>
/// ViewModel for the live book preview
/// </summary>
public partial class PreviewViewModel : ObservableObject
{
    private readonly IQuestPdfRenderer _pdfRenderer;
    private readonly IWebViewService _webViewService;
    private readonly IRenderQueue _renderQueue;

    [ObservableProperty]
    private int _currentPage = 1;

    [ObservableProperty]
    private int _totalPages = 1;

    [ObservableProperty]
    private bool _isRendering;

    [ObservableProperty]
    private byte[]? _currentPdfData;

    public PreviewViewModel(
        IQuestPdfRenderer pdfRenderer,
        IWebViewService webViewService,
        IRenderQueue renderQueue)
    {
        _pdfRenderer = pdfRenderer;
        _webViewService = webViewService;
        _renderQueue = renderQueue;

        _renderQueue.RenderStarted += (s, e) => IsRendering = true;
        _renderQueue.RenderCompleted += (s, e) => IsRendering = false;
    }

    public void RequestRender(MemoirProject project)
    {
        _renderQueue.QueueRender(
            project,
            onComplete: async (pdfData) =>
            {
                CurrentPdfData = pdfData;
                await _webViewService.LoadPdfAsync(pdfData);

                // Extract actual page count from PDF
                TotalPages = _pdfRenderer.GetPageCount(pdfData);

                // Reset to first page
                if (CurrentPage > TotalPages)
                {
                    CurrentPage = 1;
                }
            },
            onError: (ex) =>
            {
                ErrorHandler.HandleException(ex, "Fehler beim Rendern der PDF-Vorschau");
            }
        );
    }

    [RelayCommand]
    private async Task NextPageAsync()
    {
        if (CurrentPage < TotalPages)
        {
            CurrentPage += 2; // Two pages at once (book view)
            await _webViewService.NavigateToPageAsync(CurrentPage);
        }
    }

    [RelayCommand]
    private async Task PreviousPageAsync()
    {
        if (CurrentPage > 1)
        {
            CurrentPage -= 2;
            if (CurrentPage < 1) CurrentPage = 1;
            await _webViewService.NavigateToPageAsync(CurrentPage);
        }
    }
}

using MemoirEditor.Interfaces;
using MemoirEditor.Models;
using System.Threading;

namespace MemoirEditor.Services;

/// <summary>
/// Service for managing background rendering queue with debouncing
/// </summary>
public class RenderQueueService : IRenderQueue
{
    private readonly IQuestPdfRenderer _renderer;
    private CancellationTokenSource? _currentCts;
    private System.Threading.Timer? _debounceTimer;
    private const int DebounceMs = 500;

    public event EventHandler? RenderStarted;
    public event EventHandler? RenderCompleted;

    public RenderQueueService(IQuestPdfRenderer renderer)
    {
        _renderer = renderer;
    }

    public void QueueRender(MemoirProject project, Action<byte[]> onComplete, Action<Exception>? onError = null)
    {
        // Cancel any existing timer
        _debounceTimer?.Dispose();
        _currentCts?.Cancel();

        // Create new cancellation token
        _currentCts = new CancellationTokenSource();
        var cts = _currentCts;

        // Debounce the render request
        _debounceTimer = new System.Threading.Timer(async _ =>
        {
            if (cts.Token.IsCancellationRequested)
                return;

            try
            {
                RenderStarted?.Invoke(this, EventArgs.Empty);

                var pdfData = await _renderer.GeneratePdfAsync(project, cts.Token);

                if (!cts.Token.IsCancellationRequested)
                {
                    onComplete(pdfData);
                    RenderCompleted?.Invoke(this, EventArgs.Empty);
                }
            }
            catch (Exception ex)
            {
                if (!cts.Token.IsCancellationRequested)
                {
                    onError?.Invoke(ex);
                    RenderCompleted?.Invoke(this, EventArgs.Empty);
                }
            }
        }, null, DebounceMs, Timeout.Infinite);
    }

    public void CancelPending()
    {
        _currentCts?.Cancel();
        _debounceTimer?.Dispose();
    }
}

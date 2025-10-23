using MemoirEditor.Models;

namespace MemoirEditor.Interfaces;

/// <summary>
/// Service for managing background rendering queue with debouncing
/// </summary>
public interface IRenderQueue
{
    /// <summary>
    /// Queues a render request (debounced)
    /// </summary>
    void QueueRender(MemoirProject project, Action<byte[]> onComplete, Action<Exception>? onError = null);

    /// <summary>
    /// Cancels pending render requests
    /// </summary>
    void CancelPending();

    /// <summary>
    /// Event raised when rendering starts
    /// </summary>
    event EventHandler? RenderStarted;

    /// <summary>
    /// Event raised when rendering completes
    /// </summary>
    event EventHandler? RenderCompleted;
}

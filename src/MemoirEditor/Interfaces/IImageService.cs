using MemoirEditor.Models;

namespace MemoirEditor.Interfaces;

/// <summary>
/// Service for managing images in the memoir
/// </summary>
public interface IImageService
{
    /// <summary>
    /// Imports an image from a file
    /// </summary>
    Task<ImageReference> ImportImageAsync(string filePath, CancellationToken cancellationToken = default);

    /// <summary>
    /// Resizes an image to fit page constraints
    /// </summary>
    Task<byte[]> ResizeImageAsync(byte[] imageData, int maxWidth, int maxHeight, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the dimensions of an image
    /// </summary>
    (int Width, int Height) GetImageDimensions(byte[] imageData);
}

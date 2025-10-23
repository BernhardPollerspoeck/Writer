using MemoirEditor.Interfaces;
using MemoirEditor.Models;
using System.IO;

namespace MemoirEditor.Services;

/// <summary>
/// Service for managing images in the memoir
/// </summary>
public class ImageService : IImageService
{
    public async Task<ImageReference> ImportImageAsync(string filePath, CancellationToken cancellationToken = default)
    {
        var imageData = await File.ReadAllBytesAsync(filePath, cancellationToken);
        var (width, height) = GetImageDimensions(imageData);

        return new ImageReference
        {
            FileName = Path.GetFileName(filePath),
            FilePath = filePath,
            ImageData = imageData,
            Width = width,
            Height = height
        };
    }

    public async Task<byte[]> ResizeImageAsync(byte[] imageData, int maxWidth, int maxHeight, CancellationToken cancellationToken = default)
    {
        return await Task.Run(() =>
        {
            try
            {
                using (var msInput = new System.IO.MemoryStream(imageData))
                using (var originalImage = System.Drawing.Image.FromStream(msInput))
                {
                    // Calculate new dimensions while maintaining aspect ratio
                    var ratioX = (double)maxWidth / originalImage.Width;
                    var ratioY = (double)maxHeight / originalImage.Height;
                    var ratio = Math.Min(ratioX, ratioY);

                    var newWidth = (int)(originalImage.Width * ratio);
                    var newHeight = (int)(originalImage.Height * ratio);

                    // If image is already smaller, return original
                    if (ratio >= 1.0)
                        return imageData;

                    // Resize image
                    using (var resizedImage = new System.Drawing.Bitmap(newWidth, newHeight))
                    {
                        using (var graphics = System.Drawing.Graphics.FromImage(resizedImage))
                        {
                            graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                            graphics.DrawImage(originalImage, 0, 0, newWidth, newHeight);
                        }

                        // Save to byte array
                        using (var msOutput = new System.IO.MemoryStream())
                        {
                            resizedImage.Save(msOutput, originalImage.RawFormat);
                            return msOutput.ToArray();
                        }
                    }
                }
            }
            catch
            {
                // Return original image if resizing fails
                return imageData;
            }
        }, cancellationToken);
    }

    public (int Width, int Height) GetImageDimensions(byte[] imageData)
    {
        try
        {
            using (var ms = new System.IO.MemoryStream(imageData))
            {
                using (var image = System.Drawing.Image.FromStream(ms))
                {
                    return (image.Width, image.Height);
                }
            }
        }
        catch
        {
            // Fallback if image can't be read
            return (800, 600);
        }
    }
}

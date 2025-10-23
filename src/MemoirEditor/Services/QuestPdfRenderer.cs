using MemoirEditor.Interfaces;
using MemoirEditor.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MemoirEditor.Services;

/// <summary>
/// Implementation of PDF rendering using QuestPDF
/// </summary>
public class QuestPdfRenderer : IQuestPdfRenderer
{
    public QuestPdfRenderer()
    {
        // Set QuestPDF license (Community for non-commercial use)
        QuestPDF.Settings.License = LicenseType.Community;
    }

    /// <summary>
    /// Convert hex color string to QuestPDF color
    /// </summary>
    private string HexToQuestPdfColor(string hexColor)
    {
        // Remove # if present
        if (hexColor.StartsWith("#"))
            hexColor = hexColor.Substring(1);

        // Ensure it's 6 characters
        if (hexColor.Length != 6)
            return "#000000";

        return $"#{hexColor}";
    }

    /// <summary>
    /// Render rich text from FlowDocument XAML
    /// </summary>
    private void RenderRichText(QuestPDF.Infrastructure.IContainer container, string xamlContent, double lineHeight)
    {
        try
        {
            using (var stream = new System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(xamlContent)))
            {
                var doc = (System.Windows.Documents.FlowDocument)System.Windows.Markup.XamlReader.Load(stream);

                container.Text(text =>
                {
                    text.DefaultTextStyle(x => x.LineHeight((float)lineHeight));

                    foreach (var block in doc.Blocks)
                    {
                        if (block is System.Windows.Documents.Paragraph paragraph)
                        {
                            RenderParagraph(text, paragraph);
                        }
                    }
                });
            }
        }
        catch
        {
            // Fallback to plain text if parsing fails
            container.Text(xamlContent);
        }
    }

    private void RenderParagraph(QuestPDF.Fluent.TextDescriptor text, System.Windows.Documents.Paragraph paragraph)
    {
        foreach (var inline in paragraph.Inlines)
        {
            RenderInline(text, inline);
        }

        // Add line break after paragraph
        text.Line(string.Empty);
    }

    private void RenderInline(QuestPDF.Fluent.TextDescriptor text, System.Windows.Documents.Inline inline)
    {
        if (inline is System.Windows.Documents.Run run)
        {
            var span = text.Span(run.Text);

            // Apply font family
            if (run.FontFamily != null)
            {
                span.FontFamily(run.FontFamily.Source);
            }

            // Apply font size
            if (!double.IsNaN(run.FontSize))
            {
                span.FontSize((float)run.FontSize);
            }

            // Apply font weight (bold)
            if (run.FontWeight == System.Windows.FontWeights.Bold)
            {
                span.Bold();
            }

            // Apply font style (italic)
            if (run.FontStyle == System.Windows.FontStyles.Italic)
            {
                span.Italic();
            }

            // Apply text decorations (underline)
            if (run.TextDecorations != null && run.TextDecorations.Count > 0)
            {
                foreach (var decoration in run.TextDecorations)
                {
                    if (decoration.Location == System.Windows.TextDecorationLocation.Underline)
                    {
                        // QuestPDF doesn't have native underline, but we can note it
                        // For now, we'll skip underline in PDF
                    }
                }
            }

            // Apply foreground color
            if (run.Foreground is System.Windows.Media.SolidColorBrush brush)
            {
                var color = brush.Color;
                var hexColor = $"#{color.R:X2}{color.G:X2}{color.B:X2}";
                span.FontColor(hexColor);
            }
        }
        else if (inline is System.Windows.Documents.Span span)
        {
            // Recursively render spans
            foreach (var childInline in span.Inlines)
            {
                RenderInline(text, childInline);
            }
        }
    }

    public async Task<byte[]> GeneratePdfAsync(MemoirProject project, CancellationToken cancellationToken = default)
    {
        return await Task.Run(() =>
        {
            // Convert mm to points (1mm = 2.83465 points)
            var widthPt = (float)(project.Metadata.PageFormat.Width * 2.83465);
            var heightPt = (float)(project.Metadata.PageFormat.Height * 2.83465);

            var document = Document.Create(container =>
            {
                int currentPageNumber = 0;

                // Process each chapter
                foreach (var chapter in project.Chapters)
                {
                    // Handle AlwaysLeft/AlwaysRight by inserting blank page if needed
                    if (chapter.Settings.AlwaysRight && currentPageNumber % 2 == 0)
                    {
                        // Insert blank left page to ensure chapter starts on right
                        container.Page(page =>
                        {
                            page.Size(new PageSize(widthPt, heightPt));
                            page.Content().Text(""); // Blank page
                        });
                        currentPageNumber++;
                    }
                    else if (chapter.Settings.AlwaysLeft && currentPageNumber % 2 == 1)
                    {
                        // Insert blank right page to ensure chapter starts on left
                        container.Page(page =>
                        {
                            page.Size(new PageSize(widthPt, heightPt));
                            page.Content().Text(""); // Blank page
                        });
                        currentPageNumber++;
                    }

                    container.Page(page =>
                    {
                        currentPageNumber++;
                        page.Size(new PageSize(widthPt, heightPt));
                        page.MarginLeft((float)(project.Metadata.Margins.Left * 2.83465));
                        page.MarginTop((float)(project.Metadata.Margins.Top * 2.83465));
                        page.MarginRight((float)(project.Metadata.Margins.Right * 2.83465));
                        page.MarginBottom((float)(project.Metadata.Margins.Bottom * 2.83465));

                        // Header with page number
                        page.Header().AlignLeft().Text(text =>
                        {
                            text.Span("Seite ");
                            text.CurrentPageNumber();
                            text.DefaultTextStyle(x => x.FontSize(10));
                        });

                        page.Content().Column(column =>
                        {
                            // Add empty lines from previous chapter if specified
                            for (int i = 0; i < chapter.Settings.EmptyLinesFromPrevious; i++)
                            {
                                column.Item().PaddingBottom((float)project.GlobalSettings.LineHeight * (float)project.GlobalSettings.FontSize);
                            }

                            // Chapter title with underline (if enabled)
                            if (chapter.ShowTitle)
                            {
                                column.Item().Text(chapter.Title)
                                    .FontSize((float)chapter.TitleFormatting.FontSize)
                                    .Bold()
                                    .FontFamily(chapter.TitleFormatting.Font)
                                    .FontColor(HexToQuestPdfColor(chapter.TitleFormatting.Color));

                                column.Item().LineHorizontal(1).LineColor(Colors.Black);
                                column.Item().PaddingBottom(10);
                            }

                            // Render blocks
                            foreach (var block in chapter.Blocks)
                            {
                                // Block title (if enabled)
                                if (block.ShowTitle && !string.IsNullOrEmpty(block.Title))
                                {
                                    column.Item().Text(block.Title)
                                        .FontSize((float)block.TitleFormatting.FontSize)
                                        .SemiBold()
                                        .FontFamily(block.TitleFormatting.Font)
                                        .FontColor(HexToQuestPdfColor(block.TitleFormatting.Color));
                                    column.Item().PaddingBottom(5);
                                }

                                // Block content - render with rich text formatting
                                if (!string.IsNullOrEmpty(block.Content))
                                {
                                    var content = block.Content;

                                    // Split by page break markers
                                    var parts = content.Split(new[] { "<!-- PAGE_BREAK -->" }, StringSplitOptions.None);

                                    for (int i = 0; i < parts.Length; i++)
                                    {
                                        if (!string.IsNullOrWhiteSpace(parts[i]))
                                        {
                                            if (parts[i].Contains("<FlowDocument"))
                                            {
                                                // Render as rich text with per-character formatting
                                                RenderRichText(column.Item(), parts[i], project.GlobalSettings.LineHeight);
                                            }
                                            else
                                            {
                                                // Plain text fallback
                                                column.Item().Text(parts[i].Trim())
                                                    .FontSize((float)project.GlobalSettings.FontSize)
                                                    .LineHeight((float)project.GlobalSettings.LineHeight)
                                                    .FontFamily(project.GlobalSettings.Font);
                                            }
                                        }

                                        // Insert page break if not last part
                                        if (i < parts.Length - 1)
                                        {
                                            column.Item().PageBreak();
                                        }
                                    }
                                }

                                // Block images
                                foreach (var image in block.Images)
                                {
                                    if (System.IO.File.Exists(image.FilePath))
                                    {
                                        try
                                        {
                                            column.Item().Image(image.FilePath)
                                                .FitWidth();

                                            if (!string.IsNullOrEmpty(image.Caption))
                                            {
                                                column.Item().Row(row =>
                                                {
                                                    row.RelativeItem().AlignCenter().Text(image.Caption)
                                                        .FontSize(10)
                                                        .Italic();
                                                });
                                            }
                                        }
                                        catch
                                        {
                                            // Skip invalid images
                                        }
                                    }
                                }

                                column.Item().PaddingBottom(15);
                            }
                        });

                        page.Footer().AlignCenter().Text(x =>
                        {
                            x.Span("Seite ");
                            x.CurrentPageNumber();
                            x.DefaultTextStyle(style => style.FontSize(10));
                        });
                    });

                    // Force new page for next chapter if needed
                    if (chapter.Settings.StartsOnNewPage)
                    {
                        // New page is automatically created by next iteration
                    }
                }
            });

            return document.GeneratePdf();
        }, cancellationToken);
    }

    public async Task<byte[]> GeneratePreviewAsync(MemoirProject project, int startPage, int pageCount, CancellationToken cancellationToken = default)
    {
        // Generate full PDF first
        var fullPdf = await GeneratePdfAsync(project, cancellationToken);

        // If requesting all pages or page range not specified, return full PDF
        if (startPage <= 1 && pageCount <= 0)
            return fullPdf;

        try
        {
            // Extract only the requested page range using PdfSharp
            return await Task.Run(() =>
            {
                using (var inputStream = new System.IO.MemoryStream(fullPdf))
                using (var outputStream = new System.IO.MemoryStream())
                {
                    var inputDocument = PdfSharp.Pdf.IO.PdfReader.Open(inputStream, PdfSharp.Pdf.IO.PdfDocumentOpenMode.Import);
                    var outputDocument = new PdfSharp.Pdf.PdfDocument();

                    // Calculate actual page range
                    int totalPages = inputDocument.PageCount;
                    int endPage = Math.Min(startPage + pageCount - 1, totalPages);
                    startPage = Math.Max(1, startPage);

                    // Copy requested pages
                    for (int i = startPage; i <= endPage; i++)
                    {
                        outputDocument.AddPage(inputDocument.Pages[i - 1]); // Pages are 0-indexed
                    }

                    // Save to stream
                    outputDocument.Save(outputStream);
                    return outputStream.ToArray();
                }
            }, cancellationToken);
        }
        catch
        {
            // If page extraction fails, return full PDF
            return fullPdf;
        }
    }

    public int GetPageCount(byte[] pdfData)
    {
        try
        {
            // Primary method: Use PdfSharp to accurately count pages
            using (var ms = new System.IO.MemoryStream(pdfData))
            {
                var document = PdfSharp.Pdf.IO.PdfReader.Open(ms, PdfSharp.Pdf.IO.PdfDocumentOpenMode.Import);
                return document.PageCount;
            }
        }
        catch
        {
            // Fallback: Regex-based page count extraction
            // Works reliably for QuestPDF-generated PDFs
            try
            {
                var pdfString = System.Text.Encoding.ASCII.GetString(pdfData);
                var pageMatches = System.Text.RegularExpressions.Regex.Matches(pdfString, @"/Type\s*/Page\b");
                return Math.Max(1, pageMatches.Count);
            }
            catch
            {
                // Final fallback
                return 1;
            }
        }
    }
}

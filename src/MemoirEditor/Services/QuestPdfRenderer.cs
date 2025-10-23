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

                            // Chapter title with underline
                            column.Item().Text(chapter.Title)
                                .FontSize(16)
                                .Bold()
                                .FontFamily(project.GlobalSettings.Font);

                            column.Item().LineHorizontal(1).LineColor(Colors.Black);
                            column.Item().PaddingBottom(10);

                            // Render blocks
                            foreach (var block in chapter.Blocks)
                            {
                                // Block title
                                if (!string.IsNullOrEmpty(block.Title))
                                {
                                    column.Item().Text(block.Title)
                                        .FontSize(14)
                                        .SemiBold()
                                        .FontFamily(project.GlobalSettings.Font);
                                    column.Item().PaddingBottom(5);
                                }

                                // Block content - process page breaks
                                if (!string.IsNullOrEmpty(block.Content))
                                {
                                    // Extract plain text from XAML if needed
                                    var content = block.Content;
                                    if (content.Contains("<FlowDocument"))
                                    {
                                        // Extract text from XAML FlowDocument
                                        try
                                        {
                                            using (var stream = new System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(content)))
                                            {
                                                var doc = (System.Windows.Documents.FlowDocument)System.Windows.Markup.XamlReader.Load(stream);
                                                var range = new System.Windows.Documents.TextRange(doc.ContentStart, doc.ContentEnd);
                                                content = range.Text;
                                            }
                                        }
                                        catch
                                        {
                                            // Fallback: use as-is
                                        }
                                    }

                                    // Split by page break markers
                                    var parts = content.Split(new[] { "<!-- PAGE_BREAK -->" }, StringSplitOptions.None);

                                    for (int i = 0; i < parts.Length; i++)
                                    {
                                        if (!string.IsNullOrWhiteSpace(parts[i]))
                                        {
                                            column.Item().Text(parts[i].Trim())
                                                .FontSize((float)project.GlobalSettings.FontSize)
                                                .LineHeight((float)project.GlobalSettings.LineHeight)
                                                .FontFamily(project.GlobalSettings.Font);
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

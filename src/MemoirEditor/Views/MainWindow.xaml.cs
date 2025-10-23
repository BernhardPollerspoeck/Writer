using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using MemoirEditor.ViewModels;
using MemoirEditor.Interfaces;
using MemoirEditor.Models;

namespace MemoirEditor.Views;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly MainViewModel _viewModel;
    private readonly IWebViewService _webViewService;
    private bool _isUpdatingContent;
    private Point _dragStartPoint;
    private object? _draggedItem;
    private bool _isDragging;

    public MainWindow(MainViewModel viewModel, IWebViewService webViewService)
    {
        _viewModel = viewModel;
        _webViewService = webViewService;
        DataContext = viewModel;
        InitializeComponent();

        // Subscribe to property changes
        _viewModel.PropertyChanged += ViewModel_PropertyChanged;
        _viewModel.EditorViewModel.PropertyChanged += EditorViewModel_PropertyChanged;

        // Initialize WebView2
        InitializeWebView();
    }

    private void EditorViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (ContentEditor.Selection == null || ContentEditor.Selection.IsEmpty)
            return;

        switch (e.PropertyName)
        {
            case nameof(_viewModel.EditorViewModel.IsBold):
                ContentEditor.Selection.ApplyPropertyValue(
                    System.Windows.Documents.TextElement.FontWeightProperty,
                    _viewModel.EditorViewModel.IsBold ? FontWeights.Bold : FontWeights.Normal);
                break;

            case nameof(_viewModel.EditorViewModel.IsItalic):
                ContentEditor.Selection.ApplyPropertyValue(
                    System.Windows.Documents.TextElement.FontStyleProperty,
                    _viewModel.EditorViewModel.IsItalic ? FontStyles.Italic : FontStyles.Normal);
                break;

            case nameof(_viewModel.EditorViewModel.IsUnderline):
                ContentEditor.Selection.ApplyPropertyValue(
                    System.Windows.Documents.Inline.TextDecorationsProperty,
                    _viewModel.EditorViewModel.IsUnderline ? TextDecorations.Underline : null);
                break;

            case nameof(_viewModel.EditorViewModel.SelectedFont):
                ContentEditor.Selection.ApplyPropertyValue(
                    System.Windows.Documents.TextElement.FontFamilyProperty,
                    new System.Windows.Media.FontFamily(_viewModel.EditorViewModel.SelectedFont));
                break;

            case nameof(_viewModel.EditorViewModel.SelectedFontSize):
                ContentEditor.Selection.ApplyPropertyValue(
                    System.Windows.Documents.TextElement.FontSizeProperty,
                    _viewModel.EditorViewModel.SelectedFontSize);
                break;
        }

        ContentEditor.Focus();
    }

    private async void InitializeWebView()
    {
        try
        {
            await PdfPreviewWebView.EnsureCoreWebView2Async();

            // Pass the WebView2 control to the service
            _webViewService.SetWebView(PdfPreviewWebView);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to initialize WebView2: {ex.Message}\n\nPlease ensure WebView2 Runtime is installed.",
                "WebView2 Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void ViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(_viewModel.SelectedBlock))
        {
            LoadBlockContentToEditor();
        }
    }

    private void LoadBlockContentToEditor()
    {
        if (_viewModel.SelectedBlock == null)
        {
            ContentEditor.Document.Blocks.Clear();
            return;
        }

        _isUpdatingContent = true;

        try
        {
            // Try to load as XAML FlowDocument first (for formatted content)
            if (_viewModel.SelectedBlock.Content.Contains("<FlowDocument"))
            {
                var xaml = _viewModel.SelectedBlock.Content;
                using (var stream = new System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(xaml)))
                {
                    var document = (FlowDocument)System.Windows.Markup.XamlReader.Load(stream);
                    ContentEditor.Document = document;
                }
            }
            else
            {
                // Plain text - create simple paragraph
                ContentEditor.Document.Blocks.Clear();
                var paragraph = new Paragraph(new Run(_viewModel.SelectedBlock.Content));
                ContentEditor.Document.Blocks.Add(paragraph);
            }
        }
        catch
        {
            // Fallback to plain text if XAML parsing fails
            ContentEditor.Document.Blocks.Clear();
            var paragraph = new Paragraph(new Run(_viewModel.SelectedBlock.Content));
            ContentEditor.Document.Blocks.Add(paragraph);
        }

        _isUpdatingContent = false;
    }

    private void ContentEditor_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
    {
        if (_isUpdatingContent || _viewModel.SelectedBlock == null)
            return;

        try
        {
            // Save as XAML FlowDocument to preserve formatting
            using (var stream = new System.IO.MemoryStream())
            {
                var range = new TextRange(ContentEditor.Document.ContentStart, ContentEditor.Document.ContentEnd);
                range.Save(stream, DataFormats.Xaml);
                stream.Position = 0;
                using (var reader = new System.IO.StreamReader(stream))
                {
                    _viewModel.SelectedBlock.Content = reader.ReadToEnd();
                }
            }

            // Update EditorViewModel statistics with plain text
            var textRange = new TextRange(ContentEditor.Document.ContentStart, ContentEditor.Document.ContentEnd);
            _viewModel.EditorViewModel.BlockContent = textRange.Text;
        }
        catch
        {
            // Fallback to plain text if XAML serialization fails
            var textRange = new TextRange(ContentEditor.Document.ContentStart, ContentEditor.Document.ContentEnd);
            _viewModel.SelectedBlock.Content = textRange.Text;
            _viewModel.EditorViewModel.BlockContent = textRange.Text;
        }
    }

    private void Chapter_Click(object sender, MouseButtonEventArgs e)
    {
        if (sender is FrameworkElement element && element.DataContext is Chapter chapter)
        {
            _viewModel.SelectedChapter = chapter;
        }
    }

    private void Block_Click(object sender, MouseButtonEventArgs e)
    {
        if (sender is FrameworkElement element && element.DataContext is Models.Block block)
        {
            _viewModel.SelectedBlock = block;
        }
    }

    #region Drag & Drop for Chapters

    private void Chapter_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        _dragStartPoint = e.GetPosition(null);
        _draggedItem = null;
        _isDragging = false;
    }

    private void Chapter_PreviewMouseMove(object sender, MouseEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed && !_isDragging)
        {
            Point position = e.GetPosition(null);

            if (Math.Abs(position.X - _dragStartPoint.X) > SystemParameters.MinimumHorizontalDragDistance ||
                Math.Abs(position.Y - _dragStartPoint.Y) > SystemParameters.MinimumVerticalDragDistance)
            {
                if (sender is FrameworkElement element && element.DataContext is Chapter chapter)
                {
                    _draggedItem = chapter;
                    _isDragging = true;

                    var data = new DataObject("Chapter", chapter);
                    DragDrop.DoDragDrop((DependencyObject)sender, data, DragDropEffects.Move);

                    _isDragging = false;
                    _draggedItem = null;
                }
            }
        }
    }

    private void Chapter_DragOver(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent("Chapter"))
        {
            e.Effects = DragDropEffects.Move;
        }
        else
        {
            e.Effects = DragDropEffects.None;
        }
        e.Handled = true;
    }

    private void Chapter_Drop(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent("Chapter"))
        {
            var droppedChapter = e.Data.GetData("Chapter") as Chapter;
            var targetChapter = (sender as FrameworkElement)?.DataContext as Chapter;

            if (droppedChapter != null && targetChapter != null && droppedChapter != targetChapter)
            {
                var chapters = _viewModel.CurrentProject.Chapters;
                int oldIndex = chapters.IndexOf(droppedChapter);
                int newIndex = chapters.IndexOf(targetChapter);

                if (oldIndex >= 0 && newIndex >= 0)
                {
                    chapters.Move(oldIndex, newIndex);
                    _viewModel.PreviewViewModel.RequestRender(_viewModel.CurrentProject);
                }
            }
        }
    }

    #endregion

    #region Drag & Drop for Blocks

    private void Block_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        _dragStartPoint = e.GetPosition(null);
        _draggedItem = null;
        _isDragging = false;
    }

    private void Block_PreviewMouseMove(object sender, MouseEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed && !_isDragging)
        {
            Point position = e.GetPosition(null);

            if (Math.Abs(position.X - _dragStartPoint.X) > SystemParameters.MinimumHorizontalDragDistance ||
                Math.Abs(position.Y - _dragStartPoint.Y) > SystemParameters.MinimumVerticalDragDistance)
            {
                if (sender is FrameworkElement element && element.DataContext is Models.Block block)
                {
                    _draggedItem = block;
                    _isDragging = true;

                    var data = new DataObject("Block", block);
                    DragDrop.DoDragDrop((DependencyObject)sender, data, DragDropEffects.Move);

                    _isDragging = false;
                    _draggedItem = null;
                }
            }
        }
    }

    private void Block_DragOver(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent("Block"))
        {
            e.Effects = DragDropEffects.Move;
        }
        else
        {
            e.Effects = DragDropEffects.None;
        }
        e.Handled = true;
    }

    private void Block_Drop(object sender, DragEventArgs e)
    {
        if (e.Data.GetDataPresent("Block"))
        {
            var droppedBlock = e.Data.GetData("Block") as Models.Block;
            var targetBlock = (sender as FrameworkElement)?.DataContext as Models.Block;

            if (droppedBlock != null && targetBlock != null && droppedBlock != targetBlock)
            {
                // Find the parent chapters
                Chapter? sourceChapter = null;
                Chapter? targetChapter = null;

                foreach (var chapter in _viewModel.CurrentProject.Chapters)
                {
                    if (chapter.Blocks.Contains(droppedBlock))
                        sourceChapter = chapter;
                    if (chapter.Blocks.Contains(targetBlock))
                        targetChapter = chapter;
                }

                if (sourceChapter != null && targetChapter != null)
                {
                    int oldIndex = sourceChapter.Blocks.IndexOf(droppedBlock);
                    int newIndex = targetChapter.Blocks.IndexOf(targetBlock);

                    if (sourceChapter == targetChapter && oldIndex >= 0 && newIndex >= 0)
                    {
                        // Same chapter - just reorder
                        sourceChapter.Blocks.Move(oldIndex, newIndex);
                    }
                    else if (oldIndex >= 0 && newIndex >= 0)
                    {
                        // Different chapters - move block
                        sourceChapter.Blocks.RemoveAt(oldIndex);
                        targetChapter.Blocks.Insert(newIndex, droppedBlock);
                    }

                    _viewModel.PreviewViewModel.RequestRender(_viewModel.CurrentProject);
                }
            }
        }
    }

    #endregion
}
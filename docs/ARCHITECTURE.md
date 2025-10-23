# Memoir Editor - Architecture Documentation

## Overview

Memoir Editor is built using a modern, maintainable architecture based on:
- **MVVM** (Model-View-ViewModel) pattern
- **Dependency Injection** via Microsoft.Extensions.Hosting
- **Source Generators** from CommunityToolkit.Mvvm
- **Service-Oriented Design** with clear interfaces

## Core Principles

### 1. Separation of Concerns
- **Models**: Pure data structures (no business logic)
- **ViewModels**: Presentation logic and state management
- **Services**: Business logic and external interactions
- **Views**: XAML UI with minimal code-behind

### 2. Dependency Injection
All dependencies are injected via constructor injection, registered in `App.xaml.cs`:

```csharp
// Services
services.AddSingleton<IQuestPdfRenderer, QuestPdfRenderer>();
services.AddSingleton<IRenderQueue, RenderQueueService>();
services.AddSingleton<IWebViewService, WebViewService>();

// ViewModels
services.AddSingleton<MainViewModel>();
services.AddTransient<EditorViewModel>();
```

**Service Lifetimes:**
- **Singleton**: Shared state across app (renderers, queues, repositories)
- **Transient**: New instance per request (ViewModels for different contexts)
- **Scoped**: Not used in WPF (no natural scope like in web apps)

### 3. MVVM with Source Generators

Using `CommunityToolkit.Mvvm`, we leverage source generators for:

```csharp
public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private MemoirProject _currentProject;  // Auto-generates OnPropertyChanged

    [RelayCommand]
    private async Task SaveProjectAsync()   // Auto-generates ICommand
    {
        // Implementation
    }
}
```

Benefits:
- Less boilerplate code
- Compile-time safety
- Better performance than reflection-based approaches

## Data Flow

### 1. User Interaction Flow
```
User Action (View)
    ↓
Command/Binding (ViewModel)
    ↓
Service Call (Interface)
    ↓
Business Logic (Service Implementation)
    ↓
Update Model
    ↓
Property Changed Notification
    ↓
UI Update (Binding)
```

### 2. Rendering Pipeline
```
Text Edit
    ↓
ViewModel Update
    ↓
RenderQueue.QueueRender() [Debounced]
    ↓
IQuestPdfRenderer.GeneratePdfAsync()
    ↓
byte[] PDF data
    ↓
IWebViewService.LoadPdfAsync()
    ↓
WebView2 Display
```

### 3. Save/Load Flow
```
User: Save
    ↓
MainViewModel.SaveProjectCommand
    ↓
IProjectRepository.SaveAsync()
    ↓
JSON Serialization
    ↓
File.WriteAllTextAsync()
```

## Key Components

### Models
**Purpose**: Represent data structures

- `MemoirProject`: Root project container
- `Chapter`: Hierarchical content organization
- `Block`: Individual content sections
- `Metadata`: Project settings and info
- `GlobalSettings`: Application-wide preferences

**Characteristics**:
- Immutable where possible
- ObservableCollection for UI binding
- No dependencies on ViewModels or Services

### ViewModels
**Purpose**: Presentation logic and state

- `MainViewModel`: Orchestrates entire app
- `EditorViewModel`: Text editing logic
- `ChapterTreeViewModel`: Chapter/block hierarchy
- `PreviewViewModel`: PDF preview management
- `ProjectManagementViewModel`: File operations

**Responsibilities**:
- Expose properties for binding
- Provide commands for user actions
- Coordinate between services
- Transform model data for display

### Services
**Purpose**: Business logic and external interactions

- `QuestPdfRenderer`: PDF generation
- `RenderQueueService`: Debounced background rendering
- `WebViewService`: WebView2 management
- `FileProjectRepository`: Load/save projects
- `AutoSaveService`: Periodic saving
- `ImageService`: Image processing
- `ExportService`: PDF export

**Characteristics**:
- Interface-based (testable)
- Stateless where possible
- Thread-safe for background operations

### Views
**Purpose**: XAML UI

- `MainWindow`: Primary application window
- Fully responsive layout with Grid
- Minimal code-behind (DI only)

## Threading Model

### UI Thread
- All ViewModel property changes
- Command execution starts
- Binding updates

### Background Threads
- PDF rendering (`Task.Run` in QuestPdfRenderer)
- File I/O operations
- Image processing

### Synchronization
```csharp
// Example: Update UI from background thread
await Application.Current.Dispatcher.InvokeAsync(() =>
{
    CurrentPage = newPage;
});
```

## Error Handling Strategy

### Levels
1. **Service Level**: Try/catch with logging
2. **ViewModel Level**: User-friendly error messages
3. **Global Level**: Unhandled exception handler in App

### Example
```csharp
// Service
public async Task<byte[]> GeneratePdfAsync(MemoirProject project)
{
    try
    {
        return await Task.Run(() => /* PDF generation */);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "PDF generation failed");
        throw new PdfGenerationException("Failed to generate PDF", ex);
    }
}

// ViewModel
[RelayCommand]
private async Task ExportAsync()
{
    try
    {
        await _exportService.ExportToPdfAsync(CurrentProject, path);
    }
    catch (PdfGenerationException ex)
    {
        // Show error dialog to user
        MessageBox.Show($"Export failed: {ex.Message}");
    }
}
```

## Testing Strategy (Planned)

### Unit Tests
- Test ViewModels in isolation with mock services
- Test service implementations
- Test model validation logic

### Integration Tests
- Test full save/load cycle
- Test PDF generation pipeline
- Test UI workflows

### Example
```csharp
[Fact]
public async Task SaveProject_WithValidData_SavesSuccessfully()
{
    // Arrange
    var mockRepo = new Mock<IProjectRepository>();
    var vm = new MainViewModel(mockRepo.Object, ...);

    // Act
    await vm.SaveProjectCommand.ExecuteAsync(null);

    // Assert
    mockRepo.Verify(r => r.SaveAsync(It.IsAny<MemoirProject>(), It.IsAny<string>()), Times.Once);
}
```

## Performance Considerations

### Debouncing
Render requests are debounced (500ms) to avoid excessive PDF generation during typing.

### Lazy Loading
ViewModels are created on-demand (Transient lifetime) to reduce startup time.

### Caching
- Rendered PDFs are cached in memory (planned)
- Temporary files are cleaned up periodically

### Memory Management
- Large objects (PDF byte arrays) are disposed promptly
- Images are resized before embedding
- WebView2 cache is managed

## Future Enhancements

### Planned Architecture Improvements
1. **Event Aggregation**: Use `WeakReferenceMessenger` for decoupled communication
2. **Undo/Redo**: Command pattern with history stack
3. **Plugin System**: MEF-based extensibility
4. **Localization**: Resource-based multi-language support
5. **Telemetry**: Optional analytics and crash reporting

### Performance Optimizations
1. **Incremental Rendering**: Only re-render changed pages
2. **Virtual Scrolling**: For large chapter lists
3. **Background Compilation**: Pre-render next/previous pages
4. **Thumbnail Cache**: Fast chapter preview

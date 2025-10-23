# Development Guide

## Setting Up Development Environment

### Prerequisites
1. **Visual Studio 2022** (17.8 or later) or **Rider 2024.1+**
   - Workload: ".NET Desktop Development"
   - Optional: "Windows App SDK" for modern controls

2. **.NET 9.0 SDK**
   ```bash
   dotnet --version  # Should show 9.0.x
   ```

3. **WebView2 Runtime**
   - Usually pre-installed on Windows 10/11
   - [Download if needed](https://developer.microsoft.com/microsoft-edge/webview2/)

### Clone and Build
```bash
git clone <repository-url>
cd Writer
dotnet restore src/MemoirEditor
dotnet build src/MemoirEditor
```

### Run
```bash
cd src/MemoirEditor
dotnet run
```

## Project Structure

```
MemoirEditor/
├── Models/              # Data models
│   ├── MemoirProject.cs
│   ├── Chapter.cs
│   ├── Block.cs
│   └── ...
├── ViewModels/          # MVVM ViewModels
│   ├── MainViewModel.cs
│   ├── EditorViewModel.cs
│   └── ...
├── Views/               # XAML views
│   └── MainWindow.xaml
├── Services/            # Service implementations
│   ├── QuestPdfRenderer.cs
│   ├── RenderQueueService.cs
│   └── ...
├── Interfaces/          # Service contracts
│   ├── IQuestPdfRenderer.cs
│   ├── IRenderQueue.cs
│   └── ...
├── Helpers/             # Utility classes
├── Converters/          # XAML value converters
└── App.xaml.cs          # DI configuration
```

## Coding Standards

### C# Style
- Use `file-scoped namespaces`
- Use `var` for local variables when type is obvious
- Use `_camelCase` for private fields
- Use `PascalCase` for public members
- Use `async`/`await` for all I/O operations

### MVVM Patterns
```csharp
// ✅ Good: Use source generators
[ObservableProperty]
private string _title;

// ❌ Bad: Manual property
private string _title;
public string Title
{
    get => _title;
    set => SetProperty(ref _title, value);
}

// ✅ Good: RelayCommand
[RelayCommand]
private async Task SaveAsync() { }

// ❌ Bad: Manual ICommand
public ICommand SaveCommand { get; }
```

### Dependency Injection
```csharp
// ✅ Good: Constructor injection
public class MyViewModel
{
    private readonly IMyService _service;

    public MyViewModel(IMyService service)
    {
        _service = service;
    }
}

// ❌ Bad: Service locator
var service = ServiceLocator.Get<IMyService>();
```

## Adding New Features

### 1. Add a New Service

**Step 1**: Define interface in `Interfaces/`
```csharp
namespace MemoirEditor.Interfaces;

public interface IMyService
{
    Task DoSomethingAsync();
}
```

**Step 2**: Implement in `Services/`
```csharp
namespace MemoirEditor.Services;

public class MyService : IMyService
{
    public async Task DoSomethingAsync()
    {
        await Task.CompletedTask;
    }
}
```

**Step 3**: Register in `App.xaml.cs`
```csharp
services.AddSingleton<IMyService, MyService>();
```

**Step 4**: Inject into ViewModel
```csharp
public MyViewModel(IMyService myService)
{
    _myService = myService;
}
```

### 2. Add a New ViewModel

**Step 1**: Create in `ViewModels/`
```csharp
public partial class MyViewModel : ObservableObject
{
    [ObservableProperty]
    private string _myProperty;

    [RelayCommand]
    private async Task MyCommandAsync()
    {
        // Implementation
    }
}
```

**Step 2**: Register in DI
```csharp
services.AddTransient<MyViewModel>();
```

**Step 3**: Create corresponding View (if needed)
```xaml
<UserControl x:Class="MemoirEditor.Views.MyView"
             ...>
    <!-- UI -->
</UserControl>
```

### 3. Add a New Model

**Step 1**: Create in `Models/`
```csharp
namespace MemoirEditor.Models;

public class MyModel
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
}
```

**Step 2**: Update parent models (if nested)
```csharp
public class MemoirProject
{
    // ...
    public ObservableCollection<MyModel> MyModels { get; set; } = new();
}
```

## Debugging

### Debug Configuration
Set breakpoints in:
- ViewModels (command handlers)
- Services (business logic)
- Converters (data transformation)

### Debugging XAML
Use Snoop or WPF Inspector:
```bash
# Install Snoop
choco install snoop
```

### Logging
Add logging to services:
```csharp
public class MyService
{
    private readonly ILogger<MyService> _logger;

    public MyService(ILogger<MyService> logger)
    {
        _logger = logger;
    }

    public async Task DoSomethingAsync()
    {
        _logger.LogInformation("Starting operation");
        // ...
        _logger.LogInformation("Operation completed");
    }
}
```

## Testing

### Unit Tests (Planned)
```bash
dotnet new xunit -n MemoirEditor.Tests
cd ../tests/MemoirEditor.Tests
dotnet add reference ../../src/MemoirEditor/MemoirEditor.csproj
dotnet add package Moq
```

**Example Test**:
```csharp
public class MainViewModelTests
{
    [Fact]
    public async Task SaveProject_CallsRepository()
    {
        // Arrange
        var mockRepo = new Mock<IProjectRepository>();
        var vm = new MainViewModel(mockRepo.Object, ...);

        // Act
        await vm.SaveProjectCommand.ExecuteAsync(null);

        // Assert
        mockRepo.Verify(r => r.SaveAsync(
            It.IsAny<MemoirProject>(),
            It.IsAny<string>()),
            Times.Once);
    }
}
```

## Build and Release

### Debug Build
```bash
dotnet build --configuration Debug
```

### Release Build
```bash
dotnet build --configuration Release
```

### Publish (Single-File Executable)
```bash
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

### Create Installer (Future)
Consider using:
- **WiX Toolset** for MSI installer
- **Inno Setup** for lightweight installer
- **MSIX** for Microsoft Store distribution

## Performance Profiling

### Memory Profiling
Use Visual Studio Diagnostic Tools:
1. Debug → Performance Profiler
2. Select "Memory Usage"
3. Start application
4. Take snapshots before/after operations

### CPU Profiling
1. Debug → Performance Profiler
2. Select "CPU Usage"
3. Identify hot paths in rendering

## Common Tasks

### Update NuGet Packages
```bash
dotnet list package --outdated
dotnet add package <PackageName>
```

### Format Code
```bash
dotnet format
```

### Analyze Code
```bash
dotnet build /p:EnforceCodeStyleInBuild=true
```

## Troubleshooting

### Issue: WebView2 not displaying
**Solution**: Ensure WebView2 runtime is installed

### Issue: PDF rendering is slow
**Solution**: Check debounce settings in `RenderQueueService`

### Issue: DI registration errors
**Solution**: Verify all interfaces have implementations registered in `App.xaml.cs`

## Resources

- [WPF Documentation](https://docs.microsoft.com/wpf)
- [CommunityToolkit.Mvvm](https://learn.microsoft.com/windows/communitytoolkit/mvvm)
- [QuestPDF Documentation](https://www.questpdf.com)
- [WebView2 Docs](https://docs.microsoft.com/microsoft-edge/webview2/)

using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MemoirEditor.Views;
using MemoirEditor.ViewModels;
using MemoirEditor.Services;
using MemoirEditor.Interfaces;

namespace MemoirEditor;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private readonly IHost _host;

    public App()
    {
        _host = Host.CreateApplicationBuilder()
            .ConfigureServices()
            .Build();
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        await _host.StartAsync();

        var mainWindow = _host.Services.GetRequiredService<MainWindow>();
        mainWindow.Show();

        base.OnStartup(e);
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        await _host.StopAsync();
        _host.Dispose();

        base.OnExit(e);
    }
}

/// <summary>
/// Extension method for service registration
/// </summary>
public static class ServiceCollectionExtensions
{
    public static HostApplicationBuilder ConfigureServices(this HostApplicationBuilder builder)
    {
        var services = builder.Services;

        // Windows
        services.AddSingleton<MainWindow>();

        // ViewModels
        services.AddSingleton<MainViewModel>();
        services.AddTransient<EditorViewModel>();
        services.AddTransient<ChapterTreeViewModel>();
        services.AddTransient<PreviewViewModel>();
        services.AddTransient<ProjectManagementViewModel>();

        // Services
        services.AddSingleton<IQuestPdfRenderer, QuestPdfRenderer>();
        services.AddSingleton<IRenderQueue, RenderQueueService>();
        services.AddSingleton<IWebViewService, WebViewService>();
        services.AddSingleton<IProjectRepository, FileProjectRepository>();
        services.AddSingleton<IAutoSaveService, AutoSaveService>();
        services.AddSingleton<IImageService, ImageService>();
        services.AddSingleton<IExportService, ExportService>();

        return builder;
    }
}


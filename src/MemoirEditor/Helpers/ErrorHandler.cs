using System.Windows;

namespace MemoirEditor.Helpers;

/// <summary>
/// Centralized error handling and user notification
/// </summary>
public static class ErrorHandler
{
    public static void ShowError(string message, string title = "Fehler")
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
        });
    }

    public static void ShowWarning(string message, string title = "Warnung")
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Warning);
        });
    }

    public static void ShowInfo(string message, string title = "Information")
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
        });
    }

    public static bool Confirm(string message, string title = "Bestätigung")
    {
        return Application.Current.Dispatcher.Invoke(() =>
        {
            var result = MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question);
            return result == MessageBoxResult.Yes;
        });
    }

    public static void HandleException(Exception ex, string context)
    {
        var message = $"{context}\n\nFehler: {ex.Message}";

        #if DEBUG
        message += $"\n\nStack Trace:\n{ex.StackTrace}";
        #endif

        ShowError(message);

        // Log to debug output
        System.Diagnostics.Debug.WriteLine($"[ERROR] {context}: {ex}");
    }
}

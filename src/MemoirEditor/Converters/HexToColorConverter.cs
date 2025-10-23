using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace MemoirEditor.Converters;

/// <summary>
/// Converts between hex color string and WPF Color
/// </summary>
public class HexToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string hexColor)
        {
            try
            {
                // Remove # if present
                if (hexColor.StartsWith("#"))
                    hexColor = hexColor.Substring(1);

                // Ensure it's 6 characters
                if (hexColor.Length == 6)
                {
                    byte r = byte.Parse(hexColor.Substring(0, 2), NumberStyles.HexNumber);
                    byte g = byte.Parse(hexColor.Substring(2, 2), NumberStyles.HexNumber);
                    byte b = byte.Parse(hexColor.Substring(4, 2), NumberStyles.HexNumber);
                    return Color.FromRgb(r, g, b);
                }
            }
            catch
            {
                // Return black on parse error
            }
        }

        return Colors.Black;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is Color color)
        {
            return $"#{color.R:X2}{color.G:X2}{color.B:X2}";
        }

        return "#000000";
    }
}

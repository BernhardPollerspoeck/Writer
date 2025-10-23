using System;
using System.Globalization;
using System.Windows.Data;

namespace MemoirEditor.Converters
{
    /// <summary>
    /// Compares two bound values for equality and returns true if they are equal.
    /// Used with a MultiBinding in XAML where DataTrigger.Value is set to True.
    /// </summary>
    public class EqualityMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length < 2)
                return false;

            var left = values[0];
            var right = values[1];

            // Handle nulls and boxing
            if (left == null && right == null)
                return true;
            if (left == null || right == null)
                return false;

            return Equals(left, right);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}

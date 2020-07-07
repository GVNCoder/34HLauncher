using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Launcher.Converters
{
    public class String2VisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.IsNullOrWhiteSpace((string) value)
                ? Visibility.Collapsed
                : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
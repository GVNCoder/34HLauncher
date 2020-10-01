using System;
using System.Globalization;
using System.Windows.Data;

namespace Launcher.Converters
{
    public class MathNegativeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var convertedValue = (double) value;
            return -convertedValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
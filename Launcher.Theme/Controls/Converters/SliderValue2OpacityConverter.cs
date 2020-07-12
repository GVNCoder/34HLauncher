using System;
using System.Globalization;
using System.Windows.Data;

namespace Launcher.XamlThemes.Controls.Converters
{
    public class SliderValue2OpacityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var decimalValue = (double) value;
            return decimalValue / 100;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
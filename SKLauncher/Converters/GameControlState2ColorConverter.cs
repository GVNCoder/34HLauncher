using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Launcher.Converters
{
    public class GameControlState2ColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var state = (bool) value;

            return state
                ? Color.FromArgb(0xFF, 0x00, 0xDD, 0x4E)
                : Application.Current.FindResource("PrimaryLight400Color");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
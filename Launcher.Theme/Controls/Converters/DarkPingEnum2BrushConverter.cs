using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Launcher.XamlThemes.Controls.Shared;

namespace Launcher.XamlThemes.Controls.Converters
{
    public class DarkPingEnum2BrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var pingEnum = (PingVisualizerEnum) value;
            switch (pingEnum)
            {
                case PingVisualizerEnum.Good:
                    return new SolidColorBrush(Color.FromArgb(0xFF, 0x00, 0xD3, 0x27));
                case PingVisualizerEnum.Normal:
                    return new SolidColorBrush(Color.FromArgb(0xFF, 0xF5, 0xE7, 0x27));
                case PingVisualizerEnum.Middle:
                    return Brushes.Orange;
                case PingVisualizerEnum.Bad:
                    return Brushes.OrangeRed;
                case PingVisualizerEnum.VeryBad:
                    return Brushes.Red;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
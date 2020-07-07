using System;
using System.Globalization;
using System.Windows.Data;
using Launcher.XamlThemes.Controls.Shared;

namespace Launcher.XamlThemes.Controls.Converters
{
    public class LightPingEnum2IconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var pingEnum = (PingVisualizerEnum)value;
            switch (pingEnum)
            {
                case PingVisualizerEnum.Good:
                    return "";
                case PingVisualizerEnum.Normal:
                    return "";
                case PingVisualizerEnum.Middle:
                    return "";
                case PingVisualizerEnum.Bad:
                    return "";
                case PingVisualizerEnum.VeryBad:
                    return "";
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
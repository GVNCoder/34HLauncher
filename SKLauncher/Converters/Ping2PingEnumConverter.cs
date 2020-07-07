using System;
using System.Globalization;
using System.Windows.Data;
using Launcher.XamlThemes.Controls.Shared;

namespace Launcher.Converters
{
    public class Ping2PingEnumConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var pingValue = (int) value;
            var convertedValue = PingVisualizerEnum.Good;

            if (pingValue == 999)
                convertedValue = PingVisualizerEnum.VeryBad;

            if (pingValue < 999 && pingValue >= 300)
                convertedValue = PingVisualizerEnum.Bad;

            if (pingValue < 300 && pingValue >= 100)
                convertedValue = PingVisualizerEnum.Middle;

            if (pingValue < 100 && pingValue >= 50)
                convertedValue = PingVisualizerEnum.Normal;

            return convertedValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
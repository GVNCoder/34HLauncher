using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

using Launcher.Core;

namespace Launcher.Converters
{
    public class EventType2ColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // get value
            if (! (value is EventType eventType))
                throw new InvalidCastException($"{nameof(EventType2ColorConverter)} value: {value}");

            Color color;

            // select correct color
            switch (eventType)
            {
                case EventType.Info:
                    color = Colors.DodgerBlue;
                    break;
                case EventType.Success:
                    color = Colors.LimeGreen;
                    break;
                case EventType.Warn:
                    color = Colors.Yellow;
                    break;
                case EventType.Error:
                    color = Colors.Crimson;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return color;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
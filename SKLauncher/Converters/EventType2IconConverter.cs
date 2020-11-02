using System;
using System.Globalization;
using System.Windows.Data;

using Launcher.Core;

namespace Launcher.Converters
{
    public class EventType2IconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // get value
            if (! (value is EventType eventType))
                throw new InvalidCastException($"{nameof(EventType2IconConverter)} value: {value}");

            string icon;

            // select correct color
            switch (eventType)
            {
                case EventType.Info:
                    icon = "";
                    break;
                case EventType.Success:
                    icon = "";
                    break;
                case EventType.Warn:
                    icon = "";
                    break;
                case EventType.Error:
                    icon = "";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return icon;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
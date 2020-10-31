using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Launcher.Core.Shared;

namespace Launcher.Converters
{
    public class EventLevel2ColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //if (!(value is EventLogLevel level)) throw new InvalidCastException();

            //switch (level)
            //{
            //    case EventLogLevel.Message: return Color.FromArgb(0xff, 0x7d, 0x7d, 0x7d); 
            //    case EventLogLevel.Warning: return Color.FromArgb(0xff, 0xff, 0xea, 0x2a);
            //    case EventLogLevel.Error: return Color.FromArgb(0xff, 0xff, 0x2f, 0x2a);
            //    default: throw new ArgumentOutOfRangeException();
            //}

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
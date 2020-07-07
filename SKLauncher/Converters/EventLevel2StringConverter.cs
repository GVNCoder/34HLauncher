using System;
using System.Globalization;
using System.Windows.Data;
using Launcher.Core.Shared;

namespace Launcher.Converters
{
    public class EventLevel2StringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // getting value
            if (!(value is EventLogLevel level)) throw new InvalidCastException();

            switch (level)
            {
                case EventLogLevel.Error: return "";
                case EventLogLevel.Warning: return "";
                case EventLogLevel.Message: return "";
                default: return string.Empty;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
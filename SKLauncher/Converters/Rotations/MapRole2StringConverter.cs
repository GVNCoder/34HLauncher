using System;
using System.Globalization;
using System.Windows.Data;

using Zlo4NET.Api.Models.Shared;

namespace Launcher.Converters
{
    public class MapRole2StringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var role = (ZMapRole) value;
            return role.ToString().ToUpper();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
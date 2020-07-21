using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

using Zlo4NET.Api.Models.Shared;

namespace Launcher.Converters
{
    public class MapRole2VisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var role = (ZMapRole) value;
            switch (role)
            {
                case ZMapRole.Current:
                case ZMapRole.Next:
                    return Visibility.Visible;
                case ZMapRole.Other:
                default:
                    return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
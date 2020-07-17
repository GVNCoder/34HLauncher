using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using Zlo4NET.Api.Models.Shared;

namespace Launcher.Converters
{
    public class MapRole2ColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var role = (ZMapRole) value;
            switch (role)
            {
                case ZMapRole.Current:
                    return Application.Current.Resources["PrimaryLight500Brush"];
                case ZMapRole.Next:
                    return Application.Current.Resources["PrimaryDark900Brush"];
                case ZMapRole.Other:
                default:
                    return Binding.DoNothing;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
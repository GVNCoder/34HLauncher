using System.Windows.Media;
using System.Globalization;
using System.Windows.Data;
using System.Drawing;
using System;

using Launcher.Services;

namespace Launcher.Converters
{
    public class BF3RankNumberToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is byte rank)) throw new InvalidOperationException();

            if (rank > 145)
                return Resources.Service.ResourceManager.GetRankImage("BF3.145.png");

            return Resources.Service.ResourceManager.GetRankImage($"BF3.{rank}.png");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BF4RankNumberToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // getting value
            if (!(value is byte rank)) throw new InvalidOperationException();

            if (rank > 140)
                return Resources.Service.ResourceManager.GetRankImage("BF4.r140.png");

            return Resources.Service.ResourceManager.GetRankImage($"BF4.r{rank}.png");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
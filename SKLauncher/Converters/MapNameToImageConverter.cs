using System.Globalization;
using System.Windows.Media;
using System.Windows.Data;
using System;

namespace Launcher.Converters
{
    #region Base class

    /// <summary>
    /// Base class for a MapNameToImageSource converters
    /// </summary>
    public class MapNameToImageSourceConverterBase : IValueConverter
    {
        #region Protected fields

        // the image resource prefix: bf3_ bf4_ bfh_
        protected readonly string _imageResourcePrefix;

        // the image resource path
        protected readonly string _resourcePath;

        // the map name extractor
        protected readonly Func<object, string> _mapNameExtractor;

        #endregion // Protected fields

        #region Constructor

        /// <summary>
        /// The default ctor
        /// </summary>
        /// <param name="prefix">The resource prefix</param>
        public MapNameToImageSourceConverterBase(string prefix, string resource, Func<object, string> extractor)
        {
            _imageResourcePrefix = prefix;
            _resourcePath        = resource;
            _mapNameExtractor    = extractor;
        }

        #endregion // Constructor

        #region IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // getting value
            var mapName = _mapNameExtractor(value);

            // null check
            if (string.IsNullOrEmpty(mapName)) return Binding.DoNothing;

            // prepare mapName for correct searching image resource
            var resourceKey = $"{_resourcePath}.{_imageResourcePrefix}{mapName.Replace(" ", string.Empty)}.png";

            // search and create resource
            ImageSource imageResource = null;
            try
            {
                imageResource = Resources.Service.ResourceManager.GetMapImage(resourceKey);
            }
            catch
            {
                // catch any exceptions
            }

            imageResource?.Freeze();
            return imageResource ?? Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    #endregion

    // use converter parameter

    public class bf3MapNameToImageSourceConverter : MapNameToImageSourceConverterBase
    {
        public bf3MapNameToImageSourceConverter() : base("bf3_", "BF3", (value) => value as string) { }
    }

    public class bf3COOPMapNameToImageSourceConverter : MapNameToImageSourceConverterBase
    {
        public bf3COOPMapNameToImageSourceConverter() : base("bf3_", "BF3CoOp", (value) => value as string) { }
    }

    public class bf4MapNameToImageSourceConverter : MapNameToImageSourceConverterBase
    {
        public bf4MapNameToImageSourceConverter() : base("bf4_", "BF4", (value) =>
        {
            // getting value
            var mapName = value as string;

            // normalize map name
            if (mapName.Contains("Zavod")) mapName = mapName.Replace(":", "");

            return mapName;
        }) { }
    }

    public class bfhMapNameToImageSourceConverter : MapNameToImageSourceConverterBase
    {
        public bfhMapNameToImageSourceConverter() : base("bfh_", "BFH", (value) => value as string) { }
    }
}
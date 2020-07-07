using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Launcher.XamlThemes.Theming
{
    public static class ThemeManager
    {
        private const string _ResourcesDirectory = "resources";
        private const string _ResourceFileName = "resource.34h";
        private const string _BackgroundImageResourceKey = "BackgroundImage";
        private static readonly string _resourceFilePath;

        static ThemeManager()
        {
            _resourceFilePath = Path.Combine(_ResourcesDirectory, _ResourceFileName);
        }

        private static ResourceDictionary _entryPoint;
        private static ResourceDictionary _applicationResourceDictionary;
        private static ThemeResourceDictionary _currentThemeDictionary;
        private static AccentResourceDictionary _currentAccentDictionary;
        private static BitmapImage _backgroundImage;

        private static ThemeEnum _currentTheme;
        private static AccentEnum _currentAccent;
        private static BackgroundImageEnum _imageSourceType;

        private static BitmapImage _LoadImageFromStream(Stream source)
        {
            var image = new BitmapImage();

            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.StreamSource = source;
            image.EndInit();
            image.Freeze();

            return image;
        }

        private static void _SetupImage(BitmapImage image)
        {
            _entryPoint.Remove(_BackgroundImageResourceKey);
            _entryPoint.Add(_BackgroundImageResourceKey, image);
        }

        private static void _TryDeleteCustomImage()
        {
            try
            {
                File.Delete(_resourceFilePath);
            }
            catch (Exception e)
            {
                // ignore
            }
        }

        private static bool _ValidateImageFile(string source)
        {
            var stream = File.OpenRead(source);
            if ((stream.Length * .000001d) > ImageFileSizeLimit) return false;

            var customImage = _LoadImageFromStream(stream);
            if (customImage.PixelHeight > ImagePixelHeightLimit ||
                customImage.PixelWidth > ImagePixelWightLimit) return false;

            return true;
        }

        #region Public interface

        public static ThemeEnum CurrentTheme => _currentTheme;
        public static AccentEnum CurrentAccent => _currentAccent;

        public const float ImageFileSizeLimit = 10f;
        public const float ImagePixelHeightLimit = 1080f;
        public const float ImagePixelWightLimit = 2048f;

        public static void Initialize(Application application)
        {
            _entryPoint = new ResourceDictionary();
            _applicationResourceDictionary = application.Resources;
            _applicationResourceDictionary.MergedDictionaries.Add(_entryPoint);
        }

        public static void ApplyAccent(AccentEnum accent)
        {
            if (_currentAccent == accent) return;

            var newAccentDictionary = new AccentResourceDictionary
                { Source = new Uri($"/Launcher.Theme;component/Accents/{accent}.xaml", UriKind.Relative) };

            _entryPoint.MergedDictionaries.Insert(0, newAccentDictionary);
            _entryPoint.MergedDictionaries.Remove(_currentAccentDictionary);

            _currentAccentDictionary = newAccentDictionary;
            _currentAccent = accent;
        }

        public static void ApplyTheme(ThemeEnum theme)
        {
            if (_currentTheme == theme) return;

            var newThemeDictionary = new ThemeResourceDictionary
                { Source = new Uri($"/Launcher.Theme;component/Themes/Theme.{theme}.xaml", UriKind.Relative) };

            _entryPoint.MergedDictionaries.Insert(1, newThemeDictionary);
            _entryPoint.MergedDictionaries.Remove(_currentThemeDictionary);

            _currentThemeDictionary = newThemeDictionary;
            _currentTheme = theme;
        }

        public static void ApplyBackgroundImage()
        {
            var pathToCustomResource = Path.Combine(_ResourcesDirectory, _ResourceFileName);
            var isCustom = File.Exists(pathToCustomResource);
            var pathToResource = isCustom
                ? pathToCustomResource
                : "Launcher.Resources.Images.bg_default.jpg";
            var resourceStream = isCustom
                ? File.OpenRead(pathToResource)
                : Assembly.LoadFrom(
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Launcher.Resources.dll"))
                    .GetManifestResourceStream(pathToResource);

            var image = _LoadImageFromStream(resourceStream);

            _SetupImage(image);

            resourceStream.Close();
            resourceStream.Dispose();

            _backgroundImage = image;
        }

        public static bool TrySetBackgroundImage(BackgroundImageEnum imageSource, string path)
        {
            switch (imageSource)
            {
                case BackgroundImageEnum.Default:
                {
                    var filePath = Path.Combine(_ResourcesDirectory, _ResourceFileName);
                    if (File.Exists(filePath))
                    {
                        _backgroundImage?.StreamSource.Dispose();
                        File.Delete(filePath);
                    }

                    return true;
                }
                case BackgroundImageEnum.Custom:
                {
                    var stream = File.OpenRead(path);
                    var image = _LoadImageFromStream(stream);

                    if ((image.StreamSource.Length * .000001) > ImageFileSizeLimit) // image size over 10Mb
                    {
                        return false;
                    }

                    if (image.PixelHeight > ImagePixelHeightLimit || image.PixelWidth > ImagePixelWightLimit) // image pixel size over 2k 2048x1080
                    {
                        return false;
                    }

                    stream.Dispose();

                    var filePath = Path.Combine(_ResourcesDirectory, _ResourceFileName);
                    File.Copy(path, filePath, true);

                    return true;
                }
                case BackgroundImageEnum.None:
                default: throw new ArgumentOutOfRangeException(nameof(imageSource));
            }
        }

        public static bool TrySetupBackgroundImage(BackgroundImageEnum imageSource, string source = null)
        {
            var image = default(BitmapImage);
            var stream = default(Stream);

            switch (imageSource)
            {
                case BackgroundImageEnum.Default:
                    stream = Assembly.LoadFrom(
                            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Launcher.Resources.dll"))
                        .GetManifestResourceStream("Launcher.Resources.Images.bg_default.jpg");
                    image = _LoadImageFromStream(stream);

                    stream.Close();
                    stream.Dispose();
                    
                    _TryDeleteCustomImage();

                    break;
                case BackgroundImageEnum.Custom:
                    if (string.IsNullOrEmpty(source)) return false;

                    try
                    {
                        var validationResult = _ValidateImageFile(source);
                        if (!validationResult) return false;

                        File.Copy(source, _resourceFilePath, false);

                        stream = File.OpenRead(_resourceFilePath);
                        image = _LoadImageFromStream(stream);

                        stream.Close();
                        stream.Dispose();
                    }
                    catch (Exception e)
                    {
                        return false;
                    }

                    break;
                case BackgroundImageEnum.None:
                default: throw new ArgumentOutOfRangeException();
            }

            _SetupImage(image);
            _backgroundImage = image;

            return true;
        }

        #endregion
    }
}
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Reflection;
using System.Windows.Media.Imaging;

namespace Launcher.XamlThemes.Theming
{
    public static class ThemeManager
    {
        #region Public interface

        public static event EventHandler<Exception> Error;

        // ReSharper disable once ConvertToAutoPropertyWithPrivateSetter
        public static LauncherTheme CurrentTheme => _currentTheme;

        public const float BackgroundImageWightLimit = 2048f;
        public const float BackgroundImageHeightLimit = 1080f;

        public const float MenuCardImageWightLimit = 200f;
        public const float MenuCardImageHeightLimit = 330f;

        public const float BackgroundImageSizeLimit = 10f;
        public const float MenuCardImageSizeLimit = .5f;

        public static void Initialize(ResourceDictionary applicationResourceDictionary)
        {
            // we add our resource dictionary to the starting position so that we only deal with it and not affect the order of standard resources
            // create our root resource dictionary
            _rootDictionary = new ResourceDictionary();

            // inject it
            applicationResourceDictionary.MergedDictionaries.Add(_rootDictionary);

            // get resources assembly ref
            _resourceAssemblyRef = AppDomain.CurrentDomain.GetAssemblies()
                .Single(a => a.GetName().Name == "Launcher.Resources");

            // load image resources
            foreach (var imageResource in _imageResources)
            {
                // check if we have access to external image resource
                var imageStream = File.Exists(imageResource.ExternalPath)
                    ? File.OpenRead(imageResource.ExternalPath)
                    : _resourceAssemblyRef.GetManifestResourceStream(imageResource.InternalPath);

                // load image
                imageResource.ImageRef = _CreateImageFromStream(imageStream);

                // free mem
                imageStream.Close();
            }
        }

        public static void ApplyTheme(LauncherTheme theme)
        {
            // validate incoming parameter
            if (_currentTheme == theme)
            {
                return;
            }

            if (theme == LauncherTheme.None)
            {
                throw new ArgumentException(@"Theme parameter is None", nameof(theme));
            }

            // create theme resource
            var newThemeDictionary = new ThemeResourceDictionary
            {
                Source = new Uri($"/Launcher.Theme;component/Themes/Theme.{theme}.xaml", UriKind.Relative)
            };

            // inject theme resource
            _rootDictionary.MergedDictionaries.Insert(_themeResourcePositionIndex, newThemeDictionary);
            _rootDictionary.MergedDictionaries.Remove(_currentThemeDictionary);

            // save state
            _currentThemeDictionary = newThemeDictionary;
            _currentTheme = theme;
        }

        public static void ApplyAccent(AccentEnum accent)
        {
            var newAccentDictionary = new AccentResourceDictionary
                { Source = new Uri($"/Launcher.Theme;component/Accents/{accent}.xaml", UriKind.Relative) };

            _rootDictionary.MergedDictionaries.Insert(_accentResourcePositionIndex, newAccentDictionary);
        }

        public static void ResetBackgroundImage()
        {
            // get image resource instance
            var imageResource = _imageResources.Single(ir => ir.ResourceKey == _ResourceKeyBackgroundImage);

            // load resource steam in mem
            var imageStream = _resourceAssemblyRef
                .GetManifestResourceStream(imageResource.InternalPath);

            // create image to set
            var image = _CreateImageFromStream(imageStream);

            // replace current image
            _rootDictionary.Remove(_ResourceKeyBackgroundImage);
            _rootDictionary.Add(_ResourceKeyBackgroundImage, image);

            // free mem resources
            imageStream.Close();

            // update image ref
            imageResource.ImageRef = image;
        }

        public static bool SetCustomBackgroundImage(string path)
        {
            // try to load the custom resource
            try
            {
                // try get file stream
                var imageStream = File.OpenRead(path);

                // create and validate image file
                var image = _CreateImageFromStream(imageStream);

                // check file size
                // should be less then 10 MBytes
                if (imageStream.Length * .000001d > BackgroundImageSizeLimit)
                {
                    return false;
                }

                // check image size
                // should be 2k
                if (image.PixelWidth > BackgroundImageWightLimit || image.PixelHeight > BackgroundImageHeightLimit)
                {
                    return false;
                }

                // ok! the image is valid ;)
                // free mem
                imageStream.Close();

                // get image resource instance
                var imageResource = _imageResources.Single(ir => ir.ResourceKey == _ResourceKeyBackgroundImage);

                // copy resource into save directory
                File.Copy(sourceFileName: path, destFileName: imageResource.ExternalPath, overwrite: true);

                // replace current image
                _rootDictionary.Remove(_ResourceKeyBackgroundImage);
                _rootDictionary.Add(_ResourceKeyBackgroundImage, image);

                // update image ref
                imageResource.ImageRef = image;
            }
            catch (Exception exception)
            {
                // raise Error event
                OnError(exception);

                return false;
            }

            return true;
        }

        public static void ApplyImages()
        {
            // apply all image resources
            foreach (var imageResource in _imageResources)
            {
                // replace image resource
                _rootDictionary.Remove(imageResource.ResourceKey);
                _rootDictionary.Add(imageResource.ResourceKey, imageResource.ImageRef);
            }
        }

        public static bool SetCustomMenuCardImage(ThemeMenuCardImage menuCard, string path)
        {
            // try to load the custom resource
            try
            {
                // try get file stream
                var imageStream = File.OpenRead(path);

                // create and validate image file
                var image = _CreateImageFromStream(imageStream);

                // check file size
                // should be less then 0.5 MBytes
                if (imageStream.Length * .000001d > MenuCardImageSizeLimit)
                {
                    return false;
                }

                // check image size
                // should be 2k
                if (image.PixelWidth > MenuCardImageWightLimit || image.PixelHeight > MenuCardImageHeightLimit)
                {
                    return false;
                }

                // ok! the image is valid ;)
                // free mem
                imageStream.Close();

                // get resourceKey
                var resourceKey = string.Empty;

                // ReSharper disable once SwitchStatementMissingSomeCases
                switch (menuCard)
                {
                    case ThemeMenuCardImage.BF3: resourceKey = _ResourceKeyBF3CardImage;
                        break;
                    case ThemeMenuCardImage.BF4: resourceKey = _ResourceKeyBF4CardImage;
                        break;
                    case ThemeMenuCardImage.BFH: resourceKey = _ResourceKeyBFHCardImage;
                        break;
                }

                // get image resource instance
                var imageResource = _imageResources.Single(ir => ir.ResourceKey == resourceKey);

                // copy resource into save directory
                File.Copy(sourceFileName: path, destFileName: imageResource.ExternalPath, overwrite: true);

                // replace current image
                _rootDictionary.Remove(resourceKey);
                _rootDictionary.Add(resourceKey, image);

                // update image ref
                imageResource.ImageRef = image;
            }
            catch (Exception exception)
            {
                // raise Error event
                OnError(exception);

                return false;
            }

            return true;
        }

        public static void ResetMenuCardImage(ThemeMenuCardImage menuCard)
        {
            // get resourceKey
            var resourceKey = string.Empty;

            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (menuCard)
            {
                case ThemeMenuCardImage.BF3:
                    resourceKey = _ResourceKeyBF3CardImage;
                    break;
                case ThemeMenuCardImage.BF4:
                    resourceKey = _ResourceKeyBF4CardImage;
                    break;
                case ThemeMenuCardImage.BFH:
                    resourceKey = _ResourceKeyBFHCardImage;
                    break;
            }

            // get image resource instance
            var imageResource = _imageResources.Single(ir => ir.ResourceKey == resourceKey);

            // load resource steam in mem
            var imageStream = _resourceAssemblyRef
                .GetManifestResourceStream(imageResource.InternalPath);

            // create image to set
            var image = _CreateImageFromStream(imageStream);

            // replace current image
            _rootDictionary.Remove(resourceKey);
            _rootDictionary.Add(resourceKey, image);

            // free mem resources
            imageStream.Close();

            // update image ref
            imageResource.ImageRef = image;
        }

        public static BitmapImage GetImageResourceByKey(string key)
        {
            // try get image resource by key
            var imageResource = _imageResources.FirstOrDefault(ir => ir.ResourceKey == key);

            return imageResource.ImageRef;
        }

        #endregion

        #region Private helpers
        
        private static LauncherTheme _currentTheme;
        private static ResourceDictionary _rootDictionary;
        private static ThemeResourceDictionary _currentThemeDictionary;
        private static Assembly _resourceAssemblyRef;
        private static readonly _ImageResource[] _imageResources;

        private const string _ResourceKeyBackgroundImage = "BackgroundImage";
        private const string _ResourceKeyBF3CardImage = "BF3MenuCardImage";
        private const string _ResourceKeyBF4CardImage = "BF4MenuCardImage";
        private const string _ResourceKeyBFHCardImage = "BFHMenuCardImage";

        private const string _ResourcesDirectoryName = "resources";
        private const string _ResourcesFileExtension = "34h";

        private const string _BaseResourcePath = "Launcher.Resources.Images";
        private const string _DefaultBackgroundImageResourcePathPiece = "bg_default.jpg";
        private const string _DefaultBF3CardImageResourcePathPiece = "MainMenu.BF3MenuCardImage.png";
        private const string _DefaultBF4CardImageResourcePathPiece = "MainMenu.BF4MenuCardImage.png";
        private const string _DefaultBFHCardImageResourcePathPiece = "MainMenu.BFHMenuCardImage.png";

        private const int _themeResourcePositionIndex = 1;
        private const int _accentResourcePositionIndex = 0;

        static ThemeManager()
        {
            // build image resource lib
            _imageResources = new[]
            {
                new _ImageResource
                {
                    InternalPath = $"{_BaseResourcePath}.{_DefaultBackgroundImageResourcePathPiece}",
                    ExternalPath = Path.Combine(_ResourcesDirectoryName, Path.ChangeExtension("b", _ResourcesFileExtension)),
                    ResourceKey = _ResourceKeyBackgroundImage
                },
                new _ImageResource
                {
                    InternalPath = $"{_BaseResourcePath}.{_DefaultBF3CardImageResourcePathPiece}",
                    ExternalPath = Path.Combine(_ResourcesDirectoryName, Path.ChangeExtension("3", _ResourcesFileExtension)),
                    ResourceKey = _ResourceKeyBF3CardImage
                },
                new _ImageResource
                {
                    InternalPath = $"{_BaseResourcePath}.{_DefaultBF4CardImageResourcePathPiece}",
                    ExternalPath = Path.Combine(_ResourcesDirectoryName, Path.ChangeExtension("4", _ResourcesFileExtension)),
                    ResourceKey = _ResourceKeyBF4CardImage
                },
                new _ImageResource
                {
                    InternalPath = $"{_BaseResourcePath}.{_DefaultBFHCardImageResourcePathPiece}",
                    ExternalPath = Path.Combine(_ResourcesDirectoryName, Path.ChangeExtension("h", _ResourcesFileExtension)),
                    ResourceKey = _ResourceKeyBFHCardImage
                }
            };
        }

        private static BitmapImage _CreateImageFromStream(Stream source)
        {
            var image = new BitmapImage();

            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.StreamSource = source;
            image.EndInit();
            image.Freeze();

            return image;
        }

        private static void OnError(Exception e)
        {
            Error?.Invoke(null, e);
        }

        #endregion
    }
}
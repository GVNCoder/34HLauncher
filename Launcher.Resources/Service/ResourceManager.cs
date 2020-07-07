using System.IO;
using System.Reflection;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Launcher.Resources.Service
{
    public static class ResourceManager
    {
        private static string _handledMapKey;
        private static Stream _handledMapStream;

        private static string _handledRankKey;
        private static Stream _handledRankStream;

        private static Stream _GetResourceStream(string path)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var result = assembly.GetManifestResourceStream($"Launcher.Resources.Images.{path}");

            return result;
        }

        private static ImageSource _FromStream(Stream stream)
        {
            var image = new BitmapImage();
            image.BeginInit();
            image.StreamSource = stream;
            image.EndInit();

            return image;
        }

        public static ImageSource GetMapImage(string key)
        {
            if (_handledMapKey == key) return _FromStream(_handledMapStream);

            _handledMapStream?.Dispose();
            _handledMapKey = key;
            _handledMapStream = _GetResourceStream($"Maps.{key}");
            return _FromStream(_handledMapStream);
        }

        public static ImageSource GetRankImage(string key)
        {
            if (_handledRankKey == key) return _FromStream(_handledRankStream);

            _handledRankStream?.Dispose();
            _handledRankKey = key;
            _handledRankStream = _GetResourceStream($"Ranks.{key}");
            return _FromStream(_handledRankStream);
        }
    }
}
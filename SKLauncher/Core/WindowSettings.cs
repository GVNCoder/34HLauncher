using System.Windows;

namespace Launcher.Core
{
    public static class WindowSettings
    {
        public const double Height = 600d;
        public const double Width = 1100d;
        public const double ChromeCaptionHeight = 30;

        public static readonly Thickness ChromeGlassFrameThickness = new Thickness(-1);
        public static readonly Thickness ChromeResizeBorderThickness = new Thickness(5);
    }
}
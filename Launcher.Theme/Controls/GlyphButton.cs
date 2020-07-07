using System.Windows;
using System.Windows.Controls.Primitives;

namespace Launcher.XamlThemes.Controls
{
    public class GlyphButton : ButtonBase
    {
        static GlyphButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GlyphButton), new FrameworkPropertyMetadata(typeof(GlyphButton)));
        }

        public string Icon
        {
            get => (string)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(string), typeof(GlyphButton), new PropertyMetadata(string.Empty));

        public double IconSize
        {
            get => (double)GetValue(IconSizeProperty);
            set => SetValue(IconSizeProperty, value);
        }
        public static readonly DependencyProperty IconSizeProperty =
            DependencyProperty.Register("IconSize", typeof(double), typeof(GlyphButton), new PropertyMetadata(14d));
    }
}
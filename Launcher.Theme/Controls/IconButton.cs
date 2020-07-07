using System.Windows;

namespace Launcher.XamlThemes.Controls
{
    public class IconButton : GlyphButton
    {
        static IconButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(IconButton), new FrameworkPropertyMetadata(typeof(IconButton)));
        }

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(IconButton), new PropertyMetadata(string.Empty));
    }
}
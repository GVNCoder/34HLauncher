using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Launcher.XamlThemes.Controls
{
    public class PlaceholderTextBox : TextBox
    {
        static PlaceholderTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PlaceholderTextBox), new FrameworkPropertyMetadata(typeof(PlaceholderTextBox)));
        }

        public string PlaceHolder
        {
            get => (string)GetValue(PlaceHolderProperty);
            set => SetValue(PlaceHolderProperty, value);
        }
        public static readonly DependencyProperty PlaceHolderProperty =
            DependencyProperty.Register("PlaceHolder", typeof(string), typeof(PlaceholderTextBox), new PropertyMetadata(string.Empty));

        public Brush PlaceHolderBrush
        {
            get => (Brush)GetValue(PlaceHolderBrushProperty);
            set => SetValue(PlaceHolderBrushProperty, value);
        }
        public static readonly DependencyProperty PlaceHolderBrushProperty =
            DependencyProperty.Register("PlaceHolderBrush", typeof(Brush), typeof(PlaceholderTextBox), new PropertyMetadata(null));
    }
}
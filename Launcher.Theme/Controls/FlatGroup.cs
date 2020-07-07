using System.Windows.Controls;
using System.Windows;

namespace Launcher.XamlThemes.Controls
{
    public class FlatGroup : ContentControl
    {
        #region Static constructor

        static FlatGroup()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FlatGroup), new FrameworkPropertyMetadata(typeof(FlatGroup)));
        }

        #endregion

        #region Dependency properties

        public string HeaderIcon
        {
            get => (string)GetValue(HeaderIconProperty);
            set => SetValue(HeaderIconProperty, value);
        }
        public static readonly DependencyProperty HeaderIconProperty =
            DependencyProperty.Register("HeaderIcon", typeof(string), typeof(FlatGroup), new PropertyMetadata(string.Empty));

        public string Header
        {
            get => (string)GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(string), typeof(FlatGroup), new PropertyMetadata(string.Empty));

        #endregion
    }
}

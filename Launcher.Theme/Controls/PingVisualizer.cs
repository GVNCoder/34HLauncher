using System.Windows;
using System.Windows.Controls;
using Launcher.XamlThemes.Controls.Shared;

namespace Launcher.XamlThemes.Controls
{
    public class PingVisualizer : Control
    {
        static PingVisualizer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PingVisualizer), new FrameworkPropertyMetadata(typeof(PingVisualizer)));
        }

        public PingVisualizerEnum Value
        {
            get => (PingVisualizerEnum)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(PingVisualizerEnum), typeof(PingVisualizer), new PropertyMetadata(PingVisualizerEnum.Good));
    }
}
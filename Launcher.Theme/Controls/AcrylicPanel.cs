using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Launcher.XamlThemes.Controls
{
    public class AcrylicPanel : Control
    {
        public Visual Target
        {
            get => (Visual)GetValue(TargetProperty);
            set => SetValue(TargetProperty, value);
        }
        public static readonly DependencyProperty TargetProperty =
            DependencyProperty.Register("Target", typeof(Visual), typeof(AcrylicPanel), new PropertyMetadata(null));

        public FrameworkElement Source
        {
            get => (FrameworkElement)GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(FrameworkElement), typeof(AcrylicPanel), new PropertyMetadata(null));

        public Color TintColor
        {
            get => (Color)GetValue(TintColorProperty);
            set => SetValue(TintColorProperty, value);
        }
        public static readonly DependencyProperty TintColorProperty =
            DependencyProperty.Register("TintColor", typeof(Color), typeof(AcrylicPanel), new PropertyMetadata(Colors.White));

        public double TintOpacity
        {
            get => (double)GetValue(TintOpacityProperty);
            set => SetValue(TintOpacityProperty, value);
        }
        public static readonly DependencyProperty TintOpacityProperty =
            DependencyProperty.Register("TintOpacity", typeof(double), typeof(AcrylicPanel), new PropertyMetadata(.0d));

        public double NoiseOpacity
        {
            get => (double)GetValue(NoiseOpacityProperty);
            set => SetValue(NoiseOpacityProperty, value);
        }
        public static readonly DependencyProperty NoiseOpacityProperty =
            DependencyProperty.Register("NoiseOpacity", typeof(double), typeof(AcrylicPanel), new PropertyMetadata(.03d));

        static AcrylicPanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AcrylicPanel), new FrameworkPropertyMetadata(typeof(AcrylicPanel)));
        }

        public AcrylicPanel()
        {
            this.Source = this;
        }

        bool _isChanged = false;
        private bool _actionCalled = false;
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var rect = this.GetTemplateChild("rect") as Rectangle;
            if (rect != null)
            {
                rect.LayoutUpdated += (_, __) =>
                {
                    if (!this._isChanged)
                    {
                        this._isChanged = true;
                        var bindingExp = BindingOperations.GetBindingExpressionBase(rect, Rectangle.RenderTransformProperty);
                        if (bindingExp != null)
                        {
                            bindingExp.UpdateTarget();
                        }

                        if (!_actionCalled)
                        {
                            Dispatcher.BeginInvoke(new Action(() =>
                            {
                                _actionCalled = true;
                                this._isChanged = false;
                            }), System.Windows.Threading.DispatcherPriority.DataBind);
                        }
                    }
                };
            }
        }
    }
}
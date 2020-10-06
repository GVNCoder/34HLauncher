using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
using Launcher.Core.Data.Model;

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

        public AcrylicAdjustmentLevel AdjustmentLevel { get; set; }

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
        private UIElement rect;

        #region Private helpers

        private void _AdjustHandlerOnSetup(object sender, EventArgs e)
        {
            if (_isChanged) return;

            this._isChanged = true;
            var bindingExp = BindingOperations.GetBindingExpressionBase(rect, Rectangle.RenderTransformProperty);
            bindingExp?.UpdateTarget();

            if (!_actionCalled)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    _actionCalled = true;
                    this._isChanged = false;
                }), System.Windows.Threading.DispatcherPriority.DataBind);
            }
        }

        #endregion

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            rect = (UIElement) GetTemplateChild("rect");
            if (rect != null)
            {
                rect.LayoutUpdated += _AdjustHandlerOnSetup;
            }
        }
    }
}
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

using Launcher.Core.Data.Model;

namespace Launcher.XamlThemes.Controls
{
    public class AcrylicPanel : Control
    {
        private bool _isChanged;
        private bool _actionCalled;
        private UIElement _acrylicRect;
        private BindingExpressionBase _acrylicRectBindingExpression;

        #region Bindable properties

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

        public Brush TintBrush
        {
            get => (Brush)GetValue(TintBrushProperty);
            set => SetValue(TintBrushProperty, value);
        }
        public static readonly DependencyProperty TintBrushProperty =
            DependencyProperty.Register("TintBrush", typeof(Brush), typeof(AcrylicPanel), new PropertyMetadata(Brushes.White));

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

        #endregion

        #region Properties

        public AcrylicAdjustmentLevel AdjustmentLevel { get; set; }

        #endregion

        #region Private helpers

        // ReSharper disable once MemberCanBeMadeStatic.Local
        private void _AdjustHandlerNoAdjust(object sender, EventArgs e)
        {
            // this method should be empty
        }

        private void _AdjustHandlerOnSetup(object sender, EventArgs e)
        {
            if (_isChanged) return;

            // try to update binding for updating layout size and positioning
            _acrylicRectBindingExpression?.UpdateTarget();
            _isChanged = true;

            // reset flags
            if (! _actionCalled)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    _actionCalled = true;
                    _isChanged = false;
                }), DispatcherPriority.DataBind);
            }
        }

        private void _AdjustHandlerAlways(object sender, EventArgs e)
        {
            if (_isChanged) return;

            // try to update binding for updating layout size and positioning
            _acrylicRectBindingExpression?.UpdateTarget();
            _isChanged = true;

            // reset flag
            Dispatcher.BeginInvoke(new Action(() => _isChanged = false), DispatcherPriority.DataBind);
        }

        #endregion

        static AcrylicPanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AcrylicPanel), new FrameworkPropertyMetadata(typeof(AcrylicPanel)));
        }

        public AcrylicPanel()
        {
            Source = this;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            // assign some fields
            _acrylicRect = (UIElement) GetTemplateChild("rect");
            // ReSharper disable once AssignNullToNotNullAttribute
            _acrylicRectBindingExpression = BindingOperations.GetBindingExpressionBase(_acrylicRect, Rectangle.RenderTransformProperty);

            // apply adjustment callback
            switch (AdjustmentLevel)
            {
                case AcrylicAdjustmentLevel.NoAdjust:
                    _acrylicRect.LayoutUpdated += _AdjustHandlerNoAdjust;
                    break;
                case AcrylicAdjustmentLevel.OnSetup:
                    _acrylicRect.LayoutUpdated += _AdjustHandlerOnSetup;
                    break;
                case AcrylicAdjustmentLevel.Always:
                    _acrylicRect.LayoutUpdated += _AdjustHandlerAlways;
                    break;
                default: break;
            }
        }
    }
}
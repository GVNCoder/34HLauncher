using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

using Launcher.Core.Data;
using Launcher.Core.Data.Model;
using Launcher.Core.Data.Model.Event;
using Launcher.Core.Service.Base;
using Launcher.XamlThemes.Controls;

using Ninject;

namespace Launcher.Core.Behaviors
{
    public class AcrylicControlBehavior : Behavior<Grid>
    {
        private const string TINT_COLOR_KEY = "Theme850Color";

        private IVisualProvider _visualProvider;
        private AcrylicPanel _panel;

        #region Bindable properties

        public AcrylicAdjustmentLevel AdjustmentLevel
        {
            get => (AcrylicAdjustmentLevel)GetValue(AdjustmentLevelProperty);
            set => SetValue(AdjustmentLevelProperty, value);
        }
        public static readonly DependencyProperty AdjustmentLevelProperty =
            DependencyProperty.Register("AdjustmentLevel", typeof(AcrylicAdjustmentLevel), typeof(AcrylicControlBehavior), new PropertyMetadata(AcrylicAdjustmentLevel.NoAdjust));

        public VisualContext VisualContext
        {
            get => (VisualContext)GetValue(VisualContextProperty);
            set => SetValue(VisualContextProperty, value);
        }
        public static readonly DependencyProperty VisualContextProperty =
            DependencyProperty.Register("VisualContext", typeof(VisualContext), typeof(AcrylicControlBehavior), new PropertyMetadata(VisualContext.Page));

        #endregion

        #region Ctor/Dector

        protected override void OnAttached()
        {
            base.OnAttached();

            var kernel = Resolver.Kernel;

            // resolve svc
            _visualProvider = kernel.Get<IVisualProvider>();

            // get visual content by context
            var visualContent = _visualProvider.GetVisualContent(VisualContext);

            // setup acrylic effect
            _panel = new AcrylicPanel
            {
                Target = visualContent,
                NoiseOpacity = .0075,
                TintOpacity = .0,
                AdjustmentLevel = AdjustmentLevel
            };

            // setup dynamic resource ref
            _panel.SetResourceReference(AcrylicPanel.TintColorProperty, TINT_COLOR_KEY);

            // fill all free space
            var rowCount = AssociatedObject.RowDefinitions.Count;
            var columnCount = AssociatedObject.ColumnDefinitions.Count;

            if (rowCount != 0) Grid.SetRowSpan(_panel, rowCount);
            if (columnCount != 0) Grid.SetColumnSpan(_panel, columnCount);

            // inject to UI at first element
            AssociatedObject.Children.Insert(0, _panel);

            // track content change
            _visualProvider.VisualContentChanged += _ControlVisualContentChangedHandler;
            
            // onDetach trigger
            AssociatedObject.Unloaded += _DetachingTrigger;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            // dispose all resources
            _visualProvider.VisualContentChanged -= _ControlVisualContentChangedHandler;

            AssociatedObject.Unloaded -= _DetachingTrigger;
            AssociatedObject.Children.RemoveAt(0);

            _panel = null;
        }

        #endregion

        #region Private helpers

        private void _ControlVisualContentChangedHandler(VisualContentChangedEventArgs e)
        {
            // switch content by context
            switch (VisualContext)
            {
                case VisualContext.Control:
                    
                    // update visual content
                    _panel.Target = e.Content;
                    break;
                case VisualContext.Page:
                default: break;
            }
        }

        private void _DetachingTrigger(object sender, EventArgs e)
            => Detach();

        #endregion
    }
}
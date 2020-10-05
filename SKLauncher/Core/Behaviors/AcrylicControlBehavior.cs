using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using System.Windows.Media;
using Launcher.Core.Data;
using Launcher.Core.Data.Model;
using Launcher.Core.Service;
using Launcher.Core.Service.Base;
using Launcher.XamlThemes.Controls;
using Ninject;

namespace Launcher.Core.Behaviors
{
    public class AcrylicControlBehavior : Behavior<Grid>
    {
        private const string _TINT_COLOR_KEY = "Theme850Color";
        private IVisualProvider _visualProvider;
        private ResourceDictionary _resources;

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
            var kernel = Resolver.Kernel;

            // resolve svc
            var appState = kernel.Get<IApplicationState>();

            _visualProvider = kernel.Get<IVisualProvider>();
            _resources = appState.Application.Resources;

            // get visual content by context
            var vContent = _visualProvider.GetVisualContent(VisualContext);

            // setup acrylic effect
            var acrylicEffect = new AcrylicPanel
            {
                Target = vContent,
                NoiseOpacity = .0075,
                TintColor = (Color) _resources[_TINT_COLOR_KEY],
                TintOpacity = .25
            };

            var rowCount = AssociatedObject.RowDefinitions.Count;
            var columnCount = AssociatedObject.ColumnDefinitions.Count;

            if (rowCount != 0) Grid.SetRowSpan(acrylicEffect, rowCount);
            if (columnCount != 0) Grid.SetColumnSpan(acrylicEffect, columnCount);

            AssociatedObject.Children.Insert(0, acrylicEffect);
        }

        protected override void OnDetaching()
        {
            
        }

        #endregion
    }
}
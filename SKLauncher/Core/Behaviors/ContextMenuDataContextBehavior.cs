using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using Zlo4NET.Api.Models.Shared;

namespace Launcher.Core.Behaviors
{
    public class ContextMenuDataContextBehavior : Behavior<ContextMenu>
    {
        protected override void OnAttached()
        {
            var app = (App) Application.Current;
            var viewModelLocator = app
                .DependencyResolver
                .Locators
                .ViewModelLocator;
            switch (TargetGameContext)
            {
                case ZGame.BF3:
                    AssociatedObject.DataContext = viewModelLocator.BF3ServerBrowserViewModel; break;
                case ZGame.BF4:
                    AssociatedObject.DataContext = viewModelLocator.BF4ServerBrowserViewModel; break;
                case ZGame.BFH:
                    AssociatedObject.DataContext = viewModelLocator.BFHServerBrowserViewModel; break;
                case ZGame.None:
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected override void OnDetaching() { }

        public ZGame TargetGameContext
        {
            get => (ZGame)GetValue(TargetGameContextProperty);
            set => SetValue(TargetGameContextProperty, value);
        }
        public static readonly DependencyProperty TargetGameContextProperty =
            DependencyProperty.Register("TargetGameContext", typeof(ZGame), typeof(ContextMenuDataContextBehavior), new PropertyMetadata(ZGame.None));
    }
}
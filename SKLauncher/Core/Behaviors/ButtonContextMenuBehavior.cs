using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Interactivity;

namespace Launcher.Core.Behaviors
{
    public class ButtonContextMenuBehavior : Behavior<Button>
    {
        private Delegate _handler;

        protected override void OnAttached()
        {
            _handler = new RoutedEventHandler(_ButtonClickHandler);
            AssociatedObject.AddHandler(ButtonBase.ClickEvent, _handler, true);
        }

        private void _ButtonClickHandler(object sender, RoutedEventArgs e)
        {
            var contextMenu = AssociatedObject.ContextMenu;
            contextMenu.IsOpen = !contextMenu.IsOpen;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.RemoveHandler(ButtonBase.ClickEvent, _handler);
        }
    }
}
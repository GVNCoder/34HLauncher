using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace Launcher.Core.Behaviors
{
    public class DetailsRowCopyBehavior : Behavior<TextBox>
    {
        private MenuItem _copyItem;

        protected override void OnAttached()
        {
            var contextMenu = AssociatedObject.ContextMenu;

            _copyItem = (MenuItem) contextMenu.Items[0];
            _copyItem.Click += _CopyClickHandler;
        }

        protected override void OnDetaching()
        {
            _copyItem.Click -= _CopyClickHandler;
        }

        private void _CopyClickHandler(object sender, RoutedEventArgs e) => _copyItem.CommandParameter = AssociatedObject.SelectedText;
    }
}
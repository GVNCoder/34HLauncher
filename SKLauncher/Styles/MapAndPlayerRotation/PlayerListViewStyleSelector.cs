using System.Collections;
using System.Windows;
using System.Windows.Controls;

using Zlo4NET.Api.Models.Server;
using Zlo4NET.Api.Models.Shared;

namespace Launcher.Styles.MapAndPlayerRotation
{
    public class PlayerListViewStyleSelector : StyleSelector
    {
        public PlayerListViewStyleSelector(IDictionary resources)
        {
            DefaultClassStyle = (Style) resources["DisableSelectionContainerStyle"];
            SelectedClassStyle = (Style) resources["PlayerSelectionContainerStyle"];
        }

        public Style DefaultClassStyle { get; }
        public Style SelectedClassStyle { get; }

        public override Style SelectStyle(object item, DependencyObject container)
        {
            var player = (ZPlayer) item;
            return player.Role == ZPlayerRole.IAm ? SelectedClassStyle : DefaultClassStyle;
        }
    }
}
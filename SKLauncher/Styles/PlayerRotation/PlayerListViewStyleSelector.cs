using System.Collections;
using System.Windows;
using System.Windows.Controls;
using Zlo4NET.Api.Models.Server;

namespace Launcher.Styles.PlayerRotation
{
    public class PlayerListViewStyleSelector : StyleSelector
    {
        private readonly uint _myId;

        public PlayerListViewStyleSelector(uint myId, IDictionary resources)
        {
            _myId = myId;

            DefaultClassStyle = (Style) resources["DisableSelectionContainerStyle"];
            SelectedClassStyle = (Style) resources["PlayerSelectionContainerStyle"];
        }

        public Style DefaultClassStyle { get; }
        public Style SelectedClassStyle { get; }

        public override Style SelectStyle(object item, DependencyObject container)
        {
            var player = (ZPlayer) item;
            return player.Id == _myId ? SelectedClassStyle : DefaultClassStyle;
        }
    }
}
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Zlo4NET.Api.Models.Server;

namespace Launcher.Styles.MapAndPlayerRotation
{
    public class RotationsViewModel
    {
        public ObservableCollection<ZPlayer> Players { get; set; }
        public ObservableCollection<ZMap> Maps { get; set; }

        public StyleSelector PlayerStyleSelector { get; set; }
        public StyleSelector MapStyleSelector { get; set; }

        public Visibility PlayersVisibility { get; set; }
    }
}
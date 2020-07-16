using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Zlo4NET.Api.Models.Server;

namespace Launcher.Styles.PlayerRotation
{
    public class RotationsViewModel
    {
        public ObservableCollection<ZPlayer> Players { get; set; }
        public ObservableCollection<ZMap> Maps { get; set; }

        public Visibility PlayersVisibility { get; set; }
        public StyleSelector StyleSelector { get; set; }
    }
}
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Launcher.Styles.PlayerRotation;
using Zlo4NET.Api.Models.Server;

namespace Launcher.Styles
{
    public class RotationsViewModel : DependencyObject
    {
        public IList<PlayerViewModel> Players
        {
            get => (IList<PlayerViewModel>)GetValue(PlayersProperty);
            set => SetValue(PlayersProperty, value);
        }
        public static readonly DependencyProperty PlayersProperty =
            DependencyProperty.Register("Players", typeof(IList<PlayerViewModel>), typeof(RotationsViewModel), new PropertyMetadata(null));

        public IList<ZMap> Maps { get; set; }
        public Visibility PlayersVisibility { get; set; }
        public StyleSelector StyleSelector { get; set; }
    }
}
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Launcher.Core.Interaction;
using Zlo4NET.Api.Models.Server;

namespace Launcher.Styles.MapAndPlayerRotation
{
    public class RotationsViewModel : DependencyObject
    {
        public RotationsViewModel(
            ObservableCollection<ZPlayer> players,
            ObservableCollection<ZMap> maps,
            StyleSelector playerStyleSelector)
        {
            Players = players;
            Maps = maps;
            PlayerStyleSelector = playerStyleSelector;
        }

        public ICommand LoadedCommand => new DelegateCommand(obj =>
        {
            _SetupPlayersVisibility(Players);
            Players.CollectionChanged += _playersCollectionChangedHandler;
        });

        public ICommand UnloadedCommand => new DelegateCommand(obj =>
        {
            Players.CollectionChanged -= _playersCollectionChangedHandler;
        });

        private void _playersCollectionChangedHandler(object sender, NotifyCollectionChangedEventArgs e)
        {
            _SetupPlayersVisibility((IList<ZPlayer>) sender);
        }

        private void _SetupPlayersVisibility(ICollection<ZPlayer> list)
        {
            PlayersVisibility = list.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        public ObservableCollection<ZPlayer> Players { get; }
        public ObservableCollection<ZMap> Maps { get; }

        public StyleSelector PlayerStyleSelector { get; }

        public Visibility PlayersVisibility
        {
            get => (Visibility)GetValue(PlayersVisibilityProperty);
            set => SetValue(PlayersVisibilityProperty, value);
        }
        public static readonly DependencyProperty PlayersVisibilityProperty =
            DependencyProperty.Register("PlayersVisibility", typeof(Visibility), typeof(RotationsViewModel), new PropertyMetadata(Visibility.Collapsed));
    }
}
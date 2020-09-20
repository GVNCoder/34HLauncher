using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

using Launcher.Core.Data;
using Launcher.Core.Interaction;
using Launcher.Core.RPC;
using Launcher.Core.Service.Base;
using Launcher.Core.Services;
using Launcher.Core.Shared;
using Launcher.Helpers;
using Launcher.Services;

using Zlo4NET.Api.Models.Shared;
using IBlurredPage = Launcher.Core.Bases.IBlurredPage;

namespace Launcher.ViewModel
{
    public class BF3CoopViewModel : BasePageViewModel, IBlurredPage
    {
        private CollectionViewSource _collectionViewSource;
        private readonly IEnumerable<CoopMissionModel> _missions;
        private readonly IGameService _gameService;
        private readonly IDiscord _discord;

        public BF3CoopViewModel(
            IUIHostService hostService,
            IGameService gameService,
            IDiscord discord)
        {
            BackgroundContent = hostService.GetHostContainer(UIElementConstants.HostWindowBackground) as Grid;
            DifficultyEnumerable = Enum.GetNames(typeof(ZCoopDifficulty));

            _gameService = gameService;
            _discord = discord;
            _missions = new[]
            {
                new CoopMissionModel
                {
                    Name = "Operation Exodus",
                    Level = ZCoopLevels.COOP_007,
                    Description =
                        "This is a horde mode limited to three massive waves of enemies attacking your defence targets (the High Mobility Vehicles marked by a blue dot)."
                },
                new CoopMissionModel
                {
                    Name = "Fire From The Sky",
                    Level = ZCoopLevels.COOP_006,
                    Description =
                        "Chopper mission. Support allied fireteams and destroy enemies. BMP, Mobile AA, and HMVs. Once all enemies are destroyed, and your friendly fireteams extract your High Value Target, you will complete this mission."
                },
                new CoopMissionModel
                {
                    Name = "Exfiltration",
                    Level = ZCoopLevels.COOP_009,
                    Description =
                        "Infiltrate a building stealthily and extract the HVT under heavy fire. Thermal scopes are available on the default weapons. Shooting the security cameras and making stealth kills (or simultaneous firearms takedowns) will allow you to enter without alarms."
                }, 
                new CoopMissionModel
                {
                    Name = "Hit And Run",
                    Level = ZCoopLevels.COOP_002,
                    Description =
                        "This is a straight-forward mission with a straight-forward goal - get to the garage and escape the terr'ists using a French-built, petrol-burning motor vehicle. The related extra condition for this mission is Push On."
                },
                new CoopMissionModel
                {
                    Name = "Drop Em Like Liquid",
                    Level = ZCoopLevels.COOP_003,
                    Description =
                        "This is a sniper team mission. The condition Bullseye is related to this mission."
                }, 
                new CoopMissionModel
                {
                    Name = "The Eleventh Hour",
                    Level = ZCoopLevels.COOP_010,
                    Description =
                        "Basically, Operation Metro with a lot of noxious foul-smelling gas on the train platform. You venture down into the metro station and takedown enemies as they come."
                }
            };
        }

        #region Public properties

        public Grid BackgroundContent { get; }
        public IEnumerable<string> DifficultyEnumerable { get; }

        public string DifficultyName
        {
            get => (string)GetValue(DifficultyNameProperty);
            set => SetValue(DifficultyNameProperty, value);
        }
        public static readonly DependencyProperty DifficultyNameProperty =
            DependencyProperty.Register("DifficultyName", typeof(string), typeof(BF3CoopViewModel), new PropertyMetadata(string.Empty));

        public string FriendId
        {
            get => (string)GetValue(FriendIdProperty);
            set => SetValue(FriendIdProperty, value);
        }
        public static readonly DependencyProperty FriendIdProperty =
            DependencyProperty.Register("FriendId", typeof(string), typeof(BF3CoopViewModel), new PropertyMetadata(string.Empty, _FriendIdChangedHandler));

        private static void _FriendIdChangedHandler(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var vm = (BF3CoopViewModel) d;
            var value = (string) e.NewValue;

            vm.CanJoin = !string.IsNullOrWhiteSpace(value);
            vm.CanJoin = value.IsNumber();
        }

        public CoopMissionModel SelectedMission
        {
            get => (CoopMissionModel)GetValue(SelectedMissionProperty);
            set => SetValue(SelectedMissionProperty, value);
        }
        public static readonly DependencyProperty SelectedMissionProperty =
            DependencyProperty.Register("SelectedMission", typeof(CoopMissionModel), typeof(BF3CoopViewModel), new PropertyMetadata(null, _SelectedMissionChangedHandler));

        private static void _SelectedMissionChangedHandler(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var vm = (BF3CoopViewModel) d;
            vm.CanHost = e.NewValue != null;
        }

        public bool CanHost
        {
            get => (bool)GetValue(CanHostProperty);
            set => SetValue(CanHostProperty, value);
        }
        public static readonly DependencyProperty CanHostProperty =
            DependencyProperty.Register("CanHost", typeof(bool), typeof(BF3CoopViewModel), new PropertyMetadata(false));

        public bool CanJoin
        {
            get => (bool)GetValue(CanJoinProperty);
            set => SetValue(CanJoinProperty, value);
        }
        public static readonly DependencyProperty CanJoinProperty =
            DependencyProperty.Register("CanJoin", typeof(bool), typeof(BF3CoopViewModel), new PropertyMetadata(false));

        #endregion

        #region Commands

        public override ICommand LoadedCommand => new DelegateCommand(obj =>
        {
            var ui = (Page) obj;
            _collectionViewSource = (CollectionViewSource) ui.Resources["CollectionViewSource"];
            _collectionViewSource.Source = _missions;

            CanHost = SelectedMission != null;
            CanJoin = false;
            
            _discord.UpdateCoopBrowser();
        });

        public override ICommand UnloadedCommand => new DelegateCommand(obj =>
        {
            _collectionViewSource.Source = null;
            _collectionViewSource = null;
        });

        public ICommand HostCommand => new DelegateCommand(obj =>
        {
            // check can run game
            if (!_gameService.CanRun) return;

            // parse difficulty enum and assign for selected mission
            var difficulty = EnumUtil.Parse<ZCoopDifficulty>(DifficultyName);
            SelectedMission.Difficulty = difficulty;

            // create run params
            var param = new CoopJoinParams
            {
                CoopMission = SelectedMission,
                Game = ZGame.BF3,
                Mode = ZPlayMode.CooperativeHost
            };
            // run game
            _gameService.RunCoop(param).Forget();
        });

        public ICommand JoinCommand => new DelegateCommand(obj =>
        {
            // check can run game
            if (!_gameService.CanRun) return;

            // create run params
            var friendId = uint.Parse(FriendId);
            var param = new CoopJoinParams
            {
                Game = ZGame.BF3,
                Mode = ZPlayMode.CooperativeClient,
                FriendId = friendId
            };
            // run game
            _gameService.RunCoop(param).Forget();
        });

        #endregion
    }
}
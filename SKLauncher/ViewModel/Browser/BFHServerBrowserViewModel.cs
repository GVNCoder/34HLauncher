﻿using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using Launcher.Core.Bases;
using Launcher.Core.Interaction;
using Launcher.Core.Service;
using Launcher.Core.Services;
using Launcher.Core.Services.Dialog;
using Launcher.Core.Services.EventLog;
using Zlo4NET.Api;
using Zlo4NET.Api.Models.Shared;
using Zlo4NET.Core.Data;
using IDiscord = Launcher.Core.RPC.IDiscord;

namespace Launcher.ViewModel
{
    public class BFHServerBrowserViewModel : BaseServerBrowserViewModel
    {
        public BFHServerBrowserViewModel(
            IZApi api,
            IUIHostService hostService,
            IContentPresenterService presenterService,
            IEventLogService eventLogService,
            IGameService gameService,
            IDiscord discord,
            App application,
            IPageNavigator navigator,
            ISettingsService settingsService)
            : base(api, hostService, gameService, eventLogService, presenterService, discord, application, /*navigationService,*/ settingsService, navigator)
        {
            MapNames = new[] { "All" }
                .Concat(ZResource.GetBFHMapNames())
                .ToArray();
            GameModeNames = new[] { "All" }
                .Concat(ZResource.GetBFHGameModeNames())
                .ToArray();
        }

        #region Overrides

        public override ICommand LoadedCommand => new DelegateCommand(obj =>
        {
            base.OnLoadImpl(ZGame.BFH, obj as Page);
            _discord.UpdateServerBrowser(ZGame.BFH);
        });

        public override ICommand UnloadedCommand => new DelegateCommand(obj => OnUnloadedImpl());

        #endregion
    }
}
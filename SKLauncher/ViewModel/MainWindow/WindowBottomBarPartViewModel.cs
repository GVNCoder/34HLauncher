﻿using System.Windows;
using System.Windows.Input;

using Launcher.Core;
using Launcher.Core.Service;
using Launcher.Core.Service.Base;
using Launcher.Core.Services;
using Launcher.Core.Shared;
using Launcher.ViewModel.UserControl;

using Zlo4NET.Api;
using Zlo4NET.Api.Models.Shared;
//using Zlo4NET.Api.Service;

namespace Launcher.ViewModel
{
    public class WindowBottomBarPartViewModel : BaseControlViewModel
    {
        private readonly IEventService _eventService;

        private bool _isDisconnectedEventShowed;

        public WindowBottomBarPartViewModel(
            IVersionService versionService,
            IEventService eventService,
            IViewModelSource viewModelLocator,
            IZApi api)
        {
            _eventService = eventService;

            // build view models for child user controls
            UpdateControlViewModel = viewModelLocator.GetExisting<UpdateControlViewModel>();
            GameControlViewModel = viewModelLocator.GetExisting<GameControlViewModel>();
            EventsButtonControlViewModel = viewModelLocator.GetExisting<EventsButtonViewModel>();

            // setup vars
            VersionString = versionService.GetLauncherVersion().ToString();

            // track some events
            var connection = api.Connection;

            connection.ConnectionChanged += _OnConnectionChanged;
        }

        #region Public members

        public UpdateControlViewModel UpdateControlViewModel { get; }
        public GameControlViewModel GameControlViewModel { get; }
        public EventsButtonViewModel  EventsButtonControlViewModel { get; }

        #endregion

        #region Bindable properties

        public string  VersionString
        {
            get => (string )GetValue(VersionStringProperty);
            set => SetValue(VersionStringProperty, value);
        }
        public static readonly DependencyProperty VersionStringProperty =
            DependencyProperty.Register("VersionString", typeof(string ), typeof(WindowBottomBarPartViewModel), new PropertyMetadata("test version"));

        #endregion

        #region Commands

        public override ICommand LoadedCommand => throw new System.NotImplementedException();
        public override ICommand UnloadedCommand => throw new System.NotImplementedException();

        #endregion

        #region Public methods

        //public void UpdateConnected()
        //{
        //    _isDisconnectedEventShowed = false;
        //    _eventService.SuccessEvent("ZClient connection", "Successfully connected to ZClient");
        //}

        //public void UpdateDisconnected()
        //{
        //    // check already call this method
        //    if (_isDisconnectedEventShowed) return;

        //    // show event
        //    _eventService.ErrorEvent(
        //        "Unable to establish a connection to the ZClient for one of the following reasons:",
        //        "-ZClient not running\n-You did not click the Connect button\n-There is no internet connection\n" +
        //        "-You, for whatever reason, are not logged in to ZClient\n" +
        //        "-Launcher internal error (restart the launcher and contact the developer)");
        //    _isDisconnectedEventShowed = true;
        //}

        #endregion

        #region Private helpers

        private void _OnConnectionChanged(object sender, ZConnectionChangedEventArgs e)
        {
            if (e.IsConnected)
            {
                _isDisconnectedEventShowed = false;
                _eventService.SuccessEvent("ZClient connection", "Successfully connected to ZClient");
            }
            else if (_isDisconnectedEventShowed == false)
            {
                _eventService.ErrorEvent(
                    "Unable to establish a connection to the ZClient for one of the following reasons:",
                    "-ZClient not running\n-You did not click the Connect button\n-There is no internet connection\n" +
                    "-You, for whatever reason, are not logged in to ZClient\n" +
                    "-Launcher internal error (restart the launcher and contact the developer)");
                _isDisconnectedEventShowed = true;
            }
        }

        #endregion
    }
}
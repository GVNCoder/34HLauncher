using System;
using System.ComponentModel;
using Zlo4NET.Api.Models.Server;

namespace Launcher.Core.RPC
{
    public class ServerDiscordUnit
    {
        private const string __Name = "Name";
        private const string __CurrentPlayersNumber = "CurrentPlayersNumber";
        private const string __CurrentMap = "Current";

        private readonly ZServerBase _serverModel;

        public ServerDiscordUnit(ZServerBase serverModel)
        {
            _serverModel = serverModel;

            _serverModel.PropertyChanged += _ServerUpdatedHandler;
            _serverModel.MapRotation.PropertyChanged += _MapUpdatedHandler;
        }

        public void Destroy()
        {
            _serverModel.MapRotation.PropertyChanged -= _MapUpdatedHandler;
            _serverModel.PropertyChanged -= _ServerUpdatedHandler;
        }

        public event EventHandler ServerModelUpdated;

        private void _ServerUpdatedHandler(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == __Name || e.PropertyName == __CurrentPlayersNumber) ServerModelUpdated?.Invoke(_serverModel, EventArgs.Empty);
        }

        private void _MapUpdatedHandler(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == __CurrentMap) ServerModelUpdated?.Invoke(_serverModel, EventArgs.Empty);
        }
    }
}
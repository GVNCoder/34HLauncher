using System;
using System.ComponentModel;
using Zlo4NET.Api.Models.Server;

namespace Launcher.Core.RPC
{
    public class ServerModelUpdatesUnit
    {
        private const string __Name = "Name";
        private const string __CurrentMap = "Current";
        private const string __CurrentPlayersNumber = "CurrentPlayersNumber";

        private ZServerBase _serverModel;

        public ServerModelUpdatesUnit(ZServerBase serverModel)
        {
            _serverModel = serverModel;
            _EventLink(_serverModel, true);
        }

        public void Relink(ZServerBase serverModel)
        {
            _EventLink(_serverModel, false);
            _serverModel = serverModel;
            _EventLink(_serverModel, true);
        }

        public void Destroy()
        {
            _EventLink(_serverModel, false);
        }

        public event EventHandler ServerModelUpdated;

        private void _EventLink(ZServerBase model, bool subscribe)
        {
            if (subscribe)
            {
                model.PropertyChanged += _ServerUpdatedHandler;
                model.MapRotation.PropertyChanged += _MapUpdatedHandler;
            }
            else
            {
                model.MapRotation.PropertyChanged -= _MapUpdatedHandler;
                model.PropertyChanged -= _ServerUpdatedHandler;
            }
        }

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
using System;
using System.Diagnostics;
using System.Linq;
using Launcher.Core.RPC;
using Launcher.Core.Shared;
using Zlo4NET.Api.Service;
using Zlo4NET.Core.Data;

namespace Launcher.Core.Data
{
    public class DetectedGameImpl : IGameHelper
    {
        private readonly IGameControl _view;
        private readonly Process _process;
        private readonly IZProcessTracker _processTracker;
        private readonly IDiscord _discord;

        public DetectedGameImpl(
            IGameControl view,
            Process process,
            IDiscord discord)
        {
            _view = view;
            _process = process;
            _discord = discord;

            _view.CloseClick += _OnCloseRequestedHandler;

            _processTracker = new ZProcessTracker(process.ProcessName, TimeSpan.FromSeconds(3), false, ps => ps.FirstOrDefault());
            _processTracker.ProcessDetected += _ProcessDetected;
            _processTracker.ProcessLost += _ProcessLost;
        }

        public void BeginWork()
        {
            _SetUnknownVisual();
            _view.Show();

            _processTracker.StartTrack();
        }

        public event EventHandler<GameCloseEventArgs> Close;

        #region Private methods

        private void _OnClose(string pipeContent) => Close?.Invoke(this, new GameCloseEventArgs(pipeContent));

        private void _ProcessDetected(object sender, Process e)
        {
            _processTracker.ProcessDetected -= _ProcessDetected;

            _view.SetCanClose(true);
            _discord.UpdateUnknown();
        }

        private void _ProcessLost(object sender, EventArgs e)
        {
            _processTracker.ProcessDetected -= _ProcessDetected;
            _processTracker.ProcessLost -= _ProcessLost;

            _view.CloseClick -= _OnCloseRequestedHandler;
            _view.Hide();

            _discord.DisablePlay();
            _OnClose(null);
        }

        private void _OnCloseRequestedHandler(object sender, EventArgs e)
        {
            try
            {
                _process.Kill();
            }
            catch (Exception)
            {
                // ignore
            }
        }

        private void _SetUnknownVisual()
        {
            _view.SetCanClose(false);
            _view.SetState(false);
            _view.SetText("Playing");
            _view.SetToolTipText("Unknown mode");
        }

        #endregion
    }
}
using System;
using System.Diagnostics;
using System.Linq;

using Launcher.Core.RPC;
using Launcher.Core.Shared;

using Zlo4NET.Api.Service;
using Zlo4NET.Core.Data;

namespace Launcher.Core.Data
{
    public class DetectedGameWorker : BaseGameWorker
    {
        private readonly Process _process;
        private readonly IZProcessTracker _processTracker;

        public DetectedGameWorker(IDiscord discord, IGameControl view, Process process) : base(discord, view)
        {
            _process = process;

            _view.CloseClick += _OnCloseRequestedHandler;

            _processTracker = new ZProcessTracker(process.ProcessName, TimeSpan.FromSeconds(3), false, ps => ps.FirstOrDefault());
            _processTracker.ProcessDetected += _ProcessDetected;
            _processTracker.ProcessLost += _ProcessLost;
        }

        public override void BeginWork()
        {
            _SetUnknownVisual();
            _view.Show();

            _processTracker.StartTrack();
        }

        #region Private methods

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
            _OnWorkDone(null);
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
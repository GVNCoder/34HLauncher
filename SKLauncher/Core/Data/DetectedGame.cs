using System;
using System.Diagnostics;
using System.Linq;
using Zlo4NET.Api.Service;
using Zlo4NET.Core.Data;

namespace Launcher.Core.Data
{
    public class DetectedGame : IGame
    {
        private readonly IZProcessTracker _processTracker;
        private readonly Process _gameProcess;

        public DetectedGame(Process gameProcess)
        {
            _gameProcess = gameProcess;

            _processTracker = new ZProcessTracker(_gameProcess.ProcessName, TimeSpan.FromSeconds(3), false, ps => ps.FirstOrDefault());
            _processTracker.ProcessDetected += _ProcessDetected;
            _processTracker.ProcessLost += _ProcessLost;
            _processTracker.StartTrack();
        }

        public bool IsRun { get; private set; } = false;

        public bool TryClose()
        {
            try
            {
                _gameProcess.Kill();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public bool TryUnfoldWindow()
        {
            return false;
        }

        public event EventHandler<string> PipeLog;

        private void _OnLog(string content) => PipeLog?.Invoke(this, content);

        private void _ProcessDetected(object sender, Process e)
        {
            IsRun = true;

            _processTracker.ProcessDetected -= _ProcessDetected;
            _OnLog("StateChanged State_GameRunning");
        }

        private void _ProcessLost(object sender, EventArgs e)
        {
            IsRun = false;

            _processTracker.ProcessLost -= _ProcessLost;
            _OnLog("StateChanged State_GameClosing");
        }
    }
}
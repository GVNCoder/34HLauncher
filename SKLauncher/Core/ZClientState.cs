using System;

using Launcher.Core.Data.Model.Event;
using Launcher.Core.Service;
using Launcher.Helpers;

namespace Launcher.Core
{
    public class ZClientState
    {
        private readonly IApplicationState _state;

        public ZClientState(IApplicationState state)
        {
            _state = state;

            _state.StateRegistered += _newApplicationStateRegisteredHandler;
            _state.StateChanged += _applicationStateChangedHandler;
        }

        #region State changed handlers

        private void _applicationStateChangedHandler(object sender, ApplicationStateEventArgs e) =>
            _StateChangedHandler(e);

        private void _newApplicationStateRegisteredHandler(object sender, ApplicationStateEventArgs e) =>
            _StateChangedHandler(e);

        #endregion

        #region Private helpers

        private void _OnStateChanged(ApplicationStateEventArgs e) => StateChanged?.Invoke(this, e);

        private bool _CatchState(string key) => Constants.ZCLIENT_CONNECTION == key || Constants.ZCLIENT_IS_RUN == key;

        private void _StateChangedHandler(ApplicationStateEventArgs e)
        {
            // check input key to catch
            if (!_CatchState(e.Key)) return;

            // pass event
            _OnStateChanged(e);
        }

        #endregion

        #region Public members

        public void SetConnection(bool state) => _state.SetState(Constants.ZCLIENT_CONNECTION, state);
        public void SetIsRun(bool state) => _state.SetState(Constants.ZCLIENT_IS_RUN, state);

        public bool Connection => _state.GetState<bool>(Constants.ZCLIENT_CONNECTION);
        public bool IsRun => _state.GetState<bool>(Constants.ZCLIENT_IS_RUN);
        public bool AllGood => Connection && IsRun;

        public event EventHandler<ApplicationStateEventArgs> StateChanged;

        #endregion
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using Launcher.Core.Shared;

namespace Launcher.Core.Services
{
    public class ApplicationStateService : IApplicationStateService
    {
        private readonly IDictionary<string, bool> _state;

        private void _OnStateChanged(string key, bool state)
        {
            StateChanged?.Invoke(this, new ApplicationStateArgs(key, state));
        }

        public ApplicationStateService()
        {
            _state = new Dictionary<string, bool>();
        }

        public event EventHandler<ApplicationStateArgs> StateChanged;

        public void AddState(string key, bool initialState)
        {
            _state[key] = initialState;
        }

        public void ChangeState(string key, bool state)
        {
            var currentState = _state[key];
            if (currentState == state)
            {
                return;
            }

            _state[key] = state;
            _OnStateChanged(key, state);
        }

        public bool GetState(string key)
        {
            return _state[key];
        }

        public bool AllGood()
        {
            return _state.Values.All(item => item);
        }

        public bool AnyBad()
        {
            return _state.Values.Any(item => !item);
        }
    }
}
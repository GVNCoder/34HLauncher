using System;
using System.Collections;
using System.Windows;

using Launcher.Core.Data.Model.Event;
using Launcher.Core.Service;

namespace Launcher.Core.Data
{
    public class ApplicationState : IApplicationState
    {
        public ApplicationState()
        {
            Application = Application.Current;
            State = new Hashtable();
        }
        
        public Application Application { get; }
        public Hashtable State { get; }
        
        public event EventHandler<ApplicationStateEventArgs> StateChanged;
        public event EventHandler<ApplicationStateEventArgs> StateRegistered;
        
        public void SetState(string key, object state, bool raiseEvent = true)
        {
            _ThrowIfNotExists(key);
            
            // set new state
            State[key] = state;
            
            // raise event
            if (raiseEvent) _OnStateChanged(key, state);
        }

        public void RegisterState(string key, object initialValue, bool raiseEvent = false)
        {
            _ThrowIfNotExists(key);
            
            // register new state
            State.Add(key, initialValue);
            
            // raise event
            if (raiseEvent) _OnStateRegistered(key, state: initialValue);
        }

        public TCast GetState<TCast>(string key, TCast defaultValue = default(TCast))
            => (TCast) (State.ContainsKey(key) ? State[key] : defaultValue);

        #region Private helper

        private void _OnStateRegistered(string key, object state)
            => StateRegistered?.Invoke(this, new ApplicationStateEventArgs(key, state));
        
        private void _OnStateChanged(string key, object state)
            => StateChanged?.Invoke(this, new ApplicationStateEventArgs(key, state));

        private void _ThrowIfNotExists(string key)
        {
            if (! State.ContainsKey(key)) throw new InvalidOperationException($"The key({key}) already exists.");
        }

        #endregion
    }
}
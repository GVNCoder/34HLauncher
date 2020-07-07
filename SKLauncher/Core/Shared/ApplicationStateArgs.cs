using System;

namespace Launcher.Core.Shared
{
    public class ApplicationStateArgs : EventArgs
    {
        public string Key { get; }
        public bool State { get; }

        public ApplicationStateArgs(string key, bool state)
        {
            Key = key;
            State = state;
        }
    }
}
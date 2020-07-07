using System;
using Launcher.Core.Shared;

namespace Launcher.Core.Services
{
    public interface IApplicationStateService
    {
        void AddState(string key, bool initialState);
        void ChangeState(string key, bool state);
        bool GetState(string key);

        bool AllGood();
        bool AnyBad();

        event EventHandler<ApplicationStateArgs> StateChanged;
    }
}
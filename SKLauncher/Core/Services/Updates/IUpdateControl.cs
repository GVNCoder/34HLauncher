using System;

namespace Launcher.Core.Services.Updates
{
    public interface IUpdateControl
    {
        event EventHandler CancelRequested;
        void ReportProgress(int value);
        void Show();
        void Hide();
    }
}
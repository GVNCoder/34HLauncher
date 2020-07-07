using System;

namespace Launcher.Core.Shared
{
    public interface IGameControl
    {
        void Show();
        void SetText(string text);
        void SetToolTipText(string text);
        void SetState(bool isLoading);
        void SetCanClose(bool canClose);
        void Hide();

        event EventHandler CloseClick;
    }
}
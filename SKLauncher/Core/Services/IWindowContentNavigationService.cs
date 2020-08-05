using System;
using System.Windows.Controls;
using System.Windows.Input;

namespace Launcher.Core.Services
{
    public interface IWindowContentNavigationService
    {
        void NavigateTo(string target);
        void GoBack();
        void GoForward();

        void Initialize(Frame from);

        ICommand Navigate { get; }
        ICommand Back { get; }
        ICommand Forward { get; }

        event EventHandler Navigation;
    }
}
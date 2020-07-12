using System.Windows;
using System.Windows.Input;

namespace Launcher.Core.SettingsViewModelParts
{
    public abstract class UIBaseViewModel : DependencyObject
    {
        public abstract ICommand LoadedCommand { get; }
        public abstract ICommand UnloadedCommand { get; }
    }
}
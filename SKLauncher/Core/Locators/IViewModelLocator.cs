using Launcher.Core.Bases;
using Launcher.Core.Data.Updates;
using Launcher.Core.Shared;
using Launcher.ViewModel;
using Launcher.ViewModel.MainWindow;
using Launcher.ViewModel.Stats;

namespace Launcher.Core.Locators
{
    public interface IViewModelLocator
    {
        MainWindowViewModel MainWindowViewModel { get; }
        WindowNonClientPartViewModel WindowNonClientPartViewModel { get; }
        WindowBottomBarPartViewModel WindowBottomBarPartViewModel { get; }

        BaseServerBrowserViewModel BF3ServerBrowserViewModel { get; }
        BaseServerBrowserViewModel BF4ServerBrowserViewModel { get; }
        BaseServerBrowserViewModel BFHServerBrowserViewModel { get; }

        HomeViewModel HomeViewModel { get; }
        BF3CoopViewModel BF3CoopViewModel { get; }
        EventLogViewModel EventLogViewModel { get; }
        SettingsViewModel SettingsViewModel { get; }

        BF3StatsViewModel BF3StatsViewModel { get; }
        BF4StatsViewModel BF4StatsViewModel { get; }
    }
}

using System;
using Launcher.Core.Bases;
using Launcher.Core.Data.Updates;
using Launcher.Core.Shared;
using Launcher.ViewModel;
using Launcher.ViewModel.MainWindow;
using Launcher.ViewModel.Stats;
using Ninject;

namespace Launcher.Core.Locators
{
    public class ViewModelLocator : IViewModelLocator
    {
        private readonly Lazy<MainWindowViewModel> _mainWindowViewModel;
        private readonly Lazy<WindowBottomBarPartViewModel> _windowBottomBarPartViewModel;
        private readonly Lazy<WindowNonClientPartViewModel> _windowNonClientPartViewModel;

        private readonly Lazy<BF3ServerBrowserViewModel> _bf3ServerBrowserViewModel;
        private readonly Lazy<BF4ServerBrowserViewModel> _bf4ServerBrowserViewModel;
        private readonly Lazy<BFHServerBrowserViewModel> _bfhServerBrowserViewModel;

        private readonly Lazy<HomeViewModel> _homeViewModel;
        private readonly Lazy<BF3CoopViewModel> _bf3CoopViewModel;
        private readonly Lazy<EventLogViewModel> _eventLogViewModel;
        private readonly Lazy<SettingsViewModel> _settingsViewModel;
        private readonly Lazy<GameControlViewModel> _gameControlViewModel;
        private readonly Lazy<UpdateControlViewModel> _updateControlViewModel;

        private readonly Lazy<BF3StatsViewModel> _bf3StatsViewModel;
        private readonly Lazy<BF4StatsViewModel> _bf4StatsViewModel;

        public ViewModelLocator() // uses in xaml code
        {
        }

        public ViewModelLocator(IKernel kernel) // uses in code behind
        {
            _mainWindowViewModel = new Lazy<MainWindowViewModel>(() => kernel.Get<MainWindowViewModel>());
            _windowBottomBarPartViewModel = new Lazy<WindowBottomBarPartViewModel>(() => kernel.Get<WindowBottomBarPartViewModel>());
            _windowNonClientPartViewModel = new Lazy<WindowNonClientPartViewModel>(() => kernel.Get<WindowNonClientPartViewModel>());

            _bf3ServerBrowserViewModel = new Lazy<BF3ServerBrowserViewModel>(() => kernel.Get<BF3ServerBrowserViewModel>());
            _bf4ServerBrowserViewModel = new Lazy<BF4ServerBrowserViewModel>(() => kernel.Get<BF4ServerBrowserViewModel>());
            _bfhServerBrowserViewModel = new Lazy<BFHServerBrowserViewModel>(() => kernel.Get<BFHServerBrowserViewModel>());

            _bf3CoopViewModel = new Lazy<BF3CoopViewModel>(() => kernel.Get<BF3CoopViewModel>());
            _eventLogViewModel = new Lazy<EventLogViewModel>(() => kernel.Get<EventLogViewModel>());
            _settingsViewModel = new Lazy<SettingsViewModel>(() => kernel.Get<SettingsViewModel>());
            _gameControlViewModel = new Lazy<GameControlViewModel>(() => kernel.Get<GameControlViewModel>());
            _updateControlViewModel = new Lazy<UpdateControlViewModel>(() => kernel.Get<UpdateControlViewModel>());
            _homeViewModel = new Lazy<HomeViewModel>(() => kernel.Get<HomeViewModel>());

            _bf3StatsViewModel = new Lazy<BF3StatsViewModel>(() => kernel.Get<BF3StatsViewModel>());
            _bf4StatsViewModel = new Lazy<BF4StatsViewModel>(() => kernel.Get<BF4StatsViewModel>());
        }

        public MainWindowViewModel MainWindowViewModel => _mainWindowViewModel.Value;
        public WindowNonClientPartViewModel WindowNonClientPartViewModel => _windowNonClientPartViewModel.Value;
        public WindowBottomBarPartViewModel WindowBottomBarPartViewModel => _windowBottomBarPartViewModel.Value;

        public BaseServerBrowserViewModel BF3ServerBrowserViewModel => _bf3ServerBrowserViewModel.Value;
        public BaseServerBrowserViewModel BF4ServerBrowserViewModel => _bf4ServerBrowserViewModel.Value;
        public BaseServerBrowserViewModel BFHServerBrowserViewModel => _bfhServerBrowserViewModel.Value;

        public HomeViewModel HomeViewModel => _homeViewModel.Value;
        public BF3CoopViewModel BF3CoopViewModel => _bf3CoopViewModel.Value;
        public EventLogViewModel EventLogViewModel => _eventLogViewModel.Value;
        public SettingsViewModel SettingsViewModel => _settingsViewModel.Value;
        public GameControlViewModel GameControlViewModel => _gameControlViewModel.Value;
        public UpdateControlViewModel UpdateControlViewModel => _updateControlViewModel.Value;

        public BF3StatsViewModel BF3StatsViewModel => _bf3StatsViewModel.Value;
        public BF4StatsViewModel BF4StatsViewModel => _bf4StatsViewModel.Value;
    }
}
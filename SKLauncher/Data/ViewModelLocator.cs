using System;
using System.Collections.Generic;

using Launcher.Core.Service;
using Launcher.Core.Service.Base;
using Launcher.Core.Shared;
using Launcher.ViewModel;
using Launcher.ViewModel.Stats;

using Ninject;
using Ninject.Syntax;

namespace Launcher.Data
{
    /// <inheritdoc />
    public class ViewModelLocator : IViewModelSource
    {
        private readonly IDictionary<Type, Lazy<BaseViewModel>> _viewModels;
        private readonly IResolutionRoot _resolver;

        public ViewModelLocator() { }

        [Inject]
        public ViewModelLocator(IResolutionRoot resolver)
        {
            _resolver = resolver;

            // populate view model dictionary
            _viewModels = new Dictionary<Type, Lazy<BaseViewModel>>(10)
            {
                [typeof(MainWindowViewModel)] = _BuildLazyInitializer<MainWindowViewModel>(),
                [typeof(EventLogViewModel)] = _BuildLazyInitializer<EventLogViewModel>(),
                [typeof(HomeViewModel)] = _BuildLazyInitializer<HomeViewModel>(),
                [typeof(SettingsViewModel)] = _BuildLazyInitializer<SettingsViewModel>(),
                [typeof(BF4StatsViewModel)] = _BuildLazyInitializer<BF4StatsViewModel>(),
                [typeof(BF3StatsViewModel)] = _BuildLazyInitializer<BF3StatsViewModel>(),
                [typeof(BF3CoopViewModel)] = _BuildLazyInitializer<BF3CoopViewModel>(),
                [typeof(BF3ServerBrowserViewModel)] = _BuildLazyInitializer<BF3ServerBrowserViewModel>(),
                [typeof(BF4ServerBrowserViewModel)] = _BuildLazyInitializer<BF4ServerBrowserViewModel>(),
                [typeof(BFHServerBrowserViewModel)] = _BuildLazyInitializer<BFHServerBrowserViewModel>(),
                [typeof(UpdateControlViewModel)] = _BuildLazyInitializer<UpdateControlViewModel>(),
                [typeof(GameControlViewModel)] = _BuildLazyInitializer<GameControlViewModel>(),
            };
        }

        public BasePageViewModel MainWindowViewModel => (BasePageViewModel) _viewModels[typeof(MainWindowViewModel)].Value;
        public BasePageViewModel EventLogViewModel => (BasePageViewModel) _viewModels[typeof(EventLogViewModel)].Value;
        public BasePageViewModel HomeViewModel => (BasePageViewModel) _viewModels[typeof(HomeViewModel)].Value;
        public BasePageViewModel SettingsViewModel => (BasePageViewModel) _viewModels[typeof(SettingsViewModel)].Value;
        public BasePageViewModel BF3StatsViewModel => (BasePageViewModel) _viewModels[typeof(BF3StatsViewModel)].Value;
        public BasePageViewModel BF4StatsViewModel => (BasePageViewModel) _viewModels[typeof(BF4StatsViewModel)].Value;
        public BasePageViewModel BF3CoopViewModel => (BasePageViewModel) _viewModels[typeof(BF3CoopViewModel)].Value;
        public BasePageViewModel BF3ServerBrowserViewModel => (BasePageViewModel) _viewModels[typeof(BF3ServerBrowserViewModel)].Value;
        public BasePageViewModel BF4ServerBrowserViewModel => (BasePageViewModel) _viewModels[typeof(BF4ServerBrowserViewModel)].Value;
        public BasePageViewModel BFHServerBrowserViewModel => (BasePageViewModel) _viewModels[typeof(BFHServerBrowserViewModel)].Value;

        #region Private helpers

        private Lazy<BaseViewModel> _BuildLazyInitializer<TViewModel>() where TViewModel: BaseViewModel
            => new Lazy<BaseViewModel>(() => _resolver.Get<TViewModel>());

        #endregion

        #region IViewModelSource

        /// <inheritdoc />
        public TViewModel Create<TViewModel>() where TViewModel : BaseViewModel
            => _resolver.Get<TViewModel>();

        /// <inheritdoc />
        public TViewModel GetExisting<TViewModel>() where TViewModel : BaseViewModel
            => (TViewModel) _viewModels[typeof(TViewModel)].Value;

        #endregion
    }
}
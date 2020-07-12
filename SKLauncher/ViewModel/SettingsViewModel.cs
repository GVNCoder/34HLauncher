using System.Windows.Input;
using System.Windows.Controls;

using Launcher.Core.Bases;
using Launcher.Core.Interaction;
using Launcher.Core.RPC;
using Launcher.Core.Services;
using Launcher.Core.Services.EventLog;
using Launcher.Core.SettingsViewModelParts;
using Launcher.Core.Shared;

using Ninject;

using SLM = Launcher.Localization.Loc.inCodeLocalizationMap.SharedLocalizationMap;

namespace Launcher.ViewModel
{
    public class SettingsViewModel : PageViewModelBase, IBlurredPage
    {
        public ThemesSectionViewModel ThemeSectionViewModel { get; }
        public GeneralSectionViewModel GeneralSectionViewModel { get; }

        private readonly ISettingsService _settingsService;
        private readonly IEventLogService _eventLogService;

        public SettingsViewModel(
            ISettingsService settingsService,
            IEventLogService eventLogService,
            IUIHostService hostService,
            IDiscord discord,
            IKernel kernel) : base(discord)
        {
            _settingsService = settingsService;
            _eventLogService = eventLogService;

            // vm parts
            ThemeSectionViewModel = kernel.Get<ThemesSectionViewModel>();
            GeneralSectionViewModel = kernel.Get<GeneralSectionViewModel>();

            BackgroundContent = hostService.GetHostContainer(UIElementConstants.HostWindowBackground) as Grid;
        }

        public Grid BackgroundContent { get; }

        public override ICommand LoadedCommand => new DelegateCommand(obj =>
        {
            _settingsService.SetGlobalLock();
            _discord.UpdateAFK();
        });

        public override ICommand UnloadedCommand => new DelegateCommand(obj =>
        {
            var settingsSave = _settingsService.Save();
            if (!settingsSave)
            {
                _eventLogService.Log(EventLogLevel.Warning, SLM.SaveSettingsHeader, SLM.SaveSettings);
            }

            _settingsService.FreeGlobalLock();
        });
    }
}

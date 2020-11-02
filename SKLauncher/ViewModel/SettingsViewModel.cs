using System.Windows.Input;

using Launcher.Core;
using Launcher.Core.Interaction;
using Launcher.Core.RPC;
using Launcher.Core.Service.Base;
using Launcher.Core.Services;
using Launcher.Core.SettingsViewModelParts;

using Ninject;
using SLM = Launcher.Localization.Loc.inCodeLocalizationMap.SharedLocalizationMap;

namespace Launcher.ViewModel
{
    public class SettingsViewModel : BasePageViewModel
    {
        public ThemesSectionViewModel ThemeSectionViewModel { get; }
        public GeneralSectionViewModel GeneralSectionViewModel { get; }
        public UpdatesSectionViewModel UpdatesSectionViewModel { get; }

        private readonly ISettingsService _settingsService;
        private readonly IEventService _eventService;
        private readonly IDiscord _discord;

        public SettingsViewModel(
            ISettingsService settingsService,
            IEventService eventService,
            IDiscord discord,
            IKernel kernel)
        {
            _settingsService = settingsService;
            _eventService = eventService;
            _discord = discord;

            // vm parts
            ThemeSectionViewModel = kernel.Get<ThemesSectionViewModel>();
            GeneralSectionViewModel = kernel.Get<GeneralSectionViewModel>();
            UpdatesSectionViewModel = kernel.Get<UpdatesSectionViewModel>();
        }

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
                _eventService.WarnEvent(SLM.SaveSettingsHeader, SLM.SaveSettings);
            }

            _settingsService.FreeGlobalLock();
        });
    }
}

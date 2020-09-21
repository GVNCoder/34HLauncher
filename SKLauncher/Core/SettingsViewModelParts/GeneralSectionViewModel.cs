using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Launcher.Core.Interaction;
using Launcher.Core.RPC;
using Launcher.Core.Service.Base;
using Launcher.Core.Services;
using Launcher.Core.Services.Dialog;
using Launcher.Core.Services.EventLog;
using Launcher.Core.Shared;
using Launcher.Helpers;
using Launcher.Localization.Loc;
using Launcher.Localization.Loc.inCodeLocalizationMap;

using SLM = Launcher.Localization.Loc.inCodeLocalizationMap.SharedLocalizationMap;

namespace Launcher.Core.SettingsViewModelParts
{
    public class GeneralSectionViewModel : BaseControlViewModel
    {
        private readonly ITextDialogService _dialogService;
        private readonly ISettingsService _settingsService;
        private readonly IEventLogService _eventLog;
        private readonly IDiscord _discord;

        private LauncherSettings _settings;
        private bool _isLoaded;

        public GeneralSectionViewModel(
            ITextDialogService dialogService,
            ISettingsService settingsService,
            IEventLogService eventLog,
            IDiscord discord)
        {
            _dialogService = dialogService;
            _settingsService = settingsService;
            _eventLog = eventLog;
            _discord = discord;

            LocalizationEnumerable = new[]
            {
                new KeyValuePair<string, LocalizationEnum>("English", LocalizationEnum.EN),
                new KeyValuePair<string, LocalizationEnum>("Русский", LocalizationEnum.RU),
            };
        }

        #region Dependency properties

        public int LocalizationIndex
        {
            get => (int)GetValue(LocalizationIndexProperty);
            set => SetValue(LocalizationIndexProperty, value);
        }
        public static readonly DependencyProperty LocalizationIndexProperty =
            DependencyProperty.Register("LocalizationIndex", typeof(int), typeof(GeneralSectionViewModel), new PropertyMetadata(0, _localizationIndexChangedCallback));

        private static void _localizationIndexChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var viewModel = (GeneralSectionViewModel) d;
            var value = (int) e.NewValue;

            viewModel._settings.Localization = viewModel.LocalizationEnumerable[value].Value;
            LocManager.SetLocale(viewModel._settings.Localization);
        }

        public bool UnfoldGameWindow
        {
            get => (bool)GetValue(UnfoldGameWindowProperty);
            set => SetValue(UnfoldGameWindowProperty, value);
        }
        public static readonly DependencyProperty UnfoldGameWindowProperty =
            DependencyProperty.Register("UnfoldGameWindow", typeof(bool), typeof(GeneralSectionViewModel), new PropertyMetadata(false, _UnfoldGameWindowChangedCallback));

        private static void _UnfoldGameWindowChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var viewModel = (GeneralSectionViewModel) d;
            var value = (bool) e.NewValue;

            viewModel._settings.UnfoldGameWindow = value;
        }

        public string ZClientPath
        {
            get => (string)GetValue(ZClientPathProperty);
            set => SetValue(ZClientPathProperty, value);
        }
        public static readonly DependencyProperty ZClientPathProperty =
            DependencyProperty.Register("ZClientPath", typeof(string), typeof(GeneralSectionViewModel), new PropertyMetadata("None", _zClientPathChangedCallback));

        private static void _zClientPathChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var viewModel = (GeneralSectionViewModel) d;
            var value = (string) e.NewValue;

            viewModel._settings.PathToZClient = value;
        }

        public bool AutorunZClient
        {
            get => (bool)GetValue(AutorunZClientProperty);
            set => SetValue(AutorunZClientProperty, value);
        }
        public static readonly DependencyProperty AutorunZClientProperty =
            DependencyProperty.Register("AutorunZClient", typeof(bool), typeof(GeneralSectionViewModel), new PropertyMetadata(false, _autorunZClientChangedCallback));

        private static void _autorunZClientChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var viewModel = (GeneralSectionViewModel) d;
            var value = (bool) e.NewValue;

            viewModel._settings.RunZClient = value;
        }

        public bool UseDiscordPresence
        {
            get => (bool)GetValue(UseDiscordPresenceProperty);
            set => SetValue(UseDiscordPresenceProperty, value);
        }
        public static readonly DependencyProperty UseDiscordPresenceProperty =
            DependencyProperty.Register("UseDiscordPresence", typeof(bool), typeof(GeneralSectionViewModel), new PropertyMetadata(false, _useDiscordPresenceChangedCallback));

        private static async void _useDiscordPresenceChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var viewModel = (GeneralSectionViewModel) d;
            var value = (bool) e.NewValue;

            if (value)
            {
                viewModel._discord.Start();
            }
            else
            {
                viewModel._discord.Stop();
            }

            viewModel._settings.UseDiscordPresence = value;
            if (viewModel._isLoaded) await _WaitToggleAsync(viewModel);
        }

        private static async Task _WaitToggleAsync(GeneralSectionViewModel vm)
        {
            vm.CanUseDiscordPresence = false;
            await Task.Delay((int)TimeSpan.FromSeconds(7).TotalMilliseconds);
            vm.CanUseDiscordPresence = true;
        }

        public bool TryToConnect
        {
            get => (bool)GetValue(TryToConnectProperty);
            set => SetValue(TryToConnectProperty, value);
        }
        public static readonly DependencyProperty TryToConnectProperty =
            DependencyProperty.Register("TryToConnect", typeof(bool), typeof(GeneralSectionViewModel), new PropertyMetadata(false, TryToConnectPropertyChangedCallback));

        private static async void TryToConnectPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var viewModel = (GeneralSectionViewModel) d;
            var value = (bool) e.NewValue;

            if (value && viewModel._isLoaded)
            {
                await viewModel._dialogService.OpenDialog(SharedLocalizationMap.TryToConnectHeader, SharedLocalizationMap.TryToConnect,
                    TextDialogButtons.Ok);
            }

            viewModel._settings.TryToConnect = value;
        }

        public bool CanUseDiscordPresence
        {
            get => (bool)GetValue(CanUseDiscordPresenceProperty);
            set => SetValue(CanUseDiscordPresenceProperty, value);
        }
        public static readonly DependencyProperty CanUseDiscordPresenceProperty =
            DependencyProperty.Register("CanUseDiscordPresence", typeof(bool), typeof(GeneralSectionViewModel), new PropertyMetadata(true));

        #endregion

        private void _AssignSettings(LauncherSettings settings)
        {
            UnfoldGameWindow = settings.UnfoldGameWindow;
            LocalizationIndex = Array.FindIndex(LocalizationEnumerable, p => p.Value == settings.Localization);
            ZClientPath = string.IsNullOrEmpty(settings.PathToZClient)
                ? ZClientPath
                : settings.PathToZClient;
            AutorunZClient = settings.RunZClient;
            UseDiscordPresence = settings.UseDiscordPresence;
            TryToConnect = settings.TryToConnect;
        }

        public KeyValuePair<string, LocalizationEnum>[] LocalizationEnumerable { get; }

        public override ICommand LoadedCommand => new DelegateCommand(obj =>
        {
            _settings = _settingsService.GetLauncherSettings();
            _AssignSettings(_settings);
            _isLoaded = true;
        });

        public override ICommand UnloadedCommand => new DelegateCommand(obj => { _isLoaded = false; });

        public ICommand SelectZClientCommand => new DelegateCommand(obj =>
        {
            var dialog = SettingsDialogHelper.BuildOpenFileDialog(AppDomain.CurrentDomain.BaseDirectory, SLM.SelectZClientPathHeader,
                "ZClient file (*.exe)|ZClient.exe");
            var dlgResult = dialog.ShowDialog();

            if (!dlgResult.GetValueOrDefault()) return;

            ZClientPath = dialog.FileName;
        });

        public ICommand DetectZClientCommand => new DelegateCommand(obj =>
        {
            var process = Process.GetProcessesByName("ZClient").FirstOrDefault();
            if (process != null)
            {
                ZClientPath = process.MainModule.FileName;
            }
            else
            {
                _eventLog.Log(EventLogLevel.Warning, SLM.DetectingZClientProcessHeader, SLM.DetectingZClientProcess);
            }
        });
    }
}
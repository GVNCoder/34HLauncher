using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Launcher.Core.Bases;
using Launcher.Core.Interaction;
using Launcher.Core.RPC;
using Launcher.Core.Services;
using Launcher.Core.Services.Dialog;
using Launcher.Core.Services.EventLog;
using Launcher.Core.Shared;
using Launcher.Localization.Loc;
using Launcher.XamlThemes.Theming;

using Microsoft.Win32;
using Ninject;
using SLM = Launcher.Localization.Loc.inCodeLocalizationMap.SharedLocalizationMap;
using LM = Launcher.Localization.Loc.LocManager;
using TM = Launcher.XamlThemes.Theming.ThemeManager;

namespace Launcher.ViewModel
{
    public class SettingsViewModel : PageViewModelBase
    {
        private readonly ISettingsService _settingsService;
        private readonly IEventLogService _eventLogService;
        private readonly ITextDialogService _dialogService;
        private readonly IDiscord _realDiscordService;

        private LauncherSettings _launcherSettings;
        private bool _isLoaded;

        private OpenFileDialog _getOpenFileDialog(string initialDirectory, string title, string filter)
        {
            var _dialog = new OpenFileDialog();

            // set values
            _dialog.InitialDirectory = initialDirectory;
            _dialog.Title = title;
            _dialog.Filter = filter;
            _dialog.Multiselect = false;
            _dialog.CheckFileExists = true;
            _dialog.CheckPathExists = true;
            _dialog.FileName = string.Empty;

            return _dialog;
        }

        public SettingsViewModel(
            ISettingsService settingsService,
            IEventLogService eventLogService,
            IUIHostService hostService,
            ITextDialogService dialogService,
            IDiscordManager discordManager,
            IDiscord discord) : base(discordManager)
        {
            _settingsService = settingsService;
            _eventLogService = eventLogService;
            _dialogService = dialogService;
            _realDiscordService = discord;

            WindowBackgroundContent = hostService.GetHostContainer(UIElementConstants.HostWindowBackground) as Grid;

            LocalizationEnumerable = new []
            {
                new KeyValuePair<string, LocalizationEnum>("English", LocalizationEnum.EN),
                new KeyValuePair<string, LocalizationEnum>("Русский", LocalizationEnum.RU),
            };
            ThemeEnumerable = new []
            {
                new KeyValuePair<string, ThemeEnum>("Dark", ThemeEnum.Dark),
                new KeyValuePair<string, ThemeEnum>("Light", ThemeEnum.Light), 
            };
        }

        public KeyValuePair<string, LocalizationEnum>[] LocalizationEnumerable { get; }
        public KeyValuePair<string, ThemeEnum>[] ThemeEnumerable { get; }
        public Grid WindowBackgroundContent { get; }

        #region Dependency properties

        public int ThemeIndex
        {
            get => (int)GetValue(ThemeIndexProperty);
            set => SetValue(ThemeIndexProperty, value);
        }
        public static readonly DependencyProperty ThemeIndexProperty =
            DependencyProperty.Register("ThemeIndex", typeof(int), typeof(SettingsViewModel), new PropertyMetadata(0, _themeIndexChangedCallback));

        private static void _themeIndexChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var viewModel = (SettingsViewModel) d;
            var value = (int) e.NewValue;

            viewModel._launcherSettings.Theme = viewModel.ThemeEnumerable[value].Value;
            ThemeManager.ApplyTheme(viewModel._launcherSettings.Theme);
        }

        public KeyValuePair<AccentEnum, Color>? AccentScheme
        {
            get => (KeyValuePair<AccentEnum, Color>?)GetValue(AccentSchemeProperty);
            set => SetValue(AccentSchemeProperty, value);
        }
        public static readonly DependencyProperty AccentSchemeProperty =
            DependencyProperty.Register("AccentScheme", typeof(KeyValuePair<AccentEnum, Color>?), typeof(SettingsViewModel), new PropertyMetadata(null, _accentSchemeChangedCallback));

        private static void _accentSchemeChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var viewModel = (SettingsViewModel) d;
            var value = (KeyValuePair<AccentEnum, Color>?) e.NewValue;

            if (value == null) return;

            viewModel._launcherSettings.Accent = value.Value.Key;
            ThemeManager.ApplyAccent(value.Value.Key);
        }

        public int LocalizationIndex
        {
            get => (int)GetValue(LocalizationIndexProperty);
            set => SetValue(LocalizationIndexProperty, value);
        }
        public static readonly DependencyProperty LocalizationIndexProperty =
            DependencyProperty.Register("LocalizationIndex", typeof(int), typeof(SettingsViewModel), new PropertyMetadata(0, _localizationIndexChangedCallback));

        private static void _localizationIndexChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var viewModel = (SettingsViewModel) d;
            var value = (int) e.NewValue;

            viewModel._launcherSettings.Localization = viewModel.LocalizationEnumerable[value].Value;
            LocManager.SetLocale(viewModel._launcherSettings.Localization);
        }

        public bool UnfoldGameWindow
        {
            get => (bool)GetValue(UnfoldGameWindowProperty);
            set => SetValue(UnfoldGameWindowProperty, value);
        }
        public static readonly DependencyProperty UnfoldGameWindowProperty =
            DependencyProperty.Register("UnfoldGameWindow", typeof(bool), typeof(SettingsViewModel), new PropertyMetadata(false, _UnfoldGameWindowChangedCallback));

        private static void _UnfoldGameWindowChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var viewModel = (SettingsViewModel) d;
            var value = (bool) e.NewValue;

            viewModel._launcherSettings.UnfoldGameWindow = value;
        }

        public string ZClientPath
        {
            get => (string)GetValue(ZClientPathProperty);
            set => SetValue(ZClientPathProperty, value);
        }
        public static readonly DependencyProperty ZClientPathProperty =
            DependencyProperty.Register("ZClientPath", typeof(string), typeof(SettingsViewModel), new PropertyMetadata("None", _zClientPathChangedCallback));

        private static void _zClientPathChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var viewModel = (SettingsViewModel) d;
            var value = (string) e.NewValue;

            viewModel._launcherSettings.PathToZClient = value;
        }

        public bool AutorunZClient
        {
            get => (bool)GetValue(AutorunZClientProperty);
            set => SetValue(AutorunZClientProperty, value);
        }
        public static readonly DependencyProperty AutorunZClientProperty =
            DependencyProperty.Register("AutorunZClient", typeof(bool), typeof(SettingsViewModel), new PropertyMetadata(false, _autorunZClientChangedCallback));

        private static void _autorunZClientChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var viewModel = (SettingsViewModel) d;
            var value = (bool) e.NewValue;

            viewModel._launcherSettings.RunZClient = value;
        }

        public bool UseDiscordPresence
        {
            get => (bool)GetValue(UseDiscordPresenceProperty);
            set => SetValue(UseDiscordPresenceProperty, value);
        }
        public static readonly DependencyProperty UseDiscordPresenceProperty =
            DependencyProperty.Register("UseDiscordPresence", typeof(bool), typeof(SettingsViewModel), new PropertyMetadata(false, _useDiscordPresenceChangedCallback));

        private static async void _useDiscordPresenceChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var viewModel = (SettingsViewModel) d;
            var value = (bool) e.NewValue;

            if (value)
            {
                viewModel._realDiscordService.Start();
            }
            else
            {
                viewModel._realDiscordService.Stop();
            }

            viewModel._launcherSettings.UseDiscordPresence = value;

            // TODO: Refactoring
            viewModel.CanUseDiscordPresence = false;
            if (viewModel._isLoaded) await Task.Delay((int) TimeSpan.FromSeconds(7).TotalMilliseconds);
            viewModel.CanUseDiscordPresence = true;
        }

        public int GameCardOpacity
        {
            get => (int)GetValue(GameCardOpacityProperty);
            set => SetValue(GameCardOpacityProperty, value);
        }
        public static readonly DependencyProperty GameCardOpacityProperty =
            DependencyProperty.Register("GameCardOpacity", typeof(int), typeof(SettingsViewModel), new PropertyMetadata(10, _gameCardOpacityPropertyChangedCallback));

        private static void _gameCardOpacityPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var viewModel = (SettingsViewModel) d;
            var value = (double) ((int) e.NewValue);

            viewModel._launcherSettings.CardTransparency = value / 100;
        }

        public bool TryToConnect
        {
            get => (bool)GetValue(TryToConnectProperty);
            set => SetValue(TryToConnectProperty, value);
        }
        public static readonly DependencyProperty TryToConnectProperty =
            DependencyProperty.Register("TryToConnect", typeof(bool), typeof(SettingsViewModel), new PropertyMetadata(false, TryToConnectPropertyChangedCallback));

        private static async void TryToConnectPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var viewModel = (SettingsViewModel) d;
            var value = (bool) e.NewValue;

            if (value && viewModel._isLoaded)
            {
                await viewModel._dialogService.OpenDialog(SLM.TryToConnectHeader, SLM.TryToConnect,
                    TextDialogButtons.Ok);
            }

            viewModel._launcherSettings.TryToConnect = value;
        }

        public bool CanUseDiscordPresence
        {
            get => (bool)GetValue(CanUseDiscordPresenceProperty);
            set => SetValue(CanUseDiscordPresenceProperty, value);
        }
        public static readonly DependencyProperty CanUseDiscordPresenceProperty =
            DependencyProperty.Register("CanUseDiscordPresence", typeof(bool), typeof(SettingsViewModel), new PropertyMetadata(true));

        #endregion

        public override ICommand LoadedCommand => new DelegateCommand(obj =>
        {
            base.OnLoadedImpl();

            _settingsService.SetGlobalLock();
            _launcherSettings = _settingsService.GetLauncherSettings();

            UnfoldGameWindow = _launcherSettings.UnfoldGameWindow;
            LocalizationIndex = Array.FindIndex(LocalizationEnumerable, p => p.Value == _launcherSettings.Localization);
            ThemeIndex = Array.FindIndex(ThemeEnumerable, p => p.Value == _launcherSettings.Theme);
            AccentScheme = AccentCollection.Accents.FirstOrDefault(a => a.Key == _launcherSettings.Accent);
            ZClientPath = string.IsNullOrEmpty(_launcherSettings.PathToZClient)
                ? ZClientPath
                : _launcherSettings.PathToZClient;
            AutorunZClient = _launcherSettings.RunZClient;
            UseDiscordPresence = _launcherSettings.UseDiscordPresence;
            TryToConnect = _launcherSettings.TryToConnect;

            // TODO: Wait one two weeks and remove ?:
            var x = (Math.Abs(_launcherSettings.CardTransparency) < .1d ? .1d : _launcherSettings.CardTransparency);
            var b = x * 100;
            GameCardOpacity = (int) b;

            _discord.UpdateAFK();
            _isLoaded = true;
        });

        public override ICommand UnloadedCommand => new DelegateCommand(obj =>
        {
            var settingsSave = _settingsService.Save();
            if (!settingsSave)
            {
                _eventLogService.Log(EventLogLevel.Warning, SLM.SaveSettingsHeader, SLM.SaveSettings);
            }

            _settingsService.FreeGlobalLock();
            _isLoaded = false;
        });

        public ICommand SelectZClientCommand => new DelegateCommand(obj =>
        {
            var dialog = _getOpenFileDialog(AppDomain.CurrentDomain.BaseDirectory, SLM.SelectZClientPathHeader,
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
                _eventLogService.Log(EventLogLevel.Warning, SLM.DetectingZClientProcessHeader, SLM.DetectingZClientProcess);
            }
        });

        public ICommand SelectImageCommand => new DelegateCommand(obj =>
        {
            var dialog = _getOpenFileDialog(AppDomain.CurrentDomain.BaseDirectory, SLM.SelectImage,
                "Image files (*.jpg, *.png, *.bmp)|*.jpg;*.png;*.bmp");
            var dlgResult = dialog.ShowDialog();

            if (!dlgResult.GetValueOrDefault()) return;

            var result =
                XamlThemes.Theming.ThemeManager.TrySetBackgroundImage(BackgroundImageEnum.Custom, dialog.FileName);
            if (!result)
            {
                _eventLogService.Log(EventLogLevel.Warning, SLM.CannotSetImageHeader, SLM.CannotSetImage);
            }

            XamlThemes.Theming.ThemeManager.ApplyBackgroundImage();
        });

        public ICommand ResetImageCommand => new DelegateCommand(obj =>
        {
            XamlThemes.Theming.ThemeManager.TrySetBackgroundImage(BackgroundImageEnum.Default, null);
            XamlThemes.Theming.ThemeManager.ApplyBackgroundImage();
        });
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Launcher.Core.Interaction;
using Launcher.Core.Services;
using Launcher.Core.Services.EventLog;
using Launcher.Core.Shared;
using Launcher.Helpers;
using Launcher.XamlThemes.Theming;

using SLM = Launcher.Localization.Loc.inCodeLocalizationMap.SharedLocalizationMap;

namespace Launcher.Core.SettingsViewModelParts
{
    public class ThemesSectionViewModel : UIBaseViewModel
    {
        private readonly ISettingsService _settingsService;
        private readonly IEventLogService _eventLog;

        private LauncherSettings _settings;

        public ThemesSectionViewModel(ISettingsService settingsService, IEventLogService eventLog)
        {
            _settingsService = settingsService;
            _eventLog = eventLog;

            ThemeEnumerable = new[]
            {
                new KeyValuePair<string, ThemeEnum>("Dark", ThemeEnum.Dark),
                new KeyValuePair<string, ThemeEnum>("Light", ThemeEnum.Light),
            };
        }

        #region Dependency properties

        public int ThemeIndex
        {
            get => (int)GetValue(ThemeIndexProperty);
            set => SetValue(ThemeIndexProperty, value);
        }
        public static readonly DependencyProperty ThemeIndexProperty =
            DependencyProperty.Register("ThemeIndex", typeof(int), typeof(ThemesSectionViewModel), new PropertyMetadata(0, _themeIndexChangedCallback));

        private static void _themeIndexChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var viewModel = (ThemesSectionViewModel) d;
            var value = (int) e.NewValue;

            viewModel._settings.Theme = viewModel.ThemeEnumerable[value].Value;
            ThemeManager.ApplyTheme(viewModel._settings.Theme);
        }

        public KeyValuePair<AccentEnum, Color>? AccentScheme
        {
            get => (KeyValuePair<AccentEnum, Color>?)GetValue(AccentSchemeProperty);
            set => SetValue(AccentSchemeProperty, value);
        }
        public static readonly DependencyProperty AccentSchemeProperty =
            DependencyProperty.Register("AccentScheme", typeof(KeyValuePair<AccentEnum, Color>?), typeof(ThemesSectionViewModel), new PropertyMetadata(null, _accentSchemeChangedCallback));

        private static void _accentSchemeChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var viewModel = (ThemesSectionViewModel) d;
            var value = (KeyValuePair<AccentEnum, Color>?) e.NewValue;

            if (value == null) return;

            viewModel._settings.Accent = value.Value.Key;
            ThemeManager.ApplyAccent(value.Value.Key);
        }

        public int GameCardOpacity
        {
            get => (int)GetValue(GameCardOpacityProperty);
            set => SetValue(GameCardOpacityProperty, value);
        }
        public static readonly DependencyProperty GameCardOpacityProperty =
            DependencyProperty.Register("GameCardOpacity", typeof(int), typeof(ThemesSectionViewModel), new PropertyMetadata(10, _gameCardOpacityPropertyChangedCallback));

        private static void _gameCardOpacityPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var viewModel = (ThemesSectionViewModel) d;
            var value = (double) ((int)e.NewValue);

            viewModel._settings.CardTransparency = value / 100;
        }

        #endregion

        private void _AssignSettings(LauncherSettings settings)
        {
            ThemeIndex = Array.FindIndex(ThemeEnumerable, p => p.Value == settings.Theme);
            AccentScheme = AccentCollection.Accents.FirstOrDefault(a => a.Key == settings.Accent);
            GameCardOpacity = (int) (settings.CardTransparency * 100);
        }

        public KeyValuePair<string, ThemeEnum>[] ThemeEnumerable { get; }

        public override ICommand LoadedCommand => new DelegateCommand(obj =>
        {
            _settings = _settingsService.GetLauncherSettings();
            _AssignSettings(_settings);
        });

        public override ICommand UnloadedCommand => new DelegateCommand(obj =>
        {

        });

        public ICommand SelectImageCommand => new DelegateCommand(obj =>
        {
            var dialog = SettingsDialogHelper.BuildOpenFileDialog(AppDomain.CurrentDomain.BaseDirectory, SLM.SelectImage,
                "Image files (*.jpg, *.png, *.bmp)|*.jpg;*.png;*.bmp");
            var dlgResult = dialog.ShowDialog();

            if (! dlgResult.GetValueOrDefault()) return;

            var result = ThemeManager.TrySetBackgroundImage(BackgroundImageEnum.Custom, dialog.FileName);
            if (! result)
            {
                _eventLog.Log(EventLogLevel.Warning, SLM.CannotSetImageHeader, SLM.CannotSetImage);
            }

            ThemeManager.ApplyBackgroundImage();
        });

        public ICommand ResetImageCommand => new DelegateCommand(obj =>
        {
            ThemeManager.TrySetBackgroundImage(BackgroundImageEnum.Default, null);
            ThemeManager.ApplyBackgroundImage();
        });
    }
}
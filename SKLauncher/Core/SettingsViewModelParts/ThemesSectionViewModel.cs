using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

using Launcher.Core.Interaction;
using Launcher.Core.Service.Base;
using Launcher.Core.Services;
using Launcher.Core.Shared;
using Launcher.Helpers;
using Launcher.XamlThemes.Theming;

using SLM = Launcher.Localization.Loc.inCodeLocalizationMap.SharedLocalizationMap;

namespace Launcher.Core.SettingsViewModelParts
{
    public class ThemesSectionViewModel : BaseControlViewModel
    {
        private readonly ISettingsService _settingsService;
        private readonly IEventService _eventService;

        private LauncherSettings _settings;

        public ThemesSectionViewModel(ISettingsService settingsService, IEventService eventService)
        {
            _settingsService = settingsService;
            _eventService = eventService;

            ThemeEnumerable = new[]
            {
                new KeyValuePair<string, LauncherTheme>("Dark", LauncherTheme.Dark),
                new KeyValuePair<string, LauncherTheme>("Light", LauncherTheme.Light),
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

            viewModel._settings.DataTheme = viewModel.ThemeEnumerable[value].Value;
            ThemeManager.ApplyTheme(viewModel._settings.DataTheme);
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

            viewModel._settings.DataMainMenuCardTransparency = value / 100;
        }

        #endregion

        private void _AssignSettings(LauncherSettings settings)
        {
            ThemeIndex = Array.FindIndex(ThemeEnumerable, p => p.Value == settings.DataTheme);
            //AccentScheme = AccentCollection.Accents.FirstOrDefault(a => a.Key == settings.Accent);
            GameCardOpacity = (int) (settings.DataMainMenuCardTransparency * 100);
        }

        public KeyValuePair<string, LauncherTheme>[] ThemeEnumerable { get; }

        public override ICommand LoadedCommand => new DelegateCommand(obj =>
        {
            _settings = _settingsService.Current;
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
                _eventService.WarnEvent(SLM.CannotSetImageHeader, SLM.CannotSetImage);
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
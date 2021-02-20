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

        public ImageSource BackgroundImageSource
        {
            get => (ImageSource)GetValue(BackgroundImageSourceProperty);
            set => SetValue(BackgroundImageSourceProperty, value);
        }
        public static readonly DependencyProperty BackgroundImageSourceProperty =
            DependencyProperty.Register("BackgroundImageSource", typeof(ImageSource), typeof(ThemesSectionViewModel), new PropertyMetadata(null));

        public ImageSource BF3CardImageSource
        {
            get { return (ImageSource)GetValue(BF3CardImageSourceProperty); }
            set { SetValue(BF3CardImageSourceProperty, value); }
        }
        public static readonly DependencyProperty BF3CardImageSourceProperty =
            DependencyProperty.Register("BF3CardImageSource", typeof(ImageSource), typeof(ThemesSectionViewModel), new PropertyMetadata(null));

        public ImageSource BF4CardImageSource
        {
            get { return (ImageSource)GetValue(BF4CardImageSourceProperty); }
            set { SetValue(BF4CardImageSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BF4CardImageSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BF4CardImageSourceProperty =
            DependencyProperty.Register("BF4CardImageSource", typeof(ImageSource), typeof(ThemesSectionViewModel), new PropertyMetadata(null));



        public ImageSource BFHCardImageSource
        {
            get { return (ImageSource)GetValue(BFHCardImageSourceProperty); }
            set { SetValue(BFHCardImageSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BFHCardImageSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BFHCardImageSourceProperty =
            DependencyProperty.Register("BFHCardImageSource", typeof(ImageSource), typeof(ThemesSectionViewModel), new PropertyMetadata(null));




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

            BackgroundImageSource = ThemeManager.GetImageResourceByKey("BackgroundImage");
            BF3CardImageSource = ThemeManager.GetImageResourceByKey("BF3MenuCardImage");
            BF4CardImageSource = ThemeManager.GetImageResourceByKey("BF4MenuCardImage");
            BFHCardImageSource = ThemeManager.GetImageResourceByKey("BFHMenuCardImage");
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

            var result = ThemeManager.SetCustomBackgroundImage(dialog.FileName);
            if (! result)
            {
                _eventService.WarnEvent(SLM.CannotSetImageHeader, SLM.CannotSetImage);
            }
        });

        public ICommand ResetImageCommand => new DelegateCommand(obj =>
        {
            ThemeManager.ResetBackgroundImage();
        });
    }
}
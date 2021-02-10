using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;

using Launcher.Core.Dialog;
using Launcher.Core.Interaction;
using Launcher.Core.Service.Base;
using Launcher.Core.Services;
using Launcher.Core.Shared;

namespace Launcher.Core.SettingsViewModelParts
{
    public class UpdatesSectionViewModel : BaseControlViewModel
    {
        private const string __changelogFileName = "ChangeLog_";
        private const string __cacheFilePath = "update\\34H Update.exe";

        private readonly IDialogService _dialogService;
        private readonly ISettingsService _settingsService;

        private LauncherSettings _settings;

        public UpdatesSectionViewModel(
            IDialogService dialogService,
            ISettingsService settingsService)
        {
            _dialogService = dialogService;
            _settingsService = settingsService;
        }

        #region Dependency properties

        public bool DisableChangelogAutoOpen
        {
            get => (bool)GetValue(DisableChangelogAutoOpenProperty);
            set => SetValue(DisableChangelogAutoOpenProperty, value);
        }
        public static readonly DependencyProperty DisableChangelogAutoOpenProperty =
            DependencyProperty.Register("DisableChangelogAutoOpen", typeof(bool), typeof(UpdatesSectionViewModel), new PropertyMetadata(false, _DisableChangelogAutoOpenChangedCallback));

        private static void _DisableChangelogAutoOpenChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var viewModel = (UpdatesSectionViewModel) d;
            var value = (bool) e.NewValue;

            viewModel._settings.AutoOpenChangelog = value;
        }
        
        #endregion

        private void _AssignSettings(LauncherSettings settings)
        {
            DisableChangelogAutoOpen = settings.AutoOpenChangelog;
        }

        public override ICommand LoadedCommand => new DelegateCommand(obj =>
        {
            _settings = _settingsService.Current;
            _AssignSettings(_settings);
            _isLoaded = true;
        });

        public override ICommand UnloadedCommand => new DelegateCommand(obj => { _isLoaded = false; });

        public ICommand OpenChangelogCommand => new DelegateCommand(async obj =>
        {
            var changelogFileName = $"{__changelogFileName}{_settings.DataLocalization}.txt";
            if (File.Exists(changelogFileName))
            {
                Process.Start(changelogFileName);
            }
            else
            {
                await _dialogService.OpenTextDialog("File not found",
                    "For some reason, the file with changelog was not found.", DialogButtons.Ok);
            }
        });

        public ICommand DeleteCacheCommand => new DelegateCommand(async obj =>
        {
            var dialogContent = "The cache has been successfully deleted.";

            try
            {
                File.Delete(__cacheFilePath);
            }
            catch (Exception)
            {
                dialogContent = "Unknown error while deleting cache.";
            }

            await _dialogService.OpenTextDialog("Delete cache", dialogContent, DialogButtons.Ok);
        });
    }
}
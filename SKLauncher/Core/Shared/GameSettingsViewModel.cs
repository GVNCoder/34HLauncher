using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Launcher.Core.Interaction;
using Launcher.Core.Services;
using Microsoft.Win32;
using Zlo4NET.Api.Models.Shared;

namespace Launcher.Core.Shared
{
    public class GameSettingsViewModel : DependencyObject
    {
        private readonly GameSetting _settings;
        private readonly ZGameArchitecture _defaultArchitecture;
        private readonly ISettingsService _settingsService;

        public GameSettingsViewModel(GameSetting setting, ISettingsService settingsService, bool canChangeArchitecture)
        {
            _settings = setting;
            _defaultArchitecture = Environment.Is64BitOperatingSystem ? ZGameArchitecture.x64 : ZGameArchitecture.x32;
            _settingsService = settingsService;

            _settingsService.SetGlobalLock();

            BaseArchitecture = _defaultArchitecture.ToString();
            OpposeArchitecture = _defaultArchitecture == ZGameArchitecture.x32
                ? ZGameArchitecture.x64.ToString()
                : ZGameArchitecture.x32.ToString();
            OpposeArchitecture = $"Use {OpposeArchitecture}";
            UseAnotherArchitecture = _defaultArchitecture != _settings.PreferredArchitecture;
            Dlls = new ObservableCollection<string>(setting.Dlls.Select(Path.GetFileName));
            CanChangeArchitecture = canChangeArchitecture;
        }

        public ObservableCollection<string> Dlls { get; }
        public string BaseArchitecture { get; }
        public string OpposeArchitecture { get; }
        public bool CanChangeArchitecture { get; }

        public bool UseAnotherArchitecture
        {
            get => (bool)GetValue(UseAnotherArchitectureProperty);
            set => SetValue(UseAnotherArchitectureProperty, value);
        }
        public static readonly DependencyProperty UseAnotherArchitectureProperty =
            DependencyProperty.Register("UseAnotherArchitecture", typeof(bool), typeof(GameSettingsViewModel), new PropertyMetadata(false, _architectureChangedCallback));

        private static void _architectureChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var viewModel = (GameSettingsViewModel) d;
            var value = (bool) e.NewValue;

            switch (viewModel._defaultArchitecture)
            {
                case ZGameArchitecture.x32 when value:
                    viewModel._settings.PreferredArchitecture = ZGameArchitecture.x64;
                    break;
                case ZGameArchitecture.x64 when value:
                    viewModel._settings.PreferredArchitecture = ZGameArchitecture.x32;
                    break;

                default:
                    viewModel._settings.PreferredArchitecture = viewModel._defaultArchitecture;
                    break;
            }
        }

        private OpenFileDialog _getOpenFileDialog(string initialDirectory, string title, string filter)
        {
            // create dialog if is null
            var _dialog = new OpenFileDialog();

            // set values
            _dialog.InitialDirectory = initialDirectory;
            _dialog.Title = title;
            _dialog.Filter = filter;
            _dialog.Multiselect = true;
            _dialog.CheckFileExists = true;
            _dialog.CheckPathExists = true;
            _dialog.FileName = string.Empty;

            return _dialog;
        }

        public ICommand AddCommand => new DelegateCommand(obj =>
        {
            var dialog = _getOpenFileDialog(AppDomain.CurrentDomain.BaseDirectory, "Select Dll(s)",
                "DLL file (*.dll)|*.dll");
            var dlgRes = dialog.ShowDialog();

            if (dlgRes.GetValueOrDefault())
            {
                foreach (var dialogFileName in dialog.FileNames)
                {
                    Dlls.Add(Path.GetFileName(dialogFileName));
                    _settings.Dlls.Add(dialogFileName);
                }
            }
        });

        public ICommand RemoveCommand => new DelegateCommand(obj =>
        {
            var selectedItems = ((IList<object>)obj)
                .ToArray();
            foreach (var selectedItem in selectedItems)
            {
                var stringItem = (string)selectedItem;
                Dlls.Remove(stringItem);
                var item = _settings.Dlls.First(path => path.EndsWith(stringItem));
                _settings.Dlls.Remove(item);
            }
        });

        public ICommand UnloadedCommand => new DelegateCommand(_UnloadedExec);

        private void _UnloadedExec(object obj)
        {
            _settingsService.FreeGlobalLock();
        }
    }
}
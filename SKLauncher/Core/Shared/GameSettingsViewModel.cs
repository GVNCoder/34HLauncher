using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;

using Launcher.Core.Interaction;
using Launcher.Core.Service.Base;
using Launcher.Core.Services;

using Microsoft.Win32;

using Zlo4NET.Api.Models.Shared;

namespace Launcher.Core.Shared
{
    public class GameSettingsViewModel : BaseControlViewModel
    {
        private readonly GameSettings _settings;
        private readonly ZGameArchitecture _defaultArchitecture;
        private readonly ISettingsService _settingsService;

        public GameSettingsViewModel(GameSettings setting, ISettingsService settingsService, bool canChangeArchitecture)
        {
            _settings = setting;
            _defaultArchitecture = Environment.Is64BitOperatingSystem ? ZGameArchitecture.x64 : ZGameArchitecture.x32;
            _settingsService = settingsService;

            _settingsService.SetLock();

            BaseArchitecture = _defaultArchitecture.ToString();
            OpposeArchitecture = _defaultArchitecture == ZGameArchitecture.x32
                ? ZGameArchitecture.x64.ToString()
                : ZGameArchitecture.x32.ToString();
            OpposeArchitecture = $"Use {OpposeArchitecture}";
            UseAnotherArchitecture = _defaultArchitecture != _settings.DataArchitecture;
            Dlls = new ObservableCollection<string>(setting.DataCollectionInjectableDlls.Select(Path.GetFileName));
            CanChangeArchitecture = canChangeArchitecture;

            CurrentArchitecture = $"Your system architecture is {_defaultArchitecture}";
        }

        public ObservableCollection<string> Dlls { get; }
        public string BaseArchitecture { get; }
        public string OpposeArchitecture { get; }
        public bool CanChangeArchitecture { get; }

        public string CurrentArchitecture
        {
            get => (string)GetValue(CurrentArchitectureProperty);
            set => SetValue(CurrentArchitectureProperty, value);
        }
        public static readonly DependencyProperty CurrentArchitectureProperty =
            DependencyProperty.Register("CurrentArchitecture", typeof(string), typeof(GameSettingsViewModel), new PropertyMetadata(string.Empty));

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
                    viewModel._settings.DataArchitecture = ZGameArchitecture.x64;
                    break;
                case ZGameArchitecture.x64 when value:
                    viewModel._settings.DataArchitecture = ZGameArchitecture.x32;
                    break;

                default:
                    viewModel._settings.DataArchitecture = viewModel._defaultArchitecture;
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
            var dllsList = new List<string>();


            if (dlgRes.GetValueOrDefault())
            {
                foreach (var dialogFileName in dialog.FileNames)
                {
                    Dlls.Add(Path.GetFileName(dialogFileName));
                    dllsList.Add(dialogFileName);
                }

                _settings.DataCollectionInjectableDlls = _settings.DataCollectionInjectableDlls
                    .Concat(dllsList)
                    .ToArray();
            }
        });

        public ICommand RemoveCommand => new DelegateCommand(obj =>
        {
            var selectedItems = ((IList<object>)obj)
                .ToArray();
            var dllsList = new List<string>(_settings.DataCollectionInjectableDlls);

            foreach (var selectedItem in selectedItems)
            {
                var stringItem = (string) selectedItem;
                Dlls.Remove(stringItem);
                var item = _settings.DataCollectionInjectableDlls.First(path => path.EndsWith(stringItem));
                dllsList.Remove(item);
            }

            _settings.DataCollectionInjectableDlls = _settings.DataCollectionInjectableDlls
                .Concat(dllsList)
                .ToArray();
        });

        public override ICommand LoadedCommand => throw new NotImplementedException();
        public override ICommand UnloadedCommand => new DelegateCommand(_UnloadedExec);

        private void _UnloadedExec(object obj)
        {
            _settingsService.FreeLock();
        }
    }
}
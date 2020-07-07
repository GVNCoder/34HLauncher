using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Launcher.Core.Interaction;
using Launcher.Helpers;
using Microsoft.Win32;
using Zlo4NET.Api.Models.Shared;

namespace Launcher.Core.Shared
{
    public class GameTabViewModel : DependencyObject
    {
        private readonly GameSetting _setting;
        private readonly ZGameArchitecture _defaultArchitecture;

        public GameTabViewModel(GameSetting setting, ZGameArchitecture baseArchitecture)
        {
            _setting = setting;
            _defaultArchitecture = baseArchitecture;

            Header = setting.Game.ToString();
            BaseArchitecture = baseArchitecture.ToString();
            OpposeArchitecture = baseArchitecture == ZGameArchitecture.x32
                ? ZGameArchitecture.x64.ToString()
                : ZGameArchitecture.x32.ToString();
            OpposeArchitecture = $"Use {OpposeArchitecture}";
            UseAnotherArchitecture = _defaultArchitecture != _setting.PreferredArchitecture;
            Dlls = new ObservableCollection<string>(setting.Dlls.Select(Path.GetFileName));
        }

        public ObservableCollection<string> Dlls { get; }
        public string BaseArchitecture { get; }
        public string OpposeArchitecture { get; }

        #region Dependency property

        public string Header
        {
            get => (string)GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(string), typeof(GameTabViewModel), new PropertyMetadata(string.Empty));

        public bool UseAnotherArchitecture
        {
            get => (bool)GetValue(UseAnotherArchitectureProperty);
            set => SetValue(UseAnotherArchitectureProperty, value);
        }
        public static readonly DependencyProperty UseAnotherArchitectureProperty =
            DependencyProperty.Register("UseAnotherArchitecture", typeof(bool), typeof(GameTabViewModel), new PropertyMetadata(false, _architectureChangedCallback));

        private static void _architectureChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var viewModel = (GameTabViewModel) d;
            var value = (bool) e.NewValue;

            switch (viewModel._defaultArchitecture)
            {
                case ZGameArchitecture.x32 when value:
                    viewModel._setting.PreferredArchitecture = ZGameArchitecture.x64;
                    break;
                case ZGameArchitecture.x64 when value:
                    viewModel._setting.PreferredArchitecture = ZGameArchitecture.x32;
                    break;

                default:
                    viewModel._setting.PreferredArchitecture = viewModel._defaultArchitecture;
                    break;
            }
        }

        #endregion

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
                    _setting.Dlls.Add(dialogFileName);
                }
            }
        });

        public ICommand RemoveCommand => new DelegateCommand(obj =>
        {
            var selectedItems = ((IList<object>) obj)
                .ToArray();
            foreach (var selectedItem in selectedItems)
            {
                var stringItem = (string) selectedItem;
                Dlls.Remove(stringItem);
                var item = _setting.Dlls.First(path => path.EndsWith(stringItem));
                _setting.Dlls.Remove(item);
            }
        });
    }
}
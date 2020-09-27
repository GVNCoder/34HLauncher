using System.Windows;
using System.Windows.Input;

using Launcher.Core.Interaction;
using Launcher.Core.Service.Base;

using Zlo4NET.Api.Models.Shared;

using Clipboard = Launcher.Helpers.Clipboard;

namespace Launcher.ViewModel.UserControl
{
    public class PlayerPresenterViewModel : BaseControlViewModel
    {
        public override ICommand LoadedCommand => null;
        public override ICommand UnloadedCommand => null;

        #region Bindable fields

        public string PlayerName
        {
            get => (string)GetValue(PlayerNameProperty);
            set => SetValue(PlayerNameProperty, value);
        }
        public static readonly DependencyProperty PlayerNameProperty =
            DependencyProperty.Register("PlayerName", typeof(string), typeof(PlayerPresenterViewModel), new PropertyMetadata(string.Empty));

        public string PlayerId
        {
            get => (string)GetValue(PlayerIdProperty);
            set => SetValue(PlayerIdProperty, value);
        }
        public static readonly DependencyProperty PlayerIdProperty =
            DependencyProperty.Register("PlayerId", typeof(string), typeof(PlayerPresenterViewModel), new PropertyMetadata(string.Empty));

        public ICommand CopyIdCommand => new DelegateCommand(parameter =>
        {
            if (string.IsNullOrEmpty(PlayerId)) Clipboard.CopyToClipboard(PlayerId);
        });

        #endregion

        #region Public methods

        public void SetPlayerData(ZUser user) => Dispatcher.Invoke(() =>
        {
            // setup unauthorized user
            if (user == null)
            {
                // user localized string here
                PlayerName = "Unauthorized user";
                PlayerId = string.Empty;
            }
            else // setup user
            {
                PlayerName = user.Name;
                PlayerId = user.Id.ToString();
            }
        });

        #endregion
    }
}
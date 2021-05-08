using System.Windows;
using System.Windows.Input;

using Launcher.Core.Interaction;
using Launcher.Core.Service;
using Launcher.Core.Service.Base;
using Launcher.Helpers;
using Zlo4NET.Api.DTO;

using Clipboard = Launcher.Helpers.Clipboard;

namespace Launcher.ViewModel.UserControl
{
    public class UserPresenterViewModel : BaseControlViewModel
    {
        private readonly IApplicationState _state;

        public UserPresenterViewModel(IApplicationState state)
        {
            _state = state;
        }

        #region Overrides

        public override ICommand LoadedCommand => null;
        public override ICommand UnloadedCommand => null;

        #endregion

        #region Bindable fields

        public bool IsEnabled
        {
            get => (bool)GetValue(IsEnabledProperty);
            set => SetValue(IsEnabledProperty, value);
        }
        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.Register("IsEnabled", typeof(bool), typeof(UserPresenterViewModel), new PropertyMetadata(false));

        public string UserName
        {
            get => (string)GetValue(UserNameProperty);
            set => SetValue(UserNameProperty, value);
        }
        public static readonly DependencyProperty UserNameProperty =
            DependencyProperty.Register("UserName", typeof(string), typeof(UserPresenterViewModel), new PropertyMetadata(string.Empty));

        public string UserId
        {
            get => (string)GetValue(UserIdProperty);
            set => SetValue(UserIdProperty, value);
        }
        public static readonly DependencyProperty UserIdProperty =
            DependencyProperty.Register("UserId", typeof(string), typeof(UserPresenterViewModel), new PropertyMetadata(string.Empty));

        public bool PopupIsShow
        {
            get => (bool)GetValue(PopupIsShowProperty);
            set => SetValue(PopupIsShowProperty, value);
        }
        public static readonly DependencyProperty PopupIsShowProperty =
            DependencyProperty.Register("PopupIsShow", typeof(bool), typeof(UserPresenterViewModel), new PropertyMetadata(false));

        #endregion

        #region Commands

        public ICommand CopyIdCommand => new DelegateCommand(parameter =>
        {
            // this method can be call only if we`re connected
            Clipboard.CopyToClipboard(UserId);
        });

        public ICommand MouseUpHandlerCommand => new DelegateCommand(parameter =>
        {
            // if we`re not connected, then return
            var isConnected = _state.GetState<bool>(Constants.ZCLIENT_CONNECTION);
            if (! isConnected) return;

            // switch popup visibility
            PopupIsShow ^= true;
        });

        #endregion

        #region Public methods

        public void SetUserData(ZUserDto user) => Dispatcher.Invoke(() =>
        {
            // setup unauthorized user
            if (user == null)
            {
                // user localized string here
                UserName = "Unauthorized user";
                UserId = string.Empty;
                IsEnabled = false;
                PopupIsShow = false;
            }
            else // setup user
            {
                UserName = user.UserName;
                UserId = user.UserId.ToString();
                IsEnabled = true;
            }
        });

        #endregion

        #region Private helpers

        

        #endregion
    }
}
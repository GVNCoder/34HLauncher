using System.Windows;
using System.Windows.Input;

using Launcher.Core.Interaction;
using Launcher.Core.Service;
using Launcher.Core.Service.Base;
using Launcher.Helpers;

using Zlo4NET.Api.Models.Shared;

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

        public Visibility UserIdVisibility
        {
            get => (Visibility)GetValue(UserIdVisibilityProperty);
            set => SetValue(UserIdVisibilityProperty, value);
        }
        public static readonly DependencyProperty UserIdVisibilityProperty =
            DependencyProperty.Register("UserIdVisibility", typeof(Visibility), typeof(UserPresenterViewModel), new PropertyMetadata(Visibility.Collapsed));

        public Visibility CopyIdButtonVisibility
        {   
            get => (Visibility)GetValue(CopyIdButtonVisibilityProperty);
            set => SetValue(CopyIdButtonVisibilityProperty, value);
        }
        public static readonly DependencyProperty CopyIdButtonVisibilityProperty =
            DependencyProperty.Register("CopyIdButtonVisibility", typeof(Visibility), typeof(UserPresenterViewModel), new PropertyMetadata(Visibility.Collapsed));

        #endregion

        #region Commands

        public ICommand CopyIdCommand => new DelegateCommand(parameter =>
        {
            // this method can be call only if we`re connected
            Clipboard.CopyToClipboard(UserId);
        });

        public ICommand MouseOverHandlerCommand => new DelegateCommand(parameter =>
        {
            // if we`re not connected, then return
            var isConnected = _state.GetState<bool>(Constants.ZCLIENT_CONNECTION);
            if (! isConnected) return;

            // switch visibilities
            UserIdVisibility = UserIdVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            CopyIdButtonVisibility = CopyIdButtonVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        });

        #endregion

        #region Public methods

        public void SetUserData(ZUser user) => Dispatcher.Invoke(() =>
        {
            // setup unauthorized user
            if (user == null)
            {
                // user localized string here
                UserName = "Unauthorized user";
                UserId = string.Empty;
            }
            else // setup user
            {
                UserName = user.Name;
                UserId = user.Id.ToString();
            }
        });

        #endregion
    }
}
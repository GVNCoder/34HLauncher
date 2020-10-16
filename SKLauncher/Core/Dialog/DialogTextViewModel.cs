using System.Windows;
using System.Windows.Input;
using Launcher.Core.Interaction;

namespace Launcher.Core.Dialog
{
    public class DialogTextViewModel : BaseDialogViewModel
    {
        public DialogTextViewModel(string title, string content, DialogButtons buttons, bool showCheckbox)
        {
            Title = title;
            Content = content;

            OkButtonVisibility = buttons.HasFlag(DialogButtons.Ok) ? Visibility.Visible : Visibility.Collapsed;
            NoButtonVisibility = buttons.HasFlag(DialogButtons.No) ? Visibility.Visible : Visibility.Collapsed;
            CancelButtonVisibility = buttons.HasFlag(DialogButtons.Cancel) ? Visibility.Visible : Visibility.Collapsed;
            CheckboxVisibility = showCheckbox ? Visibility.Visible : Visibility.Collapsed;
        }

        public string Title { get; }
        public string Content { get; }

        #region Bindable properties

        public bool DontAskFlag
        {
            get => (bool)GetValue(DontAskFlagProperty);
            set => SetValue(DontAskFlagProperty, value);
        }
        public static readonly DependencyProperty DontAskFlagProperty =
            DependencyProperty.Register("DontAskFlag", typeof(bool), typeof(DialogTextViewModel), new PropertyMetadata(false));

        #endregion

        public Visibility OkButtonVisibility { get; }
        public Visibility NoButtonVisibility { get; }
        public Visibility CancelButtonVisibility { get; }
        public Visibility CheckboxVisibility { get; }

        public ICommand OkCommand => new DelegateCommand(obj => Dialog.CloseDialog(DialogAction.Primary, DontAskFlag));
        public ICommand NoCommand => new DelegateCommand(obj => Dialog.CloseDialog(DialogAction.Declined, DontAskFlag));
        public ICommand CancelCommand => new DelegateCommand(obj => Dialog.CloseDialog(DialogAction.Cancel, DontAskFlag));
    }
}
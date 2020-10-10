using System.Windows.Controls;

namespace Launcher.Core.Dialog
{
    public interface IDialogControlManager
    {
        void Show(UserControl content);
        void Close();
    }
}
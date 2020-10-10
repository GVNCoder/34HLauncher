using System.Windows.Controls;
using Launcher.Core.Service.Base;

namespace Launcher.Core.Dialog
{
    public interface IDialogSystemBase : IUIHostDependency
    {
        void Show(UserControl content);
        void Close();
    }
}
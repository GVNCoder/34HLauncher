using System.Windows;

namespace Launcher.Core.Shared
{
    public class BaseDialogViewModel : DependencyObject
    {
        public DialogControlHelper Dialog { get; set; }
    }
}
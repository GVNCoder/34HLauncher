using System.Windows;

namespace Launcher.Core.Shared
{
    public class BusyViewModel : BaseDialogViewModel
    {
        public string Title { get; }

        public BusyViewModel(string title)
        {
            Title = title;
        }
    }
}
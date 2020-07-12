using Microsoft.Win32;

namespace Launcher.Helpers
{
    public static class SettingsDialogHelper
    {
        public static OpenFileDialog BuildOpenFileDialog(string initialDirectory, string title, string filter)
        {
            return new OpenFileDialog
            {
                InitialDirectory = initialDirectory,
                Title = title,
                Filter = filter,
                Multiselect = false,
                CheckFileExists = true,
                CheckPathExists = true,
                FileName = string.Empty
            };
        }
    }
}
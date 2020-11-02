using System.Threading.Tasks;

namespace Launcher.Core.Dialog
{
    public class Dialog
    {
        private readonly TaskCompletionSource<DialogResult?> _completionSource;
        private readonly IDialogSystemBase _dialogSystem;

        public Dialog(TaskCompletionSource<DialogResult?> completionSource, IDialogSystemBase dialogSystem)
        {
            _completionSource = completionSource;
            _dialogSystem = dialogSystem;
        }

        public void CloseDialog(DialogAction action, object result = null)
        {
            // create result
            var resultObject = new DialogResult { Action = action, Result = result };

            // close dialog
            _dialogSystem.Close();
            _completionSource.TrySetResult(resultObject);
        }
    }
}
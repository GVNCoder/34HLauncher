using System.Threading;
using System.Threading.Tasks;
using Launcher.XamlThemes.Controls.Shared;

namespace Launcher.Core.Shared
{
    public class DialogControlHelper
    {
        private readonly IOverlayControl _control;
        private readonly TaskCompletionSource<DialogResult> _dialogAwaiter;

        public DialogControlHelper(IOverlayControl control, TaskCompletionSource<DialogResult> dialogAwaiter)
        {
            _control = control;
            _dialogAwaiter = dialogAwaiter;
        }

        public DialogResult DialogResult { get; } = new DialogResult();

        public void SetResult<TResult>(TResult result, DialogActionEnum action)
        {
            DialogResult.Action = action;
            DialogResult.Result = result;

            Close();
        }

        public void Close()
        {
            _dialogAwaiter.SetResult(DialogResult);
            _control.Hide();
        }
    }
}
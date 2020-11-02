namespace Launcher.Core.Dialog
{
    public struct DialogResult
    {
        public DialogAction Action { get; set; }
        public object Result { get; set; }

        public TResult GetResult<TResult>() => (TResult) Result;
    }
}
namespace Launcher.Core
{
    public class BusyIndicatorService : IBusyIndicatorService
    {
        private readonly IBusyIndicatorBase _busyIndicatorBase;
        private readonly string _tmpDefaultText = "Please wait...";

        private bool _isOpen;

        public BusyIndicatorService(
            IBusyIndicatorBase busyIndicatorBase)
        {
            _busyIndicatorBase = busyIndicatorBase;
        }

        #region IBusyIndicatorService

        public void Close()
        {
            if (! _isOpen) return;

            // pass call
            _busyIndicatorBase.Close();
            _isOpen = false;
        }

        public void Open(string title = null)
        {
            if (_isOpen) return;

            // pass call
            _busyIndicatorBase.Open(title ?? _tmpDefaultText);
            _isOpen = true;
        }

        #endregion
    }
}
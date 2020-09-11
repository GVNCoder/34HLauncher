namespace Launcher.Core.Service
{
    public interface IViewModelSource
    {
        #region Properties

        IControlViewModelLocator ControlLocator { get; }
        IPageViewModelLocator PageLocator { get; }

        #endregion

        TViewModel Create<TViewModel>() where TViewModel : class;
    }
}
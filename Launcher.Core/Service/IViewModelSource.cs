using Launcher.Core.Service.Base;

namespace Launcher.Core.Service
{
    /// <summary>
    /// 
    /// </summary>
    public interface IViewModelSource
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TViewModel"></typeparam>
        /// <returns></returns>
        TViewModel Create<TViewModel>() where TViewModel : BaseViewModel;
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TViewModel"></typeparam>
        /// <returns></returns>
        TViewModel Get<TViewModel>() where TViewModel : BaseViewModel;
    }
}
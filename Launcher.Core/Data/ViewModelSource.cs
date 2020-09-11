using System;
using System.Linq;

using Launcher.Core.Service;
using Launcher.Core.Service.Base;

using Ninject;
using Ninject.Infrastructure.Language;

namespace Launcher.Core.Data
{
    public class ViewModelSource : IViewModelSource
    {
        private const string _vm_base_name = nameof(BaseViewModel);
        private readonly IResolver _resolver;

        public ViewModelSource(IResolver resolver)
        {
            // assign instances
            _resolver = resolver;

            // resolve dependencies
            ControlLocator = _resolver.Core.Get<IControlViewModelLocator>();
            PageLocator    = _resolver.Core.Get<IPageViewModelLocator>();
        }

        #region IViewModelSource

        /// <inheritdoc />
        public IControlViewModelLocator ControlLocator { get; }

        /// <inheritdoc />
        public IPageViewModelLocator PageLocator { get; }

        /// <inheritdoc />
        public TViewModel Create<TViewModel>() where TViewModel : class
        {
            // extract original type
            var type = typeof(TViewModel);

            // get base type
            var baseType = type.GetAllBaseTypes()
                .FirstOrDefault(t => t.Name == _vm_base_name);

            // check type
            if (baseType == null) throw new InvalidOperationException("You are trying to get an instance of a class that is not a representative of the BaseViewModel.");

            // resolve
            return _resolver.Core.Get<TViewModel>();
        }

        #endregion
    }
}
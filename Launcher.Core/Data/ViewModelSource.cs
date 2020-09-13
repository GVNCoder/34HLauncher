using System;
using System.Linq;

using Launcher.Core.Service;
using Launcher.Core.Service.Base;

using Ninject;
using Ninject.Infrastructure.Language;
using Ninject.Syntax;

namespace Launcher.Core.Data
{
    public class ViewModelSource : IViewModelSource
    {
        private const string _vm_base_name = nameof(BaseViewModel);
        private readonly IResolutionRoot _resolver;

        public ViewModelSource(IResolutionRoot resolver)
        {
            // assign instances
            _resolver = resolver;

            // resolve dependencies
            ControlLocator = _resolver.Get<IControlViewModelLocator>();
            PageLocator    = _resolver.Get<IPageViewModelLocator>();
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
            return _resolver.Get<TViewModel>();
        }

        #endregion
    }
}
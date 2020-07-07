using System;

namespace Launcher.Helpers
{
    public interface IErrorHandler
    {
        void HandleError(Exception ex);
    }
}
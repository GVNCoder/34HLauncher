using System;
using System.Threading.Tasks;

namespace Launcher.Core.Services
{
    public interface IBusyService
    {
        void Busy(string title);
        void Free();

        Task BusyWhile(string title, Action<object> whileOperation, object state);
    }
}
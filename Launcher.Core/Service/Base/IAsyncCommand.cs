using System.Threading.Tasks;
using System.Windows.Input;

namespace Launcher.Core.Service.Base
{
    public interface IAsyncCommand : ICommand
    {
        Task ExecuteAsync(object parameter);
    }
}
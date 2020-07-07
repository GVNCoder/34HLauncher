using System.Threading.Tasks;
using System.Windows.Controls;

namespace Launcher.Core.Services.Dialog
{
    public interface IContentPresenterService
    {
        Task Show<TUserContent>(object viewModel) where TUserContent : UserControl, new();
    }
}
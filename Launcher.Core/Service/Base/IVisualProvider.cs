using System.Windows.Media;
using Launcher.Core.Data;

namespace Launcher.Core.Service.Base
{
    /// <summary>
    /// 
    /// </summary>
    public interface IVisualProvider
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        Visual GetVisualContent(VisualContext context);
    }
}
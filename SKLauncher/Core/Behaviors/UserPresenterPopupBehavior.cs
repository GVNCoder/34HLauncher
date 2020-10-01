using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Interactivity;

namespace Launcher.Core.Behaviors
{
    public class UserPresenterPopupBehavior : Behavior<Popup>
    {
        protected override void OnAttached()
        {
            AssociatedObject.CustomPopupPlacementCallback = _CalculatePopupPlacement;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.CustomPopupPlacementCallback = null;
        }

        #region Private helpers

        private CustomPopupPlacement[] _CalculatePopupPlacement(Size popupSize, Size targetSize, Point offset)
        {
            return null;
        }

        #endregion
    }
}
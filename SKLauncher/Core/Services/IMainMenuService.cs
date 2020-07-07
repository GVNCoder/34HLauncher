namespace Launcher.Core.Services
{
    /// <summary>
    /// Defines a main menu service.
    /// </summary>
    public interface IMainMenuService
    {
        /// <summary>
        /// Shows main menu
        /// </summary>
        void Show();
        /// <summary>
        /// Closes main menu
        /// </summary>
        void Close();
        /// <summary>
        /// Toggles main menu visibility
        /// </summary>
        void Toggle();

        bool CanUse { get; }
    }
}
using System.Windows;
using System;
using System.IO;
using System.Reflection;

using log4net;
using Ninject;

using Launcher.Core.Injection;
using Launcher.Core.Services;
using Launcher.Core.Shared;
using Launcher.Helpers;
using Launcher.Localization.Loc;

[assembly: AssemblyVersion("0.120.1219.0")]
[assembly: AssemblyFileVersion("0.120.1219.0")]

namespace Launcher
{
    /// <inheritdoc cref="Application" />
    public partial class App : Application
    {
        /// <summary>
        /// Settings service
        /// </summary>
        public ISettingsService SettingsService { get; }
        /// <summary>
        /// Dependency resolver
        /// </summary>
        public IDependencyResolver DependencyResolver { get; }
        /// <summary>
        /// Launcher process service
        /// </summary>
        public ILauncherProcessService ProcessService { get; }
        /// <summary>
        /// Application logger instance
        /// </summary>
        public ILog Logger { get; }

        public App()
        {
            DependencyResolver = new DependencyResolver();
            ProcessService = new LauncherProcessService();

            Logger = DependencyResolver.Resolver.Get<ILog>();
            SettingsService = DependencyResolver.Resolver.Get<ISettingsService>();

            VersionService.SetVersion(new LauncherVersion("beta"));
        }

        #region App startup/exit

        protected override void OnStartup(StartupEventArgs e)
        {
            ProcessService.HandleProcessInstance(this, e.Args);

            base.OnStartup(e);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(_logUnhandledExceptions);

            Resources["ViewModelLocator"] = DependencyResolver.Locators.ViewModelLocator;
            Resources["UserControlViewModelLocator"] = DependencyResolver.Locators.UserControlViewModelLocator;

            _SetupVars();
            _GenerateDirs();

            SettingsService.LoadLauncherSettings();
            SettingsService.LoadGameSettings();

            var settings = SettingsService.GetLauncherSettings();

            LocManager.Initialize(this);
            LocManager.SetLocale(settings.Localization);

            XamlThemes.Theming.ThemeManager.Initialize(this);
            XamlThemes.Theming.ThemeManager.ApplyAccent(settings.Accent);
            XamlThemes.Theming.ThemeManager.ApplyTheme(settings.Theme);
            XamlThemes.Theming.ThemeManager.ApplyBackgroundImage();

            ProcessService.HandleUpdate(settings, e.Args);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            // call base implementation
            base.OnExit(e);

            var saveResult = SettingsService.Save();
            if (!saveResult)
            {
                Logger.Warn("Cannot save launcher settings.");
            }

            if (e.ApplicationExitCode != 0)
            {
                Logger.Warn($"Specified {e.ApplicationExitCode} application exit code.");
            }
        }

        #endregion // App startup/exit

        private static void _SetupVars()
        {
            State.Storage["connection"] = false;
        }

        private static void _GenerateDirs()
        {
            Directory.CreateDirectory(FolderConstant.UpdateFolder);
            Directory.CreateDirectory(FolderConstant.ResourcesFolder);
            Directory.CreateDirectory(FolderConstant.LogFolder);
            Directory.CreateDirectory(FolderConstant.SettingsFolder);
        }

        private void _logUnhandledExceptions(object sender, UnhandledExceptionEventArgs e)
        {
            var logMessage = LoggingHelper.GetMessage((Exception) e.ExceptionObject);

            if (e.IsTerminating)
            {
                Logger.Fatal(logMessage);
            }
            else
            {
                Logger.Error(logMessage);
            }
        }
    }
}

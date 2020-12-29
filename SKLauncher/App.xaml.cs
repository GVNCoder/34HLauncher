using System.Windows;
using System;
using System.IO;
using System.Reflection;

using log4net;
using Launcher.Core.Data;
using Launcher.Core.Service;
using Ninject;

using Launcher.Core.Services;
using Launcher.Core.Shared;
using Launcher.Helpers;
using Launcher.Localization.Loc;

using Zlo4NET.Api;
using Zlo4NET.Api.Models.Shared;
using Zlo4NET.Core.Data;

[assembly: AssemblyVersion("0.124.5.2")]

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
        /// Launcher process service
        /// </summary>
        public ILauncherProcessService ProcessService { get; }
        /// <summary>
        /// Application logger instance
        /// </summary>
        public ILog Logger { get; }

        public App()
        {
            // initialize DI container
            Resolver.Create();

            ProcessService = new LauncherProcessService();

            Logger = Resolver.Kernel.Get<ILog>();
            SettingsService = Resolver.Kernel.Get<ISettingsService>();

            VersionService.SetVersion(new LauncherVersion("beta"));
        }

        #region App startup/exit

        protected override void OnStartup(StartupEventArgs e)
        {
            ProcessService.HandleProcessInstance(this, e.Args);

            base.OnStartup(e);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(_logUnhandledExceptions);

            // assign instance into Xaml
            Resources["ViewModelLocator"] = Resolver.Kernel.Get<IViewModelSource>();

            // Old code
            _SetupDirectories();
            _SetupLoggers(ZApi.Instance, Logger);

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
            if (! saveResult)
            {
                Logger.Warn("Cannot save launcher settings.");
            }

            if (e.ApplicationExitCode != 0)
            {
                Logger.Warn($"Specified {e.ApplicationExitCode} application exit code.");
            }
        }

        #endregion // App startup/exit

        private static void _SetupDirectories()
        {
            Directory.CreateDirectory(FolderConstant.UpdateFolder);
            Directory.CreateDirectory(FolderConstant.ResourcesFolder);
            Directory.CreateDirectory(FolderConstant.LogFolder);
            Directory.CreateDirectory(FolderConstant.SettingsFolder);
        }

        private static void _SetupLoggers(IZApi instance, ILog applicationLoggerInstance)
        {
            // setup API logger
            var logger = instance.Logger;
            logger.SetMessageFilter(ZLogLevel.Error | ZLogLevel.Warning);
            logger.OnMessage += (sender, e) => applicationLoggerInstance.Info($"Zlo4NET: {e.Message}");

            // set other loggers
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

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
using Launcher.XamlThemes.Theming;

using Zlo4NET.Api;
using Zlo4NET.Api.Models.Shared;
using Zlo4NET.Core.Data;

[assembly: AssemblyVersion("0.125.1.3")]

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

            // track some events
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(_logUnhandledExceptions);
            ThemeManager.Error                         += new EventHandler<Exception>(_logThemeExceptions);

            // assign instance into Xaml
            Resources["ViewModelLocator"] = Resolver.Kernel.Get<IViewModelSource>();

            _SetupDirectories();
            _SetupLoggers(ZApi.Instance, Logger);

            SettingsService.Load();

            var settings = SettingsService.Current;

            // apply settings
            LocManager.Initialize(this);
            LocManager.SetLocale(settings.DataLocalization);

            ThemeManager.Initialize(Resources);
            ThemeManager.LoadCustomResourcesIfExists();
            ThemeManager.ApplyAccent(AccentEnum.OrangeRed);
            ThemeManager.ApplyTheme(settings.DataTheme);

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

        private void _logThemeExceptions(object sender, Exception exception)
        {
            // build message
            var logMessage = LoggingHelper.GetMessage(exception);

            // log message
            Logger.Error(logMessage);
        }
    }
}

﻿using System.Windows;

using log4net;

using Launcher.Core;
using Launcher.Core.Data;
using Launcher.Core.Data.Updates;
using Launcher.Core.Dialog;
using Launcher.Core.RPC;
using Launcher.Core.Service;
using Launcher.Core.Service.Base;
using Launcher.Core.Services;
using Launcher.Core.Services.Updates;

using Ninject.Modules;

using Zlo4NET.Api;
using Zlo4NET.Core.Data;

using IDiscord = Launcher.Core.RPC.IDiscord;

namespace Launcher.Data
{
    public class ContainerModule : NinjectModule
    {
        public override void Load()
        {
            Kernel.Bind<IUIHostService>()
                .To<UIHostService>();
            Kernel.Bind<IVersionService>()
                .To<VersionService>();
            Kernel.Bind<INetworkAvailabilityTracker>()
                .To<NetworkAvailabilityTracker>();
            Kernel.Bind<IMainMenuService>()
                .To<MainMenuService>();
            Kernel.Bind<ISettingsService>()
                .To<SettingsService>()
                .InSingletonScope();
            Kernel.Bind<IProcessService>()
                .To<ProcessService>()
                .InSingletonScope();

            Kernel.Bind<IUpdateService>()
                .To<UpdateService>()
                .InSingletonScope();
            Kernel.Bind<IGameService>()
                .To<GameService>()
                .InSingletonScope();
            Kernel.Bind<IDiscord>()
                .To<Discord>()
                .InSingletonScope();

            var application = (App)Application.Current;

            Kernel.Bind<IZApi>()
                .ToConstant(ZApi.Instance);
            Kernel.Bind<ILauncherProcessService>()
                .ToConstant(application.ProcessService);
            Kernel.Bind<ILog>()
                .ToConstant(LogManager.GetLogger(typeof(App)));
            Kernel.Bind<App>()
                .ToConstant(application);

            // Release

            // Add singleton
            Kernel.Bind<IViewModelSource>().To<ViewModelLocator>()
                .InSingletonScope();
            Kernel.Bind<IPageNavigator>().To<PageNavigator>()
                .InSingletonScope();
            Kernel.Bind<IApplicationState>().To<ApplicationState>()
                .InSingletonScope();
            Kernel.Bind<IVisualProvider>().To<VisualProvider>()
                .InSingletonScope();
            Kernel.Bind<IDialogSystemBase>().To<DialogControlManager>()
                .InSingletonScope();
            Kernel.Bind<IDialogService>().To<DialogService>()
                .InSingletonScope();
            Kernel.Bind<IBusyIndicatorBase>().To<BusyIndicatorControlManager>()
                .InSingletonScope();
            Kernel.Bind<IBusyIndicatorService>().To<BusyIndicatorService>()
                .InSingletonScope();
            Kernel.Bind<IEventService>().To<EventService>()
                .InSingletonScope();
        }
    }
}
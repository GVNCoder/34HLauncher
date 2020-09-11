using Launcher.Core.Data;
using Launcher.Core.Data.Dialog;
using Launcher.Core.Data.EventLog;
using Launcher.Core.Data.Updates;
using Launcher.Core.RPC;
using Launcher.Core.Service;
using Launcher.Core.Services;
using Launcher.Core.Services.Dialog;
using Launcher.Core.Services.EventLog;
using Launcher.Core.Services.Updates;
using Ninject.Modules;
using IDiscord = Launcher.Core.RPC.IDiscord;

namespace Launcher.Core.Injection
{
    public class ServiceModule : NinjectModule
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
            Kernel.Bind<IBusyService>()
                .To<BusyService>()
                .InSingletonScope();
            Kernel.Bind<ISettingsService>()
                .To<SettingsService>()
                .InSingletonScope();
            Kernel.Bind<IProcessService>()
                .To<ProcessService>()
                .InSingletonScope();
            Kernel.Bind<IOverlayDialogService>()
                .To<OverlayDialogService>()
                .InSingletonScope();
            Kernel.Bind<ITextDialogService>()
                .To<TextDialogService>()
                .InSingletonScope();
            Kernel.Bind<IContentPresenterService>()
                .To<ContentPresenterService>()
                .InSingletonScope();
            Kernel.Bind<IEventLogService>()
                .To<EventLogService>()
                .InSingletonScope();
            Kernel.Bind<IUpdateService>()
                .To<UpdateService>()
                .InSingletonScope();
            Kernel.Bind<IGameService>()
                .To<GameService>()
                .InSingletonScope();
            Kernel.Bind<IDiscordPresence>()
                .To<DiscordPresence>();
            Kernel.Bind<IDiscord>()
                .To<Discord>()
                .InSingletonScope();
            
            
            // Add singleton
            Kernel.Bind<IPageNavigator>().To<PageNavigator>()
                .InSingletonScope();
            Kernel.Bind<IApplicationState>().To<ApplicationState>()
                .InSingletonScope();
        }
    }
}
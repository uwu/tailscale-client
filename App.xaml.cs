using System;
using Microsoft.UI.Xaml;
using Velopack;

namespace TailscaleClient;

public partial class App : Application
{
    public static MainWindow MainWindow { get; set; }

    public static bool CanCloseWindow { get; set; }

    public App()
    {
        VelopackApp.Build().Run();
        InitializeComponent();

        var mgr = new UpdateManager("https://tsc.xirreal.dev");
        
        try
        {
            var newVersion = mgr.CheckForUpdates();
            if (newVersion == null)
            {
                return;
            }

            mgr.DownloadUpdates(newVersion);
            mgr.ApplyUpdatesAndRestart(newVersion);
        } catch (Exception e)
        {
            // TODO: Show failed update bar, possibly a badge on settings?
            return;
        }
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        var initSuccess = Core.API.Init();
        Core.Utils.InitializeColors();

        MainWindow = new MainWindow
        {
            Content = initSuccess ? new Views.AppSkeleton() : new Views.Error()
        };

        MainWindow.Activate();
    }

    public static new void Exit()
    {
        Current.Exit();
    }
}

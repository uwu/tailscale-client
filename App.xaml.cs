using Microsoft.UI.Xaml;

namespace TailscaleClient;

public partial class App : Application
{
    public static MainWindow MainWindow { get; set; }

    public static bool CanCloseWindow { get; set; }

    public App()
    {
        InitializeComponent();
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

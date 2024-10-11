using Microsoft.UI.Xaml;

namespace TailscaleClient;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
    }

    protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
    {
        var initSuccess = Core.API.Init();
        Core.Utils.InitializeColors();

        var window = Core.Windowing.CreateWindow();

        // WIP: I don't know if i like this.
        var appWindow = Core.Windowing.GetAppWindowFromWindow(window);
        //var presenter = appWindow.Presenter as OverlappedPresenter;
        //presenter.IsResizable = false;
        // TODO: Maybe default size based on DPI/scaling?
        appWindow.Resize(new Windows.Graphics.SizeInt32(600, 1000));
        // TODO: Minimum size is hellish on WinUI 3

        window.ExtendsContentIntoTitleBar = true;
        window.Title = "Tailscale";
        window.Content = initSuccess ? new Views.AppSkeleton() : new Views.Error();

        window.Activate();
    }
}

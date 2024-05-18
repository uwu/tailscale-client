using Microsoft.UI.Windowing;
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
        Core.API.Init();
        Core.Utils.InitializeColors();

        var window = Core.Windowing.CreateWindow();

        var appWindow = Core.Windowing.GetAppWindowFromWindow(window);
        var presenter = appWindow.Presenter as OverlappedPresenter;
        presenter.IsResizable = false;
        // TODO: Maybe dynamic size based on DPI/scaling?
        appWindow.Resize(new Windows.Graphics.SizeInt32(600, 1000));

        window.ExtendsContentIntoTitleBar = true;
        window.Title = "Tailscale";
        window.Content = new Views.AppSkeleton();

        window.Activate();
    }
}

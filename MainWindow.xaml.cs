using Microsoft.UI.Xaml;
using WinUIEx;

namespace TailscaleClient;

public sealed partial class MainWindow : WindowEx
{
    public MainWindow()
    {
        InitializeComponent();

        Title = Constants.AppDisplayName;
        var frameworkElement = (FrameworkElement)Content;
        frameworkElement.ActualThemeChanged += Content_ActualThemeChanged;
        Content_ActualThemeChanged(frameworkElement, null);

        ExtendsContentIntoTitleBar = true;
    }

    private void Content_ActualThemeChanged(FrameworkElement sender, object args)
    {
        if (sender.ActualTheme == ElementTheme.Light)
        {
            AppWindow.SetIcon(Constants.AppIconLightAbsolutePath);
        }
        else
        {
            AppWindow.SetIcon(Constants.AppIconDarkAbsolutePath);
        }
    }

    private void WindowEx_Closed(object sender, WindowEventArgs args)
    {
        if (App.CanCloseWindow)
        {
            App.Exit();
        }
        else
        {
            args.Handled = true;
            this.Hide();
        }
    }
}

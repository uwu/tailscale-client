using Microsoft.UI.Xaml.Controls;

namespace TailscaleClient.Views;
public sealed partial class Error : Page
{
    public Error()
    {
        this.InitializeComponent();

        Loaded += (sender, e) =>
        {
            var window = Core.Windowing.GetCurrentWindow(this);
            window.SetTitleBar(AppTitleBar);
        };
    }
}

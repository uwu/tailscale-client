using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace TailscaleClient.Views;

public sealed partial class Error : Page
{
    public Error()
    {
        InitializeComponent();
    }

    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
        App.MainWindow.SetTitleBar(AppTitleBar);
    }
}

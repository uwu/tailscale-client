using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;

namespace TailscaleClient.Views;

public sealed partial class AppSkeleton : Page, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;
    public void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private string _loadingMessage = "Loading...";
    public string LoadingMessage
    {
        get => _loadingMessage;
        set
        {
            _loadingMessage = value;
            OnPropertyChanged();
        }
    }

    public AppSkeleton()
    {
        InitializeComponent();

        if (Core.API.GetStatus().BackendState == "NeedsLogin")
        {
            NavigationViewControl.SelectedItem = NavigationViewControl.MenuItems[1];
            ContentFrame.Navigate(typeof(Accounts), null, new SuppressNavigationTransitionInfo());
        }
        else
        {
            NavigationViewControl.SelectedItem = NavigationViewControl.MenuItems[0];
            ContentFrame.Navigate(typeof(Home), null, new SuppressNavigationTransitionInfo());
        }

        Loaded += (sender, e) =>
        {
            var window = Core.Windowing.GetCurrentWindow(this);
            window.SetTitleBar(AppTitleBar);
        };
    }

    private void OnNavigate(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        var selectedItem = (NavigationViewItem)args.SelectedItem;
        System.Type whereTo = null;

        switch (selectedItem.Tag)
        {
            case "Home":
                whereTo = typeof(Home);
                break;
            case "Accounts":
                whereTo = typeof(Accounts);
                break;
            case "Settings":
                whereTo = typeof(Settings);
                break;
        }

        if (whereTo != null)
        {
            ContentFrame.Navigate(whereTo, null, args.RecommendedNavigationTransitionInfo);
        }
    }

    public void DisableUI()
    {
        DisableUI("Loading...");
    }

    public void DisableUI(string message)
    {
        NavigationViewControl.IsEnabled = false;
        ContentFrame.IsEnabled = false;

        LoadingMessage = message;

        LoadingBackground.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
    }

    public void EnableUI()
    {
        NavigationViewControl.IsEnabled = true;
        ContentFrame.IsEnabled = true;

        LoadingBackground.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
    }
}

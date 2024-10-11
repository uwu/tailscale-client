using System.Collections.Generic;
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

        var status = Core.API.GetStatus();
        if (status.BackendState == "NeedsLogin")
        {
            NavigationViewControl.SelectedItem = NavigationViewControl.MenuItems[1];
            ContentFrame.Navigate(typeof(Accounts), null, new SuppressNavigationTransitionInfo());
        }
        else
        {
            NavigationViewControl.SelectedItem = NavigationViewControl.MenuItems[0];
            ContentFrame.Navigate(typeof(Home), null, new SuppressNavigationTransitionInfo());
        }

        Core.Messaging.Instance.MessageReceived += (sender, e) =>
        {
            if (e.Kind == Core.Messaging.MessageKind.HealthUpdate)
            {
                var warnings = Core.Types.Warning.ParseWarningsFromJson(e.Value);
                DispatcherQueue.TryEnqueue(() => UpdateWarnings(warnings));
            }
        };

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

    public void UpdateWarnings(List<Core.Types.Warning> warnings)
    {
        if (warnings.Count > 0)
        {
            InfoBarContent.Children.Clear();
            foreach (var warning in warnings)
            {
                InfoBarContent.Children.Add(warning.GetWarningComponent());
            }
            InfoBar.IsOpen = true;
            InfoBar.Visibility = Microsoft.UI.Xaml.Visibility.Visible;
            InfoBar.Title = $"{warnings.Count} warning{(warnings.Count != 1 ? "s" : "")}";
        }
        else
        {
            HideInfoBar();
        }
    }

    private void HideInfoBar()
    {
        InfoBar.IsOpen = false;
        InfoBarContent.Children.Clear();
        InfoBar.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
    }

    private void HideInfoBar(InfoBar _, object __)
    {
        HideInfoBar();
    }
}

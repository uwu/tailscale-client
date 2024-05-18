using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using TailscaleClient.Core;

namespace TailscaleClient.Views;

public sealed partial class Home : Page, INotifyPropertyChanged
{
    private Core.Types.Status _status = null;
    private Core.Types.Profile _profile = null;

    public event PropertyChangedEventHandler PropertyChanged;
    public void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private string _username = "Guest";
    public string UserName
    {
        get => _username;
        set
        {
            _username = value;
            OnPropertyChanged();
        }
    }

    private string _profilePicture = "ms-appx:///Assets/Misc/Guest.png";
    public string ProfilePicture
    {
        get => _profilePicture;
        set
        {
            _profilePicture = value;
            OnPropertyChanged();
        }
    }

    private string _amIConnected = "Connected as";
    public string AmIConnected
    {
        get => _amIConnected;
        set
        {
            _amIConnected = value;
            OnPropertyChanged();
        }
    }

    private string UserID = "";

    private void UpdateUI()
    {
        _status = API.GetStatus();
        _profile = API.GetCurrentUser();

        MyDevice.Children.Clear();
        OtherDevices.Children.Clear();

        UserID = _status.Self.UserID.ToString();
        UserName = _profile.UserProfile.DisplayName;
        var pfp = _profile.UserProfile.ProfilePicURL;

        // the spacing really bothered me.
        if (string.IsNullOrEmpty(UserName))
        {
            UserNameBlock.Margin = new(-4, 5, 12, 0);
        }
        else
        {
            UserNameBlock.Margin = new(4, 5, 12, 0);
        }

        if (Uri.TryCreate(pfp, UriKind.Absolute, out _))
        {
            ProfilePicture = pfp;
        }

        if (_status.BackendState != "Running")
        {
            AmIConnected = _status.BackendState switch
            {
                "Starting" => "Attempting to connect as",
                "NeedsLogin" => "Login required",
                _ => "Disconnected, logged in as"
            };

            MyDevice.Children.Add(new Assets.DeviceCard());
            return;
        }

        AmIConnected = "Connected as";

        MyDevice.Children.Add(new Assets.DeviceCard(_status.Self));

        foreach ((_, var device) in _status?.Peer ?? [])
        {
            OtherDevices.Children.Add(new Assets.DeviceCard(device));
        }
    }

    public Home()
    {
        InitializeComponent();

        Messaging.Instance.MessageReceived += (sender, e) =>
        {
            if (e.Kind == Messaging.MessageKind.IPNBusUpdate && (e.Key == "NetMap" || e.Key == "State"))
            {
                DispatcherQueue.TryEnqueue(() => { UpdateUI(); });
            }
            else if (e.Kind == Messaging.MessageKind.ProfileSwitch)
            {
                DispatcherQueue.TryEnqueue(() => { UpdateUI(); });
            }
        };

        UpdateUI();
    }

    private void FilterDevices(bool show)
    {
        foreach (var child in OtherDevices.Children)
        {
            if (child is Assets.DeviceCard device && device.OwnedBy != UserID)
            {
                if (show)
                {
                    device.Show();
                }
                else
                {
                    device.Hide();
                }
            }
        }
    }

    private void Hide(object s, RoutedEventArgs e) => FilterDevices(false);
    private void Show(object s, RoutedEventArgs e) => FilterDevices(true);
}

using System;
using System.Threading.Tasks;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media;
using Windows.ApplicationModel.DataTransfer;

namespace TailscaleClient.Assets;

public sealed partial class DeviceCard : UserControl
{
    private static SolidColorBrush GetStatusColor(bool state)
    {
        return Core.Utils.colors[state ? "StatusSuccess" : "StatusFail"];
    }

    private static SolidColorBrush GetStatusTextColor(bool state)
    {
        return Core.Utils.colors[state ? "TextSuccess" : "TextFail"];
    }

    private static string FormatDaysUntilExpiry(int daysUntilExpiry)
    {
        var daysText = daysUntilExpiry switch
        {
            1 => "day",
            _ => "days",
        };

        return daysUntilExpiry switch
        {
            <
                                            0 => "Expired",
            0 => "Key expiring today",
            _ => $"Expiring in {daysUntilExpiry} {daysText}"

        };
    }

    private static Windows.UI.Color FormatColorForExpiry(int daysUntilExpiry)
    {
        return daysUntilExpiry switch
        {
            <=
                0 => Windows.UI.Color.FromArgb(255, 255, 0, 0),
            _ => Windows.UI.Color.FromArgb(255, 255, 150, 0),
        };
    }

    private readonly string Hostname = "";
    private readonly string IPv4 = "";
    private readonly string IPv6 = "";
    private readonly string TailnetDNS = "";
    private readonly string StatusText = "Online";
    private readonly string OSIcon = "ms-appx:///Assets/Platforms/Unknown.png";
    private readonly SolidColorBrush StatusColor = GetStatusColor(true);
    private readonly SolidColorBrush StatusTextColor = GetStatusTextColor(true);

    public readonly string OwnedBy = "-1";

    public DeviceCard()
    {
        InitializeComponent();

        IPv4 = "Unavailable";
        IPv6 = "Unavailable";
        TailnetDNS = "Unavailable";
        Hostname = "Disconnected from this Tailnet";

        StatusText = "Disconnected";
        StatusColor = GetStatusColor(false);
        StatusTextColor = GetStatusTextColor(false);
        OSIcon = "ms-appx:///Assets/Platforms/Unknown.png";
    }

    public void Hide() => Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
    public void Show() => Visibility = Microsoft.UI.Xaml.Visibility.Visible;

    public DeviceCard(Core.Types.PeerInfo device)
    {
        InitializeComponent();

        OwnedBy = device.UserID.ToString();

        var dnsName = device.DNSName;
        var nickname = dnsName.Split(".")[0];
        if (nickname == "")
        {
            nickname = device.HostName;
        }

        if (device.TailscaleIPs != null)
        {
            var ipv4Index = 0;
            if (device.TailscaleIPs[0].Contains(':'))
            {
                ipv4Index = 1;
            }

            IPv4 = device.TailscaleIPs[ipv4Index];
            IPv6 = device.TailscaleIPs[1 - ipv4Index];
        }
        else
        {
            IPv4 = "Unavailable";
            IPv6 = "Unavailable";
        }

        TailnetDNS = dnsName.TrimEnd('.');
        if (TailnetDNS == "")
        {
            TailnetDNS = "Not connected to Tailscale.";
        }
        Hostname = nickname == device.HostName ? nickname : $"{nickname} ({device.HostName})";

        StatusText = device.Online ? "Online" : "Last seen " + device.LastSeen.ToLocalTime().ToString();
        if (device.LastSeen.Year < 2010 && !device.Online)  // Good enough to check if 1970
        {
            StatusText = "Action required";
        }
        StatusColor = GetStatusColor(device.Online);
        StatusTextColor = GetStatusTextColor(device.Online);

        var deviceType = device.OS switch
        {
            "linux" => "Linux",
            "windows" => "Windows",
            "macOS" => "Apple",
            "iOS" => "Apple",
            "android" => "Android",
            _ => "Unknown",
        };
        OSIcon = "ms-appx:///Assets/Platforms/" + deviceType + ".png";

        if (device.KeyExpiry == DateTime.MinValue)
        {
            TagSection.Children.Add(new FeatureTag(
                "Key expiry disabled",
                "The key for this device will never expire: you can change this in the admin panel."));
        }
        else
        {
            var days = (device.KeyExpiry - DateTime.Now).TotalDays;
            if (days <= 14)
            {
                var roundedDays = (int)Math.Floor(days);
                var expiryTag = new FeatureTag(FormatDaysUntilExpiry(roundedDays),
                                               "To refresh the key, reauthenticate on the device.",
                                               FormatColorForExpiry(roundedDays));
                TagSection.Children.Add(expiryTag);
            }
        }

        if (device.Online && device.sshHostKeys != null)
        {
            TagSection.Children.Add(new FeatureTag("SSH", "Tailscale SSH is enabled on this device.",
                                                   Windows.UI.Color.FromArgb(255, 0, 255, 0)));
        }

        if (device.Online && device.PrimaryRoutes != null)
        {
            var routes = "This device exposes subnet routes.\n\n";
            foreach (var route in device.PrimaryRoutes)
            {
                routes += " •  " + route + "\n";
            }

            TagSection.Children.Add(new FeatureTag("Subnets", routes.TrimEnd('\n'),
                                                   Windows.UI.Color.FromArgb(255, 0, 200, 255)));
        }

        if (device.Online && device.ExitNodeOption)
        {
            TagSection.Children.Add(new FeatureTag("Exit node",
                                                   "This device is advertising an exit node.",
                                                   Windows.UI.Color.FromArgb(255, 0, 200, 255)));
        }
    }

    private void CopyToClipboard(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        var text = (sender as TextBlock).Text;
        var dataPackage = new DataPackage();
        dataPackage.SetText(text);
        Clipboard.SetContent(dataPackage);

        var flyout = new Flyout
        {
            Content =
              new TextBlock
              {
                  Text = "Copied to clipboard",
                  FontSize = 14,
              },
            Placement = FlyoutPlacementMode.Bottom,
        };

        flyout.ShowAt(sender as TextBlock);

        Task.Delay(1500).ContinueWith(
            _ => DispatcherQueue.TryEnqueue(delegate () { flyout.Hide(); }));
    }

    private void HoverStart(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        ProtectedCursor = InputSystemCursor.Create(InputSystemCursorShape.Hand);
    }

    private void HoverEnd(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
    {
        ProtectedCursor = InputSystemCursor.Create(InputSystemCursorShape.Arrow);
    }
}

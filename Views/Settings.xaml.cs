using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using TailscaleClient.Assets;
using TailscaleClient.Core;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace TailscaleClient.Views;

public sealed partial class Settings : Page
{
    private Types.MaskedPrefs _prefs = null;

    public bool IncomingTrafficEnabled = false;
    public bool TailscaleDnsEnabled = false;
    public bool SubnetRoutesEnabled = false;
    public string ExitNode = "";

    public void UpdateUI()
    {
        _prefs = API.GetPrefs();
        IncomingTrafficEnabled = !_prefs.ShieldsUp;
        TailscaleDnsEnabled = _prefs.CorpDNS;
        SubnetRoutesEnabled = _prefs.RouteAll;
        ExitNode = _prefs.ExitNodeID;
    }

    public Settings()
    {
        InitializeComponent();

        Messaging.Instance.MessageReceived += (sender, e) =>
        {
            if (e.Kind == Messaging.MessageKind.IPNBusUpdate && e.Key == "Prefs")
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

    private void UpdatePrefsShields(object _sender, RoutedEventArgs e)
    {
        var sender = _sender as ToggleSwitch;
        if(_prefs.ShieldsUp == !sender.IsOn)
        {
            return;
        }

        _prefs.ShieldsUp = !sender.IsOn;

        API.UpdatePrefs(_prefs);
    }

    private void UpdatePrefsDns(object _sender, RoutedEventArgs e)
    {
        var sender = _sender as ToggleSwitch;
        if (_prefs.CorpDNS == sender.IsOn)
        {
            return;
        }
        _prefs.CorpDNS = sender.IsOn;
        API.UpdatePrefs(_prefs);
    }

    private void UpdatePrefsRoutes(object _sender, RoutedEventArgs e)
    {
        var sender = _sender as ToggleSwitch;
        if (_prefs.RouteAll == sender.IsOn)
        {
            return;
        }
        _prefs.RouteAll = sender.IsOn;
        API.UpdatePrefs(_prefs);
    }

    private void ExitNodeChanged(object sender, SelectionChangedEventArgs e)
    {

    }
}

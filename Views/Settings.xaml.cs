using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls;
using TailscaleClient.Core;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace TailscaleClient.Views;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class Settings : Page
{
    private Core.Types.Prefs _prefs = null;

    public bool IncomingTrafficEnabled = false;
    public bool TailscaleDnsEnabled = false;
    public bool SubnetRoutesEnabled = false;

    public void UpdateUI()
    {
        _prefs = API.GetPrefs();
        IncomingTrafficEnabled = !_prefs.ShieldsUp;
        TailscaleDnsEnabled = _prefs.CorpDNS;
        SubnetRoutesEnabled = _prefs.RouteAll;
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

    private void UpdatePrefs(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        _prefs.ShieldsUp = !IncomingTrafficEnabled;
        _prefs.CorpDNS = TailscaleDnsEnabled;
        _prefs.RouteAll = SubnetRoutesEnabled;
        API.EditPrefs(_prefs);
    }
}

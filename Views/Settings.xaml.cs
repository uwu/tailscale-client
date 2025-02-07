using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using TailscaleClient.Assets;
using TailscaleClient.Core;

namespace TailscaleClient.Views;
public sealed partial class Settings : Page, INotifyPropertyChanged
{
    private Types.MaskedPrefs _prefs = null;

    public bool IncomingTrafficEnabled = false;
    public bool TailscaleDnsEnabled = false;
    public bool SubnetRoutesEnabled = false;
    public string ExitNode = "";

    public event PropertyChangedEventHandler PropertyChanged;
    public void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private int _sourceIndex;
    public int CurrentNode
    {
        get => _sourceIndex;
        set
        {
            _sourceIndex = value;
            NotifyPropertyChanged();
        }
    }

    private List<string> _peerIds = [];
    private string _recommenddedNode = "";

    public void UpdateExitNodeList(Types.SuggestedExitNode suggestedPeer, List<Types.PeerInfo> peers)
    {
        ExitNodeList.Items.Clear();

        ExitNodeList.Items.Add(new ComboBoxItem
        {
            Content = "None",
            Tag = ""
        });

        foreach (var device in peers)
        {
            var dnsName = device.DNSName;
            var nickname = dnsName.Split(".")[0];
            if (nickname == "")
            {
                nickname = device.HostName;
            }

            var item = new ComboBoxItem
            {
                Content = $"{(device.ID == suggestedPeer.ID ? "Recommended: " : "")}{(nickname == device.HostName ? nickname : $"{nickname} ({device.HostName})")}",
                Tag = device.ID
            };

            ExitNodeList.Items.Add(item);
        }

        CurrentNode = ExitNodeList.Items.Cast<ComboBoxItem>().ToList().FindIndex(x => x.Tag as string == ExitNode);
    }

    public void UpdateUI()
    {
        _prefs = API.GetPrefs();
        IncomingTrafficEnabled = !_prefs.ShieldsUp;
        TailscaleDnsEnabled = _prefs.CorpDNS;
        SubnetRoutesEnabled = _prefs.RouteAll;
        ExitNode = _prefs.ExitNodeID;

        var suggestedNode = new Types.SuggestedExitNode
        {
            ID = "UNAVAILABLE"
        };

        try
        {
            suggestedNode = API.GetSuggestedExitNode();
        } catch (System.Exception e)
        {
            Debug.WriteLine(e);
        }

        var status = API.GetStatus();
        var peers = (status.Peer ?? []).Select(x => x.Value).Where(x => x.ExitNodeOption).ToList();
        peers.Sort((x, y) => x.ID == suggestedNode.ID ? -1 : y.ID == suggestedNode.ID ? 1 : 0);

        var peerIds = peers.Select(x => x.ID).ToList();
        if(!peerIds.All(x => _peerIds.Contains(x)) || (suggestedNode.ID != "UNAVAILABLE" && suggestedNode.ID != _recommenddedNode))
        {
            _peerIds = peerIds;
            _recommenddedNode = suggestedNode.ID;
            UpdateExitNodeList(suggestedNode, peers);
        }
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
        var selection = (sender as ComboBox).SelectedItem as ComboBoxItem;

        if (selection == null || (selection?.Tag as string) == ExitNode)
        {
            return;
        }

        _prefs.ExitNodeID = selection!.Tag as string;
        API.UpdatePrefs(_prefs);
    }
}

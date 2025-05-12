using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace TailscaleClient.Core;
public class Types
{
    public class PeerInfo
    {
        public string ID
        {
            get; set;
        }
        public string PublicKey
        {
            get; set;
        }
        public string HostName
        {
            get; set;
        }
        public string DNSName
        {
            get; set;
        }
        public string OS
        {
            get; set;
        }
        public long UserID
        {
            get; set;
        }
        public List<string> TailscaleIPs
        {
            get; set;
        }
        public List<string> AllowedIPs
        {
            get; set;
        }
        public List<string> PrimaryRoutes
        {
            get; set;
        }
        public List<string> Addrs
        {
            get; set;
        }
        public string CurAddr
        {
            get; set;
        }
        public string Relay
        {
            get; set;
        }
        public long RxBytes
        {
            get; set;
        }
        public long TxBytes
        {
            get; set;
        }
        public DateTime Created
        {
            get; set;
        }
        public DateTime LastWrite
        {
            get; set;
        }
        public DateTime LastSeen
        {
            get; set;
        }
        public DateTime LastHandshake
        {
            get; set;
        }
        public bool Online
        {
            get; set;
        }
        public bool ExitNode
        {
            get; set;
        }
        public bool ExitNodeOption
        {
            get; set;
        }
        public bool Active
        {
            get; set;
        }
        public List<string> PeerAPIURL
        {
            get; set;
        }
        public List<string> sshHostKeys
        {
            get; set;
        }
        public bool InNetworkMap
        {
            get; set;
        }
        public bool InMagicSock
        {
            get; set;
        }
        public bool InEngine
        {
            get; set;
        }
        public DateTime KeyExpiry
        {
            get; set;
        }
        public object CapMap
        {
            get; set;
        }
    }

    public class SuggestedExitNode
    {
        public string ID
        {
            get; set;
        }

        public string Name
        {
            get; set;
        }

        public Location Location
        {
            get; set;
        }
    }

    public class Location
    {
        public string Country
        {
            get; set;
        }
        public string CountryCode
        {
            get; set;
        }
        public string City
        {
            get; set;
        }

        public string CityCode
        {
            get; set;
        }

        public double Latitude
        {
            get; set;
        }
        public double Longitude
        {
            get; set;
        }

        public int Priority
        {
            get; set;
        }
}

    public class User
    {
        public long ID
        {
            get; set;
        }
        public string LoginName
        {
            get; set;
        }
        public string DisplayName
        {
            get; set;
        }
        public string ProfilePicURL
        {
            get; set;
        }
    }

    public class CurrentTailnet
    {
        public string Name
        {
            get; set;
        }
        public string MagicDNSSuffix
        {
            get; set;
        }
        public bool MagicDNSEnabled
        {
            get; set;
        }
    }

    public class Status
    {
        public string Version
        {
            get; set;
        }
        public bool TUN
        {
            get; set;
        }
        public string BackendState
        {
            get; set;
        }
        public string AuthURL
        {
            get; set;
        }
        public List<string> TailscaleIPs
        {
            get; set;
        }
        public PeerInfo Self
        {
            get; set;
        }
        public List<string> Health
        {
            get; set;
        }
        public string MagicDNSSuffix
        {
            get; set;
        }
        public CurrentTailnet CurrentTailnet
        {
            get; set;
        }
        public List<string> CertDomains
        {
            get; set;
        }
        public Dictionary<string, PeerInfo> Peer
        {
            get; set;
        }
        public Dictionary<string, User> User
        {
            get; set;
        }
        public Dictionary<string, object> ClientVersion
        {
            get; set;
        }
    }

    public class NetworkProfile
    {
        public string MagicDNSName
        {
            get; set;
        }
        public string DomainName
        {
            get; set;
        }
    }

    public class Profile
    {
        public string ID
        {
            get; set;
        }
        public string Name
        {
            get; set;
        }
        public NetworkProfile NetworkProfile
        {
            get; set;
        }
        public string Key
        {
            get; set;
        }
        public User UserProfile
        {
            get; set;
        }
        public string NodeID
        {
            get; set;
        }
        public string LocalUserID
        {
            get; set;
        }
        public string ControlURL
        {
            get; set;
        }
    }

    public class AppConnector
    {
        public bool Advertise
        {
            get; set;
        }
    }

    public class AutoUpdate
    {
        public bool Check
        {
            get; set;
        }
        public object Apply
        {
            get; set;
        }
    }

    public class MaskedPrefs : IJsonOnDeserialized
    {
        private string _controlUrl;
        public bool ControlURLSet
        {
            get; set;
        }
        public string ControlURL
        {
            get => _controlUrl;
            set
            {
                _controlUrl = value;
                ControlURLSet = true;
            }
        }

        private bool _routeAll;
        public bool RouteAllSet
        {
            get; set;
        }
        public bool RouteAll
        {
            get => _routeAll;
            set
            {
                _routeAll = value;
                RouteAllSet = true;
            }
        }



        private string _exitNodeID;
        public bool ExitNodeIDSet
        {
            get; set;
        }
        public string ExitNodeID
        {
            get => _exitNodeID;
            set
            {
                _exitNodeID = value;
                ExitNodeIDSet = true;
            }
        }

        private string _exitNodeIP;
        public bool ExitNodeIPSet
        {
            get; set;
        }
        public string ExitNodeIP
        {
            get => _exitNodeIP;
            set
            {
                _exitNodeIP = value;
                ExitNodeIPSet = true;
            }
        }

        private bool _exitNodeAllowLANAccess;
        public bool ExitNodeAllowLANAccessSet
        {
            get; set;
        }
        public bool ExitNodeAllowLANAccess
        {
            get => _exitNodeAllowLANAccess;
            set
            {
                _exitNodeAllowLANAccess = value;
                ExitNodeAllowLANAccessSet = true;
            }
        }

        private bool _corpDNS;
        public bool CorpDNSSet
        {
            get; set;
        }
        public bool CorpDNS
        {
            get => _corpDNS;
            set
            {
                _corpDNS = value;
                CorpDNSSet = true;
            }
        }

        private bool _runSSH;
        public bool RunSSHSet
        {
            get; set;
        }
        public bool RunSSH
        {
            get => _runSSH;
            set
            {
                _runSSH = value;
                RunSSHSet = true;
            }
        }

        private bool _runWebClient;
        public bool RunWebClientSet
        {
            get; set;
        }
        public bool RunWebClient
        {
            get => _runWebClient;
            set
            {
                _runWebClient = value;
                RunWebClientSet = true;
            }
        }

        private bool _wantRunning;
        public bool WantRunningSet
        {
            get; set;
        }
        public bool WantRunning
        {
            get => _wantRunning;
            set
            {
                _wantRunning = value;
                WantRunningSet = true;
            }
        }

        private bool _loggedOut;
        public bool LoggedOutSet
        {
            get; set;
        }
        public bool LoggedOut
        {
            get => _loggedOut;
            set
            {
                _loggedOut = value;
                LoggedOutSet = true;
            }
        }

        private bool _shieldsUp;
        public bool ShieldsUpSet
        {
            get; set;
        }
        public bool ShieldsUp
        {
            get => _shieldsUp;
            set
            {
                _shieldsUp = value;
                ShieldsUpSet = true;
            }
        }

        private object _advertiseTags;
        public bool AdvertiseTagsSet
        {
            get; set;
        }
        public object AdvertiseTags
        {
            get => _advertiseTags;
            set
            {
                _advertiseTags = value;
                AdvertiseTagsSet = true;
            }
        }

        private string _hostname;
        public bool HostnameSet
        {
            get; set;
        }
        public string Hostname
        {
            get => _hostname;
            set
            {
                _hostname = value;
                HostnameSet = true;
            }
        }

        private bool _notepadURLs;
        public bool NotepadURLsSet
        {
            get; set;
        }
        public bool NotepadURLs
        {
            get => _notepadURLs;
            set
            {
                _notepadURLs = value;
                NotepadURLsSet = true;
            }
        }

        private bool _forceDaemon;
        public bool ForceDaemonSet
        {
            get; set;
        }

        public bool ForceDaemon
        {
            get => _forceDaemon;
            set
            {
                _forceDaemon = value;
                ForceDaemonSet = true;
            }
        }

        private object _advertiseRoutes;
        public bool AdvertiseRoutesSet
        {
            get; set;
        }
        public object AdvertiseRoutes
        {
            get => _advertiseRoutes;
            set
            {
                _advertiseRoutes = value;
                AdvertiseRoutesSet = true;
            }
        }

        private List<string> _advertiseServices;
        public bool AdvertiseServicesSet
        {
            get; set;
        }

        public List<string> AdvertiseServices
        {
            get => _advertiseServices;
            set
            {
                _advertiseServices = value;
                AdvertiseServicesSet = true;
            }
        }

        private bool _noSNAT;
        public bool NoSNATSet
        {
            get; set;
        }
        public bool NoSNAT
        {
            get => _noSNAT;
            set
            {
                _noSNAT = value;
                NoSNATSet = true;
            }
        }

        private bool _noStatefulFiltering;
        public bool NoStatefulFilteringSet
        {
            get; set;
        }

        public bool NoStatefulFiltering
        {
            get => _noStatefulFiltering;
            set
            {
                _noStatefulFiltering = value;
                NoStatefulFilteringSet = true;
            }
        }

        private int _netfilterMode;
        public bool NetfilterModeSet
        {
            get; set;
        }
        public int NetfilterMode
        {
            get => _netfilterMode;
            set
            {
                _netfilterMode = value;
                NetfilterModeSet = true;
            }
        }

        private string _operatorUser;
        public bool OperatorUserSet
        {
            get; set;
        }

        public string OperatorUser
        {
            get => _operatorUser;
            set
            {
                _operatorUser = value;
                OperatorUserSet = true;
            }
        }

        private string _profileName;
        public bool ProfileNameSet
        {
            get; set;
        }
        public string ProfileName
        {
            get => _profileName;
            set
            {
                _profileName = value;
                ProfileNameSet = true;
            }
        }

        private AutoUpdate _autoUpdate;
        public AutoUpdateMaskedPrefs AutoUpdateSet
        {
            get; set;
        }
        public AutoUpdate AutoUpdate
        {
            get => _autoUpdate;
            set
            {
                _autoUpdate = value;
                AutoUpdateSet = new AutoUpdateMaskedPrefs { CheckSet = true, ApplySet = true }; // Good enough I think
            }
        }

        private AppConnector _appConnector;
        public bool AppConnectorSet
        {
            get; set;
        }
        public AppConnector AppConnector
        {
            get => _appConnector;
            set
            {
                _appConnector = value;
                AppConnectorSet = true;
            }
        }

        private bool _postureChecking;
        public bool PostureCheckingSet
        {
            get; set;
        }
        public bool PostureChecking
        {
            get => _postureChecking;
            set
            {
                _postureChecking = value;
                PostureCheckingSet = true;
            }
        }

        private string _netfilterKind;
        public bool NetfilterKindSet
        {
            get; set;
        }
        public string NetfilterKind
        {
            get => _netfilterKind;
            set
            {
                _netfilterKind = value;
                NetfilterKindSet = true;
            }
        }

        private object _driveShares;
        public bool DriveSharesSet
        {
            get; set;
        }
        public object DriveShares
        {
            get => _driveShares;
            set
            {
                _driveShares = value;
                DriveSharesSet = true;
            }
        }

        private object _config;
        public bool ConfigSet
        {
            get; set;
        }
        public object Config
        {
            get => _config;
            set
            {
                _config = value;
                ConfigSet = true;
            }
        }

        // Unused in current Tailscale, see https://github.com/tailscale/tailscale/issues/12058
        public bool AllowSingleHosts { get; private set; } = true;

        // Internal debug mode.
        private bool _egg;
        public bool EggSet
        {
            get; set;
        }

        public bool Egg
        {
            get => _egg;
            set
            {
                _egg = value;
                EggSet = true;
            }
        }

        // Internal field, cannot be set by clients
        public string InternalExitNodePrior
        {
            get; private set;
        }

        public void OnDeserialized()
        {
            // Set all the Set properties to false, so we can signal our own changes
            ControlURLSet = false;
            RouteAllSet = false;
            ExitNodeIDSet = false;
            ExitNodeIPSet = false;
            ExitNodeAllowLANAccessSet = false;
            CorpDNSSet = false;
            RunSSHSet = false;
            RunWebClientSet = false;
            WantRunningSet = false;
            LoggedOutSet = false;
            ShieldsUpSet = false;
            AdvertiseTagsSet = false;
            HostnameSet = false;
            NotepadURLsSet = false;
            ForceDaemonSet = false;
            EggSet = false;
            AdvertiseRoutesSet = false;
            AdvertiseServicesSet = false;
            NoSNATSet = false;
            NoStatefulFilteringSet = false;
            NetfilterModeSet = false;
            OperatorUserSet = false;
            ProfileNameSet = false;
            AutoUpdateSet = null;
            AppConnectorSet = false;
            PostureCheckingSet = false;
            NetfilterKindSet = false;
            DriveSharesSet = false;
            ConfigSet = false;
        }
    }


    // Special treatment just for auto-update, for some reason.
    public class AutoUpdateMaskedPrefs
    {
        public bool CheckSet
        {
            get; set;
        }

        public bool ApplySet
        {
            get; set;
        }
    }

    [Flags]
    public enum NotifyWatchOpt : ulong
    {
        // NotifyWatchEngineUpdates, if set, causes Engine updates to be sent to the
        // client either regularly or when they change, without having to ask for
        // each one via Engine.RequestStatus.
        NotifyWatchEngineUpdates = 1 << 0,

        // if set, the first Notify message (sent immediately) will contain the current State +
        // BrowseToURL + SessionID
        NotifyInitialState = 1 << 1,

        // if set, the first Notify message (sent immediately) will contain the current Prefs
        NotifyInitialPrefs = 1 << 2,

        // if set, the first Notify message (sent immediately) will contain the current NetMap
        NotifyInitialNetMap = 1 << 3,

        // if set, private keys that would normally be sent in updates are zeroed out
        NotifyNoPrivateKeys = 1 << 4,

        // if set, the first Notify message (sent immediately) will contain the current Taildrive Shares
        NotifyInitialDriveShares = 1 << 5,

        // if set, the first Notify message (sent immediately) will contain the current Taildrop
        // OutgoingFiles
        NotifyInitialOutgoingFiles = 1 << 6
    }

    public class Warning
    {
        public string WarnableCode
        {
            get; set;
        }
        public string Severity
        {
            get; set;
        }
        public string Title
        {
            get; set;
        }
        public string Text
        {
            get; set;
        }
        public DateTime BrokenSince
        {
            get; set;
        }
        public Dictionary<string, string> Args
        {
            get; set;
        }
        public List<string> DependsOn
        {
            get; set;
        }
        public bool ImpactsConnectivity
        {
            get; set;
        }

        public TextBlock GetWarningComponent()
        {
            var warningTextBlock = new TextBlock
            {
                Text = Text,
                TextWrapping = TextWrapping.WrapWholeWords,
            };

            // Add tooltip
            var tooltip = new ToolTip
            {
                Content = $"Code: {WarnableCode}\nBroken since: {BrokenSince}"
            };

            ToolTipService.SetToolTip(warningTextBlock, tooltip);

            return warningTextBlock;
        }

        // Static method to parse JSON and return a list of warnings
        public static List<Warning> ParseWarningsFromJson(string json)
        {
            var warningsList = new List<Warning>();

            try
            {
                // Parse the JSON into a JsonObject
                var root = JsonNode.Parse(json)?.AsObject();

                if (root != null && root.ContainsKey("Warnings"))
                {
                    try
                    {
                        if (root["Warnings"] == null) {
                            return warningsList;
                        }
                        var warnings = root["Warnings"].AsObject();

                        // Iterate through each warning entry (ignoring the key)
                        foreach (var warningEntry in warnings)
                        {
                            var warning = warningEntry.Value.Deserialize<Warning>();
                            if (warning != null)
                            {
                                warningsList.Add(warning);
                            }
                        }
                    }
                    catch (NullReferenceException)
                    {
                        return warningsList;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error parsing warnings: {ex.Message}");
            }

            return warningsList;
        }
    }
}
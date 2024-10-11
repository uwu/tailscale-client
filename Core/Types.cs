using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Nodes;
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

        public override string ToString()
        {
            return $"ID: {ID}, PublicKey: {PublicKey}, HostName: {HostName}, OS: {OS}, LastSeen: {LastSeen}, Online: {Online}";
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
        public List<object> Roles
        {
            get; set;
        }

        public override string ToString()
        {
            return $"ID: {ID}, LoginName: {LoginName}, DisplayName: {DisplayName}, ProfilePicURL: {ProfilePicURL}";
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

        public override string ToString()
        {
            return $"Name: {Name}, MagicDNSSuffix: {MagicDNSSuffix}, MagicDNSEnabled: {MagicDNSEnabled}";
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

        public override string ToString()
        {
            return $"Version: {Version}, BackendState: {BackendState}, Self: [{Self}], PeerCount: {Peer?.Count ?? 0}, UserCount: {User?.Count ?? 0}";
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

        public override string ToString()
        {
            return $"MagicDNSName: {MagicDNSName}, DomainName: {DomainName}";
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

        public override string ToString()
        {
            return $"ID: {ID}, Name: {Name}, NetworkProfile: [{NetworkProfile}], NodeID: {NodeID}, ControlURL: {ControlURL}";
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

    public class Prefs
    {
        public string ControlURL
        {
            get; set;
        }
        public bool RouteAll
        {
            get; set;
        }
        public bool AllowSingleHosts
        {
            get; set;
        }
        public string ExitNodeID
        {
            get; set;
        }
        public string ExitNodeIP
        {
            get; set;
        }
        public string InternalExitNodePrior
        {
            get; set;
        }
        public bool ExitNodeAllowLANAccess
        {
            get; set;
        }
        public bool CorpDNS
        {
            get; set;
        }
        public bool RunSSH
        {
            get; set;
        }
        public bool RunWebClient
        {
            get; set;
        }
        public bool WantRunning
        {
            get; set;
        }
        public bool LoggedOut
        {
            get; set;
        }
        public bool ShieldsUp
        {
            get; set;
        }
        public object AdvertiseTags
        {
            get; set;
        }
        public string Hostname
        {
            get; set;
        }
        public bool NotepadURLs
        {
            get; set;
        }
        public object AdvertiseRoutes
        {
            get; set;
        }
        public bool NoSNAT
        {
            get; set;
        }
        public int NetfilterMode
        {
            get; set;
        }
        public AutoUpdate AutoUpdate
        {
            get; set;
        }
        public AppConnector AppConnector
        {
            get; set;
        }
        public bool PostureChecking
        {
            get; set;
        }
        public string NetfilterKind
        {
            get; set;
        }
        public object DriveShares
        {
            get; set;
        }
        public object Config
        {
            get; set;
        }

        public bool WantRunningSet
        {
            get; set;
        }

        public bool ControlURLSet
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


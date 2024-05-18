using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using TailscaleClient.Assets;
using TailscaleClient.Core;

namespace TailscaleClient.Views;

public sealed partial class Accounts : Page, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;
    public void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    private LoginDialog dialog = null;

    public bool _connectionToggle = false;
    public bool ConnectionToggle
    {
        get => _connectionToggle;
        set
        {
            _connectionToggle = value;
            OnPropertyChanged();
        }
    }

    public void UpdateUI()
    {
        var accounts = API.GetAllProfiles();

        ConnectionToggle = API.GetPrefs().WantRunning;

        var currentAccount = accounts.Find(a => a.ID == API.GetCurrentUser().ID);
        accounts.Remove(currentAccount);

        CurrentAccount.Children.Clear();
        AccountList.Children.Clear();

        if (currentAccount != null)
        {
            CurrentAccount.Children.Add(new AccountCard(currentAccount, true));
        }

        foreach (var account in accounts)
        {
            AccountList.Children.Add(new AccountCard(account, false));
        }
    }

    public Accounts()
    {
        InitializeComponent();

        Messaging.Instance.MessageReceived += (sender, e) =>
        {
            if (e.Kind == Messaging.MessageKind.ProfileSwitch)
            {
                DispatcherQueue.TryEnqueue(() =>
                {
                    dialog.Hide();
                    UpdateUI();
                });
            }
            else if (e.Kind == Messaging.MessageKind.IPNBusUpdate)
            {
                switch (e.Key)
                {
                    case "LoginFinished":
                        while (string.IsNullOrEmpty(
                            API.GetCurrentUser().ID))  // wait for the user to be set, actually completing login
                        {
                            System.Threading.Thread.Sleep(100);
                        }
                        DispatcherQueue.TryEnqueue(() =>
                        {
                            dialog.Hide();
                            UpdateUI();
                        });
                        break;
                    case "BrowseToURL":
                        DispatcherQueue.TryEnqueue(() => { dialog.Set(e.Value); });
                        break;
                }
            }
        };

        Loaded += (sender, e) => { dialog = new LoginDialog(XamlRoot); };

        UpdateUI();
    }

    private void AddProfile(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        dialog.Show();
    }

    private void ConnectionToggled(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        var newState = (sender as ToggleSwitch).IsOn;
        if (newState == ConnectionToggle)
        {
            return;
        }

        var skeleton = ((sender as UIElement).XamlRoot.Content as AppSkeleton);
        skeleton.DisableUI("Waiting for backend...");

        var task = Task.Run(() =>
        {
            if (newState)
            {
                API.Connect();
            }
            else
            {
                API.Disconnect();
            }

            while (API.GetStatus().BackendState == "NoState")
            {
                System.Threading.Thread.Sleep(100);
            }
        });

        task.ContinueWith((_) => { DispatcherQueue.TryEnqueue(() => { skeleton.EnableUI(); }); });
    }
}

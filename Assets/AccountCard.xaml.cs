using System;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using TailscaleClient.Views;

namespace TailscaleClient.Assets;

public sealed partial class AccountCard : UserControl
{
    public string UserName = "";
    public string Tailnet = "";
    public string ProfilePicture = "ms-appx:///Assets/Misc/Guest.png";
    private readonly string UserID = "";
    private readonly Core.Types.Profile _profile;

    public AccountCard(Core.Types.Profile profile, bool current)
    {
        InitializeComponent();

        _profile = profile;

        UserID = profile.ID;
        UserName = profile.UserProfile.LoginName;
        Tailnet = "at " + profile.NetworkProfile.DomainName;
        var pfp = profile.UserProfile.ProfilePicURL;

        if (Uri.TryCreate(pfp, UriKind.Absolute, out _))
        {
            ProfilePicture = pfp;
        }

        if (current)
        {
            SwitchAccount.Content = "Current";
            SwitchAccount.IsEnabled = false;
        }
    }

    private void DeleteAccount(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        var dialog = new ConfirmDialog(XamlRoot, _profile);
        dialog.Show();
    }

    private void ChangeAccount(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        var skeleton = ((sender as UIElement).XamlRoot.Content as AppSkeleton);
        skeleton.DisableUI("Switching accounts...");

        var task = Task.Run(() =>
        {
            Core.API.SwitchProfile(UserID);

            while (Core.API.GetStatus().BackendState == "NoState")
            {
                System.Threading.Thread.Sleep(100);
            }
        });

        task.ContinueWith((_) =>
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                skeleton.EnableUI();
                Core.Messaging.Instance.SendMessage(Core.Messaging.MessageKind.ProfileSwitch, "success",
                                                    UserID);
            });
        });
    }
}

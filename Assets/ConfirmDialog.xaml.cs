using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using TailscaleClient.Core;

namespace TailscaleClient.Assets;

public sealed partial class ConfirmDialog : Page
{
    private readonly ContentDialog _dialog;
    private readonly string ProfileName;
    private readonly Types.Profile _profile;

    public ConfirmDialog(XamlRoot root, Types.Profile profile)
    {
        InitializeComponent();

        _dialog = new ContentDialog()
        {
            XamlRoot = root,
            Content = this,
            CloseButtonText = "Cancel",
            PrimaryButtonText = "Logout",
            Title = "Are you sure you want to delete this profile?",
            DefaultButton = ContentDialogButton.Close
        };
        ProfileName =
            "You will need to login again before using " + profile.UserProfile.LoginName + ".";
        _profile = profile;
    }

    public void Show()
    {
        _ = _dialog.ShowAsync();

        _dialog.PrimaryButtonClick += (sender, e) =>
        {
            API.DeleteProfile(_profile.ID);
            Messaging.Instance.SendMessage(Messaging.MessageKind.ProfileSwitch, "deleted", _profile.ID);
        };
    }
}

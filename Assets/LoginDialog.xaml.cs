using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace TailscaleClient.Assets;

public sealed partial class LoginDialog : Page
{
    private readonly ContentDialog _dialog;

    public LoginDialog(XamlRoot root)
    {
        InitializeComponent();

        _dialog = new ContentDialog()
        {
            XamlRoot = root,
            Content = this,
            CloseButtonText = "Cancel",
            PrimaryButtonText = "Login",
            DefaultButton = ContentDialogButton.Primary,
            Title = "Login to Tailscale"
        };
    }

    private async void SetQRCode(string url)
    {
        var URL = new QRCoder.PayloadGenerator.Url(url);
        var qrCodeData = new QRCoder.QRCodeGenerator().CreateQrCode(URL.ToString(),
                                                                    QRCoder.QRCodeGenerator.ECCLevel.M);
        var qrCode = new QRCoder.PngByteQRCode(qrCodeData);
        var image = qrCode.GetGraphic(20);

        QRImage.Visibility = Visibility.Visible;
        using var ms = new MemoryStream(image);
        await QRSource.SetSourceAsync(ms.AsRandomAccessStream());
    }

    public void Set(string message)
    {
        if (string.IsNullOrEmpty(message))
        {
            LoginURL.Visibility = Visibility.Collapsed;
            LoginWarn.Visibility = Visibility.Visible;
            Spinner.Visibility = Visibility.Visible;
            QRImage.Visibility = Visibility.Collapsed;
            Spinner.Height = 32;
            Spinner.Margin = new Thickness(0, 32, 0, 8);
            return;
        }

        Spinner.Visibility = Visibility.Collapsed;
        Spinner.Height = 0;
        Spinner.Margin = new Thickness(0, 0, 0, 0);
        LoginURL.Visibility = Visibility.Visible;
        LoginWarn.Visibility = Visibility.Collapsed;
        LoginURL.NavigateUri = new Uri(message);
        LoginURL.Content = message;
        SetQRCode(message);
    }

    public void Show()
    {
        Set("");
        _ = _dialog.ShowAsync();

        _dialog.PrimaryButtonClick += (sender, e) =>
        {
            e.Cancel = true;
            var server = ControlServerUrl.Text;
            // validate server is empty or a valid URL
            if (!string.IsNullOrEmpty(server) && !Uri.TryCreate(server, UriKind.Absolute, out _))
            {
                Error.Text = "Please enter a valid URL";
                return;
            }

            LoginContent.Visibility = Visibility.Visible;
            PreLoginContent.Visibility = Visibility.Collapsed;

            _dialog.PrimaryButtonText = null;
            _dialog.Title = "Logging in...";

            if (string.IsNullOrEmpty(server))
            {
                Core.API.Login();
            }
            else
            {
                Core.API.Login(server);
            }
        };

        _dialog.CloseButtonClick += (sender, e) => { Hide(); };
    }

    public void Hide()
    {
        _dialog.Hide();

        Task.Delay(300).ContinueWith((_) =>
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                LoginContent.Visibility = Visibility.Collapsed;
                PreLoginContent.Visibility = Visibility.Visible;
                Set("");
            });
        });
    }

    private void ResetError(object sender, TextChangedEventArgs e)
    {
        if (Error.Text != " ")
        {
            Error.Text = " ";
        }
    }
}

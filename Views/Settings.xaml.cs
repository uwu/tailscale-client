using Microsoft.UI.Xaml.Controls;
using Windows.ApplicationModel;

namespace TailscaleClient.Views;

public sealed partial class Settings : Page
{
    private readonly string Version;

    public static string GetAppVersion()
    {
        try
        {
            var version = Package.Current.Id.Version;
            return string.Format("{0}.{1}.{2}", version.Major, version.Minor, version.Build);
        }
        catch
        {
            return "Unpackaged build";
        }
    }

    public Settings()
    {
        this.InitializeComponent();
        Version = GetAppVersion();
    }
}

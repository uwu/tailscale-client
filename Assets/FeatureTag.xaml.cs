using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

namespace TailscaleClient.Assets;

public sealed partial class FeatureTag : UserControl
{
    private string TagText = "";
    private string ToolTipText = "";
    private static readonly SolidColorBrush DefaultColor = Application.Current.Resources["CardBackgroundFillColorDefaultBrush"] as SolidColorBrush;
    private SolidColorBrush BackgroundColor = DefaultColor;
    private SolidColorBrush ForegroundColor = Application.Current.Resources["TextFillColorPrimaryBrush"] as SolidColorBrush;

    public FeatureTag(string text, string tooltip)
    {
        InitializeComponent();
        TagText = text;
        ToolTipText = tooltip;
    }

    private static Windows.UI.Color Multiply(Windows.UI.Color color, Windows.UI.Color background)
    {
        var influence = background.A / 255f;
        var baseMultiplier = 1 - influence;

        var r = (byte)(color.R * baseMultiplier + background.R * influence);
        var g = (byte)(color.G * baseMultiplier + background.G * influence);
        var b = (byte)(color.B * baseMultiplier + background.B * influence);

        return Windows.UI.Color.FromArgb(color.A, r, g, b);
    }

    public FeatureTag(string text, string tooltip, Windows.UI.Color background)
    {
        InitializeComponent();
        TagText = text;
        ToolTipText = tooltip;

        BackgroundColor = new SolidColorBrush(Multiply(DefaultColor.Color, background));
    }
}


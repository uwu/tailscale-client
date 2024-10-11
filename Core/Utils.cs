using System.Collections.Generic;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;

namespace TailscaleClient.Core;
internal class Utils
{
    private static SolidColorBrush GetStatusColor(bool state)
    {
        return state switch
        {
            true => Application.Current.Resources["SystemFillColorSuccessBrush"],
            false => Application.Current.Resources["SystemFillColorNeutralBrush"],
        } as SolidColorBrush;
    }

    private static SolidColorBrush GetStatusTextColor(bool state)
    {
        return state switch
        {
            true => Application.Current.Resources["TextFillColorPrimaryBrush"],
            false => Application.Current.Resources["TextFillColorSecondaryBrush"],
        } as SolidColorBrush;
    }

    private static SolidColorBrush GetSeverityColor(string severity)
    {
        return severity switch
        {
            "high" => Application.Current.Resources["SystemFillColorCriticalBrush"],
            "medium" => Application.Current.Resources["SystemFillColorCautionBrush"],
            "low" => Application.Current.Resources["SystemFillColorSuccessBrush"],
            _ => Application.Current.Resources["SystemFillColorNeutralBrush"]
        } as SolidColorBrush;
    }

    public static Dictionary<string, SolidColorBrush> colors = [];

    public static void InitializeColors()
    {
        colors.Add("StatusSuccess", GetStatusColor(true));
        colors.Add("StatusFail", GetStatusColor(false));
        colors.Add("TextSuccess", GetStatusTextColor(true));
        colors.Add("TextFail", GetStatusTextColor(false));
        colors.Add("SeverityHigh", GetSeverityColor("high"));
        colors.Add("SeverityMedium", GetSeverityColor("medium"));
        colors.Add("SeverityLow", GetSeverityColor("low"));
        colors.Add("SeverityUnknown", GetSeverityColor("unknown"));
    }
}

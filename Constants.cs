using System;
using System.IO;

namespace TailscaleClient;

public static class Constants
{
    public const string AppDisplayName = "Senko Client for Tailscale";

    public readonly static string AppIconLightAbsolutePath = Path.Combine(AppContext.BaseDirectory, "Assets/Icons/AppIconBase-Light.ico");

    public readonly static string AppIconDarkAbsolutePath = Path.Combine(AppContext.BaseDirectory, "Assets/Icons/AppIconBase-Dark.ico");
}

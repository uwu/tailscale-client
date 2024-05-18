using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using WinRT.Interop;

namespace TailscaleClient.Core;

internal static class Windowing
{
    public static Window CreateWindow()
    {
        var newWindow = new Window
        {
            SystemBackdrop = new MicaBackdrop()
        };
        TrackWindow(newWindow);
        return newWindow;
    }

    private static void TrackWindow(Window window)
    {
        window.Closed += (_, _) =>
        {
            ActiveWindows.Remove(window);
        };
        ActiveWindows.Add(window);
    }

    public static Window GetCurrentWindow(Page itself)
    {
        return itself.XamlRoot == null ? null : ActiveWindows.FirstOrDefault(e => e.Content.XamlRoot == itself.XamlRoot);
    }

    public static Window GetWindowForElement(UIElement element)
    {
        return element.XamlRoot == null ? null : ActiveWindows.FirstOrDefault(e => e.Content.XamlRoot == element.XamlRoot);
    }

    public static IntPtr GetWindowHwnd(Window window)
    {
        return WindowNative.GetWindowHandle(window);
    }

    public static AppWindow GetCurrentAppWindow(Page itself)
    {
        var window = GetCurrentWindow(itself);
        return GetAppWindowFromWindow(window);
    }

    public static AppWindow GetAppWindowFromWindow(Window window)
    {
        var hWnd = WindowNative.GetWindowHandle(window);
        var myWndId = Win32Interop.GetWindowIdFromWindow(hWnd);
        return AppWindow.GetFromWindowId(myWndId);
    }

    private static List<Window> ActiveWindows { get; set; } = new();
}
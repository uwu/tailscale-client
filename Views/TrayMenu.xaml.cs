using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using WinUIEx;

namespace TailscaleClient.Views;

[ObservableObject]
public sealed partial class TrayMenu : UserControl
{
    [ObservableProperty]
    private string _appDisplayName = Constants.AppDisplayName;

    public TrayMenu()
    {
        InitializeComponent();
    }

    [RelayCommand]
    private void ShowWindow()
    {
        App.MainWindow.Show();
        App.MainWindow.BringToFront();
    }

    [RelayCommand]
    private void ExitApp()
    {
        DisposeTrayIconControl();
        App.CanCloseWindow = true;
        App.MainWindow.Close();
    }

    private void DisposeTrayIconControl()
    {
        try
        {
            TrayIcon.Dispose();
        }
        catch { }
    }
}

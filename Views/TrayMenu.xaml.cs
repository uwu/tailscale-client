using System.ComponentModel;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using WinUIEx;

namespace TailscaleClient.Views;

public sealed partial class TrayMenu : UserControl, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;
    public void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private string _appDisplayName = Constants.AppDisplayName;

    public string AppDisplayName
    {
        get => _appDisplayName;
        set
        {
            _appDisplayName = value;
            OnPropertyChanged();
        }
    }

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

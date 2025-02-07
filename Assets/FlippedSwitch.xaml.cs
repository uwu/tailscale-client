using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace TailscaleClient.Assets;

public sealed partial class FlippedSwitch : UserControl, INotifyPropertyChanged
{
    public FlippedSwitch()
    {
        InitializeComponent();
    }

    public event PropertyChangedEventHandler PropertyChanged;
    public void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public static readonly DependencyProperty IsOnProperty =
                DependencyProperty.Register(
                    nameof(IsOn),
                    typeof(bool),
                    typeof(FlippedSwitch),
                    new PropertyMetadata(false, OnIsOnChanged));

    public bool IsOn
    {
        get => (bool)GetValue(IsOnProperty);
        set => SetValue(IsOnProperty, value);
    }

    private static void OnIsOnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (FlippedSwitch)d;
        control.UpdateStatusText();
    }

    public event RoutedEventHandler Toggled;

    private void ToggleSwitch_Toggled(object sender, RoutedEventArgs e)
    {
        Toggled?.Invoke(sender, e);
        UpdateStatusText();
    }

    private string _status = "Off";
    public string Status
    {
        get => _status;
        set
        {
            _status = value;
            OnPropertyChanged();
        }
    }

    private void UpdateStatusText()
    {
        Status = ToggleSwitch.IsOn ? "On" : "Off";
    }
}

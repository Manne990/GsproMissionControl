using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using GsproMissionControl.KeyInjection;

namespace GsproMissionControl.Screen;

public partial class MainWindow : Window
{
    private readonly IKeyInjector _keyInjector = InjectorFactory.Create();

    public MainWindow()
    {
        InitializeComponent();

        Opened += (_, _) =>
        {
            WindowsNoActivate.MakeNoActivateAsync(this, toolWindow: true);
        };
    }

    private void SafeSend(Action send)
    {
        try
        {
            send();
        }
        catch (Exception ex)
        {
            // Log so user/debugger can see; don't crash the overlay
            System.Diagnostics.Debug.WriteLine(ex);
        }
    }

    private void Flyover_OnClick(object? sender, RoutedEventArgs e) => SafeSend(() => _keyInjector.SendKeyChar('o'));
    private void Aim_OnClick(object? sender, RoutedEventArgs e) => SafeSend(() => _keyInjector.SendKeyChar('J'));
    private void HeatMap_OnClick(object? sender, RoutedEventArgs e) => SafeSend(() => _keyInjector.SendKeyChar('Y'));
    private void HideObjects_OnClick(object? sender, RoutedEventArgs e) => SafeSend(() => _keyInjector.SendKeyChar('B'));
    private void HideUi_OnClick(object? sender, RoutedEventArgs e) => SafeSend(() => _keyInjector.SendKeyChar('H'));
    private void TeeBack_OnClick(object? sender, RoutedEventArgs e) => SafeSend(() => _keyInjector.SendKeyChar('c'));
    private void TeeForward_OnClick(object? sender, RoutedEventArgs e) => SafeSend(() => _keyInjector.SendKeyChar('V'));
    private void ClubUp_OnClick(object? sender, RoutedEventArgs e) => SafeSend(() => _keyInjector.SendKeyChar('I'));
    private void ClubDown_OnClick(object? sender, RoutedEventArgs e) => SafeSend(() => _keyInjector.SendKeyChar('K'));
    private void Putter_OnClick(object? sender, RoutedEventArgs e) => SafeSend(() => _keyInjector.SendKeyChar('U'));
    private void ScoreCard_OnClick(object? sender, RoutedEventArgs e) => SafeSend(() => _keyInjector.SendKeyChar('T'));
    private void CameraUp_OnClick(object? sender, RoutedEventArgs e) => SafeSend(_keyInjector.SendArrowUp);
    private void CameraDown_OnClick(object? sender, RoutedEventArgs e) => SafeSend(_keyInjector.SendArrowDown);
    private void CameraLeft_OnClick(object? sender, RoutedEventArgs e) => SafeSend(_keyInjector.SendArrowLeft);
    private void CameraRight_OnClick(object? sender, RoutedEventArgs e) => SafeSend(_keyInjector.SendArrowRight);

}

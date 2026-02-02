using Avalonia.Controls;
using Avalonia.Interactivity;
using GsproMissionControl.KeyInjection;

namespace GsproMissionControl;

public partial class MainWindow : Window
{
    private readonly IKeyInjector _keyInjector = InjectorFactory.Create();

    public MainWindow()
    {
        InitializeComponent();

        Opened += (_, _) =>
        {
            WindowsNoActivate.MakeNoActivate(this);
        };

    }

    private void Flyover_OnClick(object? sender, RoutedEventArgs e) => _keyInjector.SendKeyChar('o');
    private void Aim_OnClick(object? sender, RoutedEventArgs e) => _keyInjector.SendKeyChar('J');
    private void HeatMap_OnClick(object? sender, RoutedEventArgs e) => _keyInjector.SendKeyChar('Y');
    private void HideObjects_OnClick(object? sender, RoutedEventArgs e) => _keyInjector.SendKeyChar('B');
    private void HideUi_OnClick(object? sender, RoutedEventArgs e) => _keyInjector.SendKeyChar('H');

    private void TeeBack_OnClick(object? sender, RoutedEventArgs e) => _keyInjector.SendKeyChar('c');
    private void TeeForward_OnClick(object? sender, RoutedEventArgs e) => _keyInjector.SendKeyChar('V');

    private void ClubUp_OnClick(object? sender, RoutedEventArgs e) => _keyInjector.SendKeyChar('I');
    private void ClubDown_OnClick(object? sender, RoutedEventArgs e) => _keyInjector.SendKeyChar('K');
    private void Putter_OnClick(object? sender, RoutedEventArgs e) => _keyInjector.SendKeyChar('U');

    private void ScoreCard_OnClick(object? sender, RoutedEventArgs e) => _keyInjector.SendKeyChar('T');

    // D-pad (RepeatButton -> Click repeteras automatiskt)
    private void CameraUp_OnClick(object? sender, RoutedEventArgs e) => _keyInjector.SendArrowUp();
    private void CameraDown_OnClick(object? sender, RoutedEventArgs e) => _keyInjector.SendArrowDown();
    private void CameraLeft_OnClick(object? sender, RoutedEventArgs e) => _keyInjector.SendArrowLeft();
    private void CameraRight_OnClick(object? sender, RoutedEventArgs e) => _keyInjector.SendArrowRight();

}
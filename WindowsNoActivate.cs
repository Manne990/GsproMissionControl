using System;
using System.Runtime.InteropServices;
using Avalonia.Controls;
using Avalonia.Threading;

namespace GsproMissionControl;

public static class WindowsNoActivate
{
    // Extended Window Styles
    private const int GWL_EXSTYLE = -20;

    private const int WS_EX_NOACTIVATE = 0x08000000;

    // Valfritt men rekommenderat för “overlay/mission control”:
    // tar bort från Alt-Tab och minskar chanser att bli “aktivt” fönster.
    private const int WS_EX_TOOLWINDOW = 0x00000080;

    // SetWindowPos flags
    private const uint SWP_NOSIZE = 0x0001;
    private const uint SWP_NOMOVE = 0x0002;
    private const uint SWP_NOZORDER = 0x0004;
    private const uint SWP_NOACTIVATE = 0x0010;
    private const uint SWP_FRAMECHANGED = 0x0020;

    /// <summary>
    /// Call from Opened/AttachedToVisualTree. Posts one tick to ensure HWND exists and styles apply reliably.
    /// </summary>
    public static void MakeNoActivateAsync(Window window, bool toolWindow = true)
    {
        if (!OperatingSystem.IsWindows())
            return;

        // Post to next UI tick -> stabilare timing än direkt i Opened.
        Dispatcher.UIThread.Post(() => MakeNoActivate(window, toolWindow), DispatcherPriority.Loaded);
    }

    public static void MakeNoActivate(Window window, bool toolWindow = true)
    {
        if (!OperatingSystem.IsWindows())
            return;

        var handle = window.TryGetPlatformHandle()?.Handle ?? IntPtr.Zero;
        if (handle == IntPtr.Zero)
            return;

        var exStyle = GetWindowLongPtr(handle, GWL_EXSTYLE).ToInt64();

        long newStyle = exStyle | WS_EX_NOACTIVATE;
        if (toolWindow)
            newStyle |= WS_EX_TOOLWINDOW;

        SetWindowLongPtr(handle, GWL_EXSTYLE, new IntPtr(newStyle));

        // Force refresh/apply + säkerställ “no activate” även vid style-change
        SetWindowPos(handle,
            IntPtr.Zero,
            0, 0, 0, 0,
            SWP_NOMOVE | SWP_NOSIZE | SWP_NOZORDER | SWP_NOACTIVATE | SWP_FRAMECHANGED);
    }

    // 64-bit safe Get/SetWindowLongPtr
    private static IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex)
        => IntPtr.Size == 8 ? GetWindowLongPtr64(hWnd, nIndex) : new IntPtr(GetWindowLong32(hWnd, nIndex));

    private static IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong)
        => IntPtr.Size == 8 ? SetWindowLongPtr64(hWnd, nIndex, dwNewLong) : new IntPtr(SetWindowLong32(hWnd, nIndex, dwNewLong.ToInt32()));

    [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
    private static extern int GetWindowLong32(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
    private static extern int SetWindowLong32(IntPtr hWnd, int nIndex, int dwNewLong);

    [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr")]
    private static extern IntPtr GetWindowLongPtr64(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
    private static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool SetWindowPos(
        IntPtr hWnd,
        IntPtr hWndInsertAfter,
        int X,
        int Y,
        int cx,
        int cy,
        uint uFlags);
}
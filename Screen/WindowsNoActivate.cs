using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Threading;

namespace GsproMissionControl.Screen;

public static class WindowsNoActivate
{
    // Extended Window Styles
    private const int GWL_EXSTYLE = -20;

    private const int WS_EX_NOACTIVATE = 0x08000000;

    // Tar bort från Alt-Tab och minskar chanser att bli "aktivt" fönster.
    private const int WS_EX_TOOLWINDOW = 0x00000080;

    // SetWindowPos flags
    private const uint SWP_NOSIZE = 0x0001;
    private const uint SWP_NOMOVE = 0x0002;
    private const uint SWP_NOZORDER = 0x0004;
    private const uint SWP_NOACTIVATE = 0x0010;
    private const uint SWP_FRAMECHANGED = 0x0020;

    // WM_MOUSEACTIVATE: return MA_NOACTIVATE so window is never activated by click or hover (active window tracking).
    private const uint WM_MOUSEACTIVATE = 0x0021;
    private const int MA_NOACTIVATE = 3;

    // WM_SIZE: when window is maximized/restored/minimized, styles can be lost or window can be activated — re-apply.
    private const uint WM_SIZE = 0x0005;
    private const int SIZE_RESTORED = 0;
    private const int SIZE_MINIMIZED = 1;
    private const int SIZE_MAXIMIZED = 2;

    private static readonly Win32Properties.CustomWndProcHookCallback WndProcHook = WndProcHookCallback;

    /// <summary>Maps HWND to (Window, toolWindow) so we can re-apply styles from WndProc (e.g. on maximize).</summary>
    private static readonly Dictionary<IntPtr, (Window Window, bool ToolWindow)> _windowByHandle = new();

    /// <summary>
    /// Call from Opened. Applies no-activate styles, hooks WM_MOUSEACTIVATE, and re-applies styles after short delays to win timing races.
    /// </summary>
    public static void MakeNoActivateAsync(Window window, bool toolWindow = true)
    {
        if (!OperatingSystem.IsWindows())
            return;

        // Apply immediately if handle exists (minimize chance of a single frame with wrong style).
        ApplyStyles(window, toolWindow);

        Dispatcher.UIThread.Post(() => ApplyAndHook(window, toolWindow), DispatcherPriority.Loaded);
    }

    private static void ApplyAndHook(Window window, bool toolWindow)
    {
        if (!OperatingSystem.IsWindows())
            return;

        ApplyStyles(window, toolWindow);

        // Block activation from mouse click/hover (WS_EX_NOACTIVATE is bypassed by "active window tracking").
        var handle = window.TryGetPlatformHandle()?.Handle ?? IntPtr.Zero;
        if (handle != IntPtr.Zero)
            _windowByHandle[handle] = (window, toolWindow);

        try
        {
            Win32Properties.AddWndProcHookCallback(window, WndProcHook);
            window.Closed += OnWindowClosed;
        }
        catch
        {
            if (handle != IntPtr.Zero)
                _windowByHandle.Remove(handle);
            // Win32Properties may not be available on non-Win32 platform.
        }

        // Re-apply styles after delays; HWND can appear late or get overwritten by the framework.
        ScheduleReapply(window, toolWindow, 100);
        ScheduleReapply(window, toolWindow, 400);
    }

    private static void OnWindowClosed(object? sender, EventArgs e)
    {
        if (sender is not Window window)
            return;
        window.Closed -= OnWindowClosed;
        var handle = window.TryGetPlatformHandle()?.Handle ?? IntPtr.Zero;
        if (handle != IntPtr.Zero)
            _windowByHandle.Remove(handle);
        try
        {
            Win32Properties.RemoveWndProcHookCallback(window, WndProcHook);
        }
        catch { /* best effort */ }
    }

    private static void ScheduleReapply(Window window, bool toolWindow, int delayMs)
    {
        _ = Task.Run(async () =>
        {
            await Task.Delay(delayMs).ConfigureAwait(false);
            Dispatcher.UIThread.Post(() => ApplyStyles(window, toolWindow), DispatcherPriority.Loaded);
        });
    }

    private static void ReapplyStylesForSizeChange(IntPtr hWnd)
    {
        if (_windowByHandle.TryGetValue(hWnd, out var pair))
        {
            Dispatcher.UIThread.Post(() => ApplyStyles(pair.Window, pair.ToolWindow), DispatcherPriority.Send);
            ScheduleReapply(pair.Window, pair.ToolWindow, 50);
        }
        else
        {
            Dispatcher.UIThread.Post(() =>
            {
                foreach (var (w, tw) in _windowByHandle.Values)
                {
                    if (w.TryGetPlatformHandle()?.Handle == hWnd)
                    {
                        ApplyStyles(w, tw);
                        ScheduleReapply(w, tw, 50);
                        break;
                    }
                }
            }, DispatcherPriority.Send);
        }
    }

    private static IntPtr WndProcHookCallback(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        if (msg == WM_MOUSEACTIVATE)
        {
            handled = true;
            return (IntPtr)MA_NOACTIVATE;
        }
        // Maximize / restore from maximized / restore from minimized can activate the window or strip our styles — re-apply.
        if (msg == WM_SIZE)
        {
            var sizeType = wParam.ToInt32();
            if (sizeType == SIZE_MAXIMIZED || sizeType == SIZE_RESTORED)
                ReapplyStylesForSizeChange(hWnd);
        }
        return IntPtr.Zero;
    }

    public static void MakeNoActivate(Window window, bool toolWindow = true)
        => ApplyStyles(window, toolWindow);

    private static void ApplyStyles(Window window, bool toolWindow)
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

        SetWindowPos(handle,
            IntPtr.Zero,
            0, 0, 0, 0,
            SWP_NOMOVE | SWP_NOSIZE | SWP_NOZORDER | SWP_NOACTIVATE | SWP_FRAMECHANGED);
    }

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

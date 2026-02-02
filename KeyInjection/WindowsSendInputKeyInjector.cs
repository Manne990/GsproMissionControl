using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace GsproMissionControl.KeyInjection;

public sealed class WindowsSendInputKeyInjector : IKeyInjector
{
    private const uint INPUT_KEYBOARD = 1;

    private const uint KEYEVENTF_KEYUP = 0x0002;
    private const uint KEYEVENTF_SCANCODE = 0x0008;
    private const uint KEYEVENTF_EXTENDEDKEY = 0x0001;

    private const uint MAPVK_VK_TO_VSC = 0;

    // Virtual key codes
    private const ushort VK_SHIFT = 0x10;
    private const ushort VK_CONTROL = 0x11;
    private const ushort VK_MENU = 0x12; // ALT

    private const ushort VK_UP = 0x26;
    private const ushort VK_DOWN = 0x28;
    private const ushort VK_LEFT = 0x25;
    private const ushort VK_RIGHT = 0x27;

    public void SendKeyChar(char c)
    {
        if (!OperatingSystem.IsWindows())
            return;

        // VkKeyScanEx returnerar:
        // low byte = virtual key
        // high byte = shift state bits (1=SHIFT, 2=CTRL, 4=ALT)
        var hkl = GetKeyboardLayout(0);
        short vkAndShift = VkKeyScanEx(c, hkl);

        if (vkAndShift == -1)
            throw new InvalidOperationException($"No virtual-key mapping for char '{c}' on current keyboard layout.");

        ushort vk = (ushort)(vkAndShift & 0xFF);
        byte shiftState = (byte)((vkAndShift >> 8) & 0xFF);

        var inputs = new List<INPUT>(8);

        // Modifiers down (i ordning: CTRL, ALT, SHIFT brukar vara safe)
        if ((shiftState & 2) != 0) inputs.Add(KeyVk(vk: VK_CONTROL, keyUp: false));
        if ((shiftState & 4) != 0) inputs.Add(KeyVk(vk: VK_MENU, keyUp: false));
        if ((shiftState & 1) != 0) inputs.Add(KeyVk(vk: VK_SHIFT, keyUp: false));

        // Main key down/up
        inputs.Add(KeyVk(vk, keyUp: false));
        inputs.Add(KeyVk(vk, keyUp: true));

        // Modifiers up (reverse order)
        if ((shiftState & 1) != 0) inputs.Add(KeyVk(vk: VK_SHIFT, keyUp: true));
        if ((shiftState & 4) != 0) inputs.Add(KeyVk(vk: VK_MENU, keyUp: true));
        if ((shiftState & 2) != 0) inputs.Add(KeyVk(vk: VK_CONTROL, keyUp: true));

        Send(inputs);
    }

    public void SendArrowUp() => SendVkExtended(VK_UP);
    public void SendArrowDown() => SendVkExtended(VK_DOWN);
    public void SendArrowLeft() => SendVkExtended(VK_LEFT);
    public void SendArrowRight() => SendVkExtended(VK_RIGHT);

    private static void SendVkExtended(ushort vk)
    {
        if (!OperatingSystem.IsWindows())
            return;

        var inputs = new[]
        {
            KeyVk(vk, keyUp: false, extended: true),
            KeyVk(vk, keyUp: true,  extended: true),
        };

        Send(inputs);
    }

    private static void Send(IEnumerable<INPUT> inputsEnumerable)
    {
        var inputs = inputsEnumerable is INPUT[] arr ? arr : new List<INPUT>(inputsEnumerable).ToArray();

        int cbSize = Marshal.SizeOf<INPUT>();
        uint sent = SendInput((uint)inputs.Length, inputs, cbSize);

        if (sent != inputs.Length)
        {
            int err = Marshal.GetLastWin32Error();
            throw new InvalidOperationException($"SendInput failed. sent={sent}/{inputs.Length}, cbSize={cbSize}, GetLastError={err}");
        }
    }

    private static INPUT KeyVk(ushort vk, bool keyUp, bool extended = false)
    {
        // Konvertera VK -> scan code (layout-aware)
        // OBS: piltangenter vill ha EXTENDED flag.
        ushort scan = (ushort)MapVirtualKey(vk, MAPVK_VK_TO_VSC);

        uint flags = KEYEVENTF_SCANCODE;
        if (keyUp) flags |= KEYEVENTF_KEYUP;
        if (extended) flags |= KEYEVENTF_EXTENDEDKEY;

        return new INPUT
        {
            type = INPUT_KEYBOARD,
            U = new InputUnion
            {
                ki = new KEYBDINPUT
                {
                    wVk = 0,          // viktigt: 0 när vi kör SCANCODE
                    wScan = scan,
                    dwFlags = flags,
                    time = 0,
                    dwExtraInfo = GetMessageExtraInfo()
                }
            }
        };
    }

    [DllImport("user32.dll", SetLastError = true)]
    private static extern uint SendInput(uint nInputs, [In] INPUT[] pInputs, int cbSize);

    [DllImport("user32.dll")]
    private static extern IntPtr GetMessageExtraInfo();

    [DllImport("user32.dll")]
    private static extern uint MapVirtualKey(uint uCode, uint uMapType);

    [DllImport("user32.dll")]
    private static extern IntPtr GetKeyboardLayout(uint idThread);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern short VkKeyScanEx(char ch, IntPtr dwhkl);

    // Full INPUT definition (union måste inkludera MOUSEINPUT och HARDWAREINPUT för korrekt storlek)
    [StructLayout(LayoutKind.Sequential)]
    private struct INPUT
    {
        public uint type;
        public InputUnion U;
    }

    [StructLayout(LayoutKind.Explicit)]
    private struct InputUnion
    {
        [FieldOffset(0)] public MOUSEINPUT mi;
        [FieldOffset(0)] public KEYBDINPUT ki;
        [FieldOffset(0)] public HARDWAREINPUT hi;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct MOUSEINPUT
    {
        public int dx;
        public int dy;
        public uint mouseData;
        public uint dwFlags;
        public uint time;
        public IntPtr dwExtraInfo;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct KEYBDINPUT
    {
        public ushort wVk;
        public ushort wScan;
        public uint dwFlags;
        public uint time;
        public IntPtr dwExtraInfo;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct HARDWAREINPUT
    {
        public uint uMsg;
        public ushort wParamL;
        public ushort wParamH;
    }
}
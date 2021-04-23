using System.Runtime.InteropServices;
using DWORD = System.UInt32;
using ULONG_PTR = System.UIntPtr;

namespace SharpHotkeys.Native
{
    /// <summary>
    /// A User32 message structure that can be dereferenced as the LPARAM in <see cref="Hotkeys.Hotkey.HookCallback(int, ULONG_PTR, System.IntPtr)"/>.
    /// Read Source: https://docs.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-kbdllhookstruct?redirectedfrom=MSDN
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct KBDLLHOOKSTRUCT
    {
        public uint vkCode;
        public uint scanCode;
        public uint flags;
        public uint time;
        public ULONG_PTR dwExtraInfo;
    }
}

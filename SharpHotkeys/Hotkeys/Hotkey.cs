/*
 * Copyright (c) Chase Roth <cxr6988@rit.edu>
 * Licensed under the MIT License. See repository root directory for more info.
*/

using System;
using System.Diagnostics;
using HWND = System.IntPtr;
using HHOOK = System.IntPtr;
using LRESULT = System.IntPtr;
using WPARAM = System.UIntPtr;
using LPARAM = System.IntPtr;
using HOOKPROC = System.IntPtr;

using SharpHotkeys.Enumerations;
using SharpHotkeys.Native;
using static SharpHotkeys.Native.Delegates;
using System.Collections.Generic;

namespace SharpHotkeys.Hotkeys
{
    /// <summary>
    /// A <see cref="Hotkey"/> that can be registered / unregistered and listened to.
    /// Read Source: https://docs.microsoft.com/en-us/windows/win32/winmsg/hooks
    /// </summary>
    public class Hotkey : IDisposable
    {
        const int WH_CALLWNDPROC = 0x04;

        public event Action HotkeyClicked;

        readonly static Dictionary<int, Hotkey> hotkeys = new Dictionary<int, Hotkey>();

        /// <summary>
        /// Unique identifier for this hot-key used by the operating system.
        /// </summary>
        readonly int nCode;

        /// <summary>
        /// Handle to a window being used.
        /// </summary>
        readonly HWND hwnd = HWND.Zero;

        #region Shared Fields

        static Process process;

        /// <summary>
        /// A handle to our shared hook callback function.
        /// </summary>
        static HHOOK hhook;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the key that is associated to the hot-key.
        /// </summary>
        public Keys Key { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public Modifiers Mods { get; private set; }        

        /// <summary>
        /// Gets a boolean indicating if this hot-key is currently registered.
        /// </summary>
        public bool IsRegistered { get; private set; }
        #endregion

        /// <summary>
        /// Initializes a new <see cref="Hotkey"/> instance and associates it with the given <paramref name="windowHandle"/>
        /// </summary>
        /// <param name="key">Key to raise the <see cref="HotkeyClicked"/> event.</param>
        /// <param name="modifiers"></param>
        /// <param name="windowHandle"></param>
        public Hotkey(Keys key, Modifiers modifiers, HWND windowHandle)
        {
            nCode = Init(key, modifiers);
            hwnd = windowHandle;
        }

        /// <summary>
        /// Initializes a new <see cref="Hotkey"/> instance and associates it with the calling thread. 
        /// It is recommended to use your application's dispatcher to ensure this is executed on the main thread.
        /// </summary>
        /// <param name="key">Key to raise the <see cref="HotkeyClicked"/> event.</param>
        /// <param name="modifiers"></param>
        public Hotkey(Keys key, Modifiers modifiers)
        {
            nCode = Init(key, modifiers);
        }

        /// <summary>
        /// Shared initialization for both constructors.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="modifiers"></param>
        /// <returns>HashCode of this <see cref="Hotkey"/> instance.</returns>
        int Init(Keys key, Modifiers modifiers)
        {
            // Multiple flags set
            if ((key & (key - 1)) != 0)
                throw new NotSupportedException("The parameter <key> had multiple flags set which would result in the same hot-key needing to respond to key strokes from separate keys. This is currently unsupported.");                      

            Key = key;
            Mods = modifiers;
            return GetHashCode();
        }

        /// <summary>
        /// Registers the <see cref="Hotkey"/>.
        /// </summary>
        /// <returns>Indication of success.</returns>
        public virtual bool TryRegisterHotkey(out uint errCode)
        {
            errCode = 0;
            if (IsRegistered) return true; // Already Registered

            if (hhook == null) // Register our callback function with the system
            {
                hhook = User32.SetWindowsHookExA(WH_CALLWNDPROC, HookCallback, IntPtr.Zero, 0);
                if (hhook == null)
                {
                    errCode = User32.GetLastError();
                    return false;
                }
                // Subscribe to exit process so we can release static resources
                process = Process.GetCurrentProcess();
                process.Exited += Process_Exited;
            }

            if (!User32.RegisterHotKey(hwnd, nCode, (uint)Mods, (uint)Key))
            {
                errCode = User32.GetLastError();
                return false;
            }

            IsRegistered = true;
            hotkeys.Add(nCode, this);
            return true;
        }        

        /// <summary>
        /// Unregisters the <see cref="Hotkey"/>.
        /// </summary>
        /// <returns>Indication of success.</returns>
        public virtual bool TryUnregisterHotkey(out uint errCode)
        {
            errCode = 0;
            if (!IsRegistered) return true; // Already Unregistered
            if (!User32.UnregisterHotKey(hwnd, nCode))
            {
                errCode = User32.GetLastError();
                return false;
            }

            IsRegistered = false;
            hotkeys.Remove(nCode);
            return true;
        }

        /// <summary>
        /// Unregisters the hot-key if registered before disposing. 
        /// Further, Unhooks the callback function used to listen for hot-keys being clicked from the system if registered.
        /// </summary>
        public virtual void Dispose()
        {
            if (IsRegistered)
                TryUnregisterHotkey(out uint errCode);
        }

        /// <summary>
        /// Raises the <see cref="HotkeyClicked"/> event.
        /// </summary>
        protected virtual void OnHotkeyClicked()
            => HotkeyClicked?.Invoke();

        /// <summary>
        /// Callback function that is registered with the system using <see cref="User32.SetWindowsHookExA(int, HookCallbackDelegate, HWND, uint)"/>.
        /// </summary>
        /// <param name="nCode"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        static LRESULT HookCallback(in int nCode, in WPARAM wParam, in LPARAM lParam)
        {            
            if (hotkeys.ContainsKey(nCode)) // Raise Hot-key clicked event
                hotkeys[nCode].OnHotkeyClicked();

            return User32.CallNextHookEx(hhook, nCode, wParam, lParam);
        }

        /// <summary>
        /// Executes when the process is exiting. Will only execute if the process successfully bound a hook callback
        /// with the system.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Process_Exited(object sender, EventArgs e)
        {
            User32.UnhookWindowsHookEx(hhook);
            process.Exited -= Process_Exited;
        }
    }
}
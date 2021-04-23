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
using System.Runtime.InteropServices;

namespace SharpHotkeys.Hotkeys
{
    /// <summary>
    /// A <see cref="Hotkey"/> that can be registered / unregistered and listened to.
    /// Read Source: https://docs.microsoft.com/en-us/windows/win32/winmsg/hooks
    /// </summary>
    public class Hotkey : IDisposable
    {        
        public event Action HotkeyClicked;

        readonly static Dictionary<int, Hotkey> hotkeys = new Dictionary<int, Hotkey>();

        /// <summary>
        /// Unique identifier for this hot-key used by the operating system.
        /// </summary>
        readonly int _nCode;

        /// <summary>
        /// Handle to a window being used.
        /// </summary>
        readonly HWND hwnd = HWND.Zero;

        bool isDisposed = false;

        #region Shared Fields
        /// <summary>
        /// 
        /// </summary>
        static Process process;

        /// <summary>
        /// Our callback method for responding to hooks. It is important this delegate instance is not disposed until
        /// the end of the application because it is needed by the system once registered.
        /// </summary>
        static HookCallbackDelegate hookCallbackDelegate = HookCallback;

        /// <summary>
        /// A handle to our shared hook callback function.
        /// </summary>
        static HHOOK hhook = IntPtr.Zero;
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
            hwnd = windowHandle;
            _nCode = Init(key, modifiers);            
        }

        /// <summary>
        /// Initializes a new <see cref="Hotkey"/> instance and associates it with the calling thread. 
        /// It is recommended to use your application's dispatcher to ensure this is executed on the main thread.
        /// </summary>
        /// <param name="key">Key to raise the <see cref="HotkeyClicked"/> event.</param>
        /// <param name="modifiers"></param>
        public Hotkey(Keys key, Modifiers modifiers)
        {
            _nCode = Init(key, modifiers);
        }

        /// <summary>
        /// Shared initialization for both constructors.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="modifiers"></param>
        /// <returns>HashCode of this <see cref="Hotkey"/> instance.</returns>
        int Init(Keys key, Modifiers modifiers)
        {
            Key = key;
            Mods = modifiers;
            return GetHashCode();
        }        

        /// <summary>
        /// Unregisters the hot-key if registered before disposing. 
        /// Further, Unhooks the callback function used to listen for hot-keys being clicked from the system if registered.
        /// </summary>
        public void Dispose()
        {            
            Dispose(true);
            // Prevent Finalizer from being called twice.
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool isDisposing)
        {
            if (!this.isDisposed) // Hasn't been disposed already
            {
                if (IsRegistered)
                    TryUnregisterHotkey(out uint errCode);


                // Disposing has been done
                isDisposed = true;
            }            
        }

        ~Hotkey()
        {
            Dispose(false);
        }

        /// <summary>
        /// Clean up unmanaged shared resources used by all hot-keys. It is important this is called
        /// before your program exits to prevent a resource leak.
        /// </summary>
        public static void ReleaseStaticResources()
        {
            User32.UnhookWindowsHookEx(hhook);
            process.Dispose();
        }

        /// <summary>
        /// Registers the <see cref="Hotkey"/>.
        /// </summary>
        /// <returns>Indication of success.</returns>
        public virtual bool TryRegisterHotkey(out uint errCode)
        {
            errCode = 0;

            if (IsRegistered) return true; // Already Registered

            if (hhook == HHOOK.Zero) // Register our callback function with the system
            {
                process = Process.GetCurrentProcess();
                IntPtr hInstance = User32.GetModuleHandle(process.MainModule.ModuleName);
                hhook = User32.SetWindowsHookExA((int)HookProcedures.WH_KEYBOARD_LL, hookCallbackDelegate, hInstance, 0);
                if (hhook == HHOOK.Zero)
                {
                    errCode = User32.GetLastError();
                    return false;
                }
            }

            if (!User32.RegisterHotKey(hwnd, _nCode, (uint)Mods, (uint)Key))
            {
                errCode = User32.GetLastError();
                return false;
            }

            IsRegistered = true;
            hotkeys.Add(_nCode, this);
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
            if (!User32.UnregisterHotKey(hwnd, _nCode))
            {
                errCode = User32.GetLastError();
                return false;
            }

            IsRegistered = false;
            hotkeys.Remove(_nCode);
            return true;
        }

        /// <summary>
        /// Raises the <see cref="HotkeyClicked"/> event.
        /// </summary>
        protected virtual void OnHotkeyClicked()
            => HotkeyClicked?.Invoke();

        public override int GetHashCode()
            => (int)Key + (int)Mods + hwnd.ToInt32();

        /// <summary>
        /// Callback function that is registered with the system using <see cref="User32.SetWindowsHookExA(int, HookCallbackDelegate, HWND, uint)"/>.
        /// </summary>
        /// <param name="nCode"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        static LRESULT HookCallback(int nCode, WPARAM wParam, LPARAM lParam)
        {
            if (nCode < 0) // Do not process message
                return User32.CallNextHookEx(hhook, nCode, wParam, lParam);

            var keyMsg = Marshal.PtrToStructure<KBDLLHOOKSTRUCT>(lParam);

            Console.WriteLine($"Key: {(Keys)keyMsg.vkCode}");

            if (hotkeys.ContainsKey(nCode)) // Raise Hot-key clicked event
                hotkeys[nCode].OnHotkeyClicked();

            return User32.CallNextHookEx(hhook, nCode, wParam, lParam);
        }        
    }
}
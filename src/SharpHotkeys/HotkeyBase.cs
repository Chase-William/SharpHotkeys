/*
 * Copyright (c) Chase Roth <cxr6988@rit.edu>
 * Licensed under the MIT License. See repository root directory for more info.
*/

using System;
using System.Windows.Input;
using HWND = System.IntPtr;

using SharpHotkeys.Native;

namespace SharpHotkeys.Hotkeys
{
    /// <summary>
    /// A <see cref="HotkeyBase"/> that can be registered / unregistered and listened to.
    /// Read Source: <a href="https://docs.microsoft.com/en-us/windows/win32/winmsg/hooks">Docs</a>
    /// </summary>
    public class HotkeyBase : IDisposable
    {        
        public event Action HotkeyClicked;       

        /// <summary>
        /// Unique identifier for this hot-key used by the operating system.
        /// </summary>
        protected readonly int _nCode;

        /// <summary>
        /// Handle to a window being used.
        /// </summary>
        protected readonly HWND hwnd = HWND.Zero;

        protected bool isDisposed = false;

        #region Properties
        /// <summary>
        /// Gets the key that is associated to the hot-key.
        /// </summary>
        protected readonly int key;

        /// <summary>
        /// 
        /// </summary>
        protected readonly int mods;   

        /// <summary>
        /// Gets a boolean indicating if this hot-key is currently registered.
        /// </summary>
        public bool IsRegistered { get; private set; }
        #endregion

        /// <summary>
        /// Initializes a new <see cref="HotkeyBase"/> instance and associates it with the given <paramref name="windowHandle"/>
        /// </summary>
        /// <param name="key">Key to raise the <see cref="HotkeyClicked"/> event.</param>
        /// <param name="modifiers"></param>
        /// <param name="windowHandle"></param>
        public HotkeyBase(int key, int modifiers, HWND windowHandle)
        {
            hwnd = windowHandle;
            this.key = key;
            mods = modifiers;
            _nCode = GetHashCode();
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

        ~HotkeyBase()
        {
            Dispose(false);
        }

        /// <summary>
        /// Registers the <see cref="HotkeyBase"/>.
        /// </summary>
        /// <returns>Indication of success.</returns>
        public virtual bool TryRegisterHotkey(out uint errCode)
        {
            errCode = 0;           
            if (IsRegistered) return true; // Already Registered            

            if (!User32.RegisterHotKey(hwnd, _nCode, mods, key))
            {
                errCode = User32.GetLastError();
                return false;
            }
            IsRegistered = true;
            return true;
        }

        /// <summary>
        /// Unregisters the <see cref="HotkeyBase"/>.
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
            return true;
        }

        /// <summary>
        /// Raises the <see cref="HotkeyClicked"/> event.
        /// </summary>
        protected virtual void OnHotkeyClicked()
            => HotkeyClicked?.Invoke();
    }
}
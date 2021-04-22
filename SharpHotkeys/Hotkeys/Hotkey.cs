/*
 * Copyright (c) Chase Roth <cxr6988@rit.edu>
 * Licensed under the MIT License. See repository root directory for more info.
*/

using System;
using HWND = System.IntPtr;

using SharpHotkeys.Enumerations;
using SharpHotkeys.Native;

namespace SharpHotkeys.Hotkeys
{
    /// <summary>
    /// A <see cref="Hotkey"/> that can be registered / unregistered and listened to.
    /// Read Source: https://docs.microsoft.com/en-us/windows/win32/winmsg/hooks
    /// </summary>
    public class Hotkey : IDisposable
    {
        public event Action HotkeyClicked;

        /// <summary>
        /// Unique identifier for this hot-key used by the operating system.
        /// </summary>
        private readonly int keyId;

        /// <summary>
        /// Handle to a window being used.
        /// </summary>
        private readonly HWND hwnd = HWND.Zero;

        #region Properties
        /// <summary>
        /// Gets the key that is associated to the hot-key.
        /// </summary>
        public Keys Key { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public Modifiers Mods { get; private set; }
        #endregion

        /// <summary>
        /// Gets a boolean indicating if this hot-key is currently registered.
        /// </summary>
        public bool IsRegistered { get; private set; }

        /// <summary>
        /// Initializes a new <see cref="Hotkey"/> instance and associates it with the given <paramref name="windowHandle"/>
        /// </summary>
        /// <param name="key">Key to raise the <see cref="HotkeyClicked"/> event.</param>
        /// <param name="modifiers"></param>
        /// <param name="windowHandle"></param>
        public Hotkey(Keys key, Modifiers modifiers, HWND windowHandle)
        {
            keyId = Init(key, modifiers);
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
            keyId = Init(key, modifiers);
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
        public virtual bool TryRegisterHotkey()
        {
            if (!NativeAPI.RegisterHotKey(hwnd, keyId, (uint)Mods, (uint)Key))
                return false;   // Failure

            IsRegistered = true;
            return true;
        }

        /// <summary>
        /// Unregisters the <see cref="Hotkey"/>.
        /// </summary>
        /// <returns>Indication of success.</returns>
        public virtual bool TryUnregisterHotkey()
        {
            if (!NativeAPI.UnregisterHotKey(hwnd, keyId))
                return false;   // Failure

            IsRegistered = false;
            return true;
        }

        /// <summary>
        /// Unregisters the hot-key if registered before disposing.
        /// </summary>
        public virtual void Dispose()
        {
            if (IsRegistered)
                TryUnregisterHotkey();
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void OnHotkeyClicked()
            => HotkeyClicked?.Invoke();
    }
}
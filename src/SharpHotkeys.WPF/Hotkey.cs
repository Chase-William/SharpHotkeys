using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Threading;
using System.Windows.Input;

using SharpHotkeys.Hotkeys;

namespace SharpHotkeys.WPF
{
    /// <summary>
    /// A <see cref="Hotkey"/> class that provides <see cref="WPF"/> integration when using windows hot-keys.
    /// </summary>
    public class Hotkey : HotkeyBase
    {
        /// <summary>
        /// Posted when the user presses a registered hot-key.
        /// Read Source: https://docs.microsoft.com/en-us/windows/win32/inputdev/wm-hotkey
        /// </summary>
        const int MW_HOTKEY = 0x0312;

        /// <summary>
        /// Initializes a new  <see cref="Hotkey"/>.
        /// </summary>
        /// <param name="key">Trigger key.</param>
        /// <param name="modifiers">Modifiers for <paramref name="key"/>.</param>
        /// <param name="windowHandle">Handle to the window that is associated with this hot-key.</param>
        public Hotkey(Key key, ModifierKeys modifiers, IntPtr windowHandle) : base(KeyInterop.VirtualKeyFromKey(key), (int)modifiers, windowHandle)
            => Init();

        /// <summary>
        /// Initializes a new  <see cref="Hotkey"/>.
        /// </summary>
        /// <param name="key">Trigger key.</param>
        /// <param name="modifiers">Modifiers for <paramref name="key"/>.</param>
        /// <param name="window">Window that is associated with this hot-key.</param>
        public Hotkey(Key key, ModifierKeys modifiers, Window window) : base(KeyInterop.VirtualKeyFromKey(key), (int)modifiers, new WindowInteropHelper(window).Handle)
            => Init();

        /// <summary>
        /// Initializes a new  <see cref="Hotkey"/>.
        /// </summary>
        /// <param name="key">Trigger key.</param>
        /// <param name="modifiers">Modifiers for <paramref name="key"/>.</param>
        /// <param name="helper">
        /// Window helper that is associated with this hot-key. 
        /// Passing in <see cref="IntPtr.Zero"/> will cause the hot-key to be associated with the calling thread when <see cref="HotkeyBase.TryRegisterHotkey(out uint)"/> is called.
        /// Therefore it is recommended to use the <see cref="Dispatcher"/> to invoke the <see cref="HotkeyBase.TryRegisterHotkey(out uint)"/> method when registering via the thread.
        /// </param>
        public Hotkey(Key key, ModifierKeys modifiers, WindowInteropHelper helper) : base(KeyInterop.VirtualKeyFromKey(key), (int)modifiers, helper.Handle)
            => Init();

        /// <summary>
        /// Shared Initializer.
        /// </summary>
        void Init()
            => ComponentDispatcher.ThreadPreprocessMessage += ComponentDispatcher_ThreadPreprocessMessage;

        private void ComponentDispatcher_ThreadPreprocessMessage(ref MSG msg, ref bool handled)
        {
            if (handled) return;
            if (msg.message != MW_HOTKEY || (int)(msg.wParam) != _nCode) return;

            OnHotkeyClicked();
            handled = true;
        }        

        protected override void Dispose(bool isDisposing)
        {
            if (!isDisposed) // Detach if not disposed      
                ComponentDispatcher.ThreadPreprocessMessage -= ComponentDispatcher_ThreadPreprocessMessage;            
            base.Dispose(isDisposing);            
        }
    }
}

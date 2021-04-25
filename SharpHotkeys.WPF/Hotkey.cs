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
    public class Hotkey : HotkeyBase
    {
        /// <summary>
        /// Posted when the user presses a registered hot-key.
        /// Read Source: https://docs.microsoft.com/en-us/windows/win32/inputdev/wm-hotkey
        /// </summary>
        const int MW_HOTKEY = 0x0312;

        public Hotkey(Key key, ModifierKeys modifiers, IntPtr windowHandle) : base(KeyInterop.VirtualKeyFromKey(key), (int)modifiers, windowHandle)
        { }

        public Hotkey(Key key, ModifierKeys modifiers, Window window) : base(KeyInterop.VirtualKeyFromKey(key), (int)modifiers, new WindowInteropHelper(window).Handle)
        { }

        public Hotkey(Key key, ModifierKeys modifiers, WindowInteropHelper helper) : base(KeyInterop.VirtualKeyFromKey(key), (int)modifiers, helper.Handle)
        { }

        //public Hotkey(Key key, ModifierKeys modifiers, IntPtr windowHandle) : base(KeyInterop.VirtualKeyFromKey(key), (int)modifiers, windowHandle)
        //    => Init();

        //public Hotkey(Key key, ModifierKeys modifiers, Window window) : base(KeyInterop.VirtualKeyFromKey(key), (int)modifiers, new WindowInteropHelper(window).Handle)
        //    => Init();

        //public Hotkey(Key key, ModifierKeys modifiers, WindowInteropHelper helper) : base(KeyInterop.VirtualKeyFromKey(key), (int)modifiers, helper.Handle)
        //    => Init();

        //void Init()
        //    => ComponentDispatcher.ThreadPreprocessMessage += ComponentDispatcher_ThreadPreprocessMessage;

        //private void ComponentDispatcher_ThreadPreprocessMessage(ref MSG msg, ref bool handled)
        //{           
        //    if (handled) return;
        //    if (msg.message != MW_HOTKEY || (int)(msg.wParam) != _nCode) return;

        //    OnHotkeyClicked();
        //    handled = true;
        //}        

        //protected override void Dispose(bool isDisposing)
        //{
        //    if (!isDisposed) // Detach if not disposed      
        //        ComponentDispatcher.ThreadPreprocessMessage -= ComponentDispatcher_ThreadPreprocessMessage;            
        //    base.Dispose(isDisposing);            
        //}
    }
}

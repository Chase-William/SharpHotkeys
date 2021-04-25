using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI.Xaml;
using System.Runtime.InteropServices;

using SharpHotkeys.Hotkeys;

namespace SharpHotkeys.UWP
{
    public class Hotkey : HotkeyBase
    {
        public Window Window { get; set; }

        public Hotkey(VirtualKey key, VirtualKeyModifiers modifiers, Window window) : base((int)key, (int)modifiers, ((dynamic)window.CoreWindow as ICoreWindowInterop).WindowHandle)
        {
            //var test = ((dynamic)window.CoreWindow as ICoreWindowInterop);
            //Window = window;
            //Window.CoreWindow.Dispatcher.AcceleratorKeyActivated += Dispatcher_AcceleratorKeyActivated;
            //Window.CoreWindow.Closed += CoreWindow_Closed;
        }

        //private void CoreWindow_Closed(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.CoreWindowEventArgs args)
        //{           
        //    Window.CoreWindow.Dispatcher.AcceleratorKeyActivated -= Dispatcher_AcceleratorKeyActivated;
        //    Window.CoreWindow.Closed -= CoreWindow_Closed;
        //    this.Dispose(true);
        //}

        //private void Dispatcher_AcceleratorKeyActivated(Windows.UI.Core.CoreDispatcher sender, Windows.UI.Core.AcceleratorKeyEventArgs args)
        //{
        //    args.
        //}


    }

    [ComImport, Guid("45D64A29-A63E-4CB6-B498-5781D298CB4F")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    interface ICoreWindowInterop
    {
        IntPtr WindowHandle { get; }
        bool MessageHandled { set; }
    }
}

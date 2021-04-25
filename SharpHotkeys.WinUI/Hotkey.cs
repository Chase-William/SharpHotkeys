using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System;


using HWND = System.IntPtr;

using SharpHotkeys.Hotkeys;
using Microsoft.UI.Xaml;
using System.Runtime.InteropServices;
using WinRT;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SharpHotkeys.WinUI
{
    public class Hotkey : HotkeyBase
    {
        public Window Window { get; protected set; }

        public Hotkey(VirtualKey key, VirtualKeyModifiers modifiers, Window window) : base((int)key, (int)modifiers, window.As<IWindowNative>().WindowHandle)
        {
            Window = window;
            var test = new Microsoft.UI.Xaml.Input.KeyboardAccelerator();
            
        }            

        private void Dispatcher_AcceleratorKeyActivated(Windows.UI.Core.CoreDispatcher sender, Windows.UI.Core.AcceleratorKeyEventArgs args)
        {
            Console.WriteLine("Key: " + args.VirtualKey);
            
        }

        protected override void Dispose(bool isDisposing)
        {
            if (!isDisposed)
            {
                Window.Dispatcher.AcceleratorKeyActivated -= Dispatcher_AcceleratorKeyActivated;
            }
            base.Dispose(isDisposing);
            
        }

        //private void ComponentDispatcher_ThreadPreprocessMessage(ref MSG msg, ref bool handled)
        //{
        //    if (handled) return;
        //    if (msg.message != MW_HOTKEY || (int)(msg.wParam) != _nCode) return;

        //    OnHotkeyClicked();
        //    handled = true;
        //}

        public static void Test()
        {
            Console.WriteLine("asdkjhasksadkjjkgasdds");
        }
    }

    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("EECDBF0E-BAE9-4CB6-A68E-9598E1CB57BB")]
    internal interface IWindowNative
    {
        IntPtr WindowHandle { get; }
    }
}
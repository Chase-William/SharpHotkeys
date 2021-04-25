using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

using SharpHotkeys.WinUI;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinUISample
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        Hotkey hotkey;
        

        public MainWindow()
        {
            this.InitializeComponent();            

            Closed += MainWindow_Closed;
            Activated += MainWindow_Activated;
        }

        private void MainWindow_Activated(object sender, WindowActivatedEventArgs args)
        {
            hotkey = new Hotkey(Windows.System.VirtualKey.H, Windows.System.VirtualKeyModifiers.None, this);
            //if (!hotkey.TryRegisterHotkey(out uint errCode))
            //{
            //    throw new Exception("failed to register");
            //}
        }

        private void MainWindow_Closed(object sender, WindowEventArgs args)
        {
            hotkey.Dispose();
            Closed -= MainWindow_Closed;
            Activated -= MainWindow_Activated;
        }

        private void myButton_Click(object sender, RoutedEventArgs e)
        {
            //System.Threading.Thread.CurrentThread.
            //var dispatcher = Windows.Threading.Dispatcher.FromThread(System.Threading.Thread.CurrentThread);
            SharpHotkeys.WinUI.Hotkey.Test();
            Console.WriteLine("clicked");
        }
    }
}

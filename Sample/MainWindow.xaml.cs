using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Input;

using SharpHotkeys.WPF;
using SharpHotkeys;

namespace Sample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Hotkey f6;
        Hotkey h;

        int counter = 0;

        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            IntPtr windowHandle = new WindowInteropHelper(this).Handle;

            f6 = new Hotkey(Key.F6, 
                ModifierKeys.None, 
                windowHandle
                );

            if (!f6.TryRegisterHotkey(out uint errCode))
            {
                lbl.Content = errCode;
            }
            else
            {
                f6.HotkeyClicked += delegate
                {
                    lbl.Content = counter++;
                };
            }

            h = new Hotkey(Key.H,
                ModifierKeys.None,
                windowHandle
                );

            if (!h.TryRegisterHotkey(out errCode))
            {
                lbl.Content = errCode;
            }
            else
            {
                h.HotkeyClicked += delegate
                {
                    lbl.Content = counter++;
                };
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            f6.Dispose();
            h.Dispose();
            base.OnClosing(e);
        }
    }
}

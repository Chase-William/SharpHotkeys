using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Interop;

using SharpHotkeys.Hotkeys;

namespace Sample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Hotkey hotkey;

        int counter = 0;

        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            IntPtr windowHandle = new WindowInteropHelper(this).Handle;

            hotkey = new Hotkey(SharpHotkeys.Enumerations.Keys.F6, SharpHotkeys.Enumerations.Modifiers.MOD_CONTROL, windowHandle);
            if (!hotkey.TryRegisterHotkey(out uint errCode))
            {
                lbl.Content = errCode;
            }
            else
            {
                hotkey.HotkeyClicked += delegate
                {
                    lbl.Content = counter++;
                };
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            hotkey.Dispose();
            Hotkey.ReleaseStaticResources();
            base.OnClosing(e);
        }
    }
}

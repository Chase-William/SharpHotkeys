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
        Hotkey hotkey;

        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            IntPtr windowHandle = new WindowInteropHelper(this).Handle;

            hotkey = new Hotkey(Key.F6, 
                ModifierKeys.Shift, 
                IntPtr.Zero // Or pass in the windowHandle if you want to associate with the window instead of the calling thread.
                );

            if (!hotkey.TryRegisterHotkey(out uint errCode))
            {
                throw new Exception("ErrCode Received:" + errCode);
            }
            else
            {
                hotkey.HotkeyClicked += delegate
                {
                    MessageBox.Show("Hot-key Clicked!");
                };
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            hotkey.Dispose();
            base.OnClosing(e);
        }
    }
}

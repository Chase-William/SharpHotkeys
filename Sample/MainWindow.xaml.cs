﻿using System;
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

        int counter = 0;

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
                windowHandle
                );

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
            base.OnClosing(e);
        }
    }
}

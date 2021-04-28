# SharpHotkeys
A simple hotkey library for WPF programs. The logic for the hotkeys are decoupled and structured so that UWP and WIN-UI 3.0 integration should be possible. If interested supporting other frameworks checkout the <a href="https://github.com/ChaseRoth/SharpHotkeys/tree/experimental">experimental</a> branch.

### Example Usage:
Checkout the sample project to see a working demo.
```cs
hotkey = new Hotkey(Key.F6, 
    ModifierKeys.Shift, // Trigger key
    IntPtr.Zero // Or pass in the windowHandle if you want to associate with the window instead of the calling thread.
    );

if (!hotkey.TryRegisterHotkey(out uint errCode)) // errCode is a win32 error number that can be looked up.
    throw new Exception("ErrCode Received:" + errCode);

hotkey.HotkeyClicked += delegate
{
    MessageBox.Show("Hot-key Clicked!");
};   
```

### Cleaning Up:

The Hotkey class implements the `IDisposable` interface so make sure to call the `Dispose` method when you are done with the hotkey.

```cs
hotkey.Dispose();
```

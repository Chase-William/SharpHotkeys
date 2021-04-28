# SharpHotkeys
A simple hotkey library for WPF programs. The logic for the hotkeys are decoupled and structured so that UWP and WIN-UI 3.0 integration should be possible. If interested supporting other frameworks checkout the <a href="https://github.com/ChaseRoth/SharpHotkeys/tree/experimental">experimental</a> branch.

### Example Usage:
Checkout the sample project to see a working demo.
```cs
hotkey = new Hotkey(
    Key.F6, 
    ModifierKeys.Shift, 
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

The Hotkey class implements the `IDisposable` interface so you can call `Dispose` when you are done with it to immeditely clean up. Otherwise the hotkey's finalizer will call `Dispose` for you when the object is being cleaned up by the GC.

```cs
hotkey.Dispose();
```

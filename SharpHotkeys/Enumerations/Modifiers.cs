using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpHotkeys.Enumerations
{
    /// <summary>
    /// Modifiers that can be used to create a key combo for a hot-key.
    /// Read Source: https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-registerhotkey#parameters
    /// </summary>
    public enum Modifiers : uint
    {
        MOD_NONE = 0x0000,
        MOD_ALT = 0x0001,
        MOD_CONTROL = 0x0002,
        MOD_NOREPEAT = 0x4000,
        MOD_SHIFT = 0x0004,
        MOD_WIN = 0x0008
    }
}

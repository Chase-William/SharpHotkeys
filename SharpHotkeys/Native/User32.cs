/*
 * Copyright (c) Chase Roth <cxr6988@rit.edu>
 * Licensed under the MIT License. See repository root directory for more info.
*/

using System;
using System.Runtime.InteropServices;
using HWND = System.IntPtr; // A handle to a window
using HHOOK = System.IntPtr; // A HANDLE
using WPARAM = System.UIntPtr; // A UINT_PTR which varies in respect to target architecture (x86, x64)
using LPARAM = System.IntPtr; // A LONG_PTR which varies in respect to target architecture (x86, x64)
using HOOKPROC = System.IntPtr;
using HINSTANCE = System.IntPtr;
using DWORD = System.UInt32;
using LRESULT = System.IntPtr; // A LONG_PTR which varies in respect to target architecture (x86, x64)
using static SharpHotkeys.Native.Delegates;

/* Reference Source: https://docs.microsoft.com/en-us/windows/win32/winprog/windows-data-types
 * Type Rules:
 * HWND -> IntPtr
 * LParam -> IntPtr
 * WParam -> UIntPtr
 * DWORD -> UInt32
 * HHOOK -> IntPtr
 * HOOKPROC -> IntPtr
 * HINSTANCE -> IntPtr
 * LRESULT -> IntPtr
*/

/// <summary>
/// Contains external functions defined in windows native libraries.
/// Read Source: https://docs.microsoft.com/en-us/windows/win32/winmsg/about-hooks
/// Read Source: https://docs.microsoft.com/en-us/previous-versions/windows/desktop/legacy/ms644985(v=vs.85)
/// </summary>
namespace SharpHotkeys.Native
{
    /// <summary>
    /// Contains imported user32.dll registration functions.
    /// </summary>
    internal static class User32
    {
        /// <summary>
        /// User32 DLL that contains these external functions.
        /// </summary>
        const string USER_32 = "User32";

        const string KERNAL_32 = "Kernel32";

        // Read Source: https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-registerhotkey
        [DllImport(USER_32, SetLastError = true)]
        public static extern bool RegisterHotKey(   
            HWND hWnd, 
            int id, 
            uint fsModifiers,
            uint vk
        );

        // Read Source: https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-unregisterhotkey
        [DllImport(USER_32, SetLastError = true)]
        public static extern bool UnregisterHotKey(
            HWND hWnd,
            int id
        );

        // Read Source: https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-callnexthookex
        [DllImport(USER_32)]
        public static extern IntPtr CallNextHookEx(
          HHOOK hhk,
          int nCode,
          WPARAM wParam,
          LPARAM lParam
        );

        // Read Source: https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-setwindowshookexa
        [DllImport(USER_32, SetLastError = true)]
        public static extern HHOOK SetWindowsHookExA(
            int idHook,
            HookCallbackDelegate lpfn, // Pass method address callback function here for intercepting hooks
            HINSTANCE hmod,
            DWORD dwThreadId
        );

        // Read Source: https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-unhookwindowshookex
        [DllImport(USER_32, SetLastError = true)]
        public static extern bool UnhookWindowsHookEx(
          HHOOK hhk
        );

        // https://docs.microsoft.com/en-us/windows/win32/api/errhandlingapi/nf-errhandlingapi-getlasterror
        [DllImport(KERNAL_32)]
        public static extern DWORD GetLastError();

        // https://docs.microsoft.com/en-us/windows/win32/api/libloaderapi/nf-libloaderapi-getmodulehandlea
        [DllImport(KERNAL_32)]
        public static extern IntPtr GetModuleHandle(
            string lpModuleName
        );
    }

    internal class Delegates
    {
        /// <summary>
        /// Defines the delegate of the callback function for hooks that is given to the <see cref="User32.SetWindowsHookExA(int, HWND, HWND, DWORD)"/> function.
        /// </summary>
        /// <param name="nCode">Key unique identifier given when the hot-key was registered.</param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        public delegate LRESULT HookCallbackDelegate(int nCode, WPARAM wParam, LPARAM lParam);
    }
}
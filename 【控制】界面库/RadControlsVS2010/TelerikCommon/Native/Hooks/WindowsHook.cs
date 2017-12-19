using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace Telerik.WinControls
{
    /// <exclude/>
    public class WindowsHook
    {
        #region Win32

        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowsHookEx(HookType code, HookProc func, IntPtr hInstance, int threadID);

        [DllImport("user32.dll")]
        private static extern int UnhookWindowsHookEx(IntPtr hhook);

        [DllImport("user32.dll")]
        private static extern int CallNextHookEx(IntPtr hhook, int code, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        /// <summary>
        /// You can use hook types to monitor and change window messages.
        /// </summary>
        public enum HookType : int
        {
            /// <summary>
            /// Use this hook type to control and record input events.
            /// </summary>
            WH_JOURNALRECORD = 0,
            /// <summary>
            /// Use this hook type to insert messages in the windows system queue. 
            /// For example, you can play recorded mouse events. 
            /// </summary>
            WH_JOURNALPLAYBACK = 1,
            /// <summary>
            /// Use this hook type to monitor the keyboard events.
            /// </summary>
            WH_KEYBOARD = 2,
            /// <summary>
            /// Use this hook type to get messages returned by the GetMessage and PeekMessage
            /// functions.
            /// </summary>
            WH_GETMESSAGE = 3,
            /// <summary>
            /// Use this hook type to get the message passed to some of the windows 
            /// procedures before it's actually handled by the procedure.
            /// </summary>
            WH_CALLWNDPROC = 4,
            /// <summary>
            /// This hook type is used in CBT (computer base training) applications.
            /// </summary>
            WH_CBT = 5,
            /// <summary>
            /// Use this hook type to monitor messages passed to menus, message box, etc and
            /// the activation of different window by a keyboard shortcut.
            /// </summary>
            WH_SYSMSGFILTER = 6,
            /// <summary>
            /// Use this hook type to monitor mouse messages.
            /// </summary>
            WH_MOUSE = 7,
            /// <summary>
            /// 
            /// </summary>
            WH_HARDWARE = 8,
            /// <summary>
            /// Hook procedure that is invoked before any other hook.
            /// </summary>
            WH_DEBUG = 9,
            /// <summary>
            /// Used in shell applications.
            /// </summary>
            WH_SHELL = 10,
            /// <summary>
            /// Invoked when the foreground thread is to become idle. 
            /// </summary>
            WH_FOREGROUNDIDLE = 11,
            /// <summary>
            /// Use this hook type to get the message passed to some of the windows 
            /// procedures after it is handled by the procedure.
            /// </summary>
            WH_CALLWNDPROCRET = 12,
            /// <summary>
            ///Use this hook type to get keyboard events.
            /// </summary>
            WH_KEYBOARD_LL = 13,
            /// <summary>
            ///Use this hook type to get mouse events.
            /// </summary>
            WH_MOUSE_LL = 14
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MouseHookStruct
        {
            public POINT pt;
            public int hwnd;
            public int wHitTestCode;
            public int dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MSLLHOOKSTRUCT
        {
            public POINT pt;
            public uint mouseData;
            public uint flags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        #endregion

        public delegate int HookProc(int code, IntPtr wParam, IntPtr lParam);
        public delegate bool HookEventHandler(object sender, HookEventArgs e);

        protected IntPtr hook = IntPtr.Zero;
        protected HookProc hookFunc = null;
        protected HookType hookType;

        public event HookEventHandler HookInvoked;

        public bool IsInstalled
        {
            get
            {
                return hook != IntPtr.Zero;
            }
        }

        public WindowsHook(HookType hook)
        {
            hookType = hook;
            hookFunc = new HookProc(this.CoreHookProc);
        }

        public void Install()
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                hook = SetWindowsHookEx(hookType, hookFunc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }
        public void Uninstall()
        {
            UnhookWindowsHookEx(hook);
            hook = IntPtr.Zero;
        }

        protected int CoreHookProc(int code, IntPtr wParam, IntPtr lParam)
        {
            if (code < 0)
            {
                return CallNextHookEx(hook, code, wParam, lParam);
            }

            if (OnHookInvoked(new HookEventArgs(code, wParam, lParam)))
                return CallNextHookEx(hook, code, wParam, lParam);

            return code;
        }
        protected virtual bool OnHookInvoked(HookEventArgs e)
        {
            if (HookInvoked != null)
                return HookInvoked(this, e);

            return true;
        }
    }
}

using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Telerik.WinControls;
using System.Collections.Generic;

namespace Telerik.WinControls
{
    /// <summary>
    /// Represents a custom MessageFilter implementation, which uses Hooks rather than the IMessageFilter interface.
    /// This is needed because sometimes we are in an unmanaged environment (like in MFC or in Outlook).
    /// </summary>
    public sealed class RadMessageFilter
    {
        #region Fields

        [ThreadStatic]//the only instance is per thread - each UI thread is associated with a message queue
        private static RadMessageFilter instance;
        private InstalledHook installedHook;
        private WeakReferenceList<IMessageListener> listeners;
        private bool enabled;

        private HookProc getMessageDelegate;
        private HookProc callWndProcDelegate;
        private HookProc systemMessageDelegate;

        private IntPtr getMessageHookAddress;
        private IntPtr callWndProcHookAddress;
        private IntPtr systemMessageHookAddress;

        #endregion

        #region Constructor

        private RadMessageFilter()
        {
            this.getMessageDelegate = new HookProc(GetMessageHookProc);
            this.callWndProcDelegate = new HookProc(CallWndProcHookProc);
            this.systemMessageDelegate = new HookProc(SystemMessageHookProc);

            this.installedHook = InstalledHook.None;
            this.listeners = new WeakReferenceList<IMessageListener>(true);
            this.enabled = true;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the only instance of the filter.
        /// </summary>
        public static RadMessageFilter Instance
        {
            get
            {
                if (instance == null)
                {
                    //we do not need to perform explicit critical section entering here since the instance is unique for each thread
                    instance = new RadMessageFilter();
                }

                return instance;
            }
        }

        /// <summary>
        /// Determines whether the filter is currently enabled.
        /// If false, all hooks will be removed.
        /// If true, all the needed hooks, depending on the registered listeners, will be installed.
        /// </summary>
        public bool Enabled
        {
            get
            {
                return this.enabled;
            }
            set
            {
                if (this.enabled == value)
                {
                    return;
                }

                this.enabled = value;
                this.OnEnabledChanged();
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Registers the specified listener.
        /// </summary>
        /// <param name="listener"></param>
        public void AddListener(IMessageListener listener)
        {
            if (this.ContainsListener(listener))
            {
                return;
            }

            this.listeners.Add(listener);
            this.EnsureInstalledHooks();
        }

        /// <summary>
        /// Removes the specified listener.
        /// </summary>
        /// <param name="listener"></param>
        public void RemoveListener(IMessageListener listener)
        {
            this.listeners.Remove(listener);
            this.EnsureInstalledHooks();
        }

        /// <summary>
        /// Removes all registered listeners and uninstalls all hooks.
        /// </summary>
        public void RemoveAll()
        {
            this.listeners.Clear();
            this.Uninstall(InstalledHook.All);
        }

        /// <summary>
        /// Determines whether the specified listener is registered with this filter.
        /// </summary>
        /// <param name="listener"></param>
        /// <returns></returns>
        public bool ContainsListener(IMessageListener listener)
        {
            return this.listeners.IndexOf(listener) >= 0;
        }

        /// <summary>
        /// Gets the currently installed hooks.
        /// </summary>
        public InstalledHook InstalledHook
        {
            get
            {
                return this.installedHook;
            }
        }

        #endregion

        #region Hook Install/Uninstall

        private void OnEnabledChanged()
        {
            if (this.enabled)
            {
                this.EnsureInstalledHooks();
            }
            else
            {
                this.Uninstall(InstalledHook.All);
            }
        }

        private void Uninstall(InstalledHook toUninstall)
        {
            if ((toUninstall & InstalledHook.CallWndProc) != 0 &&
                (this.installedHook & InstalledHook.CallWndProc) != 0)
            {
                UnhookWindowsHookEx(this.callWndProcHookAddress);
                this.callWndProcHookAddress = IntPtr.Zero;
                this.installedHook &= ~InstalledHook.CallWndProc;
            }

            if ((toUninstall & InstalledHook.GetMessage) != 0 &&
                (this.installedHook & InstalledHook.GetMessage) != 0)
            {
                UnhookWindowsHookEx(this.getMessageHookAddress);
                this.getMessageHookAddress = IntPtr.Zero;
                this.installedHook &= ~InstalledHook.GetMessage;
            }

            if ((toUninstall & InstalledHook.SystemMessage) != 0 &&
                (this.installedHook & InstalledHook.SystemMessage) != 0)
            {
                UnhookWindowsHookEx(this.systemMessageHookAddress);
                this.systemMessageHookAddress = IntPtr.Zero;
                this.installedHook &= ~InstalledHook.SystemMessage;
            }
        }

        private void Install(InstalledHook toInstall)
        {
            //the corresponding windows APIs work only with the obsolete method! 
#pragma warning disable 0618
            int threadId = AppDomain.GetCurrentThreadId();
#pragma warning restore 0618

            if ((toInstall & InstalledHook.CallWndProc) != 0 &&
                (this.installedHook & InstalledHook.CallWndProc) == 0)
            {
                this.callWndProcHookAddress = SetWindowsHookEx(HookType.WH_CALLWNDPROC, this.callWndProcDelegate, IntPtr.Zero, threadId);
                this.installedHook |= InstalledHook.CallWndProc;
            }

            if ((toInstall & InstalledHook.GetMessage) != 0 &&
                (this.installedHook & InstalledHook.GetMessage) == 0)
            {
                this.getMessageHookAddress = SetWindowsHookEx(HookType.WH_GETMESSAGE, this.getMessageDelegate, IntPtr.Zero, threadId);
                this.installedHook |= InstalledHook.GetMessage;
            }

            if ((toInstall & InstalledHook.SystemMessage) != 0 &&
                (this.installedHook & InstalledHook.SystemMessage) == 0)
            {
                this.systemMessageHookAddress = SetWindowsHookEx(HookType.WH_MSGFILTER, this.systemMessageDelegate, IntPtr.Zero, threadId);
                this.installedHook |= InstalledHook.SystemMessage;
            }
        }

        /// <summary>
        /// Verifies that all the needed hooks are installed (depending on each listener's DesiredHook).
        /// </summary>
        private void EnsureInstalledHooks()
        {
            //we are not enabled, do nothing
            if (!this.enabled)
            {
                return;
            }

            InstalledHook desiredHook = this.GetDesiredHook();
            if (this.installedHook == desiredHook)
            {
                return;
            }

            InstalledHook toInstall = InstalledHook.None;
            InstalledHook toUninstall = InstalledHook.None;

            if ((desiredHook & InstalledHook.CallWndProc) == InstalledHook.CallWndProc)
            {
                toInstall |= InstalledHook.CallWndProc;
            }
            else
            {
                toUninstall |= InstalledHook.CallWndProc;
            }

            if ((desiredHook & InstalledHook.GetMessage) == InstalledHook.GetMessage)
            {
                toInstall |= InstalledHook.GetMessage;
            }
            else
            {
                toUninstall |= InstalledHook.GetMessage;
            }

            if ((desiredHook & InstalledHook.SystemMessage) == InstalledHook.SystemMessage)
            {
                toInstall |= InstalledHook.SystemMessage;
            }
            else
            {
                toUninstall |= InstalledHook.SystemMessage;
            }

            this.Install(toInstall);
            this.Uninstall(toUninstall);
        }

        private InstalledHook GetDesiredHook()
        {
            InstalledHook hook = InstalledHook.None;
            foreach (IMessageListener listener in this.listeners)
            {
                InstalledHook desired = listener.DesiredHook;
                if ((desired & InstalledHook.CallWndProc) == InstalledHook.CallWndProc)
                {
                    hook |= InstalledHook.CallWndProc;
                }
                if ((desired & InstalledHook.GetMessage) == InstalledHook.GetMessage)
                {
                    hook |= InstalledHook.GetMessage;
                }
                if ((desired & InstalledHook.SystemMessage) == InstalledHook.SystemMessage)
                {
                    hook |= InstalledHook.SystemMessage;
                }
            }

            return hook;
        }

        #endregion

        #region Hook Callbacks

        private int GetMessageHookProc(int code, IntPtr wParam, IntPtr lParam)
        {
            //code param should be non-negative and wParam should be higher than zero (PM_REMOVE - the message is removed from the pump)
            if (code >= 0 && (long)wParam > 0)
            {
                Message msg = (Message)Marshal.PtrToStructure(lParam, typeof(Message));
                if ((this.NotifyGetMessageEvent(ref msg) & MessagePreviewResult.NoDispatch) == MessagePreviewResult.NoDispatch)
                {
                    //by specifying (-1) as target hwnd we are preventing the system from dispatching the message
                    msg.HWnd = new IntPtr(-1);
                    msg.Msg = -1;
                    Marshal.StructureToPtr(msg, lParam, true);
                }
            }

            return CallNextHookEx(this.getMessageHookAddress, code, wParam, lParam);
        }

        private int CallWndProcHookProc(int code, IntPtr wParam, IntPtr lParam)
        {
            if (code >= 0)
            {
                this.NotifyCallWndProcEvent(wParam, lParam);
            }

            return CallNextHookEx(this.callWndProcHookAddress, code, wParam, lParam);
        }

        private int SystemMessageHookProc(int code, IntPtr wParam, IntPtr lParam)
        {
            if (code >= 0)
            {
                this.NotifyMessageFilterEvent((SystemMessage)code, lParam);
            }

            return CallNextHookEx(this.systemMessageHookAddress, code, wParam, lParam);
        }

        private MessagePreviewResult NotifyGetMessageEvent(ref Message msg)
        {
            MessagePreviewResult result = MessagePreviewResult.NotProcessed;
            List<IMessageListener> currentListeners = new List<IMessageListener>(this.listeners);

            foreach (IMessageListener listener in currentListeners)
            {
                if ((listener.DesiredHook & InstalledHook.GetMessage) == 0)
                {
                    continue;
                }

                MessagePreviewResult singleResult = listener.PreviewMessage(ref msg);
                if ((singleResult & MessagePreviewResult.Processed) == MessagePreviewResult.Processed)
                {
                    result |= MessagePreviewResult.Processed;
                }
                if ((singleResult & MessagePreviewResult.NoDispatch) == MessagePreviewResult.NoDispatch)
                {
                    result |= MessagePreviewResult.NoDispatch;
                }
                if ((singleResult & MessagePreviewResult.NoContinue) == MessagePreviewResult.NoContinue)
                {
                    result |= MessagePreviewResult.NoContinue;
                    break;
                }
            }

            return result;
        }

        private void NotifyCallWndProcEvent(IntPtr wParam, IntPtr lParam)
        {
            CWPSTRUCT cwp = (CWPSTRUCT)Marshal.PtrToStructure(lParam, typeof(CWPSTRUCT));
            Message msg = Message.Create(cwp.hWnd, cwp.msg, cwp.wParam, cwp.lParam);

            foreach (IMessageListener listener in this.listeners)
            {
                if ((listener.DesiredHook & InstalledHook.CallWndProc) == InstalledHook.CallWndProc)
                {
                    listener.PreviewWndProc(msg);
                }
            }
        }

        private void NotifyMessageFilterEvent(SystemMessage message, IntPtr lParam)
        {
            Message msg = (Message)Marshal.PtrToStructure(lParam, typeof(Message));

            foreach (IMessageListener listener in this.listeners)
            {
                if ((listener.DesiredHook & InstalledHook.SystemMessage) == InstalledHook.SystemMessage)
                {
                    listener.PreviewSystemMessage(message, msg);
                }
            }
        }

        #endregion

        #region Native

        /// <summary>
        /// You can use hook types to monitor and change window messages.
        /// </summary>
        private enum HookType : int
        {
            WH_MSGFILTER = -1,
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

        private delegate int HookProc(int code, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowsHookEx(HookType code, HookProc func, IntPtr hInstance, int threadID);

        [DllImport("user32.dll")]
        private static extern int UnhookWindowsHookEx(IntPtr hhook);

        [DllImport("user32.dll")]
        private static extern int CallNextHookEx(IntPtr hhook, int code, IntPtr wParam, IntPtr lParam);

        [StructLayout(LayoutKind.Sequential)]
        private struct MouseHookStruct
        {
            public NativeMethods.POINT pt;
            public int hwnd;
            public int wHitTestCode;
            public int dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct CWPSTRUCT
        {
            public IntPtr lParam;
            public IntPtr wParam;
            public int msg;
            public IntPtr hWnd;
        }

        #endregion
    }
}

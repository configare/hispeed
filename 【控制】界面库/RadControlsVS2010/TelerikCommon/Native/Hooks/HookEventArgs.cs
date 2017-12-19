using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls
{
    /// <exclude/>
    public class HookEventArgs : EventArgs
    {
        public int HookCode;
        public IntPtr wParam;
        public IntPtr lParam;

        public HookEventArgs()
        {
        }
        public HookEventArgs(int hookCode, IntPtr wParam, IntPtr lParam)
        {
            this.HookCode = hookCode;
            this.wParam = wParam;
            this.lParam = lParam;
        }
    }
}

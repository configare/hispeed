using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;
using GeoDo.RSS.UI.WinForm;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.AppEnv;

namespace SMART
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Process instance = RunningInstance();
            if (instance == null)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                frmFlash frm = new frmFlash();
#if SINGLECOMPUTER
#else
                //if (Configer.IsCheckUser)
                //    using (FrmLogin frmLogin = new FrmLogin())
                //    {
                //        if (frmLogin.ShowDialog() == DialogResult.Cancel)
                //            return;
                //    }
#endif
                frm.Show();
                using (CheckEnvironmentIsOK checker = new CheckEnvironmentIsOK(frm as IStartProgress))
                {
                    if (!checker.Check())
                    {
                        frm.Close();
                        return;
                    }
                }
                frm.PrintStartInfo("正在加载模块......");
                Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
                Application.Run(new frmMainForm(frm as IStartProgress));
            }
            else
            {
                MessageBox.Show(null, "系统已经在运行！", "系统提示");
                HandleRunningInstance(instance);
            }
        }

        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            if (e.Exception != null)
                MsgBox.ShowInfo(e.Exception.Message);
            else
                MsgBox.ShowInfo("发生未知异常");
        }

        private static Process RunningInstance()
        {
            Process current = Process.GetCurrentProcess();
            Process[] processes = Process.GetProcessesByName(current.ProcessName);
            foreach (Process process in processes)
            {
                if (process.Id != current.Id)
                {
                    if (Assembly.GetExecutingAssembly().Location.Replace("/", "\\") == current.MainModule.FileName)
                    {
                        return process;
                    }
                }
            }
            return null;
        }

        public static void HandleRunningInstance(Process instance)
        {
            ShowWindowAsync(instance.MainWindowHandle, SW_SHOW);
            SetForegroundWindow(instance.MainWindowHandle);
        }

        [DllImport("User32.dll")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int cmdShow);
        [DllImport("User32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        private const int SW_HIDE = 0;
        private const int SW_SHOWNORMAL = 1;
        private const int SW_SHOWMINIMIZED = 2;
        private const int SW_SHOWMAXIMIZED = 3;
        private const int SW_MAXIMIZE = 3;
        private const int SW_SHOWNOACTIVATE = 4;
        private const int SW_SHOW = 5;
        private const int SW_MINIMIZE = 6;
        private const int SW_SHOWMINNOACTIVE = 7;
        private const int SW_SHOWNA = 8;
        private const int SW_RESTORE = 9;
    }
}

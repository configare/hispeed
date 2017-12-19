using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Excel;
using System.IO;

namespace GeoDo.RSS.UI.AddIn.Office
{
    public partial class WinExcelControl : UserControl
    {
        #region "API usage declarations"
        [DllImport("user32.dll")]
        public static extern int FindWindow(string strclassName, string strWindowName);

        [DllImport("user32.dll")]
        static extern int SetParent(int hWndChild, int hWndNewParent);

        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        static extern bool SetWindowPos(
            int hWnd,               // handle to window
            int hWndInsertAfter,    // placement-order handle
            int X,                  // horizontal position
            int Y,                  // vertical position
            int cx,                 // width
            int cy,                 // height
            uint uFlags             // window-positioning options
            );

        [DllImport("user32.dll", EntryPoint = "MoveWindow")]
        static extern bool MoveWindow(
            int hWnd,
            int X,
            int Y,
            int hWidth,
            int hHeight,
            bool bRepaint
            );

        const int SWP_DRAWFRAME = 0x20;
        const int SWP_NOMOVE = 0x2;
        const int SWP_NOSIZE = 0x1;
        const int SWP_NOZORDER = 0x4;


        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowThreadProcessId(IntPtr hwnd, out int ID);

        #endregion

        public Excel.Application xlApp = null;
        public _Workbook workbook = null;
        private bool deactivateevents = false;
        public int xlsWnd = 0;
        public string xlsFileName = null;
        public _Worksheet worksheet;
        private object missing = System.Reflection.Missing.Value;

        public WinExcelControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 关闭当前Excel文档
        /// </summary>
        public void CloseControl()
        {
            if (xlApp != null)
            {
                xlApp.WorkbookBeforeClose -= new Excel.AppEvents_WorkbookBeforeCloseEventHandler(OnClose);
                xlApp.NewWorkbook -= new Excel.AppEvents_NewWorkbookEventHandler(OnNewXls);
                xlApp.WorkbookOpen -= new Excel.AppEvents_WorkbookOpenEventHandler(OnOpenXls);
            }
            if (workbook != null)
            {
                deactivateevents = true;
                object dummy = null;
                object dummyFalse = (object)false;
                /*
                 * 2012-09-05  by fdc
                 * Excel COM组件引用计数置零，释放Excel COM对象
                 */
                //((Excel._Workbook)workbook).Close(dummyFalse, dummy, dummy);
                //xlApp.Quit();
                Marshal.FinalReleaseComObject(workbook);
                Marshal.FinalReleaseComObject(xlApp);
                Kill();
                deactivateevents = false;
                workbook = null;
            }
        }

        /// <summary>
        /// catches Excel's close event 
        /// starts a Thread that send a ESC to the Excel window ;)
        /// </summary>
        private void OnClose(Excel.Workbook xls, ref bool cancel)
        {
            if (!deactivateevents)
            {
                cancel = true;
            }
        }

        /// <summary>
        /// catches Excel's open event
        /// just close
        /// </summary>
        public void OnNewCreateXls()
        {
            LoadNewWorkbook();
        }


        /// <summary>
        /// catches Excel's open event
        /// just close
        /// </summary>
        public void OnOpenXls(Excel.Workbook xls)
        {
            OnNewXls(xls);
        }

        /// <summary>
        /// catches Excel's newworkbook event
        /// just close
        /// </summary>
        private void OnNewXls(Excel.Workbook xls)
        {
            if (!deactivateevents)
            {
                deactivateevents = true;
                object dummy = null;
                ((Excel._Workbook)workbook).Close(dummy, dummy, dummy);
                deactivateevents = false;
            }
        }

        /// <summary>
        /// catches Excel's quit event
        /// normally it should not fire, but just to be shure
        /// safely release the internal Excel Instance 
        /// </summary>
        private void OnQuit()
        {
            xlApp = null;
        }

        public void LoadNewWorkbook()
        {
            deactivateevents = true;

            if (xlApp == null)
                xlApp = new Excel.Application();
            try
            {
                xlApp.CommandBars.AdaptiveMenus = false;
                xlApp.WorkbookBeforeClose += new Excel.AppEvents_WorkbookBeforeCloseEventHandler(OnClose);
                xlApp.NewWorkbook += new Excel.AppEvents_NewWorkbookEventHandler(OnNewXls);
                xlApp.WorkbookOpen += new Excel.AppEvents_WorkbookOpenEventHandler(OnOpenXls);
            }
            catch 
            { }

            if (workbook != null)
            {
                try
                {
                    xlApp.Workbooks.Close();
                    Kill();
                }
                catch { }
            }

            if (xlsWnd == 0)
                xlsWnd = FindWindow("XLMAIN", null);
            if (xlsWnd != 0)
            {
                SetParent(xlsWnd, this.Handle.ToInt32());
                try
                {
                    if (xlApp == null)
                        throw new ExcelInstanceException();

                    if (xlApp.Workbooks == null)
                        throw new WorkbookInstanceException();

                    if (xlApp != null && xlApp.Workbooks != null)
                    {
                        Workbooks workbooks = xlApp.Workbooks;
                        workbook = workbooks.Add(XlWBATemplate.xlWBATWorksheet);
                        Sheets sheets = workbook.Worksheets;
                        worksheet = (_Worksheet)sheets.get_Item(1);
                        worksheet.Name = "表格";
                    }

                    if (workbook == null)
                        throw new ValidWorkbookException();
                }
                catch
                { }

                try
                {
                    xlApp.Visible = true;
                    xlApp.UserControl = true;
                    SetWindowPos(xlsWnd, this.Handle.ToInt32(), 0, 0, this.Bounds.Width, this.Bounds.Height, SWP_NOZORDER | SWP_NOMOVE | SWP_DRAWFRAME | SWP_NOSIZE);
                    OnResize();
                }
                catch
                {
                    //MsgBox.ShowInfo("不能加载Excel文档!");
                }
            }
            deactivateevents = false;
        }

        public void DoActive()
        {
            xlApp.SendKeys("{F2}", missing);
        }

        /// <summary>
        /// internal resize function
        /// utilizes the size of the surrounding control
        /// 
        /// optimzed for Excel2000 but it works pretty good with WordXP too.
        /// </summary>
        private void OnResize()
        {
            //The original one that I used is shown below. Shows the complete window, but its buttons (min, max, restore) are disabled
            //// MoveWindow(xlsWnd,0,0,this.Bounds.Width,this.Bounds.Height,true);

            ///Change below
            ///The following one is better, if it works for you. We donot need the title bar any way. Based on a suggestion.
            int borderWidth = SystemInformation.Border3DSize.Width;
            int borderHeight = SystemInformation.Border3DSize.Height;
            int captionHeight = SystemInformation.CaptionHeight;
            int statusHeight = SystemInformation.ToolWindowCaptionHeight;
            MoveWindow(
                xlsWnd,
                -2 * borderWidth,
                -2 * borderHeight - captionHeight,
                this.Bounds.Width + 4 * borderWidth,
                this.Bounds.Height + captionHeight + 4 * borderHeight + statusHeight,
                true);
        }

        private void OnResize(object sender, System.EventArgs e)
        {
            OnResize();
        }

        public class ExcelInstanceException : Exception
        { }

        public class WorkbookInstanceException : Exception
        { }

        public class ValidWorkbookException : Exception
        { }

        public void Kill()
        {
            IntPtr t = new IntPtr(xlsWnd); //得到这个句柄，具体作用是得到这块内存入口
            int k = 0;
            GetWindowThreadProcessId(t, out k); //得到本进程唯一标志k
            if (k != 0)
            {
                System.Diagnostics.Process p = System.Diagnostics.Process.GetProcessById(k); //得到对进程k的引用
                p.Kill(); //关闭进程k
            }
        }

        public void Open(string file)
        {
            deactivateevents = true;
            try
            {
                if (xlApp == null)
                    xlApp = new Excel.Application();
                if (xlsWnd == 0)
                    xlsWnd = FindWindow("XLMAIN", null);
                try
                {
                    xlApp.CommandBars.AdaptiveMenus = false;
                    xlApp.WorkbookBeforeClose += new Excel.AppEvents_WorkbookBeforeCloseEventHandler(OnClose);
                    SetParent(xlsWnd, this.Handle.ToInt32());
                    xlApp.Visible = true;
                    xlApp.UserControl = true;
                    SetWindowPos(xlsWnd, this.Handle.ToInt32(), 0, 0, this.Bounds.Width, this.Bounds.Height, SWP_NOZORDER | SWP_NOMOVE | SWP_DRAWFRAME | SWP_NOSIZE);
                    OnResize();
                }
                catch { }
                xlApp.Workbooks.Open(file, missing, missing, missing, missing, missing, missing, missing, missing, missing, missing, missing, missing);
                UnLock();
                workbook = xlApp.Workbooks.get_Item(1);
            }
            finally
            {
                deactivateevents = false;
            }
        }

        public void UnLock()
        {
            SendKeys.Send("{F2}");
            SendKeys.Send("{ENTER}");
            System.Windows.Forms.Application.DoEvents();
        }
    }
}

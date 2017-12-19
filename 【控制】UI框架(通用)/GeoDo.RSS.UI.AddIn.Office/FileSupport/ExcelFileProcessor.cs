using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.UI;
using System.ComponentModel.Composition;
using Telerik.WinControls.UI.Docking;
using System.IO;

namespace GeoDo.RSS.UI.AddIn.Office
{
    [Export(typeof(IOpenFileProcessor)), ExportMetadata("VERSION", "1")]
    public class ExcelFileProcessor : OpenFileProcessor
    {
        public ExcelFileProcessor()
            : base()
        {
            _extNames.Add(".XLS");
            _extNames.Add(".XLSX");
        }

        protected override bool FileHeaderIsOK(string fname, string extName)
        {
            return true;
        }


        public override bool Open(string fname,out bool memoryIsNotEnough)
        {
            memoryIsNotEnough = false;
            ISmartToolWindow toolWindow = new frmExcelStatResultWnd();// _session.SmartWindowManager.SmartToolWindowFactory.GetSmartToolWindow(9004);
            if (toolWindow != null)
            {
                (toolWindow as ToolWindow).Name = GetTextByFileName(fname);
                (toolWindow as ToolWindow).Text = (toolWindow as ToolWindow).Name;
                (toolWindow as IStatResultDisplayWindow).Open(fname);
                _session.SmartWindowManager.DisplayWindow(toolWindow);
                return true;
            }
            return false;
        }

        public static string GetTextByFileName(string fname)
        {
            if (string.IsNullOrEmpty(fname))
                return "未命名";
            try
            {
                string txt = Path.GetFileName(fname);
                return "...\\" + txt;
            }
            catch
            {
                return "未命名";
            }
        }
    }
}

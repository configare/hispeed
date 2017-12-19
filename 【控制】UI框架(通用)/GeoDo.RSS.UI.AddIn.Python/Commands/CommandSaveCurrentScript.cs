using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.UI;
using System.Windows.Forms;
using System.IO;
using System.ComponentModel.Composition;
using Telerik.WinControls.UI.Docking;

namespace GeoDo.RSS.UI.AddIn.Python
{
    [Export(typeof(ICommand))]
    class CommandSaveCurrentScript  : Command
    {
        public CommandSaveCurrentScript()
            : base()
        {
            _id = 20004;
            _name = "PythonEditorSave";
            _text = "保存脚本";
            _toolTip = "保存脚本";
        }

        //重载EXECUTE函数
        public override void Execute(string argument)
        {
            string source_code;
            string file_name;
            try
            {
                if (argument == "ALL")
                {
                    int i = 0;
                    IPythonEditorWindow pew;
                    ISmartWindow[] wnds = _smartSession.SmartWindowManager.GetSmartWindows((wnd) => { return wnd.GetType().Equals(typeof(PythonMonitorWindow)); });
                    if (wnds == null || wnds.Length == 0)
                        return;
                    ISmartViewer[] views = wnds.Cast<ISmartViewer>().ToArray();
                    for (i = 0; i < views.Count(); i++)
                    {
                        pew = views[0] as IPythonEditorWindow;
                        source_code = pew.getTB().Text;
                        ToolWindow tw = views[0] as ToolWindow;
                        file_name = tw.Text;
                        save_source(source_code, file_name);
                    }

                    wnds = _smartSession.SmartWindowManager.GetSmartWindows((wnd) => { return wnd.GetType().Equals(typeof(PythonManagerWindow)); });
                    if (wnds == null || wnds.Length == 0)
                        return;
                    views = wnds.Cast<ISmartViewer>().ToArray();
                    IPythonManagerWindow wmx;
                    for (i = 0; i < views.Count(); i++)
                    {
                        wmx = views[i] as IPythonManagerWindow;
                        if (wmx != null)
                            wmx.RefreshWorkspace();
                    }

                }
                else
                    return;
            }
            catch
            { }
        }

        public void save_source(string source, string fn)
        {
            //MessageBox.Show(fn);
            if (fn =="") return;
            File.WriteAllText(fn,source,Encoding.Default);
        }
    }
}

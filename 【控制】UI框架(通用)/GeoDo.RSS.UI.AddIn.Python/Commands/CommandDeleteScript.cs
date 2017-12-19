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
    class CommandDeleteScript : Command
    {
        public CommandDeleteScript()
            : base()
        {
            _id = 20006;
            _name = "PythonDeleteFile";
            _text = "删除脚本文件";
            _toolTip = "删除脚本文件";
        }

        public override void Execute()
        {
           ISmartWindow[] wnds = _smartSession.SmartWindowManager.GetSmartWindows((wnd) => { return wnd.GetType().Equals(typeof(PythonManagerWindow)); });
           if (wnds == null || wnds.Length == 0)
               return;
           ISmartViewer[] views = wnds.Cast<ISmartViewer>().ToArray();
           if (views != null && views.Count() > 0)
           {
               IPythonManagerWindow pmw = views[0] as IPythonManagerWindow;
               TreeNode tn = pmw.GetTree().SelectedNode;
               if ((tn == null)||(tn.Nodes.Count != 0)) return;
    
               else
               {


                   DialogResult dr = MessageBox.Show("是否确认删除文件"+tn.Text, "提示信息", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                   if (dr == DialogResult.OK)
                   {
                       File.Delete(Application.StartupPath + "\\SystemData\\" + tn.FullPath);
                       pmw.RefreshWorkspace();
                   }
               }

           }
        
        
        }

    }
}

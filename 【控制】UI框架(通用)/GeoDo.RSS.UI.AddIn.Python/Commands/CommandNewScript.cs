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
    class CommandNewScript : Command
    {
       public CommandNewScript()
            : base()
        {
            _id = 20005;
            _name = "CreatePythonFile";
            _text = "新建脚本";
            _toolTip = "新建脚本";
        }

       public override void Execute()
       {
           using (FileDialog dlg = new SaveFileDialog())
           {
               dlg.InitialDirectory = Application.StartupPath+"\\SystemData\\Scripts";
               dlg.Title = "新建脚本文件";
               dlg.Filter = "*.py|*.PY";
               if (dlg.ShowDialog() == DialogResult.OK)
                   _smartSession.CommandEnvironment.Get(20003).Execute("FILL|FILL|" + dlg.FileName);
               else
                   return;
           }
       }
    }
}

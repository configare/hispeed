using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.UI;

namespace GeoDo.RSS.UI.AddIn.AddInMgr
{
    [Export(typeof(ICommand)), ExportMetadata("VERSION", "1")]
    public class CommandPluginExecute:Command
    {

        public CommandPluginExecute()
        {
            _id = 8200;
            _name = "PluginExecute";
            _text = _toolTip = "执行插件";
        }

        public override void Execute()
        {
        }

        public override void Execute(string argument)
        {
            string[] arguments = argument.Split(new char[] { ':' });
            string fileDir = arguments[0] + ":" + arguments[1];
            if (!File.Exists(fileDir))
                return;
            System.Diagnostics.Process pExecuteEXE = new System.Diagnostics.Process();
            pExecuteEXE.StartInfo.FileName = fileDir;
            pExecuteEXE.StartInfo.Arguments = arguments[2];
            pExecuteEXE.Start();
            pExecuteEXE.WaitForExit();
        }
    }
}

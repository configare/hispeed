using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.UI;
using System.IO;
using System.Diagnostics;

namespace GeoDo.RSS.UI.AddIn.Help
{
    [Export(typeof(ICommand))]
    class CommandProgramyHelp:Command
    {
        public CommandProgramyHelp()
            : base()
        {
            _id = 21002;
            _text = _toolTip = "操作手册";
        }

        public override void Execute()
        {
            string filename = AppDomain.CurrentDomain.BaseDirectory + "卫星监测分析与遥感应用系统用户手册.pdf";
            if (File.Exists(filename))
                Process.Start(filename);
        }
    }
}

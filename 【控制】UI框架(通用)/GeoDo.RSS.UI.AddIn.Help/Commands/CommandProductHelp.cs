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
    class CommandProductHelp:Command
    {
        public CommandProductHelp()
            : base()
        {
            _id = 21001;
            _text = _toolTip = "算法手册";
        }

        public override void Execute()
        {
            string filename = AppDomain.CurrentDomain.BaseDirectory + "卫星监测分析与遥感应用系统监测产品原理方法简易手册.pdf";
            if (File.Exists(filename))
                Process.Start(filename);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.UI;
using System.ComponentModel.Composition;

namespace GeoDo.RSS.UI.AddIn.DataPro
{
    [Export(typeof(ICommand)), ExportMetadata("VERSION", "1")]
    public class cmdSaveAdjustArgsToSolution : Command
    {
        public cmdSaveAdjustArgsToSolution()
            : base()
        {
            _id = 30005;
            _text = _name = "保存为方案";
        }

        public override void Execute()
        {
            base.Execute();
        }
    }
}

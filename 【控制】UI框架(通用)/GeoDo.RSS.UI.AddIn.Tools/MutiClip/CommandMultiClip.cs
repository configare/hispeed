using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.UI;
using System.ComponentModel.Composition;
using GeoDo.RSS.RasterTools;
using GeoDo.RSS.Core.RasterDrawing;
using System.IO;
using GeoDo.RSS.Core.DF;
using System.Windows.Forms;

namespace GeoDo.RSS.UI.AddIn.Tools
{
    [Export(typeof(ICommand)), ExportMetadata("VERSION", "1")]
    public class CommandMultiClip : Command
    {
        public CommandMultiClip()
            : base()
        {
            _name = "Multiclip";
            _text = _toolTip = "批量裁切";
            _id = 27101;
        }

        public override void Execute(string argument)
        {
            base.Execute(argument);
        }

        public override void Execute()
        {
            frmMultiClip frm = new frmMultiClip();
            frm.Show();
        }

       
    }
}

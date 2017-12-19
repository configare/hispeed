using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.UI;
using System.Windows.Forms;

namespace GeoDo.RSS.UI.AddIn.Tools
{
    [Export(typeof(ICommand)), ExportMetadata("VERSION", "1")]
    public class CommandVector2Raster:Command
    {
        public CommandVector2Raster()
            : base()
        {
            _name = "CommandVector2Raster";
            _text = _toolTip = "矢量栅格化工具";
            _id = 7112;
        }

        public override void Execute(string argument)
        {
            base.Execute();
        }

        public override void Execute()
        {
           using(frmVectorToRaster frm=new frmVectorToRaster())
           {
               frm.StartPosition = FormStartPosition.CenterScreen;
               frm.ShowDialog();
           }
        }
    }
}

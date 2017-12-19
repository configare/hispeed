using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.Core.RasterDrawing;
using GeoDo.RSS.Core.DF;
using System.Drawing;

namespace GeoDo.RSS.UI.AddIn.Tools
{
    [Export(typeof(ICommand)), ExportMetadata("VERSION", "1")]
    public class cmdProductColorTable : Command
    {
        public cmdProductColorTable()
            : base()
        {
            _name = "cmdProductColorTable";
            _text = _toolTip = "产品颜色表";
            _id = 10012;
        }

        public override void Execute()
        {
            Execute(null);
        }

        public override void Execute(string argument)
        {
            using (frmProductColorTable frm = new frmProductColorTable())
            {
                frm.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
                frm.ShowDialog();
            }
        }
    }
}

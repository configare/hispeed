using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.UI;
using System.Windows.Forms;
using GeoDo.RSS.Layout;

namespace GeoDo.RSS.UI.AddIn.Layout
{
    [Export(typeof(ICommand)), ExportMetadata("VERSION", "1")]
    public class CmdSaveDocument : GeoDo.RSS.Core.UI.Command
    {
        public CmdSaveDocument()
            : base()
        {
            _id = 6036;
            _name = "SaveThemeDocument";
            _text = _toolTip = "保存专题图文档";
        }

        public override void Execute()
        {
            ILayoutViewer viewer = _smartSession.SmartWindowManager.ActiveViewer as ILayoutViewer;
            if (viewer == null)
                return;
            ILayoutHost host = viewer.LayoutHost;
            if (host == null)
                return;
            using (SaveFileDialog dlg = new SaveFileDialog())
            {
                dlg.Filter = "Smart Theme Document(*.gxd)|*.gxd";
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    host.SaveAsDocument(dlg.FileName);
                }
            }
        }
    }
}
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.UI;
using System.Windows.Forms;
using GeoDo.RSS.Layout;
using GeoDo.RSS.Layout.GDIPlus;
using System.IO;

namespace GeoDo.RSS.UI.AddIn.Layout
{
    [Export(typeof(ICommand)), ExportMetadata("VERSION", "1")]
    public class CmdOpenTemplate : GeoDo.RSS.Core.UI.Command
    {
        public CmdOpenTemplate()
            : base()
        {
            _id = 6028;
            _name = "OpenTemplate";
            _text = _toolTip = "打开专题图模版";
        }

        public override void Execute()
        {
            ILayoutViewer viewer = _smartSession.SmartWindowManager.ActiveViewer as ILayoutViewer;
            if (viewer == null)
                return;
            string fname = null;
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Filter = "专题图模版(*.gxt)|*.gxt";
                dlg.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory + "LayoutTemplate\\自定义";
                if (dlg.ShowDialog() == DialogResult.OK)
                    fname = dlg.FileName;
            }
            ILayoutTemplate template = new LayoutTemplate(fname);
            if (viewer.LayoutHost == null)
                return;
            viewer.LayoutHost.ApplyTemplate(template);
            viewer.LayoutHost.Render();
        }
    }
}

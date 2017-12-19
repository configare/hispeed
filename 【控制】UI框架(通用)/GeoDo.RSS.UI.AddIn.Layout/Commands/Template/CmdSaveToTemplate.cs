using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.Layout;
using System.Windows.Forms;
using System.IO;
using GeoDo.RSS.Layout.GDIPlus;
using System.Drawing.Imaging;

namespace GeoDo.RSS.UI.AddIn.Layout
{
    [Export(typeof(ICommand)), ExportMetadata("VERSION", "1")]
    public class CmdSaveToTemplate : GeoDo.RSS.Core.UI.Command
    {
        public CmdSaveToTemplate()
            : base()
        {
            _id = 6006;
            _name = "SaveToTemplate";
            _text = _toolTip = "另存为模版";
        }

        public override void Execute()
        {
            ILayoutViewer viewer = _smartSession.SmartWindowManager.ActiveViewer as ILayoutViewer;
            if (viewer == null)
                return;
            using (SaveFileDialog dlg = new SaveFileDialog())
            {
                dlg.Filter = "专题图模板(*.gxt)|*.gxt";
                dlg.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory + "LayoutTemplate\\自定义";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    ILayoutTemplate template = new LayoutTemplate(viewer.LayoutHost);
                    template.Name = template.Text = Path.GetFileNameWithoutExtension(dlg.FileName);
                    template.SaveTo(dlg.FileName);
                    using (System.Drawing.Bitmap bm = viewer.LayoutHost.ExportToBitmap(PixelFormat.Format32bppArgb, new System.Drawing.Size(165, 165)))
                    {
                        bm.Save(dlg.FileName.ToLower().Replace(".gxt", ".png"),ImageFormat.Png);
                    }
                }
            }
        }
    }
}

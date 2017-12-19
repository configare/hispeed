using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.UI;
using System.Windows.Forms;
using GeoDo.RSS.Core.VectorDrawing;
using System.IO;
using GeoDo.RSS.RasterTools;
using GeoDo.RSS.Core.RasterDrawing;

namespace GeoDo.RSS.UI.AddIn.Tools
{
    [Export(typeof(ICommand)), ExportMetadata("VERSION", "1")]
    public class CommandSaveContour : Command
    {
        public CommandSaveContour()
            : base()
        {
            _name = "CommandSaveContour";
            _text = _toolTip = "保存等值线";
            _id = 7107;
        }

        public override void Execute()
        {
            if (_smartSession.SmartWindowManager.ActiveCanvasViewer == null)
                return;
            ICanvasViewer canvasViewer = _smartSession.SmartWindowManager.ActiveCanvasViewer;
            var layers = canvasViewer.Canvas.LayerContainer.Layers.Where((lyr) => { return lyr is IContourLayer; });
            if (layers == null || layers.Count() == 0)
                return;
            using (SaveFileDialog dlg = new SaveFileDialog())
            {
                dlg.Filter = "等值线(*.xml)|*.xml";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    Save(dlg.FileName, layers.ToArray(), canvasViewer);
                }
            }
        }

        private void Save(string fname, Core.DrawEngine.ILayer[] layers,ICanvasViewer canvasViewer)
        {
            string fname0 = Path.Combine(Path.GetDirectoryName(fname), Path.GetFileNameWithoutExtension(fname)) + "_({0}).xml";
            if (layers.Length == 1)
                fname0 = fname;
            int idx = 0;
            foreach (IContourLayer lyr in layers)
            {
                fname = string.Format(fname0, idx++);
                lyr.Save(fname,true);
            }
        }

        public override void Execute(string argument)
        {
        }
    }
}

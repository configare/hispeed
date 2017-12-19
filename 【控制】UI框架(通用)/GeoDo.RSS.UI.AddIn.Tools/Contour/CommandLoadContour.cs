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

namespace GeoDo.RSS.UI.AddIn.Tools
{
    [Export(typeof(ICommand)), ExportMetadata("VERSION", "1")]
    public class CommandLoadContour : Command
    {
        public CommandLoadContour()
            : base()
        {
            _name = "CommandLoadContour";
            _text = _toolTip = "打开等值线";
            _id = 7108;
        }

        public override void Execute()
        {
            if (_smartSession.SmartWindowManager.ActiveCanvasViewer == null)
                return;
            ICanvasViewer canvasViewer = _smartSession.SmartWindowManager.ActiveCanvasViewer;
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Filter = "等值线(*.xml)|*.xml";
                if (dlg.ShowDialog() == DialogResult.OK)
                    OpenFileFactory.Open(dlg.FileName);
            }
        }

        public override void Execute(string argument)
        {
            OpenFileFactory.Open(argument);
        }
    }
}

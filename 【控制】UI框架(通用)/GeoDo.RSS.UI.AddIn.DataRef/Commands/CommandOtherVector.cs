using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.UI;
using System.ComponentModel.Composition;
using System.Windows.Forms;
using System.IO;
using GeoDo.RSS.Core.DrawEngine;

namespace GeoDo.RSS.UI.AddIn.DataRef
{
    [Export(typeof(ICommand)), ExportMetadata("VERSION", "1")]
    public class CommandOtherVector : Command
    {
        public CommandOtherVector()
        {
            _id = 4020;
            _name = "AddRasterData";
            _text = _toolTip = "叠加其它矢量数据";
        }

        public override void Execute()
        {
            if (_smartSession.SmartWindowManager.ActiveViewer == null)
                return;
            if (!(_smartSession.SmartWindowManager.ActiveViewer is ICanvasViewer || _smartSession.SmartWindowManager.ActiveViewer is ILayoutViewer))
                return;
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Filter = "矢量文件(*.shp)|*.shp|矢量地图(*.mcd)|*.mcd";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    if (dlg.Multiselect)
                        foreach (string fname in dlg.FileNames)
                            Execute(fname);
                    else
                        Execute(dlg.FileName);
                }
            }
        }

        public override void Execute(string filename)
        {
            ICommand cmd =  _smartSession.CommandEnvironment.Get(4040);
            if (cmd != null)
                cmd.Execute(filename);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.UI;
using System.ComponentModel.Composition;
using System.IO;
using GeoDo.RSS.Core.DrawEngine;
using GeoDo.RSS.Core.RasterDrawing;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.Layout;

namespace GeoDo.RSS.UI.AddIn.DataRef
{
    [Export(typeof(ICommand)), ExportMetadata("VERSION", "1")]
    public class CommandAddVectorData : Command
    {
        public CommandAddVectorData()
        {
            _id = 4040;
            _name = "AddRasterData";
            _text = _toolTip = "叠加矢量数据";
        }

        public override void Execute(string filename)
        {
            if (!File.Exists(filename))
                return;
            OpenFileFactory.Open(filename);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.Core.RasterDrawing;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.CA;
using System.IO;
using System.Windows.Forms;
using GeoDo.RSS.Core.DF;

namespace GeoDo.RSS.UI.AddIn.Theme
{
    [Export(typeof(ICommand))]
    public class CommandSaveImageEnhance : Command
    {
        private string _enhanceName = string.Empty;

        public CommandSaveImageEnhance()
        {
            _id = 9111;
            _name = "CommandSaveEnhance";
            _text = _toolTip = "保存图像增强方案";
        }

        public override void Execute()
        {
            ImageEnhancor enhancor = new ImageEnhancor(_smartSession);
            enhancor.SaveImageEnhance();
        }
    }
}

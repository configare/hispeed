using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.Core.RasterDrawing;
using GeoDo.RSS.Core.CA;
using System.IO;

namespace GeoDo.RSS.UI.AddIn.Theme
{
    [Export(typeof(ICommand))]
    public class CommandApplyImageEnhance:Command
    {
        public CommandApplyImageEnhance()
        {
            _id = 9112;
            _name = "CommandApplyEnhance";
            _text = _toolTip = "应用图像增强方案";
        }

        public override void Execute(string argument)
        {
            ImageEnhancor enhance = new ImageEnhancor(_smartSession);
            enhance.ApplyImageEnhance(argument);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.UI;
using System.ComponentModel.Composition;
using Telerik.WinControls.UI.Docking;

namespace GeoDo.RSS.UI.AddIn.Windows
{
    [Export(typeof(ICommand))]
    public class CommandFileAttributeWindow : CommandToolWindow
    {
        public CommandFileAttributeWindow()
        {
            _id = 9001;
            _name = "FileAttributeWindow";
            _text = _toolTip = "文件属性窗口";
        }
    }
}

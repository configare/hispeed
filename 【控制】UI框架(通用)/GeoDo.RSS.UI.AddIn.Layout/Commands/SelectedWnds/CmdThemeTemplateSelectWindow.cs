using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.Windows.Forms;
using GeoDo.RSS.Core.UI;

namespace GeoDo.RSS.UI.AddIn.Layout
{
    [Export(typeof(ICommand)), ExportMetadata("VERSION", "1")]
    public class CmdThemeTemplateSelectWindow : Command
    {
        public CmdThemeTemplateSelectWindow()
        {
            _id = 5002;
            _name = "ThemeTemplateSelect";
            _text = "专题图模版浏览器";
        }

        public override void Execute()
        {
            ISmartToolWindow wnd = _smartSession.SmartWindowManager.SmartToolWindowFactory.GetSmartToolWindow(_id);
            if (wnd != null)
                _smartSession.SmartWindowManager.DisplayWindow(wnd, new WindowPosition(DockStyle.Left, true));
        }

    }
}

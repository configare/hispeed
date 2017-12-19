using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.UI.AddIn.Windows;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel.Composition;

namespace GeoDo.RSS.MIF.Prds.Comm
{
    [Export(typeof(ICommand))]
    public class CommandThemeGraphRegion : Command
    {
        public CommandThemeGraphRegion()
            : base()
        {
            _id = 19020;
            _name = "CommandThemeGraphRegion";
            _text = "专题图范围";
        }

        public override void Execute(string argument)
        {
            ISmartToolWindow wnd = _smartSession.SmartWindowManager.SmartToolWindowFactory.GetSmartToolWindow(_id); 
            if (wnd != null)
            {
                wnd.Init(_smartSession);
                Form mainFrm = _smartSession.SmartWindowManager.MainForm as Form;
                int wndWidth = 260;
                int wndHeight = 200;
                int x = mainFrm.Right - wndWidth - 7;
                int y = mainFrm.Bottom - wndHeight - 8;
                Rectangle rect = new Rectangle(x, y, wndWidth, wndHeight);
                _smartSession.SmartWindowManager.DisplayWindow(wnd, new WindowPosition(System.Windows.Forms.DockStyle.None, false), rect);
            }
        }
        public override void Execute()
        {
            Execute("");
        }
    }
}

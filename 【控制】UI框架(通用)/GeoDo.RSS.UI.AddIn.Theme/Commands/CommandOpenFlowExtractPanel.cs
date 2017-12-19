using GeoDo.RSS.Core.UI;
using System.ComponentModel.Composition;
using Telerik.WinControls.UI.Docking;
using System.Windows.Forms;

namespace GeoDo.RSS.UI.AddIn.Theme
{
    [Export(typeof(ICommand))]
    public class CommandOpenFlowExtractPanel : Command
    {
        private const int WND_DEFAULT_WIDTH = 320;

        public CommandOpenFlowExtractPanel()
        {
            _id =  90191;
            _name = "CommandOpenFlowExtractPanel";
            _text = _toolTip = "流程面板";
        }

        public override void Execute()
        {
            Execute(null);
        }

        public override void Execute(string argument)
        {
            ISmartToolWindow wnd = _smartSession.SmartWindowManager.SmartToolWindowFactory.GetSmartToolWindow(_id);
            if (wnd != null)
            {
                _smartSession.SmartWindowManager.DisplayWindow(wnd, new WindowPosition(DockStyle.Right, false));
                if((wnd as DockWindow).TabStrip != null)
                    (wnd as DockWindow).TabStrip.SizeInfo.AbsoluteSize = new System.Drawing.Size(WND_DEFAULT_WIDTH, 0);
            }
        }
    }
}

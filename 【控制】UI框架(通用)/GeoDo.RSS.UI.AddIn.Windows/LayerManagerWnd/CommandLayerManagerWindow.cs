using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.UI;
using System.ComponentModel.Composition;
using System.Windows.Forms;
using Telerik.WinControls.UI.Docking;
using GeoDo.RSS.Core.RasterDrawing;
using GeoDo.RSS.Core.DrawEngine;

namespace GeoDo.RSS.UI.AddIn.Windows
{
    [Export(typeof(ICommand))]
    public class CommandLayerManagerWindow : CommandToolWindow
    {
        private const int WND_DEFAULT_WIDTH = 270;

        public CommandLayerManagerWindow()
        {
            _id = 9002;
            _name = "LayerManagerWindow";
            _text = _toolTip = "层管理器";
        }

        public override void Execute()
        {
            ISmartViewer v = _smartSession.SmartWindowManager.ActiveViewer;
            ISmartToolWindow wnd = _smartSession.SmartWindowManager.SmartToolWindowFactory.GetSmartToolWindow(_id);
            if (wnd != null)
            {
                UCLayerManagerPanel layerManeger = new UCLayerManagerPanel(_smartSession);
                if (v == null)
                {
                    layerManeger.Apply(null);
                }
                if (v is ICanvasViewer)
                {
                    layerManeger.Apply((v as ICanvasViewer).LayerProvider as ILayersProvider);
                }
                else if (v is ILayoutViewer)
                {
                    layerManeger.Apply((v as ILayoutViewer).LayerProvider as ILayersProvider);
                }
                (wnd as ToolWindow).Controls.Add(layerManeger);
                layerManeger.Dock = DockStyle.Fill;
                _smartSession.SmartWindowManager.DisplayWindow(wnd, new WindowPosition(DockStyle.Left, false));
                (wnd as DockWindow).TabStrip.SizeInfo.AbsoluteSize = new System.Drawing.Size(WND_DEFAULT_WIDTH, 0);
            }
        }

        public override void Execute(string argument)
        {
            Execute();
        }
    }
}

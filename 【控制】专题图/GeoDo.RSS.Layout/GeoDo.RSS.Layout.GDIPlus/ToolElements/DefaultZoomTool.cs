using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace GeoDo.RSS.Layout.GDIPlus
{
    public class DefaultZoomTool:ControlTool
    {
        private Point _prePoint = Point.Empty;
        private bool _isPaning = false;
        private Cursor _oldCursor = Cursors.Default;

        public override void Event(object sender, enumCanvasEventType eventType, CanvasEventArgs e)
        {
            switch (eventType)
            { 
                case enumCanvasEventType.MouseDown:
                    if (Control.MouseButtons == MouseButtons.Left && !_isPaning)
                    {
                        _oldCursor = (sender as ILayoutHost).Container.Cursor;
                        (sender as ILayoutHost).Container.Cursor = Cursors.Hand;
                        _prePoint = new Point(e.ScreenX, e.ScreenY);
                        _isPaning = true;
                    }
                    break;
                case enumCanvasEventType.MouseMove:
                    if (_isPaning)
                    {
                        int offsetX = e.ScreenX - _prePoint.X;
                        int offsetY = e.ScreenY - _prePoint.Y;
                        Apply(sender as ILayoutHost, offsetX, offsetY);
                        _prePoint = new Point(e.ScreenX, e.ScreenY);
                    }
                    break;
                case enumCanvasEventType.MouseUp:
                    _isPaning = false;
                    (sender as ILayoutHost).Container.Cursor = _oldCursor;
                    break;
                case enumCanvasEventType.MouseWheel:
                    ApplyWheel(sender as ILayoutHost, e);
                    break;
            }
        }

        private void Apply(ILayoutHost host, int offsetX, int offsetY)
        {
            host.LayoutRuntime.ApplyOffset(offsetX, offsetY);
            host.Render();
        }

        private void ApplyWheel(ILayoutHost host, CanvasEventArgs e)
        {
            if(e.WheelDelta>0)
                host.LayoutRuntime.Scale -= 0.1f;
            else
                host.LayoutRuntime.Scale += 0.1f;
            host.Render();
        }
    }
}

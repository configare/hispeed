using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using CodeCell.AgileMap.Core;

namespace CodeCell.AgileMap.Components
{
    public class MapPanTool : MapPanToolBase
    {
        private RectangleF _extentBeforDraging = RectangleF.Empty;
        private bool _isPanningByMouseDrag = false;
        private Point _startMousePoint = Point.Empty;

        public MapPanTool()
            : base()
        { 
        }

        protected override void MouseDown(IMapControl mapcontrol, MouseEventArgs e)
        {
            if (!_isPanningByMouseDrag && e.Button == MouseButtons.Left)
            {
                _isPanningByMouseDrag = true;
                _startMousePoint = e.Location;
                _extentBeforDraging = mapcontrol.ExtentPrj;
                (mapcontrol as IMapControlDummySupprot).SetToDummyRenderMode();
            }
        }

        protected override void MouseMove(IMapControl mapcontrol, MouseEventArgs e)
        {
            if (_isPanningByMouseDrag)
            {
                HandlePanByStartAndStopPoint(mapcontrol,_startMousePoint, e.Location);
                _startMousePoint = e.Location;
            }
        }

        protected override void MouseUp(IMapControl mapcontrol, MouseEventArgs e)
        {
            if (_isPanningByMouseDrag)
            {
                _isPanningByMouseDrag = false;
                (mapcontrol as IMapControlDummySupprot).ResetToNormalRenderMode();
                //
                if (mapcontrol.OperationStack.Enabled)
                {
                    OprChangeExtent opr = new OprChangeExtent(mapcontrol, _extentBeforDraging, mapcontrol.ExtentPrj);
                    mapcontrol.OperationStack.Do(opr);
                }
                else
                {
                    mapcontrol.ReRender();
                }
            }
        }

        private void HandlePanByStartAndStopPoint(IMapControl mapcontrol, Point startPoint, Point endPoint)
        {
            if (!startPoint.IsEmpty && !endPoint.IsEmpty)
            {
                Point[] pts = new Point[] { startPoint, endPoint };
                mapcontrol.CoordinateTransfrom.PixelCoord2PrjCoord(pts);
                Point prjStart = pts[0];
                Point prjEnd = pts[1];
                float panX = prjEnd.X - prjStart.X;
                float panY = prjEnd.Y - prjStart.Y;
                RectangleF vpt = mapcontrol.ExtentPrj;
                vpt.Offset(-panX, panY);
                mapcontrol.ExtentPrj = vpt;
                mapcontrol.ReRender();
            }
        }
   }
}

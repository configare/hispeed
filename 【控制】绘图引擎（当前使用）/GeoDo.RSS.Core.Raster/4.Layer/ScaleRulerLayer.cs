using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DrawEngine;
using System.Drawing;

namespace GeoDo.RSS.Core.RasterDrawing
{
    public class ScaleRulerLayer:Layer,IFlyLayer
    {
        public bool Visible
        {
            get { return true; }
            set { ;}
        }

        public void Render(object sender, IDrawArgs drawArgs)
        {
            DrawRulerBar(sender as ICanvas, drawArgs);
        }

        private void DrawRulerBar(ICanvas canvas,IDrawArgs drawArgs)
        {
            if (canvas == null)
                return;
            IPrimaryDrawObject primaryObj = canvas.PrimaryDrawObject;
            if (primaryObj == null)
                return;
            CoordEnvelope evp = primaryObj.OriginalEnvelope.Clone();
            double x1 = evp.MinX, y1 = evp.MaxY, x2 = evp.MaxX, y2 = evp.MaxY;
            canvas.CoordTransform.QuickTransform.Transform(ref x1, ref y1);
            canvas.CoordTransform.QuickTransform.Transform(ref x2, ref y2);
            //
            Graphics g = drawArgs.Graphics as Graphics;
            using (Pen p = new Pen(Brushes.Red, 10))
            {
                //g.DrawLine(p, (int)x1, (int)y1, (int)x2, (int)y2);
                g.DrawLine(p, (int)x1, 0, (int)x2, 0);
            }
        }

        public void Event(object sender, enumCanvasEventType eventType, DrawingMouseEventArgs e)
        {
        }
    }
}

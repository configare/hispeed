using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DrawEngine;
using System.Drawing;

namespace Test.Layers
{
    public class SimpleRasterLayer : Layer, IRasterLayer
    {
        protected bool _visible = true;
        protected Bitmap _bitmap = null;

        public SimpleRasterLayer(Bitmap bitmap)
            : base()
        {
            _bitmap = bitmap;
        }

        public bool Visible
        {
            get { return _visible; }
            set { _visible = value; }
        }

        private double prjx = 0;
        private double prjy = 0;
        private double prjWidth = 4600;
        private double prjHeight = 3400;
        public void Render(object sender, IDrawArgs drawArgs)
        {
            if (_bitmap != null)
            {
                double x1 = prjx ;
                double y1 = prjy;
                double x2 = prjWidth + prjx;
                double y2 = prjHeight + prjy;
                drawArgs.QuickTransformArgs.Transform(ref x1, ref y1);
                drawArgs.QuickTransformArgs.Transform(ref x2, ref y2);
                (drawArgs.Graphics as Graphics).DrawImage(_bitmap, 
                    RectangleF.FromLTRB((float)Math.Min(x1,x2),(float)Math.Min(y1, y2), (float)Math.Max(x1,x2),(float)Math.Max(y1,y2))
                );
            }
        }
    }
}

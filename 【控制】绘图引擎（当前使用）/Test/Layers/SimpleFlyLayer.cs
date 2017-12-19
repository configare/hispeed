using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DrawEngine;
using System.Drawing;

namespace Test.Layers
{
    public class SimpleFlyLayer:Layer,IVectorLayer
    {
        protected bool _visible = true;

        public SimpleFlyLayer()
            : base()
        { 
        }

        public bool Visible
        {
            get { return _visible; }
            set { _visible = false; }
        }

        public void Render(object sender, IDrawArgs drawArgs)
        {
            Graphics g = drawArgs.Graphics as Graphics;
        }
    }
}

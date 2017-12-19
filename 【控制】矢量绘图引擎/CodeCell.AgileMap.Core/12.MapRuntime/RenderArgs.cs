using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace CodeCell.AgileMap.Core
{
    public class RenderArgs
    {
        private Graphics _gragpics = null;
        private bool _isReRender = false;
        private bool _isRendering = false;

        public RenderArgs()
        { 
        }

        public RenderArgs(Graphics graphics)
        {
            _gragpics = graphics;
        }

        public Graphics Graphics
        {
            get { return _gragpics; }
        }

        public bool IsReRender
        {
            get { return _isReRender; }
            set { _isReRender = value; }
        }

        public bool IsRendering
        {
            get { return _isRendering; }
            set { _isRendering = value; }
        }

        public void BeginRender(Graphics graphics)
        {
            _gragpics = graphics;
        }

        public void EndRender()
        {
            _gragpics = null;
        }
    }
}

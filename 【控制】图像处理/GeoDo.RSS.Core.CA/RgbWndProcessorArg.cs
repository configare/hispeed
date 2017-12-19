using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GeoDo.RSS.Core.CA
{
    public class RgbWndProcessorArg:SimpleRgbProcessorArg
    {
        public const int MIN_WND_SIZE = 3;
        public const int MAX_WND_SIZE = 20;
        protected int _wndWidth = 0;
        protected int _wndHeight = 0;

        public RgbWndProcessorArg()
            : base()
        { 
        }

        public RgbWndProcessorArg(int wndWidth, int wndHeight)
            : this()
        {
            _wndHeight = wndHeight;
            _wndWidth = wndWidth;
        } 

        public int WndWidth 
        {
            get { return _wndWidth; }
            set { _wndWidth = value; }
        }

        public int WndHeight
        {
            get { return _wndHeight; }
            set { _wndHeight = value; }
        }

        public override RgbProcessorArg Clone()
        {
            return new RgbWndProcessorArg(_wndWidth, _wndHeight);
        }
    }
}

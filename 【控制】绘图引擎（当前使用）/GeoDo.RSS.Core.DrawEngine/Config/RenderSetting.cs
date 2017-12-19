using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GeoDo.RSS.Core.DrawEngine
{
    public class RenderSetting
    {
        private bool _enabledparallel;
        private bool _enableddummymode;
        private Color _backColor = Color.White;

        public RenderSetting()
        {

        }

        public Color BackColor
        {
            get { return _backColor; }
            set { _backColor = value; }
        }

        public bool EnabledParallel
        {
            get { return _enabledparallel; }
            set { _enabledparallel = value; }
        }

        public bool EnabledDummymode
        {
            get { return _enableddummymode; }
            set { _enableddummymode = value; }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GeoDo.RSS.Core.UI
{
    public class WindowPosition
    {
        private DockStyle _dockStyle = DockStyle.Fill;
        private bool _isSharePanel = false;

        public WindowPosition()
        { }

        public WindowPosition(DockStyle dockStyle, bool isSharePanel)
        {
            _dockStyle = dockStyle;
            _isSharePanel = isSharePanel;
        }

        public DockStyle DockStyle
        {
            get { return _dockStyle; }
        }

        public bool IsSharePanel
        {
            get { return _isSharePanel; }
        }
    }
}

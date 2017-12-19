using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.RSS.Core.UI;
using Telerik.WinControls.UI.Docking;
using GeoDo.RSS.UI.Bricks;
using GeoDo.RSS.Core.VectorDrawing;
using GeoDo.FileProject;
using GeoDo.RasterProject;
using CodeCell.AgileMap.Core;

namespace GeoDo.RSS.UI.AddIn.HdService
{
    public partial class toDbPage : ToolWindow, ISmartWindow
    {
        private EventHandler _onWindowClosed;
        private toDbPageControl _toDbPageControl;

        public toDbPage()
        {
            InitializeComponent();
            InitControl();
        }

        private void InitControl()
        {
            this.Text = "产品入库";
            _toDbPageControl = new toDbPageControl();
            _toDbPageControl.Dock = DockStyle.Fill;
            this.Controls.Add(_toDbPageControl);
        }

        public void UpdateSession(ISmartSession smartSession)
        {
            _toDbPageControl.UpdateData(smartSession);
        }

        public EventHandler OnWindowClosed
        {
            get
            {
                return _onWindowClosed;

            }
            set
            {
                _onWindowClosed = value;
            }
        }

        public void Free()
        {
            if (_onWindowClosed != null)
                _onWindowClosed(this, null);
        }
    }
}

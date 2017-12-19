using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.Core.RasterDrawing;

namespace GeoDo.RSS.UI.AddIn.Windows
{
    public partial class RasterPropertyWndContent : UserControl, IToolWindowContent
    {
        internal event EventHandler OnApplyClicked;

        public RasterPropertyWndContent()
        {
            InitializeComponent();
            ucRasterInfoTree1.OnBandNoClicked += new EventHandler(ucRasterInfoTree1_OnBandNoClicked);
            SizeChanged += new EventHandler(RasterPropertyWndContent_SizeChanged);
            Load += new EventHandler(RasterPropertyWndContent_Load);
            ucRasterInfoTree1.treeView1.NodeMouseDoubleClick += new TreeNodeMouseClickEventHandler(treeView1_NodeMouseDoubleClick);
        }

        void treeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (ucSelectBandForRgb1.rdGray.Checked)
                btnApply_Click(null, null);
        }

        void RasterPropertyWndContent_Load(object sender, EventArgs e)
        {
            if (Height < 1)
                return;
            panel1.Height = 222;
        }

        void RasterPropertyWndContent_SizeChanged(object sender, EventArgs e)
        {
            if (Height < 1)
                return;
            panel1.Height = 222;
        }

        void ucRasterInfoTree1_OnBandNoClicked(object sender, EventArgs e)
        {
            int b = 0;
            if (int.TryParse(sender.ToString(), out b))
            {
                ucSelectBandForRgb1.SetBandNo(b);
            }
        }

        public void Free()
        {
        }

        public void Apply(ISmartSession session)
        {
            ICanvasViewer viewer = session.SmartWindowManager.ActiveViewer as ICanvasViewer;
            if (viewer == null)
            {
                ucRasterInfoTree1.Apply(null);
                ucSelectBandForRgb1.Apply(null);
                return;
            }
            ucSelectBandForRgb1.Apply(viewer.ActiveObject as IRasterDrawing);
            ucRasterInfoTree1.Apply(viewer.ActiveObject as IRasterDrawing);
        }

        public int[] SelectedBandNos
        {
            get { return ucSelectBandForRgb1.SelectedBandNos; }
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            if (OnApplyClicked != null)
                OnApplyClicked(this, null);
        }

        private void ucSelectBandForRgb1_Load(object sender, EventArgs e)
        {

        }
    }
}

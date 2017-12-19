using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CodeCell.AgileMap.Core;

namespace CodeCell.AgileMap.Components
{
    public partial class frmLayerManager : Form
    {
        public delegate void OnOpenFileHandler(object sender, string filename);
        public event OnOpenFileHandler OnOpenFile = null;

        public frmLayerManager()
        {
            InitializeComponent();
       }

        public void Apply(IMap map)
        {
            ucLayerManager1.Apply(map);
        }

        private void frmLayerManager_Load(object sender, EventArgs e)
        {

        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Filter = "所有支持的文件(*.*)|*.*";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    if (OnOpenFile != null)
                        OnOpenFile(this, dlg.FileName);
                }
            }
        }

        private void btnProperty_Click(object sender, EventArgs e)
        {
            btnProperty.Checked = !btnProperty.Checked;
            ucLayerManager1.VisiblePropertyWnd = btnProperty.Checked;
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            ucLayerManager1.RemoveSelected();
        }

        private void btnLock_Click(object sender, EventArgs e)
        {
            btnLock.Checked = !btnLock.Checked;
            ucLayerManager1.EnabledOrderAdjust = btnLock.Checked;
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            ucLayerManager1.RefreshLayers();
        }
    }
}

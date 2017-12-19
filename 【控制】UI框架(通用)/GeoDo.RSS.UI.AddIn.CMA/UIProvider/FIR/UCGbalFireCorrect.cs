using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.RSS.UI.Bricks;
using System.IO;
using GeoDo.RSS.Core.VectorDrawing;
using GeoDo.RSS.Core.UI;

namespace GeoDo.RSS.UI.AddIn.CMA.UIProvider.FIR
{
    public partial class UCGbalFireCorrect : UserControl
    {
        private ISimpleMapControl _simpleMapControl;
        private bool _mapAoiChanging = false;
        private ISimpleVectorObjectHost _aoiHost;
        private ISmartSession _session = null;
        private int _argControlWidth = 400;

        public UCGbalFireCorrect(ISmartSession session)
        {
            InitializeComponent();
            _session = session;
            LoadMapViews();
        }

        public int argControlWidth
        {
            set
            {
                _argControlWidth = value;
                this.argPanel.Width = value;
            }
        }

        private void LoadMapViews()
        {
            UCSimpleMapControl map = new UCSimpleMapControl();
            _simpleMapControl = map as ISimpleMapControl;
            map.AOIIsChanged += new EventHandler(map_AOIIsChanged);
            map.Dock = DockStyle.Fill;
            cvPanel.Visible = true;
            cvPanel.Controls.Add(map);            //
            map.Load += new EventHandler(map_Load);
        }

        void map_Load(object sender, EventArgs e)
        {
            if (_simpleMapControl == null)
                return;
            _aoiHost = _simpleMapControl.CreateObjectHost("AOI");
        }

        void map_AOIIsChanged(object sender, EventArgs e)
        {
            try
            {
                _mapAoiChanging = true;
                GeoDo.RSS.Core.DrawEngine.CoordEnvelope aoi = _simpleMapControl.DrawedAOI;
            }
            finally
            {
                _mapAoiChanging = false;
            }
        }

        private void btnPath_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                txtPath.Text = dialog.SelectedPath;
                InitFileList(txtPath.Text);
            }
        }

        private void InitFileList(string path)
        {
            lstFileList.Items.Clear();
            string[] files = Directory.GetFiles(path, "*.HDF", SearchOption.AllDirectories);
            if (files == null || files.Length == 0)
                return;
            foreach (string file in files)
                CreateListItem(file);
        }

        private void CreateListItem(string file)
        {
            ListViewItem li = new ListViewItem(Path.GetFileName(file));
            li.Tag = file;
            lstFileList.Items.Add(li);
        }

        public void Free()
        {
            //清理空间
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.RSS.DF.GDAL.HDF5GEO;

namespace GeoDo.RSS.DF.AddIn.HDF5GEO
{
    public partial class frmFY3L2ProDataSelect : Form
    {
        private L2ProductDefind[] _l2Pros = null;

        public frmFY3L2ProDataSelect(L2ProductDefind[] l2Pros)
        {
            InitializeComponent();
            _l2Pros = l2Pros;
            Load += new EventHandler(frmFY3L2ProDataSelect_Load);
        }

        void frmFY3L2ProDataSelect_Load(object sender, EventArgs e)
        {
            if (_l2Pros == null || _l2Pros.Length == 0 || _l2Pros.Length == 1)
                this.Close();
            foreach (L2ProductDefind item in _l2Pros)
            {
                ListViewItem lvi = new ListViewItem(item.Name);
                lvi.SubItems.Add(item.Desc);
                lvi.SubItems.Add(item.ProInfo == null ? "" : item.ProInfo.ProDatasets);
                lvi.Tag = item;
                lstDataDef.Items.Add(lvi);
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        public string ProDesc
        {
            get
            {
                if (lstDataDef.SelectedItems == null || lstDataDef.SelectedItems.Count == 0)
                    return null;
                if (lstDataDef.SelectedItems[0].Tag == null)
                    return null;
                L2ProductDefind l2Pro = lstDataDef.SelectedItems[0].Tag as L2ProductDefind;
                if (l2Pro == null)
                    return null;
                return "l2prodsc=" + l2Pro.Desc;
            }
        }
    }
}

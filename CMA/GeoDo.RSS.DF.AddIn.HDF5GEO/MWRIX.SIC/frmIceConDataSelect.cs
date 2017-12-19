using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GeoDo.RSS.DF.AddIn.HDF5GEO
{
    public partial class frmIceConDataSelect : Form
    {
        public frmIceConDataSelect()
        {
            InitializeComponent();
            asc.Text = "升轨";
            avg.Text = "平均";
            des.Text = "降轨";
        }

        public string ComponentID
        {
            get
            {
                if (lvComponents.SelectedIndices.Count == 0)
                    return null;
                //List<string> selectedsets = new List<string>();
                string id = "";
                string pole = lvComponents.SelectedItem.ToString();
                if (asc.Checked)
                    id+=(pole +"_" +asc.Name+",");
                if (avg.Checked)
                    id += (pole + "_" + avg.Name + ",");
                if (des.Checked)
                    id += (pole + "_" + des.Name + ",");
                if (id.EndsWith(","))
                    id = id.Substring(0, id.LastIndexOf(","));
                return id;
            }
        }

        public void Apply(string [] alldatasets)
        {
            foreach (string ln in alldatasets)
            {
                    lvComponents.Items.Add(ln);
            }
            lvComponents.SetSelected(0,true);
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (lvComponents.SelectedIndices.Count == 0)
                return;

            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}

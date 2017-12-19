using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CodeCell.AgileMap.Core;

namespace GeoDo.RSS.MIF.Core
{
    public partial class frmFeatureSelector : Form
    {
        private Feature[] _vectorFeatures = null;

        public frmFeatureSelector(Feature[] vectorFeatures)
        {
            _vectorFeatures = vectorFeatures;
            InitializeComponent();
            InitVectorFeatures();
        }

        private void InitVectorFeatures()
        {
            Feature fet = _vectorFeatures[0];
            if (fet.FieldNames != null && fet.FieldNames.Length > 0)
            {
                foreach (string field in fet.FieldNames)
                {
                    ColumnHeader header = new ColumnHeader(field);
                    header.Text = field;
                    lvFeatures.Columns.Add(header);
                }
            }
            foreach (Feature f in _vectorFeatures)
            {
                string text = (f.FieldValues != null && f.FieldValues.Length > 0) ? f.FieldValues[0] : f.OID.ToString();
                ListViewItem it = new ListViewItem(text);
                if (f.FieldValues != null && f.FieldValues.Length > 0)
                {
                    for (int i = 1; i < f.FieldValues.Length; i++)
                        it.SubItems.Add(f.GetFieldValue(i));
                }
                it.Tag = f;
                lvFeatures.Items.Add(it);
            }
        }

        public Feature[] GetSelectedVectorFeatures()
        {
            if (lvFeatures.SelectedIndices.Count < 1)
                return null;
            Feature[] fets = new Feature[lvFeatures.SelectedIndices.Count];
            for (int i = 0; i < lvFeatures.SelectedIndices.Count; i++)
                fets[i] = lvFeatures.SelectedItems[i].Tag as Feature;
            return fets;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}

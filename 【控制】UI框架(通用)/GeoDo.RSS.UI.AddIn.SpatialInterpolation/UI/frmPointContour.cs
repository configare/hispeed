using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace GeoDo.RSS.UI.AddIn.SpatialInterpolation
{
    public partial class frmPointContour : Form
    {
        public class ContourItem
        {
            public double ContourValue;
            public Color Color;
        }

        private Dictionary<string, ArrayList> _dicFieldValues;

        public frmPointContour()
        {
            InitializeComponent();
            ckNeedOutputShp_CheckedChanged(null, null);
        }

        public Dictionary<string, ArrayList> DicFieldValues
        {
            set { _dicFieldValues = value; }
        }

        private void frmPointContour_Load(object sender, EventArgs e)
        {
            FieldName.DataSource = _dicFieldValues.Keys.ToArray();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (lvValues.Items.Count == 0)
            {
                MessageBox.Show("等值线级别不能为空！", "系统消息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (!ckIsNeedDisplay.Checked && !ckNeedOutputShp.Checked)
            {
                MessageBox.Show("是否显示和是否输出矢量必须勾选一项！", "系统消息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        public string GetSelFieldName()
        {
            return FieldName.SelectedItem.ToString();
        }

        private void btnStat_Click(object sender, EventArgs e)
        {
            string selFieldName = GetSelFieldName();
            double minValue, maxValue;
            minValue = double.MaxValue;
            maxValue = double.MinValue;
            int nCount = _dicFieldValues[selFieldName].Count;
            for (int i=0; i<nCount; i++)
            {
                double dValue = Convert.ToDouble(_dicFieldValues[selFieldName][i]);
                if (dValue < minValue)
                {
                    minValue = dValue;
                }
                if (dValue > maxValue)
                {
                    maxValue = dValue;
                }
            }
            txtMinValue.Text = minValue.ToString("0.####");
            txtMaxValue.Text = maxValue.ToString("0.####");
            double spanValue = (maxValue - minValue) / 10;
            txtSpan.Text = spanValue.ToString("0.####");
        }

        private void btnAddValue_Click(object sender, EventArgs e)
        {
            AddValueByRange();
        }

        private void AddValueByRange()
        {
            double v1, v2, v3;
            if (!double.TryParse(txtMinValue.Text, out v1))
            {
                txtMinValue.Focus();
                txtMinValue.SelectAll();
                return;
            }
            if (!double.TryParse(txtMaxValue.Text, out v2))
            {
                txtMaxValue.Focus();
                txtMaxValue.SelectAll();
                return;
            }
            if (!double.TryParse(txtSpan.Text, out v3))
            {
                txtSpan.Focus();
                txtSpan.SelectAll();
                return;
            }
            while (v1 < v2)
            {
                lvValues.Items.Add(ToListView(v1));
                v1 += v3;
            }
            if (v1 != v2)
                lvValues.Items.Add(ToListView(v2));
        }

        private ListViewItem ToListView(double v1)
        {
            ListViewItem it = new ListViewItem(v1.ToString());
            Color c = GetNextColor();
            it.SubItems.Add(c.ToString());
            it.Tag = new object[] { v1, c };
            it.BackColor = c;
            return it;
        }

        Random _random = new Random();
        private Color GetNextColor()
        {
            byte[] rgb = new byte[3];
            _random.NextBytes(rgb);
            return Color.FromArgb(rgb[0], rgb[1], rgb[2]);
        }

        private void btnRemoveAll_Click(object sender, EventArgs e)
        {
            lvValues.Items.Clear();
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (lvValues.SelectedIndices.Count == 0)
                return;
            for (int i = lvValues.SelectedIndices.Count - 1; i >= 0; i--)
                lvValues.Items.RemoveAt(lvValues.SelectedIndices[i]);
        }

        public ContourItem[] ContourValues
        {
            get
            {
                if (lvValues.Items.Count == 0)
                    return null;
                ContourItem[] vs = new ContourItem[lvValues.Items.Count];
                for (int i = 0; i < vs.Length; i++)
                {
                    ContourItem it = new ContourItem();
                    object[] args = lvValues.Items[i].Tag as object[];
                    it.ContourValue = (double)args[0];
                    it.Color = (Color)args[1];
                    vs[i] = it;
                }
                return vs;
            }
        }

        public bool IsNeedDisplay
        {
            get { return ckIsNeedDisplay.Checked; }
        }

        public bool IsNeedLabel
        {
            get { return ckNeedLabel.Checked; }
        }

        public string ShpFileName
        {
            get { return ckNeedOutputShp.Checked ? txtSaveAs.Text : null; }
        }

        private void ckNeedOutputShp_CheckedChanged(object sender, EventArgs e)
        {
            if (ckNeedOutputShp.Checked)
            {
                this.Height = 340;
                lbOutShp.Visible = true;
                txtSaveAs.Visible = true;
                btnSaveAs.Visible = true;
                btnOK.Location = new Point(btnOK.Location.X, 270);
                btnCancel.Location = new Point(btnCancel.Location.X, 270);
            }
            else
            {
                this.Height = 300;
                lbOutShp.Visible = false;
                txtSaveAs.Visible = false;
                btnSaveAs.Visible = false;
                btnOK.Location = new Point(btnOK.Location.X, 225);
                btnCancel.Location = new Point(btnCancel.Location.X, 225);
            }
        }

        private void btnSaveAs_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog dlg = new SaveFileDialog())
            {
                dlg.Filter = "ESRI Shape Files(*.shp)|*.shp";
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    txtSaveAs.Text = dlg.FileName;
                }
            }
        }

        public void SetShpFile(string pointFileName)
        {
            if (string.IsNullOrEmpty(pointFileName))
                return;
            string fname = Path.Combine(Path.GetDirectoryName(pointFileName),
                Path.GetFileNameWithoutExtension(pointFileName) + "_Contour.shp");
            txtSaveAs.Text = fname;
        }

        private void lvValues_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListViewItem it = lvValues.GetItemAt(e.X, e.Y);
            if (it == null)
                return;
            using (ColorDialog dlg = new ColorDialog())
            {
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    object[] args = it.Tag as object[];
                    args[1] = dlg.Color;
                    it.SubItems[1].Text = args[1].ToString();
                    it.BackColor = (Color)args[1];
                }
            }
        }
    }
}

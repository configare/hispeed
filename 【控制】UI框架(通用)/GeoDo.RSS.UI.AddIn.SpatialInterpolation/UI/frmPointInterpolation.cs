using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GeoDo.RSS.UI.AddIn.SpatialInterpolation
{
    public partial class frmPointInterpolation : Form
    {
        private string[] _fieldNames;
        private double _resolutionX;
        private double _resolutionY;
        private double _resolutionXmax;
        private double _resolutionXmin;
        private double _resolutionYmax;
        private double _resolutionYmin;
        private string _outputImg;

        public frmPointInterpolation()
        {
            InitializeComponent();
        }

        private void frmPointInterpolation_Load(object sender, EventArgs e)
        {
            FieldName.DataSource = _fieldNames;
            ResolutionX.Text = _resolutionX.ToString();
            resolutionY.Text = _resolutionY.ToString();
            OutPath.Text = _outputImg;
        }

        public string[] FieldNames
        {
            set { _fieldNames = value; }
        }

        public double ResXmax
        {
            set { _resolutionXmax = value; }
        }

        public double ResXmin
        {
            set { _resolutionXmin = value; }
        }

        public double ResYmax
        {
            set { _resolutionYmax = value; }
        }

        public double ResYmin
        {
            set { _resolutionYmin = value; }
        }

        public double ResX
        {
            get { return _resolutionX; }
            set { _resolutionX = value; }
        }

        public double ResY
        {
            get { return _resolutionY; }
            set { _resolutionY = value; }
        }

        public string OutputImg
        {
            get 
            {
                _outputImg = OutPath.Text;
                return _outputImg; 
            }
            set { _outputImg = value; }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if ((!double.TryParse(ResolutionX.Text, out _resolutionX)) || (!double.TryParse(resolutionY.Text, out _resolutionY)))
            {
                MessageBox.Show("分辨率非数值型！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_resolutionX < _resolutionXmin || _resolutionX > _resolutionXmax || _resolutionY < _resolutionYmin || _resolutionY > _resolutionYmax)
            {
                MessageBox.Show("请输入合理的分辨率值！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        public string GetSelFieldName()
        {
            return FieldName.SelectedItem.ToString();
        }

        private void Browse_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "LDF文件(*.ldf)|*.ldf|普通影像文件(*.tif)|*.tif";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                OutPath.Text = dlg.FileName;
            }
        }
    }
}

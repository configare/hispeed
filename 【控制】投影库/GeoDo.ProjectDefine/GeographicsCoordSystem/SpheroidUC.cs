using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.Project;

namespace GeoDo.ProjectDefine
{
    public partial class SpheroidUC : UserControl
    {
        private ISpatialReference _spatialReference = null;
        private enumControlType _controlType = enumControlType.Creat;
        private string _spheroidName = null;
        private double _spheroidSemimajorAxis = 0;
        private double _spheroidSemiminorAxis = 0;
        private double _spheroidInverseFlattening = 0;
        private const double SEMIMAJORAXIS = 6378137.0d;
        private const double SEMIMINORAXIS = 6356752.3142451793d;
        private const double INVERSEFLATTENING = 298.257223563d;

        public SpheroidUC()
        {
            InitializeComponent();
        }

        public SpheroidUC(ISpatialReference SpatialReference, enumControlType ControlType)
        {
            _spatialReference = SpatialReference;
            _controlType = ControlType;
            InitializeComponent();
            Init();
        }

        public string SpheroidName
        {
            get { return _spheroidName; }
            set { _spheroidName = value; }
        }

        public double SpheroidSemimajorAxis
        {
            get { return _spheroidSemimajorAxis; }
            set { _spheroidSemimajorAxis = value; }
        }

        public double SpheroidSemiminorAxis
        {
            get { return _spheroidSemiminorAxis; }
            set { _spheroidSemiminorAxis = value; }
        }

        public double SpheroidInverseFlattening
        {
            get { return _spheroidInverseFlattening; }
            set { _spheroidInverseFlattening = value; }
        }

        private void Init()
        {
            cmbSpheroidName.ItemHeight = 36;
            cmbSpheroidName.Items.Add("WGS_1984");
            txtInverseFlattening.Enabled = false;
            if (_controlType == enumControlType.Modify)
                ShowSpatialReference();
        }

        public void InitSpheroid()
        {
            cmbSpheroidName.Text = "WGS_1984";
            txtSemimajorAxis.Text = SEMIMAJORAXIS.ToString();
            txtSemiminorAxis.Text = SEMIMINORAXIS.ToString();
            txtInverseFlattening.Text = INVERSEFLATTENING.ToString();
        }

        private void ShowSpatialReference()
        {
            cmbSpheroidName.Text = _spatialReference.GeographicsCoordSystem.Datum.Spheroid.Name;
            txtSemimajorAxis.Text = _spatialReference.GeographicsCoordSystem.Datum.Spheroid.SemimajorAxis.ToString();
            txtSemiminorAxis.Text = _spatialReference.GeographicsCoordSystem.Datum.Spheroid.SemiminorAxis.ToString();
            txtInverseFlattening.Text = _spatialReference.GeographicsCoordSystem.Datum.Spheroid.InverseFlattening.ToString();
        }

        public void CollectionAguments()
        {
            if (!String.IsNullOrEmpty(cmbSpheroidName.Text) && cmbSpheroidName.Text != "<自定义>")
                _spheroidName = cmbSpheroidName.Text;
            else
                throw new ArgumentNullException("椭球体名称为空");
            if (!String.IsNullOrEmpty(txtSemimajorAxis.Text))
            {
                bool isNumber = double.TryParse(txtSemimajorAxis.Text,out _spheroidSemimajorAxis);
                if (!isNumber)
                    MessageBox.Show("投影坐标系统参数不完整!", "系统消息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
                throw new ArgumentNullException("长半轴值为空");
            if (rioSemiminorAxis.Checked)
            {
                if (!String.IsNullOrEmpty(txtSemiminorAxis.Text))
                {
                    bool isNumber = double.TryParse(txtSemiminorAxis.Text,out _spheroidSemiminorAxis);
                    if (!isNumber)
                        MessageBox.Show("投影坐标系统参数不完整!", "系统消息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                    throw new ArgumentNullException("短半轴值为空");
            }
            else
            {
                if (!String.IsNullOrEmpty(txtInverseFlattening.Text))
                {
                    bool isNumber = double.TryParse(txtInverseFlattening.Text,out _spheroidInverseFlattening);
                    if (!isNumber)
                        MessageBox.Show("投影坐标系统参数不完整!", "系统消息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                    throw new ArgumentNullException("反扁率值为空");
            }
        }

        private void rioSemiminorAxis_CheckedChanged(object sender, EventArgs e)
        {
            txtSemiminorAxis.Enabled = true;
            txtInverseFlattening.Enabled = false;
        }

        private void rioInverseFlattening_CheckedChanged(object sender, EventArgs e)
        {
            txtInverseFlattening.Enabled = true;
            txtSemiminorAxis.Enabled = false;
        }

        private void cmbSpheroidName_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cmbSpheroidName.Text)
            {
                case "WGS_1984":
                    InitSpheroid();
                    break;
            }
        }
    }
}

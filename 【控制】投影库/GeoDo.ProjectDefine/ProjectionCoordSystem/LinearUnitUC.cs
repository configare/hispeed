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
    public partial class LinearUnitUC : UserControl
    {
        private ISpatialReference _spatialReference = null;
        private enumControlType _controlType = enumControlType.Modify;
        private string _linearUnitName = "Meter";
        private double _linearUnitValue = 1d;

        public LinearUnitUC()
        {
            InitializeComponent();
        }

        public LinearUnitUC(ISpatialReference SpatialReference, enumControlType ControlType)
        {
            _spatialReference = SpatialReference;
            _controlType = ControlType;
            InitializeComponent();
            Init();
        }

        public string LinearUnitName
        {
            get { return _linearUnitName; }
            set { _linearUnitName = value; }
        }

        public double LinearUnitValue
        {
            get { return _linearUnitValue; }
            set { _linearUnitValue = value; }
        }

        private void Init()
        {
            cmbLinearUnitName.Items.Add("Meter");
            cmbLinearUnitName.ItemHeight = 36;
            if (_spatialReference == null)
                return;
            if (_controlType == enumControlType.Modify)
                ShowSpatialReference();
        }

        private void ShowSpatialReference()
        {
            if (_spatialReference != null)
            {
                cmbLinearUnitName.Text = _spatialReference.ProjectionCoordSystem.Unit.Name;
                txtValue.Text = _spatialReference.ProjectionCoordSystem.Unit.Value.ToString();
            }
        }

        public void CollectionAguments()
        {
            if (!String.IsNullOrEmpty(cmbLinearUnitName.Text) && cmbLinearUnitName.Text != "<自定义>")
                _linearUnitName = cmbLinearUnitName.Text;
            else
                throw new ArgumentNullException("地理坐标系统名字为空");

            if (!String.IsNullOrEmpty(txtValue.Text))
            {
                bool isNumber = double.TryParse(txtValue.Text,out _linearUnitValue);
                if (!isNumber)
                    MessageBox.Show("投影坐标系统参数不完整!", "系统消息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
                throw new ArgumentNullException("地理坐标系统名字为空");
        }

        private void cmbLinearUnitName_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cmbLinearUnitName.Text)
            {
                case "Meter":
                    txtValue.Text = "1";
                    break;
            }
        }
    }
}

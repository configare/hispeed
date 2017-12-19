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
    public partial class AngularUnitUC : UserControl
    {
        private ISpatialReference _spatialReference = null;
        private enumControlType _controlType = enumControlType.Modify;
        private string _angularUnitName = "Degree";
        private double _angularUnitValue = 0d;

        public AngularUnitUC()
        {
            InitializeComponent();
        }

        public AngularUnitUC(ISpatialReference SpatialReference, enumControlType ControlType)
        {
            _spatialReference = SpatialReference;
            _controlType = ControlType;
            InitializeComponent();
            Init();
        }

        public string AngularUnitName
        {
            get { return _angularUnitName; }
            set { _angularUnitName = value; }
        }

        public double AngularUnitValue
        {
            get { return _angularUnitValue; }
            set { _angularUnitValue = value; }
        }

        /// <summary>
        ///  修改模式则先显示所选空间参考对象的参数，然后再收集界面上的信息填充到空间参考对象中；
        ///  创建模式则先初始换相关参数，然后收集界面上的信息填充到空间参考对象中。
        /// </summary>
        private void Init()
        {
            cmbAngleUnitName.ItemHeight = 36;
            cmbAngleUnitName.Items.Add("Degree");
            if (_controlType == enumControlType.Modify)
                ShowSpatialReference();
        }

        /// <summary>
        ///  显示所选空间参考对象的参数
        /// </summary>
        private void ShowSpatialReference()
        {
            cmbAngleUnitName.Text = _spatialReference.GeographicsCoordSystem.AngularUnit.Name;
            txtValue.Text = _spatialReference.GeographicsCoordSystem.AngularUnit.Value.ToString();
        }

        /// <summary>
        /// 初始化相关参数
        /// </summary>
        private void InitAngularUnit()
        {
            cmbAngleUnitName.Text = "Degree";
            txtValue.Text = "0.0174532925199433";
        }

        /// <summary>
        /// 收集界面上的信息填充到空间参考对象中
        /// </summary>
        public void CollectionAguments()
        {
            if (!String.IsNullOrEmpty(cmbAngleUnitName.Text) && cmbAngleUnitName.Text != "<自定义>")
                _angularUnitName = cmbAngleUnitName.Text;
            else
                throw new ArgumentNullException("角度单位名字为空");
            if (!String.IsNullOrEmpty(txtValue.Text))
            {
                bool _isNumber = Double.TryParse(txtValue.Text, out _angularUnitValue);
                if (!_isNumber)
                    MessageBox.Show("投影坐标系统参数不完整!", "系统消息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
                throw new ArgumentNullException("角度单位值为空");
        }

        private void cmbAngleUnitName_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cmbAngleUnitName.Text)
            {
                case "Degree":
                    txtValue.Text = "0.0174532925199433";
                    break;
            }
        }
    }
}

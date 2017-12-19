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
    public partial class PrimeMeridianUC : UserControl
    {
        private ISpatialReference _spatialReference = null;
        private enumControlType _controlType = enumControlType.Modify;
        private string _primeMeridianName = "Greenwich";
        private double _primeMeridianValue = 0d;

        public PrimeMeridianUC()
        {
            InitializeComponent();
        }

        public PrimeMeridianUC(ISpatialReference SpatialReference, enumControlType ControlType)
        {
            _spatialReference = SpatialReference;
            _controlType = ControlType;
            InitializeComponent();
            Init();
        }

        public string PrimeMeridianName
        {
            get { return _primeMeridianName; }
            set { _primeMeridianName = value; }
        }

        public double PrimeMeridianValue
        {
            get { return _primeMeridianValue; }
            set { _primeMeridianValue = value; }
        }

        /// <summary>
        ///  修改模式则先显示所选空间参考对象的参数，然后再收集界面上的信息填充到空间参考对象中；
        ///  创建模式则直接收集界面上的信息填充到空间参考对象中。
        /// </summary>
        private void Init()
        {
            cmbPrimeMeridianName.ItemHeight = 36;
            cmbPrimeMeridianName.Items.Add("Greenwich");
            if (_controlType == enumControlType.Modify)
                ShowSpatialReference();
        }

        public void InitPrimeMeridian()
        {
            cmbPrimeMeridianName.Text = "Greenwich";
            txtValue.Text = "0";
        }

        /// <summary>
        ///  显示所选空间参考对象的参数
        /// </summary>
        private void ShowSpatialReference()
        {
            cmbPrimeMeridianName.Text = _spatialReference.GeographicsCoordSystem.PrimeMeridian.Name;
            txtValue.Text = _spatialReference.GeographicsCoordSystem.PrimeMeridian.Value.ToString();
        }

        /// <summary>
        /// 收集界面上的信息填充到空间参考对象中
        /// </summary>
        public void CollectionAguments()
        {
            if (!String.IsNullOrEmpty(cmbPrimeMeridianName.Text) && cmbPrimeMeridianName.Text != "<自定义>")
                _primeMeridianName = cmbPrimeMeridianName.Text;
            else
                throw new ArgumentNullException("本初子午线名称为空");
            if (!String.IsNullOrEmpty(txtValue.Text))
            {
                bool _isNumber = double.TryParse(txtValue.Text, out _primeMeridianValue);
                if (!_isNumber)
                    MessageBox.Show("投影坐标系统参数不完整!", "系统消息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
                throw new ArgumentNullException("本初子午线经度值为空");
        }

        private void cmbPrimeMeridianName_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cmbPrimeMeridianName.Text)
            {
                case "Greenwich":
                    txtValue.Text = "0";
                    break;
            }
        }
    }
}

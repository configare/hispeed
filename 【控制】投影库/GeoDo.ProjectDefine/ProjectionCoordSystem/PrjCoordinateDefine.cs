using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.Project;
using System.IO;

namespace GeoDo.ProjectDefine
{
    public partial class PrjCoordinateDefine : Form
    {
        protected ISpatialReference _spatialReference = new SpatialReference(new GeographicCoordSystem(),new ProjectionCoordSystem());
        protected ProjectionCoordSystem _projectionCoordSystem = null;
        private enumControlType _controlType = enumControlType.Modify;
        private string _spatialSystemName = null;
        private IGeographicCoordSystem _geographicCoordSystem = null;

        public PrjCoordinateDefine(ISpatialReference SpatialReference, enumControlType ControlType)
        {
            _spatialReference = SpatialReference;
            _controlType = ControlType;
            InitializeComponent();
            Init();
        }

        public ISpatialReference SpatialReference
        {
            get { return _spatialReference; }
            set { _spatialReference = value; }
        }

        public ProjectionCoordSystem ProjectionCoordSystem
        {
            get { return _projectionCoordSystem; }
            set { _projectionCoordSystem = value; }
        }

        private void Init()
        {
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            if (_controlType == enumControlType.Modify)
                ShowSpatialReference();
        }

        private void ShowSpatialReference()
        {
            txtPrjName.Text = _spatialReference.Name;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(txtPrjName.Text) || txtPrjName.Text == "<自定义>")
                {
                    MessageBox.Show("投影坐标系统名称不能为空!", "系统消息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtPrjName.Focus();
                    return;
                }
                linearUnit.CollectionAguments();
                projectionParam.CollectionAguments();
                if (geoCoordParamDisplay._dialogResult == System.Windows.Forms.DialogResult.OK)
                    _geographicCoordSystem = geoCoordParamDisplay.GeoCoordSystem;
                else
                    _geographicCoordSystem = _spatialReference.GeographicsCoordSystem;
            }
            catch
            {
                MessageBox.Show("投影坐标系统参数不完整!", "系统消息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            CreatProjSystem();
            if (!String.IsNullOrEmpty(txtPrjName.Text) && txtPrjName.Text != "<自定义>")
                _spatialSystemName = txtPrjName.Text;
            _spatialReference.Name = _spatialSystemName;
            DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void CreatProjSystem()
        {
            AngularUnit prjLinearUnit = new AngularUnit(linearUnit.LinearUnitName, linearUnit.LinearUnitValue);
            foreach(NameValuePair projParam in projectionParam.ProjectParams)
            {
                if (projParam.Name == null)
                    projectionParam.CollectionAguments();
            }
            NameValuePair[] prjParams = projectionParam.ProjectParams;
            using (PrjStdsMapTableParser p = new PrjStdsMapTableParser())
            {
                if (projectionParam.CurrentEnviPrjInfoArgDefs == null)
                {
                    projectionParam.GetPrjNameItem();
                }
                NameMapItem prjName = p.GetPrjNameItemByEnviName(projectionParam.CurrentEnviPrjInfoArgDefs.PrjId.ToString()); 
                if (prjName == null)
                {
                    MessageBox.Show("投影名错误!", "系统消息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                _projectionCoordSystem = new ProjectionCoordSystem(prjName, projectionParam.ProjectParams, prjLinearUnit);
            }
            _spatialReference = new SpatialReference(_geographicCoordSystem, _projectionCoordSystem);
            _spatialReference.Name = _spatialSystemName;
        }

        private void btnCancle_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
            Close();
        }
    }
}

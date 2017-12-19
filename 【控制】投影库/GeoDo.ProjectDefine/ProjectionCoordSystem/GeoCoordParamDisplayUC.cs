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
    public partial class GeoCoordParamDisplayUI : UserControl
    {
        private ISpatialReference _spatialReference = null;
        private enumControlType _controlType = enumControlType.Creat;
        private IGeographicCoordSystem _geoCoordSystem = null;
        public EventHandler _isOKHanlder = null;
        public DialogResult _dialogResult = DialogResult.None;

        public GeoCoordParamDisplayUI()
        {
            InitializeComponent();
        }

        public GeoCoordParamDisplayUI(ISpatialReference SpatialReference, enumControlType ControlType)
        {
            _spatialReference = SpatialReference;
            _controlType = ControlType;
            InitializeComponent();
            Init();
        }

        public IGeographicCoordSystem GeoCoordSystem
        {
            get { return _geoCoordSystem; }
            set { _geoCoordSystem = value; }
        }

        private void Init()
        {
            txtShow.ReadOnly = true;
            if (_spatialReference == null || _spatialReference.GeographicsCoordSystem == null)
                btnModify.Enabled = false;
            else if (_controlType == enumControlType.Modify)
                ShowGeoCoordSystem();
            else if (_controlType == enumControlType.Creat)
                btnModify.Enabled = false;
        }

        private void ShowGeoCoordSystem()
        {
            if (_spatialReference==null||_spatialReference.GeographicsCoordSystem == null)
            {
                txtShow.Text = "此空间参考错误，地理坐标系统为空";
                return;
            }
            txtShow.Text = _spatialReference.GeographicsCoordSystem.ToString();
            _geoCoordSystem = _spatialReference.GeographicsCoordSystem;
        }

        private void btnCreatGeoCoord_Click(object sender, EventArgs e)
        {
            CollectionArgumentsCreat();
            if (_geoCoordSystem != null)
            {
                txtShow.Text = _geoCoordSystem.ToString();
                btnModify.Enabled = true;
            }
            else
                btnModify.Enabled = false;
        }

        public void CollectionArgumentsCreat()
        {
            using (GeoCoordinateDefine geoCoordinateDefine = new GeoCoordinateDefine(_spatialReference, enumControlType.Creat))
            {
                if (geoCoordinateDefine.ShowDialog() == DialogResult.OK)
                    _geoCoordSystem = geoCoordinateDefine.GeographicCoordSystem;
                else if (_spatialReference != null)
                    _geoCoordSystem = _spatialReference.GeographicsCoordSystem;
            }
        }

        private void btnModify_Click(object sender, EventArgs e)
        {
            if (_spatialReference == null)
                return;
            CollectionArgumentsModify();
            txtShow.Text = _geoCoordSystem.ToString();
            _dialogResult = DialogResult.OK;
        }

        public void CollectionArgumentsModify()
        {
            _geoCoordSystem = _spatialReference.GeographicsCoordSystem;
            GeoCoordinateDefine geoCoordinateDefine = new GeoCoordinateDefine(_spatialReference, enumControlType.Modify);
            geoCoordinateDefine.ShowDialog();
            if (geoCoordinateDefine.DialogResult == DialogResult.OK)
                //{
                _geoCoordSystem = geoCoordinateDefine.GeographicCoordSystem;
            // btnModify.Enabled = true;
            // }
            //else if (geoCoordinateDefine.DialogResult == DialogResult.Cancel)
            //    return;
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            GeoSystemTree spatialRefTree = new GeoSystemTree();
            spatialRefTree.ShowDialog();
            if (spatialRefTree.CurrentGeoCoordSystem != null)
            {
                _geoCoordSystem = spatialRefTree.CurrentGeoCoordSystem;
                txtShow.Text = _geoCoordSystem.ToString();
                _dialogResult = DialogResult.OK;
            }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Filter = "数据文件(*.TIF)|*.TIF";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                }
            }
        }
    }
}

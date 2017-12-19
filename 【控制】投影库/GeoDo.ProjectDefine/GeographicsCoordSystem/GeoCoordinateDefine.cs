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
    public partial class GeoCoordinateDefine : Form
    {
        private ISpatialReference _spatialReference = null;
        private enumControlType _controlType = enumControlType.Modify;
        private string _geoSystemName = null;
        private IGeographicCoordSystem _geographicCoordSystem = null;
        private bool _isSaveToFile = false;

        public GeoCoordinateDefine(ISpatialReference SpatialReference, enumControlType ControlType)
        {
            _spatialReference = SpatialReference;
            _controlType = ControlType;
            InitializeComponent();
            Init();
        }

        public GeoCoordinateDefine(IGeographicCoordSystem GeoCoordSystem, enumControlType ControlType)
        {
            _geographicCoordSystem = GeoCoordSystem;
            _controlType = ControlType;
            InitializeComponent();
            Init();
        }

        public IGeographicCoordSystem GeographicCoordSystem
        {
            get { return _geographicCoordSystem; }
            set { _geographicCoordSystem = value; }
        }

        public bool IsSaveToFile
        {
            get { return _isSaveToFile; }
        }

        private void Init()
        {
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            if (_controlType == enumControlType.Modify)
                ShowSpatialReference();
        }

        private void ShowSpatialReference()
        {
            txtName.Text = _spatialReference.Name;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                CollectAguments();
            }
            catch
            {
                MessageBox.Show("地理坐标系统参数不完整!", "系统消息", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            CreatGeoReference();
            DialogResult = System.Windows.Forms.DialogResult.OK;
        }       

        private void CreatGeoReference()
        {
            AngularUnit geoAngularUnit = new AngularUnit(angularUnit.AngularUnitName, angularUnit.AngularUnitValue);
            Datum geoDatum = new Datum(datum.DatumName, datum.DatumSpheroid);
            PrimeMeridian geoPrimeMeridian = new PrimeMeridian(primeMeridian.PrimeMeridianName, primeMeridian.PrimeMeridianValue);
            _geographicCoordSystem = new GeographicCoordSystem(_geoSystemName, geoAngularUnit, geoPrimeMeridian, geoDatum);
            _spatialReference = new SpatialReference(_geographicCoordSystem);
        }

        private void CollectAguments()
        {
            if (!String.IsNullOrEmpty(txtName.Text) && txtName.Text != "<自定义>")
                _geoSystemName = txtName.Text;
            else
                throw new ArgumentNullException("地理坐标系统名字为空");
            angularUnit.CollectionAguments();
            datum.CollectionAguments();
            primeMeridian.CollectionAguments();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
            Close();
        }
    }
}

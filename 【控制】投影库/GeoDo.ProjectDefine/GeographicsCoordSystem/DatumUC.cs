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
    public partial class DatumUC : UserControl
    {
        private ISpatialReference _spatialReference = null;
        private enumControlType _controlType = enumControlType.Modify;
        private string _datumName = null;
        private Spheroid _datumSpheroid = null;

        public DatumUC()
        {
            InitializeComponent();
        }

        public DatumUC(ISpatialReference SpatialReference, enumControlType ControlType)
        {
            _spatialReference = SpatialReference;
            _controlType = ControlType;
            InitializeComponent();
            Init();
        }

        public string DatumName
        {
            get { return _datumName; }
            set { _datumName = value; }
        }

        public Spheroid DatumSpheroid
        {
            get { return _datumSpheroid; }
            set { _datumSpheroid = value; }
        }

        private void Init()
        {
            InitDatumName();
            cmbDatumName.ItemHeight = 36;
            if (_controlType == enumControlType.Modify)
                ShowSpatialReference();
        }

        private void InitDatum()
        {
            cmbDatumName.Text = "D_WGS_1984";
        }

        private void InitDatumName()
        {
            cmbDatumName.Items.Add("D_WGS_1984");
        }

        private void ShowSpatialReference()
        {
            cmbDatumName.Text = _spatialReference.GeographicsCoordSystem.Datum.Name;
        }

        public void CollectionAguments()
        {
            if (!String.IsNullOrEmpty(cmbDatumName.Text) && cmbDatumName.Text != "<自定义>")
                _datumName = cmbDatumName.Text;
            else
                throw new ArgumentNullException("基准面的名字为空");
            paramSpheroid.CollectionAguments();
            _datumSpheroid = new Spheroid(paramSpheroid.SpheroidName, paramSpheroid.SpheroidSemimajorAxis,
                                          paramSpheroid.SpheroidSemiminorAxis, paramSpheroid.SpheroidInverseFlattening);
        }

        private void cmbDatumName_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cmbDatumName.Text)
            {
                case "D_WGS_1984":
                    paramSpheroid.SpheroidName = "WGS_1984";
                    paramSpheroid.InitSpheroid();
                    break;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.RasterProject;
using GeoDo.Project;

namespace GeoDo.RSS.UI.AddIn.BatchProjectionMosaic
{
    public partial class GeoRegionEditCenter : UserControl
    {
        private bool _valueChanging = false;
        private PrjEnvelope _envelope = null;
        private ISpatialReference _spatialRef = null;
        private ErrorProvider _error = null;
        private PrjPoint _center;
        private float _resolutionX;
        private float _resolutionY;
        private Size _size;

        public GeoRegionEditCenter()
        {
            InitializeComponent();
            _error = new ErrorProvider();
            _spatialRef = SpatialReference.GetDefault();
            _resolutionX = 0.01f;
            _resolutionY = 0.01f;
            _center = new PrjPoint(0d, 0d);
            _size = new System.Drawing.Size(1024, 1024);
            _envelope = PrjEnvelope.CreateByCenter(_center.X, _center.Y, 0.01d * _size.Width, 0.01d * _size.Height);
        }

        /// <summary>
        /// 地理坐标范围
        /// </summary>
        public PrjEnvelope GeoEnvelope
        {
            get { return _envelope; }
        }

        /// <summary>
        /// 投影坐标范围(暂未启用)
        /// </summary>
        public PrjEnvelope PrjEnvelope
        {
            get
            {
                return null;
            }
        }

        public event EventHandler ValueChanged;

        private void DoValueChanged()
        {
            if (_valueChanging)
                return;
            _error.Clear();
            double resolutionX;
            double resolutionY;
            if (!double.TryParse(cmbResolutionX.Text, out resolutionX) || resolutionX <= 0f)
            {
                _error.SetError(cmbResolutionX, "数值不合法");
                return;
            }
            if (!double.TryParse(cmbResolutionY.Text, out resolutionY) || resolutionY <= 0f)
            {
                _error.SetError(cmbResolutionY, "数值不合法");
                return;
            }
            double centerX;
            double centerY;
            if (!double.TryParse(txtCenterLongitude.Text, out centerX))
            {
                _error.SetError(txtCenterLongitude, "数值不合法");
                return;
            }
            if (!double.TryParse(txtCenterLatitude.Text, out centerY))
            {
                _error.SetError(txtCenterLatitude, "数值不合法");
                return;
            }
            int sizex;
            int sizey;
            if (!int.TryParse(cmbSizeX.Text, out sizex) || sizex < 1f)
            {
                _error.SetError(cmbSizeX, "数值不合法");
                return;
            }
            if (!int.TryParse(cmbSizeY.Text, out sizey) || sizey < 1f)
            {
                _error.SetError(cmbSizeY, "数值不合法");
                return;
            }
            _resolutionX = (float)resolutionX;
            _resolutionY = (float)resolutionY;
            _center = new PrjPoint(centerX, centerY);
            _size = new Size(sizex, sizey);
            _envelope = PrjEnvelope.CreateByCenter(centerX, centerY, resolutionX * sizex, resolutionY * sizey);

            txtPrjenvelope.Text = string.Format("坐标范围" + Environment.NewLine
                + "X:[{0},{1}]{4}" + Environment.NewLine + "Y:[{2},{3}]{5}",
                _envelope.MinX.ToString("f6"), 
                _envelope.MaxX.ToString("f6"),
                _envelope.MinY.ToString("f6"),
                _envelope.MaxY.ToString("f6"),
                _envelope.Width.ToString("f6"),
                _envelope.Height.ToString("f6"));
            if (ValueChanged != null)
                ValueChanged(this, null);
        }

        public void SetValue(ISpatialReference spatialRef, PrjPoint center, float resolutionX, float resolutionY, Size size)
        {
            try
            {
                BeginChangedValue();
                if (spatialRef == null)
                    spatialRef = SpatialReference.GetDefault();
                _spatialRef = spatialRef;
                _center = center;
                _resolutionX = resolutionX;
                _resolutionY = resolutionY;
                _size = size;
                SetDefaultControl();
                cmbResolutionX.Text = resolutionX.ToString();
                cmbResolutionY.Text = resolutionY.ToString();
                if (size != null)
                {
                    cmbSizeX.Text = size.Width.ToString();
                    cmbSizeY.Text = size.Height.ToString();
                }
                if (center != null)
                {
                    txtCenterLongitude.Text = center.X.ToString();
                    txtCenterLatitude.Text = center.Y.ToString();
                }
            }
            finally
            {
                EndChangedValue();
            }
        }

        public void SetGeoEnvelope(PrjEnvelope geoEnvelope)
        {
            try
            {
                BeginChangedValue();
                txtCenterLongitude.Text = geoEnvelope.CenterX.ToString();
                txtCenterLatitude.Text = geoEnvelope.CenterY.ToString();
                Size size = geoEnvelope.GetSize(_resolutionX, _resolutionY);
                cmbSizeX.Text = size.Width.ToString();
                cmbSizeY.Text = size.Height.ToString();
            }
            finally
            {
                EndChangedValue();
            }
        }

        private void SetDefaultControl()
        {
            cmbResolutionX.Items.Clear();
            cmbResolutionY.Items.Clear();
            if (_spatialRef.ProjectionCoordSystem == null)
            {
                cmbResolutionX.Items.Add("0.01");
                cmbResolutionX.Items.Add("0.005");
                cmbResolutionX.Items.Add("0.0025");
                cmbResolutionX.Items.Add("0.05");

                cmbResolutionY.Items.Add("0.01");
                cmbResolutionY.Items.Add("0.005");
                cmbResolutionY.Items.Add("0.0025");
                cmbResolutionY.Items.Add("0.05");

                lbOutputResolutionX.Text = "度/像素";
                lbOutputResolutionY.Text = "度/像素";
            }
            else
            {
                cmbResolutionX.Items.Add("1000");
                cmbResolutionX.Items.Add("500");
                cmbResolutionX.Items.Add("250");
                cmbResolutionX.Items.Add("5000");
                cmbResolutionY.Items.Add("1000");
                cmbResolutionY.Items.Add("500");
                cmbResolutionY.Items.Add("250");
                cmbResolutionY.Items.Add("5000");
                lbOutputResolutionX.Text = "米/像素";
                lbOutputResolutionY.Text = "米/像素";
            }
        }

        public void BeginChangedValue()
        {
            _valueChanging = true;
        }

        public void EndChangedValue()
        {
            _valueChanging = false;
            DoValueChanged();
        }

        private void cmbResolutionX_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (linkResolution.Text == "+")
                {
                    _valueChanging = true;
                    cmbResolutionY.Text = cmbResolutionX.Text;
                    _valueChanging = false;
                }
                DoValueChanged();
            }
            finally
            { 
            }
        }

        private void cmbResolutionY_TextChanged(object sender, EventArgs e)
        {
            DoValueChanged();
        }

        private void txtCenterLongitude_TextChanged(object sender, EventArgs e)
        {
            DoValueChanged();
        }

        private void txtCenterLatitude_TextChanged(object sender, EventArgs e)
        {
            DoValueChanged();
        }

        private void cmbSizeX_TextChanged(object sender, EventArgs e)
        {
            if (linkSize.Text == "+")
            {
                _valueChanging = true;
                cmbSizeY.Text = cmbSizeX.Text;
                _valueChanging = false;
            }
            DoValueChanged();
        }

        private void cmbSizeY_TextChanged(object sender, EventArgs e)
        {
            DoValueChanged();
        }

        private void linkResolution_Click(object sender, EventArgs e)
        {
            if (linkResolution.Text == "+")
            {
                linkResolution.Text = "-";
                cmbResolutionY.Enabled = true;
            }
            else
            {
                linkResolution.Text = "+";
                cmbResolutionY.Enabled = false;
                if (cmbResolutionY.Text != cmbResolutionX.Text)
                {
                    _valueChanging = true;
                    cmbResolutionY.Text = cmbResolutionX.Text;
                    _valueChanging = false;
                }
            }
        }

        private void linkSize_Click(object sender, EventArgs e)
        {
            if (linkSize.Text == "+")
            {
                linkSize.Text = "-";
                cmbSizeY.Enabled = true;
            }
            else
            {
                linkSize.Text = "+";
                cmbSizeY.Enabled = false;
                if (cmbSizeY.Text != cmbSizeX.Text)
                {
                    _valueChanging = true;
                    cmbSizeY.Text = cmbSizeX.Text;
                    _valueChanging = false;
                }
            }
        }
    }
}

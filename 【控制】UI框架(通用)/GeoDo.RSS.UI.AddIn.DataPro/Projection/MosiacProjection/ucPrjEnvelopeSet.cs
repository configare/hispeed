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
using GeoDo.FileProject;

namespace GeoDo.RSS.UI.AddIn.DataPro
{
    public partial class ucPrjEnvelopeSet : UserControl
    {
        private PrjPoint _center = null;
        private float _resolutionX;
        private float _resolutionY;
        private Size _size;
        private ISpatialReference _spatialRef;
        private int _mode = 0;
        private bool _isBegingChangedValue = false;

        public event EventHandler OnEnvelopeChanged;

        public ucPrjEnvelopeSet()
        {
            InitializeComponent();
            this.Load += new EventHandler(ucPrjEnvelopeSet_Load);
        }

        void ucPrjEnvelopeSet_Load(object sender, EventArgs e)
        {
            ucDefinedRegion1.CheckedChanged += new Action<FileProject.PrjEnvelopeItem[]>(ucDefinedRegion1_CheckedChanged);
        }

        /// <summary>
        /// 0：整轨
        /// 1：自定义范围
        /// 2：预定义经纬度范围
        /// </summary>
        /// <param name="mode"></param>
        public void SetEnvelopeMode(int mode)
        {
            _mode = mode;
            if (mode == 0)
            {
                gpBoxOutputSize.Visible = false;
                gbEnvelope.Visible = false;
                groupBox1.Visible = false;
            }
            else if (mode == 1)
            {
                gpBoxOutputSize.Visible = true;
                gbEnvelope.Visible = true;
                groupBox1.Visible = false;
            }
            else if (mode == 2)
            {
                gpBoxOutputSize.Visible = false;
                gbEnvelope.Visible = false;
                groupBox1.Visible = true;
            }
        }

        public Size PixelSize
        {
            get
            {
                int x,y;
                int.TryParse(cmbSizeX.Text,out x);
                int.TryParse(cmbSizeY.Text,out y);
                return new Size(x, y);
            }
        }

        public float ResolutionX
        {
            get
            {
                float ret;
                if (float.TryParse(cmbResolutionX.Text, out ret))
                    return ret;
                else
                    return 0;
            }
        }

        public float ResolutionY
        {
            get
            {
                float ret;
                if (float.TryParse(cmbResolutionY.Text, out ret))
                    return ret;
                else
                    return 0;
            }
        }

        public PrjPoint CenterLongLat
        {
            get
            {
                double x, y;
                double.TryParse(txtCenterLongitude.Text, out x);
                double.TryParse(txtCenterLatitude.Text, out y);
                return new PrjPoint(x, y);
            }
        }

        /// <summary>
        /// 左上角经纬度
        /// </summary>
        public PrjPoint LeftTopLongLat
        {
            get
            {
                double x, y;
                double.TryParse(txtCenterLongitude.Text, out x);
                double.TryParse(txtCenterLatitude.Text, out y);
                double width, height;
                double.TryParse(cmbSizeX.Text,out width);
                double.TryParse(cmbSizeY.Text, out height);
                PrjEnvelope env = PrjEnvelope.CreateByCenter(x, y, width, height);
                return env.LeftTop;
            }
        }

        public PrjEnvelopeItem[] PrjEnvelopes
        {
            get
            {
                switch (_mode)
                {
                    case 0:
                    case 1:
                        if (_spatialRef != null && _spatialRef.ProjectionCoordSystem != null)
                        {
                            return new PrjEnvelopeItem[]
                            {
                                  new PrjEnvelopeItem("DXX", 
                                      PrjEnvelope.CreateByCenter(CenterLongLat.X, CenterLongLat.Y, 
                                      PixelSize.Width * ResolutionX/100000, PixelSize.Height * ResolutionY/100000))
                            };
                        }
                        else
                        {
                            return new PrjEnvelopeItem[]
                            {
                                  new PrjEnvelopeItem("DXX", 
                                      PrjEnvelope.CreateByCenter(CenterLongLat.X, CenterLongLat.Y, 
                                      PixelSize.Width * ResolutionX, PixelSize.Height * ResolutionY))
                            };
                        }
                    case 2:
                        return ucDefinedRegion1.CheckedItem;
                    default:
                        return null;
                }
            }
        }

        public void SetPrjEnvelope(PrjEnvelope env)
        {
            SetEnvelopeMode(1);
            if (_spatialRef != null && _spatialRef.ProjectionCoordSystem != null)
            {
                PrjEnvelope geoEnv = PrjToGeoEnv(env, _spatialRef);
                txtCenterLongitude.Text = geoEnv.CenterX.ToString();
                txtCenterLatitude.Text = geoEnv.CenterY.ToString();
                cmbSizeX.Text = ((int)(geoEnv.Width * 100000 / ResolutionX + 0.5f)).ToString();
                cmbSizeY.Text = ((int)(geoEnv.Height * 100000 / ResolutionY + 0.5f)).ToString();
            }
            else
            {
                //txtLeftTopLongitude.Text = env.LeftTop.X.ToString();
                //txtLeftTopLatitude.Text = env.LeftTop.Y.ToString();
                txtCenterLongitude.Text = env.CenterX.ToString();
                txtCenterLatitude.Text = env.CenterY.ToString();
                cmbSizeX.Text = ((int)(env.Width / ResolutionX + 0.5f)).ToString();
                cmbSizeY.Text = ((int)(env.Height / ResolutionY + 0.5f)).ToString();
            }
        }

        private PrjEnvelope PrjToGeoEnv(PrjEnvelope geoEnv, ISpatialReference dstSpatialRef)
        {
            IProjectionTransform transform = ProjectionTransformFactory.GetProjectionTransform(SpatialReference.GetDefault(), dstSpatialRef);
            double[] xs = new double[] { geoEnv.MinX, geoEnv.MaxX };
            double[] ys = new double[] { geoEnv.MinY, geoEnv.MaxY };
            transform.InverTransform(xs, ys);
            return new PrjEnvelope(Math.Min(xs[0], xs[1]), Math.Max(xs[0], xs[1]), Math.Min(ys[0], ys[1]), Math.Max(ys[0], ys[1]));
        }

        public void SetValue(ISpatialReference spatialRef, PrjPoint centerLat, float resolutionX, float resolutionY, Size size)
        {
            BeginChangedValue();
            if (spatialRef == null)
                spatialRef = SpatialReference.GetDefault();
            _spatialRef = spatialRef;
            _center = centerLat;
            _resolutionX = resolutionX;
            _resolutionY = resolutionY;
            _size = size;
            UpdateListView();
            if (_spatialRef.ProjectionCoordSystem == null)
            {
                lbOutputResolutionX.Text = "度/像素";
                lbOutputResolutionY.Text = "度/像素";
            }
            else
            {
                lbOutputResolutionX.Text = "米/像素";
                lbOutputResolutionY.Text = "米/像素";
            }
            cmbResolutionX.Text = resolutionX.ToString();
            cmbResolutionY.Text = resolutionY.ToString();
            if (size != null)
            {
                cmbSizeX.Text = size.Width.ToString();
                cmbSizeY.Text = size.Height.ToString();
            }
            if (centerLat != null)
            {
                txtCenterLongitude.Text = centerLat.X.ToString();
                txtCenterLatitude.Text = centerLat.Y.ToString();
            }
            EndChangedValue();
        }
        
        public void SetSpatialReference(ISpatialReference spatialRef)
        {
        }

        public void SetResolution(float resolutionX, float resolutionY)
        {
            try
            {
                BeginChangedValue();
                cmbResolutionX.Text = resolutionX.ToString();
                cmbResolutionY.Text = resolutionX.ToString();
            }
            finally
            {
                EndChangedValue();
            }
        }

        private void UpdateListView()
        {
            cmbResolutionX.Items.Clear();
            cmbResolutionY.Items.Clear();
            if (_spatialRef == null || _spatialRef.ProjectionCoordSystem == null)
            {
                cmbResolutionX.Items.Add(0.01);
                cmbResolutionX.Items.Add(0.005);
                cmbResolutionX.Items.Add(0.0025);
                cmbResolutionX.Items.Add(0.05);
                cmbResolutionY.Items.Add(0.01);
                cmbResolutionY.Items.Add(0.005);
                cmbResolutionY.Items.Add(0.0025);
                cmbResolutionY.Items.Add(0.05);
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
            }
        }

        void ucDefinedRegion1_CheckedChanged(FileProject.PrjEnvelopeItem[] obj)
        {
            DoValueChanged();
        }

        private void UpdateLeftTop()
        {
            if (txtLeftTopX.Visible)
            {
                if (PrjEnvelopes != null)
                {
                    txtLeftTopX.Text = Math.Round(PrjEnvelopes[0].PrjEnvelope.LeftTop.X, 2).ToString();
                    txtLeftTopY.Text = Math.Round(PrjEnvelopes[0].PrjEnvelope.LeftTop.Y, 2).ToString();
                }
            }
        }

        private void txtLeftTopLongitude_TextChanged(object sender, EventArgs e)
        {
            DoValueChanged();
        }

        private void txtLeftTopLatitude_TextChanged(object sender, EventArgs e)
        {
            DoValueChanged();
        }

        private void cmbResolutionX_TextChanged(object sender, EventArgs e)
        {
            if (linkResolution.Text == "+")
            {
                try
                {
                    _isBegingChangedValue = true;
                    cmbResolutionY.Text = cmbResolutionX.Text;
                }
                finally
                {
                    _isBegingChangedValue = false;
                }
            }
            DoValueChanged();
        }

        private void cmbResolutionY_TextChanged(object sender, EventArgs e)
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
                    try
                    {
                        _isBegingChangedValue = true;
                        cmbResolutionY.Text = cmbResolutionX.Text;
                    }
                    finally
                    {
                        _isBegingChangedValue = false;
                    }
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
                    try
                    {
                        _isBegingChangedValue = true;
                        cmbSizeY.Text = cmbSizeX.Text;
                    }
                    finally
                    {
                        _isBegingChangedValue = false;
                    }
                }
            }
        }

        private void cmbSizeX_TextChanged(object sender, EventArgs e)
        {
            if (linkSize.Text == "+")
            {
                BeginChangedValue();
                cmbSizeY.Text = cmbSizeX.Text;
            }
            EndChangedValue();
        }

        private void cmbSizeY_TextChanged(object sender, EventArgs e)
        {
            DoValueChanged();
        }

        public void BeginChangedValue()
        {
            _isBegingChangedValue = true;
        }

        public void EndChangedValue()
        {
            _isBegingChangedValue = false;
            DoValueChanged();
        }

        private void DoValueChanged()
        {
            if (_isBegingChangedValue)
                return;
            //Update AllValue
            //{  }
            if (OnEnvelopeChanged != null)
                OnEnvelopeChanged(this, null);
            UpdateLeftTop();
        }
    }
}

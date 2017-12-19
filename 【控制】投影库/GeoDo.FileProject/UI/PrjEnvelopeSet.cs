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

namespace GeoDo.FileProject
{
    internal partial class PrjEnvelopeSet : UserControl
    {
        private ISpatialReference _spatialRef;
        private PrjEnvelope _prjEnvelope = PrjEnvelope.Empty;
        private float _resolutionX;
        private float _resolutionY;
        private Size _piexlSize = Size.Empty;
        private PrjPoint _centerPoint = PrjPoint.Empty;

        public PrjEnvelopeSet()
        {
            InitializeComponent();
            txtPixelSizeX.TextChanged += new EventHandler(txtPixelSizeX_TextChanged);
            txtPixelSizeY.TextChanged += new EventHandler(txtPixelSizeY_TextChanged);
            txtResolutionX.TextChanged += new EventHandler(txtResolutionX_TextChanged);
            txtResolutionY.TextChanged += new EventHandler(txtResolutionY_TextChanged);
            txtX.TextChanged += new EventHandler(txtX_TextChanged);
            txtY.TextChanged += new EventHandler(txtY_TextChanged);
        }

        void txtY_TextChanged(object sender, EventArgs e)
        {
            _centerPoint.Y = double.Parse(txtY.Text);
        }

        void txtX_TextChanged(object sender, EventArgs e)
        {
            _centerPoint.X = double.Parse(txtX.Text);
        }

        void txtResolutionY_TextChanged(object sender, EventArgs e)
        {
            _resolutionY = float.Parse(txtResolutionY.Text);
        }

        void txtResolutionX_TextChanged(object sender, EventArgs e)
        {
            _resolutionX = float.Parse(txtResolutionX.Text);
            int sizeX = (int)(_prjEnvelope.Width / _resolutionX + 0.5f);
            txtPixelSizeX.Text = sizeX.ToString();
        }

        void txtPixelSizeY_TextChanged(object sender, EventArgs e)
        {
            _piexlSize.Height = int.Parse(txtPixelSizeY.Text);
            int sizeY = (int)(_prjEnvelope.Height / _resolutionY + 0.5f);
            txtPixelSizeY.Text = sizeY.ToString();
        }

        void txtPixelSizeX_TextChanged(object sender, EventArgs e)
        {
            _piexlSize.Width = int.Parse(txtPixelSizeX.Text);
        }

        internal void SetArgs(ISpatialReference spatialRef, PrjEnvelope prjEnvelope, float resolutionX, float resolutionY)
        {
            _spatialRef = spatialRef;
            _prjEnvelope = prjEnvelope;
            _resolutionX = resolutionX;
            _resolutionY = resolutionY;
            _piexlSize = _prjEnvelope.GetSize(resolutionX, resolutionY);
            txtX.Text = _prjEnvelope.CenterX.ToString();
            txtY.Text = _prjEnvelope.CenterY.ToString();
            txtPixelSizeX.Text = _piexlSize.Width.ToString();
            txtPixelSizeY.Text = _piexlSize.Height.ToString();
            txtResolutionX.Text = resolutionX.ToString();
            txtResolutionY.Text = resolutionY.ToString();
        }

        public float ResolutionX
        {
            get 
            {
                return _resolutionX;
            }
        }

        public float ResolutionY
        {
            get { return _resolutionY; }
        }

        public Size PixelSize
        {
            get { return _piexlSize; }
        }

        public PrjPoint CenterPoint
        {
            get { return _centerPoint; }
        }

        public PrjEnvelope PrjEnvelope
        {
            get 
            {
                return PrjEnvelope.CreateByCenter(_centerPoint.X, _centerPoint.Y, _piexlSize.Width * _resolutionX, _piexlSize.Height * _resolutionY); 
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.RasterProject;

namespace GeoDo.ProjectUI
{
    public partial class UCSetOutRange : UserControl
    {
        private string _selectedName;
        private PointF _outResolution;
        private List<PrjEnvelope> _prjEnvelopeList;
        //中心分辨率
        private double _centerX;
        private double _centerY;
        private double _imgWidth;
        private double _imgHeight;
        //四角经纬度
        private double _minX;
        private double _maxX;
        private double _minY;
        private double _maxY;

        public UCSetOutRange()
        {
            InitializeComponent();
            if (DesignMode)
                return;
            groupBox2.Visible = true;
            _selectedName = "Whole";
        }

        public string SelectedName
        {
            get { return _selectedName; }
            set { _selectedName = value; }
        }

        public PointF OutResolution
        {
            get { return _outResolution; }
            set { _outResolution = value; }
        }

        public List<PrjEnvelope> PrjEnvelopeList
        {
            get { return _prjEnvelopeList; }
            set { _prjEnvelopeList = value; }
        }

        private void tsbWhole_Click(object sender, EventArgs e)
        {
            CheckedEffect(tsbWhole);
            _selectedName = "Whole";
            groupBox2.Visible = true;
            groupBox3.Visible = false;
            groupBox5.Visible = false;
            groupBox7.Visible = false;
        }

        private void tsbCenter_Click(object sender, EventArgs e)
        {
            CheckedEffect(tsbCenter);
            _selectedName = "Center";
            groupBox2.Visible = true;
            groupBox3.Visible = true;
            groupBox5.Visible = false;
            groupBox7.Visible = false;
        }

        private void tsbCorners_Click(object sender, EventArgs e)
        {
            CheckedEffect(tsbCorners);
            _selectedName = "Corners";
            groupBox2.Visible = false;
            groupBox3.Visible = false;
            groupBox5.Visible = true;
            groupBox7.Visible = false;
        }

        private void tsbDefined_Click(object sender, EventArgs e)
        {
            CheckedEffect(tsbDefined);
            _selectedName = "Defined";
            groupBox2.Visible = false;
            groupBox3.Visible = false;
            groupBox5.Visible = false;
            groupBox7.Visible = true;
        }

        private void CheckedEffect(ToolStripButton tsbtn)
        {
            tsbCenter.Checked = false;
            tsbCorners.Checked = false;
            tsbWhole.Checked = false;
            tsbDefined.Checked = false;
            tsbtn.Checked = true;
        }

        private void numImgWidth_ValueChanged(object sender, EventArgs e)
        {
             _imgWidth= (double)numImgWidth.Value;
        }

        private void numImgHeight_ValueChanged(object sender, EventArgs e)
        {
            _imgHeight = (double)numImgWidth.Value;
        }

        private void txtResolutionX_KeyUp(object sender, KeyEventArgs e)
        {
            _outResolution.X = (float)Convert.ToDouble(txtResolutionX.Text);
        }

        private void txtResolutionY_KeyUp(object sender, KeyEventArgs e)
        {
            _outResolution.Y = (float)Convert.ToDouble(txtResolutionY.Text);
        }

        private void txtCenterX_KeyUp(object sender, KeyEventArgs e)
        {
            _centerX = Convert.ToDouble(txtCenterX.Text);
        }

        private void txtCenterY_KeyUp(object sender, KeyEventArgs e)
        {
            _centerY = Convert.ToDouble(txtCenterY.Text);
        }

        private void txtMinX_KeyUp(object sender, KeyEventArgs e)
        {
            _minX = Convert.ToDouble(txtMinX.Text);
        }

        private void txtMaxX_KeyUp(object sender, KeyEventArgs e)
        {
            _maxX = Convert.ToDouble(txtMaxX.Text);
        }

        private void txtMinY_KeyUp(object sender, KeyEventArgs e)
        {
            _minY = Convert.ToDouble(txtMinY.Text);
        }

        private void txtMaxY_KeyUp(object sender, KeyEventArgs e)
        {
            _maxY = Convert.ToDouble(txtMaxY.Text);
        }

        public void SetValue()
        {
            txtCenterX.Text = _centerX.ToString();
            txtCenterY.Text = _centerY.ToString();
            txtResolutionX.Text = _outResolution.X.ToString();
            txtResolutionY.Text = _outResolution.Y.ToString();
            numImgWidth.Value = (decimal)_imgWidth;
            numImgHeight.Value = (decimal)_imgHeight;
            txtMinX.Text = _minX.ToString();
            txtMaxX.Text = _maxX.ToString();
            txtMinY.Text = _minY.ToString();
            txtMaxY.Text = _maxY.ToString();
        }

    }
}

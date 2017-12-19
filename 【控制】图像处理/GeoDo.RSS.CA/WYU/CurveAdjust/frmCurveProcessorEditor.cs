using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.RSS.Core.CA;
using System.ComponentModel.Composition;
using System.Text.RegularExpressions;
using System.Collections;
using System.Diagnostics;

namespace GeoDo.RSS.CA
{
    [Export(typeof(IRgbProcessorArgEditor))]
    public partial class frmCurveProcessorArgEditor : frmRgbArgsEditor
    {
        private enumInterpolatorType _interpolatorType = enumInterpolatorType.Curve;
        private IInterpolator _interpolator = InterpolatorFactory.GetInterpolator(enumInterpolatorType.Curve);
        private CurveAdjustProcessorArg _actualArg = null;
        private byte[] _rgbs = null;
        private CurveControl CurveControl;
        private frm3BandViewer _3bandViewer = null;
        private EventHandler _refreshSubscriber = null;

        public frmCurveProcessorArgEditor()
        {
            InitializeComponent();
            CreateCurveControl();
            Load += new EventHandler(frmCurveProcessorArgEditor_Load);
            _refreshSubscriber = new EventHandler(SubscribeRefresh);
            FormClosed += new FormClosedEventHandler(frmCurveProcessorArgEditor_FormClosed);
        }

        void SubscribeRefresh(object sender, EventArgs e)
        {
            if (_3bandViewer != null)
            {
                _3bandViewer.UpdateView();
            }
        }

        private void CreateCurveControl()
        {
            this.CurveControl = new CA.CurveControl();
            this.groupBox1.Controls.Add(this.CurveControl);
            this.CurveControl.Dock = DockStyle.Top;
            this.CurveControl.Width = 276;
            this.CurveControl.Height = 276;
            this.CurveControl.ControlPointChanged += new EventHandler(CurveControl_ControlPointChanged);
        }

        void CurveControl_ControlPointChanged(object sender, EventArgs e)
        {
            txtInput.Text = this.CurveControl.ControlToPoint(this.CurveControl.CurrentPixelPoint.Location).X.ToString();
            txtOutput.Text = this.CurveControl.ControlToPoint(this.CurveControl.CurrentPixelPoint.Location).Y.ToString();
            TryApply();
        }

        void frmCurveProcessorArgEditor_Load(object sender, EventArgs e)
        {
            InitDefaultArguments();
            Init();
            _env.CanvasRefreshSubscribers.Add(_refreshSubscriber);
        }

        public void StartPickColor(IPickColorIsFinished onPickColorIsFinished)
        {
            return;
        }

        public Bitmap ActiveDrawing
        {
            get
            {
                return _env != null ? (_env.ActiveDrawing != null ? _env.ActiveDrawing : null) : null;
            }
        }

        private void InitDefaultArguments()
        {
            _rgbs = new byte[256];
            for (int i = 0; i < 256; i++)
                _rgbs[i] = (byte)i;
        }

        /// <summary>
        /// 参数编辑窗口初始化
        /// </summary>
        private void Init()
        {
            _actualArg = _arg as CurveAdjustProcessorArg;
            InitBandSelect();
            InitInterpolatorTypeSelect();
            InitLstControlPoints();
        }

        /// <summary>
        /// 插值器类型选择下拉框初始化
        /// </summary>
        private void InitInterpolatorTypeSelect()
        {
            cbxInterpolatorType.Items.Add("折线");
            cbxInterpolatorType.Items.Add("曲线");
            cbxInterpolatorType.SelectedIndexChanged += new EventHandler(cbxInterpolatorType_SelectedIndexChanged);
            cbxInterpolatorType.SelectedIndex = 0;
        }

        /// <summary>
        /// 波段选择下拉框初始化
        /// </summary>
        private void InitBandSelect()
        {
            if (_processor.BytesPerPixel == 1)
            {
                cbxBandSelect.Items.Add("RGB");
                ck3BandView.Visible = false;
            }
            else
            {
                cbxBandSelect.Items.Add("RGB");
                cbxBandSelect.Items.Add("R");
                cbxBandSelect.Items.Add("G");
                cbxBandSelect.Items.Add("B");
            }
            cbxBandSelect.SelectedIndexChanged += new EventHandler(cbxBandSelect_SelectedIndexChanged);
            cbxBandSelect.SelectedIndex = 0;
        }

        /// <summary>
        /// 控制点显示列表初始化
        /// </summary>
        private void InitLstControlPoints()
        {
            lstControlPoints.Items.Clear();
            if (rdPoint.Checked == true)
            {
                lstControlPoints.Items.Add("(0,0)");
                lstControlPoints.Items.Add("(255,255)");
            }
            else if (rdSegment.Checked == true)
            {
                lstControlPoints.Items.Add("(0,255)->(0,255)");
            }
        }

        /// <summary>
        /// 选择插值器类型
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxInterpolatorType_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cbxInterpolatorType.SelectedItem.ToString())
            {
                case "折线":
                    _interpolatorType = enumInterpolatorType.Line;
                    break;
                case "曲线":
                    _interpolatorType = enumInterpolatorType.Curve;
                    break;
            }
            _interpolator = InterpolatorFactory.GetInterpolator(_interpolatorType);
            this.CurveControl.Apply(_interpolator);
            this.CurveControl.Invalidate();
        }

        /// <summary>
        /// 选择波段
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxBandSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            _actualArg.RGB.Values = _actualArg.Red.Values = _actualArg.Green.Values = _actualArg.Blue.Values = null;
            switch (cbxBandSelect.SelectedItem.ToString())
            {
                case "RGB":
                    CurveControl.ColorSel = Color.Black;
                    _actualArg.RGB.Values = _rgbs;
                    break;
                case "R":
                    CurveControl.ColorSel = Color.Red;
                    _actualArg.Red.Values = _rgbs;
                    break;
                case "G":
                    CurveControl.ColorSel = Color.Green;
                    _actualArg.Green.Values = _rgbs;
                    break;
                case "B":
                    CurveControl.ColorSel = Color.Blue;
                    _actualArg.Blue.Values = _rgbs;
                    break;
            }
            this.CurveControl.Apply(_interpolator);
            this.CurveControl.Invalidate();
        }

        /// <summary>
        /// 将控件参数传递到参数对象中
        /// </summary>
        protected override void CollectArguments()
        {
            base.CollectArguments();
            //CurveAdjustProcessorArg actualArg = _arg as CurveAdjustProcessorArg;
            for (int i = 0; i < 256; i++)
                _rgbs[i] = _interpolator.Interpolate((byte)i);
        }

        public void btnReset_Click(object sender, EventArgs e)
        {
            ControlPointInterpolator cpInp = _interpolator as ControlPointInterpolator;
            (_interpolator as ControlPointInterpolator).UpdateControlPoints(
                new Point[] { new Point(0, 0), new Point(255, 255) });
            this.CurveControl.Reset();
            TryApply();
            InitLstControlPoints();
            CurveControl.Invalidate();
        }

        private void rdPoint_CheckedChanged(object sender, EventArgs e)
        {
            lblTip.Text = "(oRgb,tRgb)";
            ChangePointSegment();
        }

        private void rdSegment_CheckedChanged(object sender, EventArgs e)
        {
            lblTip.Text = "(oSegBein,oSegEnd)->(tSegBein,tSegEnd)";
            ChangePointSegment();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (txtInputPoint.Text == null)
                return;

            Point newPoint = new Point();
            string inputString = txtInputPoint.Text;
            if (inputString.Length == 0 || inputString.Length > 11)
                return;
            try
            {
                string[] ins = inputString.Split(',', '，');
                string inputNumber = GetNumbers(ins[0]);
                string outputNumber = GetNumbers(ins[1]);
                if (inputNumber == null || outputNumber == null)
                    return;
                if (inputNumber.Length == 0 || outputNumber.Length == 0)
                    return;
                newPoint.X = int.Parse(inputNumber);
                newPoint.Y = int.Parse(outputNumber);
                if (newPoint.X > 255 || newPoint.X < 0 || newPoint.Y > 255 || newPoint.Y < 0)
                    return;
            }
            finally
            {
                newPoint = this.CurveControl.PointToControl(newPoint);
                this.CurveControl.AddControlPoint(newPoint.X, newPoint.Y);
            }
        }

        private void btnSub_Click(object sender, EventArgs e)
        {
            if (lstControlPoints.SelectedItem == null)
                return;
            int index = lstControlPoints.SelectedIndex;
            int x = this.CurveControl.PixelControlPoints[index].Location.X;
            int y = this.CurveControl.PixelControlPoints[index].Location.Y;
            this.CurveControl.DeleteControlPoint(x, y);
        }

        private void btnEmpty_Click(object sender, EventArgs e)
        {
            InitLstControlPoints();
        }

        private void txtInputPoint_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != 13)
                return;
            btnAdd_Click(this, e);
        }

        /// <summary>
        /// 改变点或线段的选择
        /// </summary>
        private void ChangePointSegment()
        {
            if ((_interpolator as ControlPointInterpolator).ControlPoints.Count() < 3)
                InitLstControlPoints();
            else
                ApplyLstControlPoint();
        }

        /// <summary>
        /// 获取字符串中的数字
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private string GetNumbers(string s)
        {
            string strReturn = string.Empty;
            if (s == null || s.Trim() == null)
                return strReturn;
            foreach (char c in s)
            {
                if (Char.IsNumber(c))
                    strReturn += c.ToString();
            }
            return strReturn;
        }

        protected override void TryApply()
        {
            ApplyLstControlPoint();
            base.TryApply();
        }

        private void ApplyLstControlPoint()
        {
            lstControlPoints.Items.Clear();
            Point[] controlPoints = (_interpolator as ControlPointInterpolator).ControlPoints;
            Point[] pixelControlPoints = controlPoints;// new Point[controlPoints.Length];
            //pixelControlPoints = (Point[])controlPoints.Clone();
            int count = pixelControlPoints.Count();
            if (count < 3)
                return;
            string currentCoordinate = null;
            if (rdPoint.Checked == true)
            {
                foreach (Point pt in pixelControlPoints)
                {
                    currentCoordinate = null;
                    currentCoordinate = "(" + pt.X.ToString() + "," + pt.Y.ToString() + ")";
                    lstControlPoints.Items.Add(currentCoordinate);
                }
            }
            else if (rdSegment.Checked == true)
            {
                for (int i = 0; i < count - 1; i++)
                {
                    currentCoordinate = null;
                    currentCoordinate = "(" + pixelControlPoints[i].X.ToString() + "," + pixelControlPoints[i + 1].X.ToString()
                                   + ") -> (" + pixelControlPoints[i].Y.ToString() + "," + pixelControlPoints[i + 1].Y.ToString()
                                   + ")";
                    lstControlPoints.Items.Add(currentCoordinate);
                }
            }
        }

        public override bool IsSupport(System.Type type)
        {
            return typeof(CurveAdjustProcessor).Equals(type);
        }

        private void threeBandViewer_CheckedChanged(object sender, EventArgs e)
        {
            if (ck3BandView.Checked)
            {
                if (_3bandViewer == null)
                    _3bandViewer = new frm3BandViewer();
                _3bandViewer.Owner = this;
                _3bandViewer.Location = new Point(this.Location.X + this.Width, this.Location.Y);
                _3bandViewer.Show();
                _3bandViewer.UpdateView();
                _3bandViewer.FormClosed += new FormClosedEventHandler(_3bandViewer_FormClosed);
            }
            else
            {
                (_3bandViewer as frm3BandViewer).Close();
                _3bandViewer = null;
            }
        }

        void _3bandViewer_FormClosed(object sender, FormClosedEventArgs e)
        {
            ck3BandView.Checked = false;
        }

        private void Update3BandViewer()
        {
            if (_3bandViewer != null)
                _3bandViewer.UpdateView();
        }

        private void frmCurveProcessorArgEditor_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (DialogResult != System.Windows.Forms.DialogResult.OK)
                if (_argEditorCancelClick != null && !Modal)
                    _argEditorCancelClick(this, _arg);
            if (_3bandViewer != null)
            {
                (_3bandViewer as Form).Close();
                _3bandViewer = null;
            }
            if (_env.CanvasRefreshSubscribers.Contains(_refreshSubscriber))
                _env.CanvasRefreshSubscribers.Remove(_refreshSubscriber);
        }
    }
}

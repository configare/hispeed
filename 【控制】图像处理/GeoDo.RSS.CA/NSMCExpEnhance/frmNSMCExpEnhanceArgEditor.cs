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

namespace GeoDo.RSS.CA
{
    [Export(typeof(IRgbProcessorArgEditor))]
    public partial class frmNSMCExpEnhanceArgEditor : frmRgbArgsEditor, IActiveDrawing
    {
        private PointsControl _curveControl;
        private Dictionary<string, PointsControl> _curveControls;
        private Point[] ControlPoints;

        private EventHandler<Point> curveControlCoordinatesChangedDelegate;
        private System.EventHandler curveControlValueChangedDelegate;        

        public frmNSMCExpEnhanceArgEditor()
        {
            InitializeComponent();

            curveControlValueChangedDelegate = this.curveControl_ValueChanged;
            curveControlCoordinatesChangedDelegate = this.curveControl_CoordinatesChanged;

            this._curveControls = new Dictionary<string, PointsControl>();
            _curveControls.Add("RGB", new PointsControlRGB());
            _curveControls.Add("Luminosity", new PointsControlLuminosity());

            InitMaskComboBox();

            _refreshSubscriber = new EventHandler(SubscribeRefresh);

            this.FormClosed += new FormClosedEventHandler(frmCurveProcessorArgEditor_FormClosed);
            this.Load += new EventHandler(frmCurveProcessorArgEditor_Load);
        }

        [Browsable(false)]
        public event EventHandler EffectTokenChanged;
        protected virtual void OnEffectTokenChanged()
        {
            if (EffectTokenChanged != null)
            {
                EffectTokenChanged(this, EventArgs.Empty);
            }
        }

        public void FinishTokenUpdate()
        {
            InitTokenFromDialog();
            OnEffectTokenChanged();
            //controlPoint1.ControlPoints = this.ControlPoints[selectedChannel];
            TryApply();
        }

        protected void InitTokenFromDialog()
        {
            //ColorTransferMode = curveControl.ColorTransferMode;
            ControlPoints = (Point[])_curveControl.ControlPoints.Clone();
        }

        bool[] _maskModeSelected;
        private void InitMaskComboBox()
        {
            maskComboBox.Items.Clear();
            maskComboBox.Items.AddRange(new string[] { "RGB", "R", "G", "B" });
            maskComboBox.SelectedIndex = 0;
            _maskModeSelected = new bool[maskComboBox.Items.Count];
        }

        private void maskComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            MaskMode maskMode;
            if (this.maskComboBox.SelectedIndex >= 0)
            {
                maskMode = (MaskMode)Enum.Parse(typeof(MaskMode), this.maskComboBox.SelectedItem.ToString());
            }
            else
            {
                maskMode = MaskMode.RGB;
            }
            InitDialogFromMaskMode(maskMode);
        }

        protected void InitDialogFromMaskMode()
        {
            InitDialogFromMaskMode(MaskMode.RGB);
        }

        private void InitDialogFromMaskMode(MaskMode maskMode)
        {
            switch (maskMode)
            {
                case MaskMode.R:
                    UpdateColorTransferMode("RGB");
                    _curveControl.SetSelected(0, true);
                    _curveControl.SetSelected(1, false);
                    _curveControl.SetSelected(2, false);
                    break;
                case MaskMode.G:
                    UpdateColorTransferMode("RGB");
                    _curveControl.SetSelected(0, false);
                    _curveControl.SetSelected(1, true);
                    _curveControl.SetSelected(2, false);
                    break;
                case MaskMode.B:
                    UpdateColorTransferMode("RGB");
                    _curveControl.SetSelected(0, false);
                    _curveControl.SetSelected(1, false);
                    _curveControl.SetSelected(2, true);
                    break;
                default:
                case MaskMode.RGB:
                    UpdateColorTransferMode("Luminosity");
                    _curveControl.SetSelected(0, true);
                    break;
            }
        }

        private void UpdateColorTransferMode(string colorTransferMode)
        {
            PointsControl newCurveControl;

            newCurveControl = _curveControls[colorTransferMode];

            if (_curveControl != newCurveControl)
            {
                tableLayoutMain.Controls.Remove(_curveControl);
                _curveControl = newCurveControl;
                _curveControl.Bounds = new Rectangle(0, 0, 258, 258);
                _curveControl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
                //_curveControl.ResetControlPoints();
                _curveControl.Dock = System.Windows.Forms.DockStyle.Fill;
                _curveControl.ValueChanged += curveControlValueChangedDelegate;
                _curveControl.CoordinatesChanged += curveControlCoordinatesChangedDelegate;
                tableLayoutMain.Controls.Add(_curveControl, 1, 0);

                int channels = newCurveControl.Channels;
            }

            FinishTokenUpdate();
        }

        private void curveControl_ValueChanged(object sender, EventArgs e)
        {
            this.FinishTokenUpdate();
        }

        private void curveControl_CoordinatesChanged(object sender, EventArgs<Point> e)
        {
            Point pt = e.Data;
            string newText;

            if (pt.X >= 0)
            {
                string format = "({0},{1})";
                newText = string.Format(format, pt.X, pt.Y);
            }
            else
            {
                newText = string.Empty;
            }

            if (newText != labelCoordinates.Text)
            {
                labelCoordinates.Text = newText;
                labelCoordinates.Update();
            }
        }

        void SubscribeRefresh(object sender, EventArgs e)
        {
            Update3BandViewer();
        }

        private void frmCurveProcessorArgEditor_Load(object sender, EventArgs e)
        {
            _env.CanvasRefreshSubscribers.Add(_refreshSubscriber);
            ck3BandView.Checked = true;
        }

        private void frmCurveProcessorArgEditor_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (DialogResult != System.Windows.Forms.DialogResult.OK)
            {
                if (_argEditorCancelClick != null && !Modal)
                    _argEditorCancelClick(this, _arg);
            }
            if (_3bandViewer != null)
            {
                (_3bandViewer as Form).Close();
                _3bandViewer = null;
            }
            if (_env.CanvasRefreshSubscribers.Contains(_refreshSubscriber))
                _env.CanvasRefreshSubscribers.Remove(_refreshSubscriber);
        }
       
        #region IActiveDrawing
        public Bitmap ActiveDrawing
        {
            get
            {
                return _env != null ? (_env.ActiveDrawing != null ? _env.ActiveDrawing : null) : null;
            }
        }

        private EventHandler _refreshSubscriber = null;
        private frm3BandViewer _3bandViewer = null;
        private void ck3BandView_CheckedChanged(object sender, EventArgs e)
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
                if (_3bandViewer != null)
                {
                    (_3bandViewer as frm3BandViewer).Close();
                    _3bandViewer = null;
                }
            }
        }

        void _3bandViewer_FormClosed(object sender, FormClosedEventArgs e)
        {
            _3bandViewer = null;
            ck3BandView.Checked = false;
        }

        private void Update3BandViewer()
        {
            if (_3bandViewer != null)
                _3bandViewer.UpdateView();
            //_curveControl.UpdateActiveDrawing(ActiveDrawing);
        }
        #endregion

        protected override void CollectArguments()
        {
            if (_arg == null)
                return;
            NSMCExpEnhanceArg actualArg = _arg as NSMCExpEnhanceArg;
            if (_curveControl.Channels == 3)
            {
                for (int i = 0; i < _curveControl.Channels; i++)
                {
                    PointsControl curve = _curveControl;
                    if (i == 0)
                        actualArg.RedControlPoint = curve.ControlPoints[i];
                    else if (i == 1)
                        actualArg.GreenControlPoint = curve.ControlPoints[i];
                    else if (i == 2)
                        actualArg.BlueControlPoint = curve.ControlPoints[i];
                }
            }
            else
            {
                PointsControl curve = _curveControl;
                actualArg.RedControlPoint = curve.ControlPoints[0];
                actualArg.GreenControlPoint = curve.ControlPoints[0];
                actualArg.BlueControlPoint = curve.ControlPoints[0];
            }

            base.CollectArguments();
        }

        public override bool IsSupport(System.Type type)
        {
            return typeof(NSMCExpEnhanceProcessor).Equals(type);
        }

        private void resetButton_Click(object sender, EventArgs e)
        {
            _curveControl.ResetControlPoints();
            this.FinishTokenUpdate();
        }
    }
}

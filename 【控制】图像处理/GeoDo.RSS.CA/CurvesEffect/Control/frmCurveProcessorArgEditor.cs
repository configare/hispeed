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
//CurvesEffectConfigDialog
namespace GeoDo.RSS.CA
{
    [Export(typeof(IRgbProcessorArgEditor))]
    public partial class frmCurveProcessorArgEditor : frmRgbArgsEditor
    {
        private CurveControl curveControl;
        private Dictionary<ColorTransferMode, CurveControl> curveControls;
        private SortedList<int, int>[] ControlPoints;
        private ColorTransferMode ColorTransferMode;
        private bool finishTokenOnDropDownChanged = true;
        //private CheckBox[] maskCheckBoxes;
        private EventHandler maskCheckChanged;
        private System.EventHandler curveControlValueChangedDelegate;
        private EventHandler<Point> curveControlCoordinatesChangedDelegate;

        public frmCurveProcessorArgEditor()
        {
            InitializeComponent();

            curveControlValueChangedDelegate = this.curveControl_ValueChanged;
            curveControlCoordinatesChangedDelegate = this.curveControl_CoordinatesChanged;

            this.maskCheckChanged = new EventHandler(MaskCheckChanged);

            this.curveControls = new Dictionary<ColorTransferMode, CurveControl>();
            this.curveControls.Add(ColorTransferMode.Luminosity, new CurveControlLuminosity());
            this.curveControls.Add(ColorTransferMode.Rgb, new CurveControlRgb());

            controlPoint1.ControlPointChanged += new EventHandler(controlPoint1_ControlPointChanged);

            InitMaskComboBox();

            this.interpolatorTypeComBox.Items.Clear();
            this.interpolatorTypeComBox.Items.AddRange(new string[] {"折线","曲线" });
            this.interpolatorTypeComBox.SelectedIndex = 0;
            this.interpolatorTypeComBox.SelectedIndexChanged+=new EventHandler(interpolatorTypeComBox_SelectedIndexChanged);

            _refreshSubscriber = new EventHandler(SubscribeRefresh);
            this.FormClosed += new FormClosedEventHandler(frmCurveProcessorArgEditor_FormClosed);
            this.Load += new EventHandler(frmCurveProcessorArgEditor_Load);
        }

        public Bitmap ActiveDrawing
        {
            get
            {
                return _env != null ? (_env.ActiveDrawing != null ? _env.ActiveDrawing : null) : null;
            }
        }

        void frmCurveProcessorArgEditor_Load(object sender, EventArgs e)
        {
            _env.CanvasRefreshSubscribers.Add(_refreshSubscriber);
        }

        void SubscribeRefresh(object sender, EventArgs e)
        {
            Update3BandViewer();
        }

        void controlPoint1_ControlPointChanged(object sender, EventArgs e)
        {
            this.FinishTokenUpdate();
            curveControl.Invalidate();
        }

        protected void InitTokenFromDialog()
        {
            ColorTransferMode = curveControl.ColorTransferMode;
            ControlPoints = (SortedList<int, int>[])curveControl.ControlPoints.Clone();
        }

        //protected void InitDialogFromToken(ColorTransferMode colorTransferMode)
        //{
        //    switch (colorTransferMode)
        //    {
        //        case ColorTransferMode.Luminosity:
        //            modeComboBox.SelectedItem = colorTransferNames.EnumValueToLocalizedName(ColorTransferMode.Luminosity);
        //            break;

        //        case ColorTransferMode.Rgb:
        //            modeComboBox.SelectedItem = colorTransferNames.EnumValueToLocalizedName(ColorTransferMode.Rgb);
        //            break;
        //    }
        //    curveControl.ControlPoints = (SortedList<int, int>[])ControlPoints.Clone();
        //    curveControl.Invalidate();
        //    curveControl.Update();
        //}

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
                string format = "({0},{1})";//PdnResources.GetString("CurvesEffectConfigDialog.Coordinates.Format");// format = 
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

        private void resetButton_Click(object sender, System.EventArgs e)
        {
            curveControl.ResetControlPoints();
            this.FinishTokenUpdate();
        }

        private void MaskCheckChanged(object sender, System.EventArgs e)
        {
            //for (int i = 0; i < maskCheckBoxes.Length; ++i)
            //{
            //    if (maskCheckBoxes[i] == sender)
            //    {
            //        curveControl.SetSelected(i, maskCheckBoxes[i].Checked);
            //    }
            //}

            //UpdateCheckboxEnables();
        }
        //private EnumLocalizer colorTransferNames = EnumLocalizer.Create(typeof(ColorTransferMode));
        
        //private void modeComboBox_SelectedIndexChanged(object sender, System.EventArgs e)
        //{
        //    ColorTransferMode colorTransferMode;

        //    if (modeComboBox.SelectedIndex >= 0)
        //    {
        //        colorTransferMode = (ColorTransferMode)colorTransferNames.LocalizedNameToEnumValue(modeComboBox.SelectedItem.ToString());
        //    }
        //    else
        //    {
        //        colorTransferMode = ColorTransferMode.Rgb;
        //    }
        //    UpdateColorTransferMode(colorTransferMode);
        //}

        private void UpdateColorTransferMode(ColorTransferMode colorTransferMode)
        {
            CurveControl newCurveControl;

            newCurveControl = curveControls[colorTransferMode];

            if (curveControl != newCurveControl)
            {
                tableLayoutMain.Controls.Remove(curveControl);
                curveControl = newCurveControl;
                curveControl.Bounds = new Rectangle(0, 0, 258, 258);
                curveControl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
                //curveControl.ResetControlPoints();
                tableLayoutMain.SetColumnSpan(this.curveControl, 3);
                curveControl.Dock = System.Windows.Forms.DockStyle.Fill;
                curveControl.ValueChanged += curveControlValueChangedDelegate;
                curveControl.CoordinatesChanged += curveControlCoordinatesChangedDelegate;
                tableLayoutMain.Controls.Add(curveControl, 1, 2);

                int channels = newCurveControl.Channels;

                //maskCheckBoxes = new CheckBox[channels];
                
                //for (int i = 0; i < channels; ++i)
                //{
                //    CheckBox checkbox = new CheckBox();

                //    checkbox.Dock = DockStyle.Fill;
                //    checkbox.Checked = curveControl.GetSelected(i);
                //    checkbox.CheckedChanged += maskCheckChanged;
                //    checkbox.Text = curveControl.GetChannelName(i);

                //    maskCheckBoxes[i] = checkbox;
                //}

                //UpdateCheckboxEnables();
            }

            if (finishTokenOnDropDownChanged)
            {
                FinishTokenUpdate();
            }
        }

        //private void UpdateCheckboxEnables()
        //{
        //    int countChecked = 0;

        //    for (int i = 0; i < maskCheckBoxes.Length; ++i)
        //    {
        //        if (maskCheckBoxes[i].Checked)
        //        {
        //            ++countChecked;
        //        }
        //    }

        //    if (maskCheckBoxes.Length == 1)
        //    {
        //        maskCheckBoxes[0].Enabled = false;
        //    }
        //}

        [Browsable(false)]
        public event EventHandler EffectTokenChanged;
        protected virtual void OnEffectTokenChanged()
        {
            if (EffectTokenChanged != null)
            {
                EffectTokenChanged(this, EventArgs.Empty);
            }
        }

        [Obsolete("Use FinishTokenUpdate() instead", true)]
        public void UpdateToken()
        {
            FinishTokenUpdate();
        }

        public void FinishTokenUpdate()
        {
            InitTokenFromDialog();
            OnEffectTokenChanged();
            controlPoint1.ControlPoints = this.ControlPoints[selectedChannel];
            TryApply();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            //InitDialogFromToken();
            InitDialogFromMaskMode();
            FinishTokenUpdate();
        }

        //protected void InitDialogFromToken()
        //{
        //    // If we don't check for null, we get awful errors in the designer.
        //    // Good idea to check for that anyway, yeah?
        //    //if (theEffectToken != null)
        //    //{
        //        InitDialogFromToken(ColorTransferMode.Rgb);
        //    //}
        //}

        protected void InitDialogFromMaskMode()
        {
            InitDialogFromMaskMode(MaskMode.RGB);
        }

        private int selectedChannel = 0;

        private void InitDialogFromMaskMode(MaskMode maskMode)
        {
            switch (maskMode)
            {
                case MaskMode.R:
                    selectedChannel = 0;
                    UpdateColorTransferMode(ColorTransferMode.Rgb);
                    curveControl.SetSelected(0, true);
                    curveControl.SetSelected(1, false);
                    curveControl.SetSelected(2, false);
                    break;
                case MaskMode.G:
                    selectedChannel = 1;
                    UpdateColorTransferMode(ColorTransferMode.Rgb);
                    curveControl.SetSelected(0, false);
                    curveControl.SetSelected(1, true);
                    curveControl.SetSelected(2, false);
                    break;
                case MaskMode.B:
                    selectedChannel = 2;
                    UpdateColorTransferMode(ColorTransferMode.Rgb);
                    curveControl.SetSelected(0, false);
                    curveControl.SetSelected(1, false);
                    curveControl.SetSelected(2, true);
                    break;
                default:
                case MaskMode.RGB:
                    selectedChannel = 0;
                    UpdateColorTransferMode(ColorTransferMode.Luminosity);
                    curveControl.SetSelected(0, true);
                    break;
            }
            //UpdateCheckboxEnables();
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

        /// <summary>
        /// 将控件参数传递到参数对象中
        /// </summary>
        protected override void CollectArguments()
        {
            if (_arg == null)
                return;
            CurveAdjustProcessorArg actualArg = _arg as CurveAdjustProcessorArg;
            IInterpolator interpolator = GetInterpolator();
            if (curveControl.Channels == 3)
            {
                for (int i = 0; i < curveControl.Channels; i++)
                {
                    interpolator.Clear();
                    CurveControl curve = curveControl;// curveControls[CA.ColorTransferMode.Rgb];
                    SortedList<int, int> cps = curve.ControlPoints[i];
                    for (int s = 0; s < cps.Count; s++)
                    {
                        interpolator.Add(cps.Keys[s], cps.Values[s]);
                    }
                    byte[] rgbMap = new byte[curveControl.Entries];
                    for (int j = 0; j < curveControl.Entries; j++)
                    {
                        rgbMap[j] = (byte)Utility.Clamp(interpolator.Interpolate((byte)j), 0, curveControl.Entries-1);
                    }
                    if (i == 0)
                        actualArg.Red.Values = rgbMap;
                    else if (i == 1)
                        actualArg.Green.Values = rgbMap;
                    else if (i == 2)
                        actualArg.Blue.Values = rgbMap;
                }
                actualArg.RGB.Values = null;
            }
            else
            {
                interpolator.Clear();
                CurveControl curve = curveControl;// curveControls[CA.ColorTransferMode.Luminosity];                
                SortedList<int, int> cps = curve.ControlPoints[0];
                for (int s = 0; s < cps.Count; s++)
                {
                    interpolator.Add(cps.Keys[s], cps.Values[s]);
                }
                byte[] rgbMap = new byte[curveControl.Entries];
                for (int j = 0; j < curveControl.Entries; j++)
                {
                    rgbMap[j] = (byte)Utility.Clamp(interpolator.Interpolate((byte)j), 0, curveControl.Entries - 1);
                }
                actualArg.RGB.Values = rgbMap;
                actualArg.Red.Values = null;
                actualArg.Green.Values = null;
                actualArg.Blue.Values = null;
            }
            base.CollectArguments();
        }

        protected override void TryApply()
        {
            base.TryApply();
        }

        public override bool IsSupport(System.Type type)
        {
            return typeof(CurveAdjustProcessor).Equals(type);
        }

        private string interpolatorType = "折线";

        private void interpolatorTypeComBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            interpolatorType = interpolatorTypeComBox.SelectedItem.ToString();
            InterpolatorTypeChanged();
        }

        private void InterpolatorTypeChanged()
        {
            curveControl.InterpolatorType = interpolatorType;
            this.FinishTokenUpdate();
            curveControl.Invalidate();
        }

        private IInterpolator GetInterpolator()
        {
            return InterpolatorFactory.GetInterpolator(interpolatorType);
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

        #region 加载预定义

        private void LoadDefLine(IInterpolator line)
        {
            if (line is LineInterpolator)
            {
                interpolatorTypeComBox.SelectedIndex = 0;
                maskComboBox.SelectedIndex = 0;
                SortedList<int, int> sf = new SortedList<int, int>();
                sf.Add(0, 255);
                sf.Add(255, 0);
                curveControl.ControlPoints = new SortedList<int, int>[] { sf };
            }
            else
            {
            }
        }
        #endregion

        private void button1_Click(object sender, EventArgs e)
        {
            LoadDefLine(new LineInterpolator());
        }
    }
}

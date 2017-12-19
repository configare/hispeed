using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.CA;

namespace GeoDo.RSS.CA
{
    [Export(typeof(IRgbProcessorArgEditor))]
    public partial class frmReplaceColorArgEditor : frmRgbArgsEditor, IPickColorIsFinished
    {

        private ReplaceColorArg _actualArg = null;
        private bool _isFromTxt = false;
        private OnColorPickIsFinishedHandler _onColorPickIsFinished = null;

        public frmReplaceColorArgEditor()
        {
            InitializeComponent();
            Load += new EventHandler(frmReplaceColorArgEditor_Load);
        }

        void frmReplaceColorArgEditor_Load(object sender, EventArgs e)
        {
            Init();
        }

        /// <summary>
        /// 参数编辑窗口自己的初始化
        /// </summary>
        private void Init()
        {
            _actualArg = _arg as ReplaceColorArg;
            if (_onColorPickIsFinished == null)
                _onColorPickIsFinished = new OnColorPickIsFinishedHandler(OnColorPicked);
            _actualArg.TargetColor = panelTargetColor.BackColor;
        }

        private void OnColorPicked(object sender, Color rgb)
        {
            panelTargetColor.BackColor = rgb;
            _actualArg.TargetColor = rgb;
            TryApply();
        }

        private void panelTargetColor_MouseDown(object sender, MouseEventArgs e)
        {
            Panel p = sender as Panel;
            using (ColorDialog dlg = new ColorDialog())
            {
                dlg.Color = p.BackColor;
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    p.BackColor = dlg.Color;
                    if (p.Equals(panelTargetColor))
                        _actualArg.TargetColor = p.BackColor;
                    FillArgToControls();
                }
            }
        }

        private void FillArgToControls()
        {
            panelTargetColor.BackColor = _actualArg.TargetColor;
            txtTolerance.Value = _actualArg.ColorTorence;
            txtHue.Value = (int)_actualArg.Hue;
            txtSaturation.Value = (int)_actualArg.Saturation;
            txtLightness.Value = (int)_actualArg.Lightness;
        }

        private void btnPickColor_Click(object sender, EventArgs e)
        {
            _env.StartPickColor(this as IPickColorIsFinished);
        }

        private void trackTolerance_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_isFromTxt)
                txtTolerance.Value = trackTolerance.Value;
            _isFromTxt = false;
            TryApply();
        }

        private void txtTolerance_ValueChanged(object sender, EventArgs e)
        {
            _isFromTxt = true;
            trackTolerance.Value = (int)txtTolerance.Value;
            TryApply();
        }

        private void trackHue_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_isFromTxt)
                txtHue.Value = trackHue.Value;
            _isFromTxt = false;
            TryApply();
        }

        private void txtHue_ValueChanged(object sender, EventArgs e)
        {
            _isFromTxt = true;
            trackHue.Value = (int)txtHue.Value;
            TryApply();
        }

        private void trackSaturation_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_isFromTxt)
                txtSaturation.Value = trackSaturation.Value;
            _isFromTxt = false;
            TryApply();
        }

        private void txtSaturation_ValueChanged(object sender, EventArgs e)
        {
            _isFromTxt = true;
            trackSaturation.Value = (int)txtSaturation.Value;
            TryApply();
        }

        private void trackBrightness_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_isFromTxt)
                txtLightness.Value = trackLightness.Value;
            _isFromTxt = false;
            TryApply();
        }

        private void txtBrightness_ValueChanged(object sender, EventArgs e)
        {
            _isFromTxt = true;
            trackLightness.Value = (int)txtLightness.Value;
            TryApply();
        }

        /// <summary>
        /// 将控件中的参数值赋值到参数对象中
        /// </summary>
        protected override void CollectArguments()
        {
            _actualArg.Hue = (int)txtHue.Value;
            _actualArg.Saturation = (int)txtSaturation.Value;
            _actualArg.Lightness = (int)txtLightness.Value;
            _actualArg.TargetColor = panelTargetColor.BackColor;
            _actualArg.ColorTorence = (int)txtTolerance.Value;
        }

        public override bool IsSupport(Type type)
        {
            return type.Equals(typeof(RgbProcessorReplacedColor));
        }

        protected override void TryApply()
        {
            byte red = _actualArg.TargetColor.R;
            byte green = _actualArg.TargetColor.G;
            byte blue = _actualArg.TargetColor.B;
            (_processor as RgbProcessorReplacedColor).ColorAdjust(ref red, ref green, ref blue, (int)_actualArg.Hue, (int)_actualArg.Saturation, (int)_actualArg.Lightness);
            panelResultColor.BackColor = Color.FromArgb(red, green, blue);
            base.TryApply();
        }

        public void PickColorFinished(Color color)
        {
            panelTargetColor.BackColor = color;
        }

        public void Picking(Color color)
        {
            panelTargetColor.BackColor = color;
        }
    }
}

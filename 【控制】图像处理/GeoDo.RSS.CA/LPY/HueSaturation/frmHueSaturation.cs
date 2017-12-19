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
    public partial class frmHueSaturation : frmRgbArgsEditor
    {
        private HSLColorArg _actualArg = null;
        private bool _isFromTxt = false;
        private byte[] _rgbs = null;
        private byte[] _reds = null;
        private byte[] _greens = null;
        private byte[] _blues = null;

        public frmHueSaturation()
        {
            InitializeComponent();
            Load += new EventHandler(frmHueSaturation_Load);
        }

        void frmHueSaturation_Load(object sender, EventArgs e)
        {
            Init();
        }

        private void Init()
        {
            _actualArg = _arg as HSLColorArg;
            _rgbs = new byte[256];
            _reds = new byte[256];
            _greens = new byte[256];
            _blues = new byte[256];
            for (int i = 0; i < 256; i++)
                _reds[i] = _greens[i] = _blues[i] = _rgbs[i] = (byte)i;
        }

        private void comboTargetColor_SelectedIndexChanged(object sender, EventArgs e)
        {
            trackBarHue.Value = _actualArg.PixelHue;
            trackBarSaturation.Value = _actualArg.PixelSaturation;
            trackBarLum.Value = _actualArg.PixelLum;
        }
        private void trackBarHue_Scroll(object sender, EventArgs e)
        {
            if (!_isFromTxt)
                numericHue.Value = trackBarHue.Value;
            _isFromTxt = false;
            _actualArg.PixelHue = (int)numericHue.Value;
            //尝试应用新的参数（触发外部渲染操作）
            TryApply();
        }

        private void trackBarSaturation_Scroll(object sender, EventArgs e)
        {
            if (!_isFromTxt)
                numericSaturation.Value = trackBarSaturation.Value;
            _isFromTxt = false;
            _actualArg.PixelSaturation = (int)numericSaturation.Value;
            //尝试应用新的参数（触发外部渲染操作）
            TryApply();
        }

        private void trackBarLum_Scroll(object sender, EventArgs e)
        {
            if (!_isFromTxt)
                numericLum.Value = trackBarLum.Value;
            _isFromTxt = false;
            _actualArg.PixelSaturation = (int)numericSaturation.Value;
            //尝试应用新的参数（触发外部渲染操作）
            TryApply();
        }

        private void numericHue_ValueChanged(object sender, EventArgs e)
        {
            _isFromTxt = true;
            trackBarHue.Value = (int)numericHue.Value;
            TryApply();
        }

        private void numericSaturation_ValueChanged(object sender, EventArgs e)
        {
            _isFromTxt = true;
            trackBarSaturation.Value = (int)numericSaturation.Value;
            TryApply();
        }

        private void numericLum_ValueChanged(object sender, EventArgs e)
        {
            _isFromTxt = true;
            trackBarLum.Value = (int)numericLum.Value;
            TryApply();
        }

        protected override void CollectArguments()
        {
            _actualArg.PixelHue = (int)numericHue.Value;
            _actualArg.PixelSaturation = (int)numericSaturation.Value;
            _actualArg.PixelLum = (int)numericLum.Value;
            _actualArg.IsChanged = true;
        }

        public override bool IsSupport(Type type)
        {
            return type.Equals(typeof(HueSaturationProcess));
        }

    }
}

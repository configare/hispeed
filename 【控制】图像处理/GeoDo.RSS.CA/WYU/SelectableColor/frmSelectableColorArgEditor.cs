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
    public partial class frmSelectableColor : frmRgbArgsEditor
    {
        private SelectableColorArg _actualArg = null;
        private SelectableColorArgItem _currentArgItem = null;
        private bool _isFromTxt = false;

        public frmSelectableColor()
        {
            InitializeComponent();
            Load += new EventHandler(frmSelectableColor_Load);
        }
        
        void frmSelectableColor_Load(object sender, EventArgs e)
        {
            Init();
        }

        /// <summary>
        /// 参数编辑窗口自己的初始化
        /// </summary>
        private void Init()
        {
            _actualArg = _arg as SelectableColorArg;
            InitTargetColors();
        }

        private void InitTargetColors()
        {
            foreach (enumSelectableColor color in Enum.GetValues(typeof(enumSelectableColor)))
            {
                switch (color)
                {
                    case enumSelectableColor.Red:
                        cbxTargetColor.Items.Add(new SelectableColorTargetColorItem("红色", color));
                        break;
                    case enumSelectableColor.Yellow:
                        cbxTargetColor.Items.Add(new SelectableColorTargetColorItem("黄色", color));
                        break;
                    case enumSelectableColor.Green:
                        cbxTargetColor.Items.Add(new SelectableColorTargetColorItem("绿色", color));
                        break;
                    case enumSelectableColor.Cyan:
                        cbxTargetColor.Items.Add(new SelectableColorTargetColorItem("青色", color));
                        break;
                    case enumSelectableColor.Blue:
                        cbxTargetColor.Items.Add(new SelectableColorTargetColorItem("蓝色", color));
                        break;
                    case enumSelectableColor.Magenta:
                        cbxTargetColor.Items.Add(new SelectableColorTargetColorItem("洋红", color));
                        break;
                    case enumSelectableColor.Black:
                        cbxTargetColor.Items.Add(new SelectableColorTargetColorItem("黑色", color));
                        break;
                    case enumSelectableColor.Neutrals:
                        cbxTargetColor.Items.Add(new SelectableColorTargetColorItem("中性色", color));
                        break;
                    case enumSelectableColor.White:
                        cbxTargetColor.Items.Add(new SelectableColorTargetColorItem("白色", color));
                        break;
                }
            }
            cbxTargetColor.SelectedIndexChanged += new EventHandler(comboTargetColor_SelectedIndexChanged);
            cbxTargetColor.SelectedIndex = 0;
        }

        private void comboTargetColor_SelectedIndexChanged(object sender, EventArgs e)
        {
            _currentArgItem = _actualArg.GetSelectableColorArgItem((cbxTargetColor.SelectedItem as SelectableColorTargetColorItem).Color);
            trackCyan.Value = _currentArgItem.CyanAdjustValue;
            trackMagenta.Value = _currentArgItem.MagentaAdjustValue;
            trackYellow.Value = _currentArgItem.YellowAdjustValue;
            trackBlack.Value = _currentArgItem.BlackAdjustValue;
            txtCyan.Value = trackCyan.Value;
            txtMagenta.Value = trackMagenta.Value;
            txtYellow.Value = trackYellow.Value;
            txtBlack.Value = trackBlack.Value;
        }

        private void trackCyan_ValueChanged(object sender, EventArgs e)
        {
            if (!_isFromTxt)
                txtCyan.Value = trackCyan.Value;
            _isFromTxt = false;
            _currentArgItem.CyanAdjustValue = (int)txtCyan.Value;
            //尝试应用新的参数（触发外部渲染操作）
            TryApply();
        }

        private void txtCyan_ValueChanged(object sender, EventArgs e)
        {
            _isFromTxt = true;
            trackCyan.Value = (int)txtCyan.Value;
            TryApply();
        }

        private void trackMagenta_ValueChanged(object sender, EventArgs e)
        {
            if (!_isFromTxt)
                txtMagenta.Value = trackMagenta.Value;
            _isFromTxt = false;
            _currentArgItem.MagentaAdjustValue = (int)txtMagenta.Value;
            //尝试应用新的参数（触发外部渲染操作）
            TryApply();
        }

        private void txtMagenta_ValueChanged(object sender, EventArgs e)
        {
            _isFromTxt = true;
            trackMagenta.Value = (int)txtMagenta.Value;
            TryApply();
        }

        private void trackYellow_ValueChanged(object sender, EventArgs e)
        {
            if (!_isFromTxt)
                txtYellow.Value = trackYellow.Value;
            _isFromTxt = false;
            _currentArgItem.YellowAdjustValue = (int)txtYellow.Value;
            //尝试应用新的参数（触发外部渲染操作）
            TryApply();
        }

        private void txtYellow_ValueChanged(object sender, EventArgs e)
        {
            _isFromTxt = true;
            trackYellow.Value = (int)txtYellow.Value;
            TryApply();
        }

        private void trackBlack_ValueChanged(object sender, EventArgs e)
        {
            if (!_isFromTxt)
                txtBlack.Value = trackBlack.Value;
            _isFromTxt = false;
            _currentArgItem.BlackAdjustValue = (int)txtBlack.Value;
            //尝试应用新的参数（触发外部渲染操作）
            TryApply();
        }

        private void txtBlack_ValueChanged(object sender, EventArgs e)
        {
            _isFromTxt = true;
            trackBlack.Value = (int)txtBlack.Value;
            TryApply();
        }

        private void rdRelative_CheckedChanged(object sender, EventArgs e)
        {
            _actualArg.ApplyType = rdAbsolute.Checked ? enumSelectableColorApplyType.Absolute : enumSelectableColorApplyType.Relative;
        }

        private void rdAbsolute_CheckedChanged(object sender, EventArgs e)
        {
            _actualArg.ApplyType = rdAbsolute.Checked ? enumSelectableColorApplyType.Absolute : enumSelectableColorApplyType.Relative;
        }

        /// <summary>
        /// 将控件中的参数值赋值到参数对象中
        /// </summary>
        protected override void CollectArguments()
        {
            _currentArgItem.MagentaAdjustValue = (int)txtMagenta.Value;
            _currentArgItem.CyanAdjustValue = (int)txtCyan.Value;
            _currentArgItem.YellowAdjustValue = (int)txtYellow.Value;
            _currentArgItem.BlackAdjustValue = (int)txtBlack.Value;
        }

        public override bool IsSupport(System.Type type)
        {
            return type.Equals(typeof(RgbProcessorSelectableColor));
        }
    }
}

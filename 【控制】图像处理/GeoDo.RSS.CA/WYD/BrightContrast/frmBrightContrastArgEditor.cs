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
    /// <summary>
    /// 亮度/对比度
    /// </summary>
    [Export(typeof(IRgbProcessorArgEditor))]
    public partial class frmBrightContrastColor : frmRgbArgsEditor
    {
        private BrightContrastArg _bcArg = null;

        public frmBrightContrastColor()
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
            _bcArg = _arg as BrightContrastArg;
            InitRGBChannel();
            InitControl();
        }

        /// <summary>
        /// 初始化控件值
        /// </summary>
        private void InitControl()
        {
            trackBarBright.Minimum = trackBarContrast.Minimum = -100;
            trackBarBright.Maximum = trackBarContrast.Maximum = 100;
            trackBarBright.Value = _bcArg.BrightAdjustValue;
            trackBarContrast.Value = _bcArg.ContrastAdjustValue;
            numericBright.Value = _bcArg.BrightAdjustValue;
            numericContrast.Value = _bcArg.ContrastAdjustValue;
            txtChangeValue.Text = _bcArg.Brightchangevalue.ToString();
        }

        /// <summary>
        /// 初始化下拉框
        /// </summary>
        private void InitRGBChannel()
        {
            cbxChannel.Items.Clear();
            if (_processor.BytesPerPixel == 1)
                cbxChannel.Items.Add(enumChannel.RGB);
            else
                foreach (enumChannel channel in Enum.GetValues(typeof(enumChannel)))
                    cbxChannel.Items.Add(channel);
            cbxChannel.SelectedIndex = 0;
        }

        /// <summary>
        /// 将控件中的参数值赋值到参数对象中
        /// </summary>
        protected override void CollectArguments()
        {
            _bcArg.IsChanged = true;
            _bcArg.Channel = (enumChannel)cbxChannel.SelectedItem;
            _bcArg.ContrastAdjustValue = trackBarContrast.Value;
            _bcArg.BrightAdjustValue = trackBarBright.Value;
        }

        public override bool IsSupport(System.Type type)
        {
            return type.Equals(typeof(BrightContrastProcessor));
        }

        /// <summary>
        /// 亮度滑块滑动事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void trackBarBright_ValueChanged(object sender, EventArgs e)
        {
            numericBright.Value = trackBarBright.Value;
            //尝试应用新的参数（触发外部渲染操作）
            TryApply();
        }

        /// <summary>
        /// 对比度滑块滑动事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void trackBarContrast_ValueChanged(object sender, EventArgs e)
        {
            numericContrast.Value = trackBarContrast.Value;
            //尝试应用新的参数（触发外部渲染操作）
            TryApply();
        }

        private void cbxChannel_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void txtChangeValue_KeyDown(object sender, KeyEventArgs e)
        {
        }

        /// <summary>
        /// 调整值修改事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtChangeValue_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (Convert.ToDouble(txtChangeValue.Text.Trim()) >= 0 && Convert.ToDouble(txtChangeValue.Text.Trim()) <= 5)
                {
                    string s = txtChangeValue.Text.Trim();
                    _bcArg.Brightchangevalue = Convert.ToDouble(s);
                }
                else
                {
                    MessageBox.Show("只能输入0-5之间的数,请重新输入!", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("只能输入0-5之间的数,请重新输入!", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
            finally
            {
                txtChangeValue.Text = _bcArg.Brightchangevalue.ToString();
            }
        }

        private void numericBright_ValueChanged(object sender, EventArgs e)
        {
            trackBarBright.Value = Convert.ToInt32(numericBright.Value);
            TryApply();
        }

        private void numericContrast_ValueChanged(object sender, EventArgs e)
        {
            trackBarContrast.Value = Convert.ToInt32(numericContrast.Value);
            TryApply();
        }
    }
}

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
using System.Drawing.Imaging;

namespace GeoDo.RSS.CA.ArgEditor
{
    /// <summary>
    /// 色阶参数类
    /// </summary>
    [Export(typeof(IRgbProcessorArgEditor))]
    public partial class frmLevelColorArgEditor : frmRgbArgsEditor
    {
        private LevelColorArg _lcArg = null;
        private bool _isFirst = true;
        private float _midValue;

        public frmLevelColorArgEditor()
        {
            InitializeComponent();

            Load += new EventHandler(frmSelectableColor_Load);
        }

        void frmSelectableColor_Load(object sender, EventArgs e)
        {
            Init();
        }

        private void Init()
        {
            _lcArg = _arg as LevelColorArg;
            InitRGBChannel();
            InitTextBox();
           // TryApply();
            InitUcCtrl();
        }

        private void InitUcCtrl()
        {
            ucSlider2.Display(3);
            ucSlider3.Display(2);
            ucHistogram.Display(_lcArg.ObjPixelCount.PixelCountRGB);
            if (_isFirst)
            {
                ucSlider2.Value[1] = 127.5F;
                ucSlider2.Value[2] = 255;
                ucSlider3.Value[1] = 255;
                _isFirst = false;
            }
        }

        /// <summary>
        /// 设定控件初始值
        /// </summary>
        private void InitTextBox()
        {
            numericOutputMin.Value = 0;
            numericOutputMax.Value = 255;
            txtInputMid.Text = "1";
            numericInputMin.Value = 0;
            numericInputMax.Value = 255;
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
            _lcArg.IsChanged = true;
            _lcArg.Channel = (enumChannel)cbxChannel.SelectedItem;
            _lcArg.InputMin = (int)numericInputMin.Value;
            _lcArg.InputMiddle = Convert.ToDouble(txtInputMid.Text);
            _lcArg.InputMax = (int)numericInputMax.Value;
            _lcArg.OutputMin = (int)numericOutputMin.Value;
            _lcArg.OutputMax = (int)numericOutputMax.Value;
        }

        public override bool IsSupport(System.Type type)
        {
            return type.Equals(typeof(LevelColorProcessor));
        }

        private void cbxChannel_SelectedIndexChanged(object sender, EventArgs e)
        {
            _lcArg.Channel = (enumChannel)cbxChannel.SelectedItem;
            switch (_lcArg.Channel)
            {
                case enumChannel.RGB:
                    {
                        ucHistogram.Display(_lcArg.ObjPixelCount.PixelCountRGB, 0, 255, Color.Black);
                        break;
                    }
                case enumChannel.R:
                    {
                        ucHistogram.Display(_lcArg.ObjPixelCount.PixelCountRed, 0, 255, Color.Red);
                        break;
                    }
                case enumChannel.G:
                    {
                        ucHistogram.Display(_lcArg.ObjPixelCount.PixelCountGreen, 0, 255, Color.Green);
                        break;
                    }
                case enumChannel.B:
                    {
                        ucHistogram.Display(_lcArg.ObjPixelCount.PixelCountBlue, 0, 255, Color.Blue);
                        break;
                    }
            }
        }

        private void numericInputMin_ValueChanged(object sender, EventArgs e)
        {
            _midValue = (float)Convert.ToDouble(txtInputMid.Text);
            ucSlider2.SetSliderValue((float)numericInputMin.Value, 0);
            TryApply();
            SetMidValue();
        }

        private void numericInputMax_ValueChanged(object sender, EventArgs e)
        {
            _midValue = (float)Convert.ToDouble(txtInputMid.Text);
            ucSlider2.SetSliderValue((float)numericInputMax.Value, 2);
            TryApply();
            SetMidValue();
        }

        private void ucSlider2_BarValueChanged(object sender)
        {
            numericInputMin.Value = (int)(ucSlider2.Value[0]);
            numericInputMax.Value = (int)(ucSlider2.Value[2]);
            double value = ucSlider2.Value[2] - ucSlider2.Value[1];
            double valueCha = (ucSlider2.Value[2] - ucSlider2.Value[0]) / 2;
            if (ucSlider2.Value[1] > (ucSlider2.Value[2] + ucSlider2.Value[0]) / 2)
            {
                txtInputMid.Text = (YanZhengInMid(value / valueCha)).ToString();
            }
            else
            {
                txtInputMid.Text = (YanZhengInMid(((value - valueCha) / valueCha) * 9 + 1)).ToString();
            }
            TryApply();
        }

        private void SetMidValue()
        {
            float mid = (ucSlider2.Value[2] - ucSlider2.Value[0]) / 2;
            if (_midValue <= 1)
            {
                ucSlider2.SetSliderValue(ucSlider2.Value[2] - mid * _midValue, 1);
            }
            else
            {
                ucSlider2.SetSliderValue(ucSlider2.Value[0] + mid - (_midValue - 1) * mid / 9, 1);
            }
        }

        private float YanZhengInMid(float value)
        {
            if (value > 9.99)
                value = 9.99F;
            if (value < 0.01)
                value = 0.01F;
            return value;
        }

        private float YanZhengInMid(double value)
        {
            if (value > 9.99)
                value = 9.99F;
            if (value < 0.01)
                value = 0.01F;
            return (float)value;
        }

        private void numericOutputMin_ValueChanged(object sender, EventArgs e)
        {
            if (numericOutputMin.Value < numericOutputMax.Value)
            {
                ucSlider3.SetSliderValue((int)numericOutputMin.Value, 0);
            }
            else
            {
                ucSlider3.SetSliderValue((int)numericOutputMax.Value - 1, 1);
                numericOutputMin.Value = numericOutputMax.Value - 1;
            }
            ucSlider3.Value[0] = (int)numericOutputMin.Value;
            TryApply();
        }

        private void numericOutputMax_ValueChanged(object sender, EventArgs e)
        {
            if (numericOutputMax.Value > numericOutputMin.Value)
            {
                ucSlider3.SetSliderValue((int)numericOutputMax.Value, 1);
            }
            else
            {
                ucSlider3.SetSliderValue((int)numericOutputMin.Value + 1, 1);
                numericOutputMax.Value = numericOutputMin.Value + 1;
            }
            ucSlider3.Value[1] = (int)numericOutputMax.Value;
            TryApply();
        }

        private void ucSlider3_BarValueChanged(object sender)
        {
            numericOutputMin.Value = (int)(ucSlider3.GetValue(0));
            numericOutputMax.Value = (int)(ucSlider3.GetValue(1));
        }

        private void txtInputMid_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                try
                {
                    _midValue = YanZhengInMid(Convert.ToDouble(txtInputMid.Text.Trim()));
                }
                catch (Exception)
                {
                    MessageBox.Show("请输入0~10的整数或小数！");
                    _midValue = 1;
                }

                SetMidValue();
                txtInputMid.Text = _midValue.ToString();
                TryApply();
            }
        }
    }
}

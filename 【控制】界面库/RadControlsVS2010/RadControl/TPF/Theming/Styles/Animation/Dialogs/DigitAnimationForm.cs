using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Telerik.WinControls
{
    public partial class DigitAnimationForm : AnimationForm
    {
        public DigitAnimationForm()
        {
            InitializeComponent();
        }

        private object ConvertFromDecimal(Decimal value)
        {
            if (this.Property.PropertyType == typeof(int))
            {
                return Convert.ToInt32(value);
            }
            
            if (this.Property.PropertyType == typeof(float))
            {
                return (float)Convert.ToDouble(value);
            }

            if (this.Property.PropertyType == typeof(double))
            {
                return Convert.ToDouble(value);
            }

            if (this.Property.PropertyType == typeof(decimal))
            {
                return value;
            }

            throw new ArgumentException("Internal error: Expected property type int, float, double or decimal type, but was: " + this.Property.PropertyType.FullName);
        }

        public override void GetSetting()
        {
            base.GetSetting();

            this.generalAnimationSettings1.GetValues();
            this.AnimatedSetting.StartValueIsCurrentValue = (this.checkBoxUseCurrentValue.Checked);
        
            this.AnimatedSetting.Property = AnimatedSetting.Property;

            if (this.checkBoxUseCurrentValue.Checked)
                this.AnimatedSetting.Value = null;
            else
                this.AnimatedSetting.Value = this.ConvertFromDecimal(this.numericUpDownStart.Value);

            if (this.AnimatedSetting.AnimationType == RadAnimationType.ByStartEndValues)
            {
                this.AnimatedSetting.EndValue = this.ConvertFromDecimal(this.numericUpDownEnd.Value);
            }
            else
            {

                this.AnimatedSetting.Step = XmlAnimatedPropertySetting.SerializeStep(this.ConvertFromDecimal(this.numericUpDownStep.Value));
                if (!this.checkBoxAutomaticReverse.Checked)
                {
                    this.AnimatedSetting.ReverseStep = XmlAnimatedPropertySetting.SerializeStep(this.ConvertFromDecimal(this.numericUpDownReverseStep.Value));
                }
            }
        }

        private void IntAnimationForm_Load(object sender, EventArgs e)
        {
            Decimal minValue = Decimal.MinValue;
            Decimal maxValue = Decimal.MaxValue;
            int decimalPlaces  = 0;

            if (Property.PropertyType == typeof(float))
            {
                decimalPlaces = 2;
            }
            else if (Property.PropertyType == typeof(double))
            {
                decimalPlaces = 3;
            }
           
            this.numericUpDownStart.Minimum = minValue;
            this.numericUpDownStart.Maximum = maxValue;
            this.numericUpDownStart.DecimalPlaces = decimalPlaces;
            this.numericUpDownEnd.Minimum = minValue;
            this.numericUpDownEnd.Maximum = maxValue;
            this.numericUpDownEnd.DecimalPlaces = decimalPlaces;
            this.numericUpDownStep.Minimum = minValue;
            this.numericUpDownStep.Maximum = maxValue;
            this.numericUpDownStep.DecimalPlaces = decimalPlaces;
            this.numericUpDownReverseStep.Minimum = minValue;
            this.numericUpDownReverseStep.Maximum = maxValue;
            this.numericUpDownReverseStep.DecimalPlaces = decimalPlaces;

            if (AnimatedSetting.Value != null)
            {
                if (Property.PropertyType == typeof(float))
                {
                    this.numericUpDownStart.Value = Convert.ToDecimal((float)AnimatedSetting.Value); //Convert.ToDecimal((float)XmlPropertySetting.DeserializeValue(Property, AnimatedSetting.Value));
                }
                else
                    if (Property.PropertyType == typeof(double))
                {
                    this.numericUpDownStart.Value = Convert.ToDecimal((double)AnimatedSetting.Value); // Convert.ToDecimal((double)XmlPropertySetting.DeserializeValue(Property, AnimatedSetting.Value));
                }
                if (this.numericUpDownStart.Value == 0)
                    this.checkBoxUseCurrentValue.Checked = true;
            }
            if (AnimatedSetting.EndValue != null)
                this.numericUpDownEnd.Value = Convert.ToDecimal(AnimatedSetting.EndValue);//Convert.ToDecimal(XmlPropertySetting.DeserializeValue(Property, AnimatedSetting.EndValue));
            if (AnimatedSetting.Step != null)
                this.numericUpDownStep.Value = Convert.ToDecimal(XmlPropertySetting.DeserializeValue(Property, AnimatedSetting.Step.Value));
            if (AnimatedSetting.ReverseStep != null)
                this.numericUpDownReverseStep.Value = Convert.ToDecimal(XmlPropertySetting.DeserializeValue(Property, AnimatedSetting.ReverseStep.Value));
            else
            {
                this.checkBoxAutomaticReverse.Checked = true;
                this.numericUpDownReverseStep.Enabled = false;  
            }
        }

        private void checkBoxAutomaticReverse_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBoxAutomaticReverse.Checked)
                this.numericUpDownReverseStep.Enabled = false;
            else
                this.numericUpDownReverseStep.Enabled = true;
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            this.GetSetting();
            if (this.AnimatedSetting.AnimationType == RadAnimationType.ByStartEndValues)
            {
                if (this.AnimatedSetting.EndValue == null)//string.IsNullOrEmpty(this.AnimatedSetting.EndValue))
                {
                    MessageBox.Show("Please specify animation EndValue or change animation type to 'By Step'");
                    return;
                }
            }

            this.DialogResult = DialogResult.OK;
        }

        private void checkBoxUseCurrentValue_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxUseCurrentValue.Checked)
            {
                this.numericUpDownStart.Enabled = false;
            }
            else
            {
                this.numericUpDownStart.Enabled = true;
            }
        }

        private void numericUpDownStart_ValueChanged(object sender, EventArgs e)
        {

        }

        private void generalAnimationSettings1_Load(object sender, EventArgs e)
        {

        }

        private void groupBox3_Enter(object sender, EventArgs e)
        {

        }

        private void groupBox4_Enter(object sender, EventArgs e)
        {

        }
    }
}
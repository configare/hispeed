using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Telerik.WinControls
{
    // NOT TESTED REASON:
    // NO POINT PROPERTY AVAILABLE AT THE MOMENT 
  
    public partial class PointAnimationForm : AnimationForm
    {

        public PointAnimationForm()
        {
            InitializeComponent();
            this.generalAnimationSettings1.groupBox2.Visible = false;
        }

        private void PointFAnimationForm_Load(object sender, EventArgs e)
        {
            Decimal minValue = Decimal.MinValue;
            Decimal maxValue = Decimal.MaxValue;

            int decimalPlaces = 0;

            if (Property.PropertyType == typeof(float))
            {
                decimalPlaces = 2;
            }
            else if (Property.PropertyType == typeof(double))
            {
                decimalPlaces = 3;
            }

            this.numericUpDownStart1.Minimum = minValue;
            this.numericUpDownStart1.Maximum = maxValue;
            this.numericUpDownStart1.DecimalPlaces = decimalPlaces;
            this.numericUpDownEnd1.Minimum = minValue;
            this.numericUpDownEnd1.Maximum = maxValue;
            this.numericUpDownEnd1.DecimalPlaces = decimalPlaces;

            if (AnimatedSetting.Value != null)
            {
                if (Property.PropertyType == typeof(Point))
                {
                    Point pt = (Point)AnimatedSetting.Value;
                    this.numericUpDownStart1.Value = pt.X; //Convert.ToDecimal((float)XmlPropertySetting.DeserializeValue(Property, AnimatedSetting.Value));
                    this.numericUpDownEnd1.Value = pt.Y;
                }
                  if (this.numericUpDownStart1.Value == 0)
                    this.checkBoxUseCurrentValue.Checked = true;
            }
            if (AnimatedSetting.EndValue != null)
            {
                Point pt = (Point)AnimatedSetting.EndValue;
                this.numericUpDownStart2.Value = pt.X; //Convert.ToDecimal((float)XmlPropertySetting.DeserializeValue(Property, AnimatedSetting.Value));
                this.numericUpDownEnd2.Value = pt.Y;
            }
        }

        public override void GetSetting()
        {
            base.GetSetting();

            this.generalAnimationSettings1.GetValues();

            this.AnimatedSetting.Property = AnimatedSetting.Property;
            this.AnimatedSetting.StartValueIsCurrentValue = (this.checkBoxUseCurrentValue.Checked);
        
            if (this.checkBoxUseCurrentValue.Checked)
                this.AnimatedSetting.Value = null;
            else
                this.AnimatedSetting.Value = new Point((int)(this.numericUpDownStart1.Value),
                    (int)(this.numericUpDownEnd1.Value));


            if (this.AnimatedSetting.AnimationType == RadAnimationType.ByStartEndValues)
            {
                this.AnimatedSetting.EndValue = new Point((int)(this.numericUpDownStart2.Value),
                    (int)(this.numericUpDownEnd2.Value));
            }

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

        private void buttonCancel_Click(object sender, EventArgs e)
        {

        }

        private void checkBoxUseCurrentValue_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxUseCurrentValue.Checked)
            {
            //    this.tbStartRectangle.Enabled = false;
            }
            else
            {
             //   this.tbStartRectangle.Enabled = true;
            }
        }

        private void numericUpDownStart_ValueChanged(object sender, EventArgs e)
        {

        }
    }
}
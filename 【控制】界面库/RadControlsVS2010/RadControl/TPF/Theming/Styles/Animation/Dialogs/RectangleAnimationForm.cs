using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Telerik.WinControls
{
    public partial class RectangleAnimationForm : AnimationForm
    {
        public RectangleAnimationForm()
        {
            InitializeComponent();
        }

        private void RectangleAnimationForm_Load(object sender, EventArgs e)
        {
            if (AnimatedSetting.Value != null)
            {
                this.tbStartRectangle.Text = XmlPropertySetting.SerializeValue(this.Property, AnimatedSetting.Value);
                this.checkBoxUseCurrentValue.Checked = false;
            }
            else
                this.checkBoxUseCurrentValue.Checked = true;

            if (AnimatedSetting.EndValue != null)
                this.tbEndRectangle.Text = XmlPropertySetting.SerializeValue(this.Property, AnimatedSetting.EndValue);

            if (AnimatedSetting.Step != null)
            {
                //TODO: What if the step is different type?
                this.tbStepRectangle.Text = AnimatedSetting.Step.Value;
            }
            else
            {
                this.checkBoxUseCurrentValue.Checked = true;
            }

            if (AnimatedSetting.ReverseStep != null)
            {
                //TODO: What if the step is different type?
                this.tbReversedStepRectangle.Text = AnimatedSetting.ReverseStep.Value;
            }

            if (AnimatedSetting.ReverseStep == null)
            {
                this.checkBoxAutomaticReverse.Checked = true;
            }
        }

        private Rectangle GetValidatedRectangle(string stringRectangle)
        {
            if (string.IsNullOrEmpty(stringRectangle) || stringRectangle.Trim() == string.Empty)
            {
                return Rectangle.Empty;
            }

            TypeConverter converter = TypeDescriptor.GetConverter(typeof(Rectangle));

            return (Rectangle)converter.ConvertFrom(stringRectangle);
        }

		public override void GetSetting()
		{
			base.GetSetting();

			this.generalAnimationSettings1.GetValues();
            this.AnimatedSetting.StartValueIsCurrentValue = (this.checkBoxUseCurrentValue.Checked);
        
			if (!this.checkBoxUseCurrentValue.Checked)
			{
				this.AnimatedSetting.Value = XmlPropertySetting.DeserializeValue(this.Property, this.tbStartRectangle.Text);
			}
			else
			{
				this.AnimatedSetting.Value = null;
			}

			if (this.AnimatedSetting.AnimationType == RadAnimationType.ByStartEndValues)
			{
				this.AnimatedSetting.EndValue = XmlPropertySetting.DeserializeValue(Property, this.tbEndRectangle.Text);
			}
			else
			{
				this.AnimatedSetting.Step = XmlAnimatedPropertySetting.SerializeStep(XmlPropertySetting.DeserializeValue(this.Property, tbStepRectangle.Text));

				if (!checkBoxAutomaticReverse.Checked)
				{
					this.AnimatedSetting.ReverseStep =
						XmlAnimatedPropertySetting.SerializeStep(XmlPropertySetting.DeserializeValue(this.Property, tbReversedStepRectangle.Text));
				}
			}
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
                this.tbStartRectangle.Enabled = false;
            }
            else
            {
                this.tbStartRectangle.Enabled = true;
            }
        }

        private void tbStartSize_Validating(object sender, CancelEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            if (tb.Text == string.Empty)
            {
                return;
            }

            try
            {
                this.GetValidatedRectangle(tb.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Please enter a valid rectangle formated: 'left, top, width, heigh', or clear the existing value; Exception message: " + ex.Message);
                e.Cancel = true;
            }
        }
    }
}
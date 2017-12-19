using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Telerik.WinControls
{
    public partial class FontAnimationForm : AnimationForm
    {
        public FontAnimationForm()
        {
            InitializeComponent();
        }

        private void FontAnimationForm_Load(object sender, EventArgs e)
        {
            if (AnimatedSetting.Step != null)
            {
                //TODO: What if the step is different type?
                FontAnimationStep step = (FontAnimationStep)XmlAnimatedPropertySetting.DeserializeStep(AnimatedSetting.Step);

                this.tbStepSize.Text = step.SizeStep.ToString();
            }
            else
            {
                this.checkBoxUseCurrentValue.Checked = true;
            }

            if (AnimatedSetting.ReverseStep != null)
            {
                //TODO: What if the step is different type?
                FontAnimationStep step = (FontAnimationStep)XmlAnimatedPropertySetting.DeserializeStep(AnimatedSetting.ReverseStep);

                this.tbReversedStepSize.Text = step.SizeStep.ToString();                
            }

            if (AnimatedSetting.Value != null)
            {
                this.tbStartSize.Text = ((Font)AnimatedSetting.Value).Size.ToString(); //((Font)XmlPropertySetting.DeserializeValue(Property, AnimatedSetting.Value)).Size.ToString();
                this.checkBoxUseCurrentValue.Checked = false;
            }
            else
                this.checkBoxUseCurrentValue.Checked = true;


            if (AnimatedSetting.EndValue != null)
                this.tbEndSize.Text = ((Font)AnimatedSetting.EndValue).Size.ToString(); //((Font)XmlPropertySetting.DeserializeValue(Property, AnimatedSetting.EndValue)).Size.ToString();

            if (AnimatedSetting.ReverseStep == null)
            {
                this.checkBoxAutomaticReverse.Checked = true;
            }
        }

        private Font GetFont(string text)
        {
            try
            {
                return new Font(this.Font.FontFamily, float.Parse(text));
            }
            catch
            {
                return new Font(this.Font.FontFamily, 0);
            }
        }

        public override void GetSetting()
        {
            base.GetSetting();

            this.generalAnimationSettings1.GetValues();
            this.AnimatedSetting.StartValueIsCurrentValue = (this.checkBoxUseCurrentValue.Checked);
        
            if (!this.checkBoxUseCurrentValue.Checked)
            {
                this.AnimatedSetting.Value = this.GetFont(this.tbStartSize.Text);
            }
            else
            {
                this.AnimatedSetting.Value = null;
            }

            if (this.AnimatedSetting.AnimationType == RadAnimationType.ByStartEndValues)
            {
                this.AnimatedSetting.EndValue = this.GetFont(this.tbEndSize.Text);
            }
            else
            {
                this.AnimatedSetting.Step = XmlAnimatedPropertySetting.SerializeStep(
                    new FontAnimationStep(this.GetFont(this.tbStepSize.Text).Size)
                    );

                if (!checkBoxAutomaticReverse.Checked)
                {
                    this.AnimatedSetting.ReverseStep = XmlAnimatedPropertySetting.SerializeStep(
                        new FontAnimationStep(this.GetFont(this.tbReversedStepSize.Text).Size)
                        );
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
                this.tbStartSize.Enabled = false;
            }
            else
            {
                this.tbStartSize.Enabled = true;
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
                float.Parse(tb.Text);
            }
            catch(Exception ex)
            {
                MessageBox.Show("Please enter a valid floating point value or clear the existing value; Exception message: " + ex.Message);
                e.Cancel = true;
            }
        }        
    }
}
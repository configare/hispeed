using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Telerik.WinControls
{
    public partial class ColorAnimationForm : AnimationForm
    {
        public ColorAnimationForm()
        {
            InitializeComponent();
        }

        private void ColorAnimationForm_Load(object sender, EventArgs e)
        {
            if (AnimatedSetting.Step != null)
            {
                //TODO: What if the step is different type?
                ColorAnimationStep step = (ColorAnimationStep)XmlAnimatedPropertySetting.DeserializeStep(AnimatedSetting.Step);

                this.upDownStepA.Value = step.A;
                this.upDownStepR.Value = step.R;
                this.upDownStepG.Value = step.G;
                this.upDownStepB.Value = step.B;
            }
            else
            {
                this.checkBoxUseCurrentValue.Checked = true;
            }

            if (AnimatedSetting.ReverseStep != null)
            {
                //TODO: What if the step is different type?
                ColorAnimationStep step = (ColorAnimationStep)XmlAnimatedPropertySetting.DeserializeStep(AnimatedSetting.ReverseStep);

                this.upDownReverseStepA.Value = step.A;
                this.upDownReverseStepR.Value = step.R;
                this.upDownReverseStepG.Value = step.G;
                this.upDownReverseStepB.Value = step.B;
            }

            if (AnimatedSetting.Value != null)
            {
                this.colorComboStart.SelectedColor = (Color)AnimatedSetting.Value;//(Color)XmlPropertySetting.DeserializeValue(Property, AnimatedSetting.Value);
                this.checkBoxUseCurrentValue.Checked = false;                
            }
            else
                this.checkBoxUseCurrentValue.Checked = true;


            if (AnimatedSetting.EndValue != null)
                this.colorComboEnd.SelectedColor = (Color)AnimatedSetting.EndValue;//(Color)XmlPropertySetting.DeserializeValue(Property, AnimatedSetting.EndValue);

            if (AnimatedSetting.ReverseStep == null)
            {
                this.checkBoxAutomaticReverse.Checked = true;
            }
        }

        public override void GetSetting()
        {
            base.GetSetting();

            this.generalAnimationSettings1.GetValues();
            this.AnimatedSetting.StartValueIsCurrentValue = (this.checkBoxUseCurrentValue.Checked);
        
            if (!this.checkBoxUseCurrentValue.Checked)
            {
                this.AnimatedSetting.Value = this.colorComboStart.SelectedColor;
            }
            else
            {
                this.AnimatedSetting.Value = null;
            }

            if (this.AnimatedSetting.AnimationType == RadAnimationType.ByStartEndValues)
            {
                this.AnimatedSetting.EndValue = this.colorComboEnd.SelectedColor;
            }
            else
            {
                this.AnimatedSetting.Step = XmlAnimatedPropertySetting.SerializeStep(
                    new ColorAnimationStep(
                        (int)this.upDownStepA.Value,
                        (int)this.upDownStepR.Value,
                        (int)this.upDownStepG.Value,
                        (int)this.upDownStepB.Value
                        )
                    );

                if (!checkBoxAutomaticReverse.Checked)
                {
                    this.AnimatedSetting.ReverseStep = XmlAnimatedPropertySetting.SerializeStep(
                        new ColorAnimationStep(
                            (int)this.upDownReverseStepA.Value,
                            (int)this.upDownReverseStepR.Value,
                            (int)this.upDownReverseStepG.Value,
                            (int)this.upDownReverseStepB.Value
                            )
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
                    MessageBox.Show("Please specify animation EndValue or change animation type to 'By Step'", "Telerik Visual Style Builder", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            this.DialogResult = DialogResult.OK;
        }

        private void checkBoxUseCurrentValue_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxUseCurrentValue.Checked)
            {
                this.colorComboStart.Enabled = false;
            }
            else
            {
                this.colorComboStart.Enabled = true;
            }
        }
    }
}
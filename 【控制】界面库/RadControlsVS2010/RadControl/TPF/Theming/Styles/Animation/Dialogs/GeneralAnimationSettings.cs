using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Telerik.WinControls
{
	[ToolboxItem(false), ComVisible(false)]
    public partial class GeneralAnimationSettings : UserControl
    {
        public GeneralAnimationSettings()
        {
            InitializeComponent();
        }

        public RadAnimationType AnimationType
        {
            get
            {
                if (this.radioButtonByStartEnd.Checked)
                    return RadAnimationType.ByStartEndValues;
                else
                    return RadAnimationType.ByStep;
            }
            set 
            {
                if (value == RadAnimationType.ByStep)
                    radioButtonByStep.Checked = true;
                else
                    radioButtonByStartEnd.Checked = true;
            }
        }

        private void GeneralAnimationSettings_Load(object sender, EventArgs e)
        {
            XmlAnimatedPropertySetting setting = (Parent as AnimationForm).AnimatedSetting;
            if (setting != null)
            {
                this.comboBoxAnimationStyle.DataSource = Enum.GetValues(typeof(AnimatorStyles));
                this.comboBoxAnimationStyle.SelectedIndex = (int)setting.AnimatorStyle;
                this.comboBoxApplyET.DataSource = Enum.GetValues(typeof(RadEasingType));
                this.comboBoxUnApplyET.DataSource = Enum.GetValues(typeof(RadEasingType));
                this.comboBoxApplyET.SelectedIndex = (int)setting.ApplyEasingType;
                this.comboBoxUnApplyET.SelectedIndex = (int)setting.UnapplyEasingType;
                if (setting.Interval == 0) setting.Interval = 1;
                this.numericUpDownInterval.Value = 1000/setting.Interval;
                this.numericUpDownFrames.Value = setting.Interval*setting.NumFrames;
                if (setting.NumFrames == -1)
                {
                    this.checkBoxInfinitePositions.Checked = true;
                    this.numericUpDownFrames.Enabled = false;
                }

                this.AnimationType = setting.AnimationType;
            }
        }

        public void GetValues()
        {
            XmlAnimatedPropertySetting setting = (Parent as AnimationForm).AnimatedSetting;
            setting.AnimatorStyle = (AnimatorStyles)this.comboBoxAnimationStyle.SelectedIndex;
            setting.ApplyEasingType = (RadEasingType)this.comboBoxApplyET.SelectedIndex;
            setting.UnapplyEasingType = (RadEasingType)this.comboBoxUnApplyET.SelectedIndex;
            if (this.numericUpDownInterval.Value == 0) this.numericUpDownInterval.Value = 1;
            setting.Interval = (int)Math.Round((1000/this.numericUpDownInterval.Value));
            setting.NumFrames = (int)Math.Round((this.numericUpDownFrames.Value/(1000/this.numericUpDownInterval.Value)));
            if (this.checkBoxInfinitePositions.Checked)
                setting.NumFrames = -1;

            setting.AnimationType = this.AnimationType;
        }

        private void numericUpDownInterval_ValueChanged(object sender, EventArgs e)
        {

        }

        private void comboBoxApplyET_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

    }
}

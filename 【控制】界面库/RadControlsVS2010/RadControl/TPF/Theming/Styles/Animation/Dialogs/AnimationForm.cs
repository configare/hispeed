using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Telerik.WinControls
{
    public partial class AnimationForm : Form
    {
        public AnimationForm()
        {
            InitializeComponent();
        }


        [Browsable(false)]
        XmlAnimatedPropertySetting animatedSetting;
        public XmlAnimatedPropertySetting AnimatedSetting
        {
            get { return animatedSetting; }
            set { animatedSetting = value; }
        }

        [Browsable(false)]
        RadProperty property;
        public RadProperty Property
        {
            get { return property; }
            set { property = value; }
        }

        public virtual void GetSetting()
        {
        }

        private void AnimationForm_Load(object sender, EventArgs e)
        {

        }
    }
}
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Telerik.WinControls.Themes.ControlDefault
{
    public partial class RadioButton : ControlDefaultThemeComponent
    {
        public RadioButton()
        {
            InitializeComponent();
        }

        public RadioButton(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }
    }
}

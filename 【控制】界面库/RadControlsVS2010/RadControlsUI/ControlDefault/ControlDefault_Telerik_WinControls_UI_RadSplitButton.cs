using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;

namespace Telerik.WinControls.Themes.ControlDefault
{
    public partial class SplitButton : ControlDefaultThemeComponent
    {
        public SplitButton()
        {
            InitializeComponent();
        }

        public SplitButton(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }
    }
}

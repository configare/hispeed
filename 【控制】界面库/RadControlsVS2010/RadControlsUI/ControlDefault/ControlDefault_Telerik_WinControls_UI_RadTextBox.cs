using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Telerik.WinControls.Themes.ControlDefault
{
    public partial class TextBox : ControlDefaultThemeComponent
    {
        public TextBox()
        {
            InitializeComponent();
        }

        public TextBox(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }
    }
}

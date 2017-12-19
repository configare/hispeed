using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Telerik.WinControls.Themes.ControlDefault
{
    public partial class ComboBox : ControlDefaultThemeComponent
    {
        public ComboBox()
        {
            InitializeComponent();
        }

        public ComboBox(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }
    }
}

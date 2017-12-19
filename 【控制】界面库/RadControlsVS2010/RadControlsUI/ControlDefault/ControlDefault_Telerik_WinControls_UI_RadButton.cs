using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Telerik.WinControls.Themes.ControlDefault
{
    public partial class Button : ControlDefaultThemeComponent
    {
        public Button()
        {
            InitializeComponent();
        }

        public Button(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;

namespace Telerik.WinControls.Themes.ControlDefault
{
    public partial class SizablePopup : ControlDefaultThemeComponent
    {
        public SizablePopup()
        {
            InitializeComponent();
        }

        public SizablePopup(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }
    }
}

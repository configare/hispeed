using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Telerik.WinControls.Themes.ControlDefault
{
    public partial class RadMaskEditBox : ControlDefaultThemeComponent
    {
        public RadMaskEditBox()
        {
            InitializeComponent();
        }

        public RadMaskEditBox(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }
    }
}

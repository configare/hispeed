using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Telerik.WinControls.Themes.ControlDefault
{
    public partial class MaskEditBox : ControlDefaultThemeComponent
    {
        public MaskEditBox()
        {
            InitializeComponent();
        }

        public MaskEditBox(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }
    }
}

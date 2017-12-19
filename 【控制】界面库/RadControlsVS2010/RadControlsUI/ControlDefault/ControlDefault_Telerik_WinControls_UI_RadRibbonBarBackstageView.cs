using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Telerik.WinControls.Themes.ControlDefault
{
    public partial class RadRibbonBarBackstageView : ControlDefaultThemeComponent
    {
        public RadRibbonBarBackstageView()
        {
            InitializeComponent();
        }

        public RadRibbonBarBackstageView(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }
    }
}
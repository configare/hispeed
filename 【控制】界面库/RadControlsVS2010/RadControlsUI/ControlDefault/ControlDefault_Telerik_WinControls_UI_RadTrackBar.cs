using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Telerik.WinControls.Themes.ControlDefault
{
    public partial class TrackBar : ControlDefaultThemeComponent
    {
        public TrackBar()
        {
            InitializeComponent();
        }

        public TrackBar(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }
    }
}

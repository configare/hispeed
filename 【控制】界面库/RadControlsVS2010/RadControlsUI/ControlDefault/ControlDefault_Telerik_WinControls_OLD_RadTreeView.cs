using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Telerik.WinControls.Themes.ControlDefault
{
    public partial class OLDTreeView : ControlDefaultThemeComponent
    {
        public OLDTreeView()
        {
            InitializeComponent();
        }

        public OLDTreeView(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }
    }
}

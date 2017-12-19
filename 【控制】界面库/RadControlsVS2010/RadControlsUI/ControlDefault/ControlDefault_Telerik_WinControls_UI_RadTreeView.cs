using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Telerik.WinControls.Themes.ControlDefault
{
    public partial class TreeView : ControlDefaultThemeComponent
    {
        public TreeView()
        {
            InitializeComponent();
        }

        public TreeView(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }
    }
}

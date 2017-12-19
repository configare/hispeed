using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Telerik.WinControls;
using System.Windows.Forms;
using Telerik.WinControls.UI.Docking;
using System.Drawing;
using Telerik.WinControls.Paint;
using Telerik.WinControls.Primitives;

namespace Telerik.WinControls.UI.Docking
{
    [ToolboxItem(false)]
    public class DockingGuidesControl : RadControl
    {
        #region Fields

        private DockingGuidesElement dockGuidesItem;

        #endregion

        #region Properties

        public DockingGuidesElement GuidesElement
        {
            get
            {
                return this.dockGuidesItem;
            }
        }       

        #endregion

        #region Initialization

        protected override void CreateChildItems(RadElement parent)
        {
            this.dockGuidesItem = new DockingGuidesElement();
            parent.Children.Add(this.dockGuidesItem);
        }

        #endregion
    }
}

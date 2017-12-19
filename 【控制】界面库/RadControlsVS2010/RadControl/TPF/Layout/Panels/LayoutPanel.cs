using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Drawing;

namespace Telerik.WinControls.Layouts
{
    /// <summary>
    /// Represents a base class for all layout panels. Layout panels are RadElements. 
    /// They are the elements in the control tree responsible for the layout of primitives. 
    /// Layout panels determine the position and size of the primitives inside them. 
    /// Because panels are RadElements, panels can be nested thus providing an 
    /// arbitrary complex layout. 
    /// </summary>
    public abstract class LayoutPanel : RadElement
	{
        /// <summary>
        /// The property is always set to true. RadElements may override the 
        /// default layout. Layout panels are RadElements and always override the 
        /// default layout.
        /// </summary>
		public override bool OverridesDefaultLayout
		{
			get
			{
				return true;
			}
		}

        internal override bool VsbVisible
        {
            get
            {
                return false;
            }
        }

        //keep bit state declaration consistency
        internal const ulong LayoutPanelLastStateKey = RadElementLastStateKey;
	}
}
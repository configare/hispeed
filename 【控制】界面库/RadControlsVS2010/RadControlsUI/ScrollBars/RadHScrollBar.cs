using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Drawing;
using Telerik.WinControls.Design;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// 	<span id="ctl00_ContentPlaceHolder1_src1_resRC_ctl04_LabelAbstract">Implements
    /// the basic functionality of a horizontal scroll bar control.</span>
    /// </summary>
	[ComVisible(false)]
	#if RIBBONBAR
	[ToolboxItem(false)]
    internal class RadHScrollBar : RadScrollBar
	#else
    [ToolboxItem(true)]
	[Description("Enables its parent component to scroll content vertically")]
	public class RadHScrollBar : RadScrollBar
	#endif
    {
        /// <summary>
        /// Gets or sets the ScrollType. Possible values are defined in the ScrollType 
        /// enumeration: Vertical, and Horizontal. 
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override ScrollType ScrollType
        {
            get
            {
                return ScrollType.Horizontal;
            }
            set
            {
                throw new InvalidOperationException("Cannot change ScrollType of RadHScrollBar, use RadScrollBar instead.");
            }
        }

        protected override Size DefaultSize
        {
            get
            {
                return new Size(80, SystemInformation.HorizontalScrollBarHeight);
            }
        }
    }
}

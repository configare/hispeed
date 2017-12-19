using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Drawing;
using Telerik.WinControls.Themes.Design;
using Telerik.WinControls.Design;

namespace Telerik.WinControls.UI
{
    /// <summary>Represents a vertical scroll bar.</summary>
	[ComVisible(false)]
	[RadThemeDesignerData(typeof(RadVScrollBarDesignTimeData))]
	#if RIBBONBAR
	[ToolboxItem(false)]
    internal class RadVScrollBar : RadScrollBar
	#else
    [ToolboxItem(true)]
	[Description("Enables its parent component to scroll content vertically")]
	public class RadVScrollBar : RadScrollBar
	#endif
    {
        /// <summary>
        ///     Gets or sets the <see cref="ScrollType">ScrollType</see>. Possible values are
        ///     defined in the ScrollType enumeration: Horizontal and Vertical.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override ScrollType ScrollType
        {
            get
            {
                return ScrollType.Vertical;
            }
            set
            {
                throw new InvalidOperationException("Cannot change ScrollType of RadVScrollBar, use RadScrollBar instead.");
            }
        }

        protected override Size DefaultSize
        {
            get
            {
                return new Size(SystemInformation.VerticalScrollBarWidth, 80);
            }
        }
    }
}

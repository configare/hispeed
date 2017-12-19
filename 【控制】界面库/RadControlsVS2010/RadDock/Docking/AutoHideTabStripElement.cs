using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.UI;
using System.Windows.Forms;
using System.Drawing;

namespace Telerik.WinControls.UI.Docking
{
    /// <summary>
    /// A predefined <see cref="RadTabStripElement">RadPageViewStripElement</see> that holds the associated <see cref="TabStripItem">RadPageViewStripItem</see> for each auto-hidden DockWindow.
    /// </summary>
    public class AutoHideTabStripElement : RadPageViewStripElement
    {
        #region Dependency properties

        public static RadProperty AutoHidePopupOffsetProperty = RadProperty.Register(
            "AutoHidePopupOffset", typeof(int), typeof(AutoHideTabStripElement), new RadElementPropertyMetadata(0));

        #endregion

        #region Fields

        private TabPositions tabsPosition;

        #endregion

        #region Properties

        public TabPositions TabsPosition
        {
            get { return this.tabsPosition; }
            set { this.tabsPosition = value; }
        }

        /// <summary>
        /// Gets or sets the offset of the auto-hide popup.
        /// </summary>
        public int AutoHidePopupOffset
        {
            get { return (int)GetValue(AutoHidePopupOffsetProperty); }
            set { SetValue(AutoHidePopupOffsetProperty, value); }
        }

        #endregion

        #region Overrides

        protected internal override PageViewContentOrientation GetAutomaticItemOrientation(bool content)
        {
            switch (this.StripAlignment)
            {
                case StripViewAlignment.Left:
                    return PageViewContentOrientation.Vertical90;
                case StripViewAlignment.Right:
                    return PageViewContentOrientation.Vertical270;
                case StripViewAlignment.Bottom:
                    return content ? PageViewContentOrientation.Horizontal : PageViewContentOrientation.Horizontal180;
                default:
                    return PageViewContentOrientation.Horizontal;
            }
        }

        #endregion
    }
}

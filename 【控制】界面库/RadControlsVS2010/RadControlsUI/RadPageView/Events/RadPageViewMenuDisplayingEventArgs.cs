using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing;

namespace Telerik.WinControls.UI
{
    public class RadPageViewMenuDisplayingEventArgs : CancelEventArgs
    {
        #region Fields

        private Rectangle alignRect;
        private List<RadMenuItemBase> items;
        private HorizontalPopupAlignment hAlign;
        private VerticalPopupAlignment vAlign;

        #endregion

        #region Constructor

        public RadPageViewMenuDisplayingEventArgs(List<RadMenuItemBase> items, Rectangle alignRect)
            : this(items, alignRect, HorizontalPopupAlignment.LeftToLeft, VerticalPopupAlignment.TopToBottom)
        {
        }

        public RadPageViewMenuDisplayingEventArgs(List<RadMenuItemBase> items,
            Rectangle alignRect,
            HorizontalPopupAlignment hAlign, 
            VerticalPopupAlignment vAlign)
        {
            this.items = items;
            this.hAlign = hAlign;
            this.vAlign = vAlign;
            this.alignRect = alignRect;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the rectangle (in screen coordinates) which will be used to align the menu.
        /// </summary>
        public Rectangle AlignRect
        {
            get
            {
                return this.alignRect;
            }
            set
            {
                this.alignRect = value;
            }
        }

        /// <summary>
        /// Gets a list with all the items that will be displayed.
        /// </summary>
        public List<RadMenuItemBase> Items
        {
            get
            {
                return this.items;
            }
        }

        /// <summary>
        /// Gets or sets the horizontal alignment of the menu that will display the items.
        /// </summary>
        public HorizontalPopupAlignment HAlign
        {
            get
            {
                return this.hAlign;
            }
            set
            {
                this.hAlign = value;
            }
        }

        /// <summary>
        /// Gets or sets the vertical alignment of the menu that will display the items.
        /// </summary>
        public VerticalPopupAlignment VAlign
        {
            get
            {
                return this.vAlign;
            }
            set
            {
                this.vAlign = value;
            }
        }

        #endregion
    }
}

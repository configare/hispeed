using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Represents a page on the <c ref="RadRibbonBarBackstageView"/> on which you can add any type of controls.
    /// </summary>
    [Designer(DesignerConsts.BackstageViewPageDesignerString)]
    [ToolboxItem(false)]
    public class BackstageViewPage : Panel
    {
        #region Fields

        protected BackstageTabItem item;

        #endregion

        #region Constructors

        public BackstageViewPage()
        {
            this.SetStyle(ControlStyles.DoubleBuffer, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.SupportsTransparentBackColor | ControlStyles.OptimizedDoubleBuffer, true);
            

            this.BackColor = Color.Transparent;
            this.Visible = false;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the <c ref="BackstageTabItem"/> that this page is attached to.
        /// </summary>
        public BackstageTabItem Item
        {
            get
            {
                return item;
            }
            internal set
            {
                item = value;
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override DockStyle Dock
        {
            get
            {
                return base.Dock;
            }
            set
            {
                base.Dock = DockStyle.None;
            }
        }


        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new bool Visible
        {
            get
            {
                return base.Visible;
            }
            set
            {
                base.Visible = value;
            }
        }


        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override AnchorStyles Anchor
        {
            get
            {
                return base.Anchor;
            }
            set
            {
                base.Anchor = value;
            }
        }

        #endregion
    }
}

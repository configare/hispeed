using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;

namespace Telerik.WinControls.UI
{
    [ToolboxItem(false)]
    public class TabPanel : ContainerControl,IGeoDoFree
    {
        private BorderStyle borderStyle;
        private TabStripItem tabItem;
        private string toolTip;
        private Image image = null;

        public TabPanel()
        {
            this.toolTip = string.Empty;
            base.SetStyle(ControlStyles.UserPaint |
                          ControlStyles.AllPaintingInWmPaint |
                          ControlStyles.OptimizedDoubleBuffer |
                          ControlStyles.SupportsTransparentBackColor, true);

        }

        [DefaultValue(typeof(BorderStyle), "None")]
        public BorderStyle BorderStyle
        {
            get
            {
                return this.borderStyle;
            }
            set
            {
                if (this.borderStyle != value)
                {
                    if (!ClientUtils.IsEnumValid(value, (int)value, 0, 2))
                    {
                        throw new InvalidEnumArgumentException("value", (int)value, typeof(BorderStyle));
                    }

                    this.borderStyle = value;
                    base.UpdateStyles();
                }
            }
        }

        protected override Size DefaultSize
        {
            get
            {
                return new Size(200, 200);
            }
        }

        /// <summary>
        /// Gets or sets the Image associated with the panel.
        /// </summary>
        [Localizable(true)]
        [DefaultValue(null)]
        [Description("Gets or sets the Image associated with the panel.")]
        public Image Image
        {
            get
            {
                return this.image;
            }
            set
            {
                if (this.image != value)
                {
                    this.image = value;
                    this.OnImageChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Notifies for a change in the Image member of this panel.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnImageChanged(EventArgs e)
        {
            if (this.tabItem != null)
            {
                this.tabItem.Image = this.image;
            }
        }


        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public TabStripItem TabStripItem
        {
            get { return this.tabItem; }
            internal set
            {
                this.tabItem = value;
                if (this.tabItem != null)
                {
                    this.tabItem.ToolTipText = this.toolTip;
                }
            }
        }

        /// <summary>
        /// Gets or sets the tooltip to be displayed when the mouse hovers the tabitem of this panel.
        /// </summary>
        [Description("Gets or sets the tooltip to be displayed when the mouse hovers the tabitem of this panel.")]
        public string ToolTipText
        {
            get
            {
                return this.toolTip;
            }
            set
            {
                if(value == null)
                {
                    value = string.Empty;
                }
                if (value == this.toolTip)
                {
                    return;
                }

                this.toolTip = value;
                this.OnToolTipTextChanged(EventArgs.Empty);
            }
        }

        protected virtual void OnToolTipTextChanged(EventArgs e)
        {
            if (this.tabItem != null)
            {
                this.tabItem.ToolTipText = this.toolTip;
            }
        }

        /// <summary>
        /// Determines whether the ToolTip property should be serialized.
        /// </summary>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeToolTipText()
        {
            return !string.IsNullOrEmpty(this.toolTip);
        }

        [Browsable(false)]
        public TabStripPanel TabStrip
        {
            get { return this.Parent as TabStripPanel; }
        }

        public T GetTabStrip<T>() where T : TabStripPanel
        {
            return this.Parent as T;
        }

        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
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

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override bool AutoSize
        {
            get
            {
                return base.AutoSize;
            }
            set
            {
                base.AutoSize = value;
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new Size Size
        {
            get
            {
                return base.Size;
            }
            set
            {
                base.Size = value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override DockStyle Dock
        {
            get
            {
                return base.Dock;
            }
            set
            {
                base.Dock = value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new bool Enabled
        {
            get
            {
                return base.Enabled;
            }
            set
            {
                base.Enabled = value;
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new Point Location
        {
            get
            {
                return base.Location;
            }
            set
            {
                base.Location = value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
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

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DefaultValue(typeof(Size), "0, 0")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override Size MaximumSize
        {
            get
            {
                return base.MaximumSize;
            }
            set
            {
                base.MaximumSize = value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override Size MinimumSize
        {
            get
            {
                return base.MinimumSize;
            }
            set
            {
                base.MinimumSize = value;
            }
        }


        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new int TabIndex
        {
            get
            {
                return base.TabIndex;
            }
            set
            {
                base.TabIndex = value;
            }
        }


        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new bool TabStop
        {
            get
            {
                return base.TabStop;
            }
            set
            {
                base.TabStop = value;
            }
        }        

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new event EventHandler AutoSizeChanged
        {
            add
            {
                base.AutoSizeChanged += value;
            }
            remove
            {
                base.AutoSizeChanged -= value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new event EventHandler DockChanged
        {
            add
            {
                base.DockChanged += value;
            }
            remove
            {
                base.DockChanged -= value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public new event EventHandler EnabledChanged
        {
            add
            {
                base.EnabledChanged += value;
            }
            remove
            {
                base.EnabledChanged -= value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new event EventHandler LocationChanged
        {
            add
            {
                base.LocationChanged += value;
            }
            remove
            {
                base.LocationChanged -= value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public new event EventHandler TabIndexChanged
        {
            add
            {
                base.TabIndexChanged += value;
            }
            remove
            {
                base.TabIndexChanged -= value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public new event EventHandler TabStopChanged
        {
            add
            {
                base.TabStopChanged += value;
            }
            remove
            {
                base.TabStopChanged -= value;
            }
        }


        [Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        public new event EventHandler TextChanged
        {
            add
            {
                base.TextChanged += value;
            }
            remove
            {
                base.TextChanged -= value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public new event EventHandler VisibleChanged
        {
            add
            {
                base.VisibleChanged += value;
            }
            remove
            {
                base.VisibleChanged -= value;
            }
        }

        public void Free()
        {
            if (tabItem != null)
            {
                (tabItem as IGeoDoFree).Free();
                tabItem = null;
            }
        }
    }
}

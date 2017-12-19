using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using Telerik.WinControls.Themes.Design;
using System.Drawing.Design;
using Telerik.WinControls.Design;
using System.Collections;
using System.Drawing;

namespace Telerik.WinControls.UI
{

    /// <summary>
    ///     Represents a RadStatusStrip. The RadStatusStrip class is a simple wrapper for the
    ///     <see cref="RadStatusBarElement">RadStatusBarElement</see> class. The RadStatusStrip acts
    ///     to transfer events to and from its corresponding    
    /// </summary>
    [TelerikToolboxCategory(ToolboxGroupStrings.MenuAndToolbarsGroup)]
    [RadThemeDesignerData(typeof(RadStatusBarDesignTimeData))]
    [ToolboxItem(true)]
    [ProvideProperty("Spring", typeof(RadItem))]    
	[Description("A themable component which displays status information in an application")]
	[DefaultProperty("Items"), DefaultEvent("StatusBarClick"), Docking(DockingBehavior.Ask)]
    public class RadStatusStrip : RadControl, IExtenderProvider
    {
        //members
        private RadStatusBarElement statusBarElement;
        private bool isInRibbonForm = false;

        /// <summary>
        /// custom event handle for the click  event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public delegate void RadStatusBarClickEvenHandler(object sender, RadStatusBarClickEventArgs args);

        /// <summary>
        /// status bar click events
        /// </summary>                
        [Description("StatusBarClick")]
        public event RadStatusBarClickEvenHandler StatusBarClick;


        /// <summary>
        /// this event fired before Layout Style Changing 
        /// </summary>                
        public event ValueChangingEventHandler LayoutStyleChanging;


        /// <summary>
        /// this event fired after LayoutStyle Changed
        /// </summary>                
        public event EventHandler LayoutStyleChanged;


        static RadStatusStrip()
        {
        }

        /// <summary>
        /// create RadStatusStrip instance
        /// </summary>
        public RadStatusStrip()
        {            
            this.AutoSize = true;
            this.Dock = DockStyle.Bottom;            
        }

        protected override Size DefaultSize
        {
            get
            {
                // Non-empty default size is necessary to ensure non-empty size constrains in the following scenario:
                // Open a form in design-time, drag a status strip, add new items and build the application.
                // The form must stay open while while building.
                return new Size(300, 24);
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        internal bool IsInRibbonForm
        {
            get { return this.isInRibbonForm; }
            set { this.isInRibbonForm = value; }
        }

        /// <summary>
        /// implement default Dock style
        /// </summary>
        [DefaultValue(DockStyle.Bottom)]
        public override DockStyle Dock
        {
            get
            {
                return base.Dock;
            }
            set
            {
                if (base.Dock != value)
                {
                    switch (value)
                    {
                        case DockStyle.Bottom:
                        case DockStyle.Top:
                            this.StatusBarElement.Orientation = Orientation.Horizontal;
                            break;
                        case DockStyle.Left:                        
                        case DockStyle.Right:
                            this.StatusBarElement.Orientation = Orientation.Vertical;
                            break;
                    }

                    base.Dock = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the visibility of the grip used to reposition the control.
        /// </summary>
        [DefaultValue(true)]
        public bool  SizingGrip
        {
            get
            {
                return this.statusBarElement.GripStyle == ToolStripGripStyle.Visible;
            }
            set
            {
                this.statusBarElement.GripStyle = (value ? ToolStripGripStyle.Visible : ToolStripGripStyle.Hidden);
            }
        }

        
        /// <summary>
        ///Gets or sets the text associated with this control.
        /// </summary>
        public override string Text
        {
            get
            {
                return this.statusBarElement.Text;
            }
            set
            {
                this.statusBarElement.Text = value;
            }
        }

        /// <summary>
        /// Gets all the items that belong to a RadStatusStrip.
        /// </summary>
        [RadEditItemsAction]
        [RadNewItem("Type here", true)]
        [Browsable(true), Category(RadDesignCategory.DataCategory)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [RefreshProperties(RefreshProperties.All)]
        public  RadItemOwnerCollection Items
        {
            get
            {                
                return this.StatusBarElement.Items;
            }
        }

        /// <summary>
        /// Set the RadStatusStrip's layout style 
        /// </summary>
        [Description("ToolStripLayoutStyle"),
        AmbientValue(0),
        Category("Layout")]
        [RefreshProperties(RefreshProperties.All)]
        public RadStatusBarLayoutStyle LayoutStyle
        {
            get
            {

                return this.StatusBarElement.LayoutStyle;
            }

            set
            {
                this.StatusBarElement.LayoutStyle = value;
            }
        }

        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            base.SetBoundsCore(x, y, width, height, specified);
        }

        /// <summary>
        /// Gets the instance of RadStatusBarElement wrapped by this control. RadStatusBarElement
        /// is the main element in the hierarchy tree and encapsulates the actual functionality of RadStatusStrip.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RadStatusBarElement StatusBarElement
        {
            get
            {
                return this.statusBarElement;
            }            
        }

        void statusBarElement_LayoutStyleChanging(object sender, ValueChangingEventArgs e)
        {
            this.OnLayoutStyleChanging(sender, e);
        }

        private void OnLayoutStyleChanging(object sender, ValueChangingEventArgs e)
        {
            if (this.LayoutStyleChanging != null)
            {
                this.LayoutStyleChanging(sender, e);
            }
        }

        void statusBarElement_LayoutStyleChanged(object sender, EventArgs e)
        {
            this.OnLayoutStyleChanged(sender, e);
        }

        private void OnLayoutStyleChanged(object sender, EventArgs e)
        {
            if (this.LayoutStyleChanged != null)
            {
                this.LayoutStyleChanged(sender, e);
            }
        }

        private void OnStatusBarClick(object sender, RadStatusBarClickEventArgs args)
        {
            if (this.StatusBarClick != null)
            {
                this.StatusBarClick(sender, args);
            }
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case NativeMethods.WM_NCHITTEST:
                    if (WmNCHitTest(ref m))
                        return;
                    break;
            }
            base.WndProc(ref m);
		}

#pragma warning disable 0618
		protected virtual bool WmNCHitTest(ref Message msg)
        {
            bool isMaximized = false;
            Form f = this.FindForm();
            if (f != null)
                isMaximized = f.WindowState == FormWindowState.Maximized;
            if (this.IsInRibbonForm && !isMaximized)
            {
                Point mousePos = new Point((int)msg.LParam);
                mousePos = this.PointToClient(mousePos);
                if ((this.Height - mousePos.Y) <= 4)
                {
                    msg.Result = new IntPtr(NativeMethods.HTTRANSPARENT);
                    return true;
                }
            }
            return false;
		}
#pragma warning restore 0618

		/// <summary>
        /// fire the StatusBarClick event
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseClick(MouseEventArgs e)
        {
            RadElement clickedElement = this.ElementTree.GetElementAtPoint(e.Location);

            RadStatusBarClickEventArgs args = new RadStatusBarClickEventArgs(clickedElement, e);

            this.OnStatusBarClick(this, args);

            base.OnMouseClick(e);
        }

        #region IExtenderProvider Members

        

        public bool CanExtend(object extendee)
        {
            RadItem item = extendee as RadItem;
            if( item != null && extendee != this  && this.Items.Contains(item))
                return true;
            return false;
        }

        [RefreshProperties(RefreshProperties.All)]        
        public void SetSpring(RadItem control, bool value)
        {            
            control.SetValue(StatusBarBoxLayout.SpringProperty, value);
        }

        public bool GetSpring(RadItem control)
        {
            bool result = (bool)control.GetValue(StatusBarBoxLayout.SpringProperty);
            
            return result;
        }

        
        #endregion

        /// <summary>
        /// create child items
        /// </summary>
        /// <param name="parent"></param>
        protected override void CreateChildItems(RadElement parent)
        {
            this.statusBarElement = new RadStatusBarElement();
            this.RootElement.Children.Add(statusBarElement);
            this.statusBarElement.Padding = new Padding(2);
            this.statusBarElement.MinSize = new Size(0, 19);
            this.statusBarElement.LayoutStyleChanged += new EventHandler(statusBarElement_LayoutStyleChanged);
            this.statusBarElement.LayoutStyleChanging += new ValueChangingEventHandler(statusBarElement_LayoutStyleChanging);
            this.statusBarElement.GripStyle = ToolStripGripStyle.Visible;
            base.CreateChildItems(parent);
        }

        protected override void InitializeRootElement(RootRadElement rootElement)
        {
            base.InitializeRootElement(rootElement);
            rootElement.StretchHorizontally = true;
            rootElement.StretchVertically = false;
        }

        protected override AccessibleObject CreateAccessibilityInstance()
        {
            return new RadStatusStripAccessibleObject(this);
        }
    }    
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Represents a form that holds the items of a <c ref="CommandBarStripElement"/> that has been undocked and is floating.
    /// </summary>
    public partial class CommandBarFloatingForm : RadForm
    {
        #region Custom form behavior

        class CommandBarFloatingFormBehavior : RadFormBehavior
        {
            public override Padding ClientMargin
            {
                get { return new Padding(2, GetDynamicCaptionHeight(), 2, 2); }
            }

            public CommandBarFloatingFormBehavior(IComponentTreeHandler treeHandler)
                : base(treeHandler, true)
            {

            }
        }

        #endregion 

        #region Fields

        protected RadDropDownMenu dropDownMenuElement;
        private bool isWindowMoving;
        private Point dragOffset = Point.Empty;
        protected Control parentControl;
        protected RadCommandBarOverflowPanelHostContol hostControl;
        protected CommandBarStripElement stripElement;
        protected CommandBarStripInfoHolder stripInfoHolder;
        protected RadImageButtonElement customizeMenuButtonElement;

        #endregion

        #region Ctors

        public CommandBarFloatingForm()
        {
            InitializeComponent();

            this.HorizontalScroll.Visible = false;
            this.VerticalScroll.Visible = false;
            this.FormBehavior = new CommandBarFloatingFormBehavior(this);
            this.CreateHostControl();

            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint /*| ControlStyles.UserPaint | ControlStyles.Opaque*/, true);
            this.StartPosition = FormStartPosition.Manual;
            
            customizeMenuButtonElement = new RadImageButtonElement();
            customizeMenuButtonElement.ThemeRole = "CommandBarFloatingOverflowButton";
            customizeMenuButtonElement.Class = "CommandBarFloatingOverflowButton";
            customizeMenuButtonElement.Click += new EventHandler(customizeMenuButtonElement_Click);
            customizeMenuButtonElement.Visibility = ElementVisibility.Collapsed;

            this.FormElement.TitleBar.Children[2].Children[0].Children.Insert(2, customizeMenuButtonElement);
            this.FormElement.TitleBar.MouseDown += new MouseEventHandler(TitleBar_MouseDown);

            this.FormElement.VerticalScrollbar.Visibility = ElementVisibility.Collapsed;
            this.FormElement.HorizontalScrollbar.Visibility = ElementVisibility.Collapsed;
        }
        
        protected virtual void CreateHostControl()
        {
            this.hostControl = new RadCommandBarOverflowPanelHostContol();
            this.hostControl.Dock = DockStyle.Fill;
            this.hostControl.Element.Padding = new Padding(2, 2, 2, -2);
            this.hostControl.Element.MinSize = new Size(25, 25);
            this.hostControl.SizeChanged += new EventHandler(hostControl_SizeChanged);
            this.Controls.Add(this.hostControl);
            this.PerformLayout();
        } 
        #endregion

        #region Properties

        /// <summary>
        /// Gets the control that hosts the floating items.
        /// </summary>
        public RadCommandBarOverflowPanelHostContol ItemsHostControl
        {
            get
            {
                return hostControl;
            }
        }

        /// <summary>
        /// Gets or sets the parent of the <c ref="RadCommandBar"/> control to which the floating strip belongs.
        /// </summary>
        public Control ParentControl
        {
            get
            {
                return parentControl;
            }
            set
            {
                parentControl = value;
            }
        }

        /// <summary>
        /// Gets the <c ref="CommandBarStripInfoHolder"/> which contains information about the floating strip.
        /// </summary>
        public CommandBarStripInfoHolder StripInfoHolder
        {
            get
            {
                return this.stripInfoHolder;
            }
        }

        /// <summary>
        /// Gets or sets the <c ref="CommandBarStripElement"/> which the floating form is hosting.
        /// </summary>
        public CommandBarStripElement StripElement
        {
            get
            {
                return this.stripElement;
            }
            set
            {
                if (value != null)
                {
                    CommandBarRowElement row = (value.Parent as CommandBarRowElement);
                    if (row != null && row.Owner != null)
                    {
                        this.ThemeName = (row.Owner.ElementTree.Control as RadCommandBar).ThemeName;
                        this.hostControl.ThemeName = this.ThemeName;
                        this.stripInfoHolder = row.Owner.StripInfoHolder;
                    }
                }
                else
                {
                    this.stripInfoHolder = null;
                }

                SetStripElementCore(value);
            }
        }

        private void SetStripElementCore(CommandBarStripElement value)
        {
            if (this.stripElement != null)
            {
                this.stripElement.RadPropertyChanged -= new RadPropertyChangedEventHandler(stripElement_RadPropertyChanged);
            }

            this.stripElement = value;

            if (this.stripElement != null)
            {
                this.stripElement.RadPropertyChanged += new RadPropertyChangedEventHandler(stripElement_RadPropertyChanged);
                this.stripElement.Items.Owner = this.hostControl.Element.Layout;

                while (this.stripElement.OverflowButton.ItemsLayout.Children.Count > 0)
                {
                    RadElement element = this.stripElement.OverflowButton.ItemsLayout.Children[0];
                    this.stripElement.OverflowButton.ItemsLayout.Children.Remove(element);
                    this.hostControl.Element.Layout.Children.Add(element);
                }
            }
        }

        #endregion

        #region Event handlers

        private void hostControl_SizeChanged(object sender, EventArgs e)
        {
            this.Size = new Size(this.Width, this.hostControl.Element.Size.Height + this.FormBehavior.ClientMargin.Top + this.FormBehavior.ClientMargin.Bottom);
        }

        private void customizeMenuButtonElement_Click(object sender, EventArgs e)
        {
            this.StripElement.OverflowButton.RightToLeft = (this.RightToLeft == System.Windows.Forms.RightToLeft.Yes);
            this.StripElement.OverflowButton.PopulateDropDownMenu();
            Point offset = Point.Empty;
            if (this.RightToLeft == System.Windows.Forms.RightToLeft.Yes)
            {
                offset = new Point(this.customizeMenuButtonElement.Size.Width - this.stripElement.OverflowButton.DropDownMenu.PreferredSize.Width, 0);
            }
            this.StripElement.OverflowButton.DropDownMenu.Show(this.customizeMenuButtonElement as RadItem, offset, RadDirection.Left);
        }

        private void stripElement_RadPropertyChanged(object sender, RadPropertyChangedEventArgs e)
        {
            if (e.Property == CommandBarStripElement.VisibleInCommandBarProperty)
            {
                this.Visible = this.stripElement.VisibleInCommandBar;
            }
        }

        private void TitleBar_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                this.InitializeMove(e.Location);
            } 
        }

        #endregion

        #region Floating and docking

        internal void InitializeMove(Point offset)
        {
            this.isWindowMoving = true;
            this.Capture = true;
            dragOffset = new Point(-offset.X, -offset.Y);
        }

        internal void EndMove()
        {
            this.isWindowMoving = false;
            this.Capture = false;
        }
          
        /// <summary>
        /// Tries to dock the floating strip in a specified <c ref="RadCommandBar"/>.
        /// </summary>
        /// <param name="commandBar">The <c ref="RadCommandBar"/> control into which the strip should be docked.</param>
        public void TryDocking(RadCommandBar commandBar)
        {
            if (commandBar == null || commandBar.CommandBarElement.CallOnFloatingStripDocking(this.stripElement))
            {
                return;
            }

            this.Capture = false;
            this.isWindowMoving = false;
            this.stripElement.EnableDragging = true;
            this.stripElement.Capture = false;
            this.stripElement.ForceEndDrag();
            this.stripElement.Items.Owner = this.stripElement.ItemsLayout;

            bool enableFloatingOldValue = this.stripElement.EnableFloating;
            this.stripElement.EnableFloating = false;

            while (this.hostControl.Element.Layout.Children.Count > 0)
            {
                RadElement item = this.hostControl.Element.Layout.Children[0];
                if (item is RadCommandBarBaseItem || !this.stripElement.Items.Contains(item as RadCommandBarBaseItem))
                {
                    this.hostControl.Element.Layout.Children.Remove(item);
                    this.stripElement.Items.Add(item as RadCommandBarBaseItem);
                }
            }

            this.stripElement.FloatingForm = null;
            this.stripInfoHolder.RemoveStripInfo(this.stripElement);

            if (commandBar.Rows.Count == 0)
            {
                commandBar.Rows.Add(new CommandBarRowElement());
            }

            commandBar.Rows[0].Strips.Add(this.stripElement);
            this.stripElement.OverflowButton.HostControlThemeName = commandBar.ThemeName;

            this.stripElement.Capture = false;
            this.stripElement.ForceEndDrag();
            this.stripElement.ElementTree.Control.Capture = false;
            this.stripElement.CallDoMouseUp(new MouseEventArgs(System.Windows.Forms.MouseButtons.Left, 1, 0, 0, 0));

            Point location = this.parentControl.PointToClient(this.Location);
            if (stripElement.RightToLeft)
            {
                location.X = this.parentControl.Size.Width - location.X - this.Size.Width;
            }

            this.stripElement.DesiredLocation = location;

            this.stripElement.EnableFloating = enableFloatingOldValue;
            commandBar.CommandBarElement.CallOnFloatingStripDocked(this.stripElement);
            this.StripElement = null;
            this.Close();
        }

        /// <summary>
        /// Tries to dock the floating strip on a specified point of screen. The docking will be completed only if 
        /// the control under that point is <c ref="RadCommandBar"/>.
        /// </summary>
        /// <param name="screenLocation">The location in screen coordinates where the strip should try to dock.</param>
        public void TryDocking(Point screenLocation)
        {
            if (this.parentControl == null)
            {
                return;
            }

            RadCommandBar commandBar = (this.ParentControl.GetChildAtPoint(this.parentControl.PointToClient(screenLocation)) as RadCommandBar);
            this.TryDocking(commandBar);
        }

        #endregion

        #region Overrides

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == NativeMethods.WM_SIZING)
            {
                NativeMethods.RECT rc = (NativeMethods.RECT)Marshal.PtrToStructure(m.LParam, typeof(NativeMethods.RECT));
                this.hostControl.Element.InvalidateMeasure(true);
                this.hostControl.Element.Measure(this.ClientSize);
                int maxWidth = this.hostControl.Element.GetChildMaxWidth();
                rc.bottom = rc.top + this.hostControl.Element.Size.Height + this.FormBehavior.ClientMargin.Top + this.FormBehavior.ClientMargin.Bottom;

                if (rc.right - rc.left < maxWidth + this.hostControl.Element.Padding.Left + this.hostControl.Element.Padding.Right)
                {
                    rc.right = rc.left + maxWidth + this.hostControl.Element.Padding.Left + this.hostControl.Element.Padding.Right
                        + this.FormBehavior.ClientMargin.Left + this.FormBehavior.ClientMargin.Right;
                }
                //   rc.Right = rc.Left + this.hostControl.Element.Size.Width + 2; 
                Marshal.StructureToPtr(rc, m.LParam, true);
            }
            base.WndProc(ref m);
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams baseParams = base.CreateParams;

                baseParams.ExStyle |= (
                  NativeMethods.WS_EX_TOOLWINDOW |
                  0x08000000); //WS_EX_NOACTIVATE

                return baseParams;
            }
        }


        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            if (this.StripElement != null)
            {
                e.Cancel = true;
                this.StripElement.VisibleInCommandBar = false;
                this.Visible = false;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);


            if (e.Button != MouseButtons.Left)
            {
                return;
            }

            if (this.isWindowMoving)
            {
                Point location = Control.MousePosition;
                location.Offset(dragOffset);
                this.Location = location;
                TryDocking(Control.MousePosition);
            }

            this.Invalidate(true);
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Primitives;
using Telerik.WinControls.Layouts;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Telerik.WinControls.UI
{
    public class RadFormMdiControlStripItem : RadItem
    {
        private RadButtonElement closeButton;
        private RadButtonElement minimizeButton;
        private RadButtonElement maximizeButton;
        private ImagePrimitive mdiFormIcon;
        private DockLayoutPanel layout;
        private StackLayoutPanel systemButtons;
        private FillPrimitive fill;
        private Form activeMDIForm;

        internal RadFormMdiControlStripItem() 
        {
        }

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.UseNewLayoutSystem = true;
        }

        public Form ActiveMDIChild
        {
            get
            {
                return this.activeMDIForm;
            }
            set
            {
                this.activeMDIForm = value;

            }
        }

        /// <summary>
        /// Gets an instance of the <see cref="FillPrimitive"/>
        /// class that represents the fill of the MDI strip.
        /// </summary>
        public FillPrimitive Fill
        {
            get
            {
                return this.fill;
            }
        }
        /// <summary>
        /// Gets the Minimize button
        /// </summary>
        public RadButtonElement MinimizeButton
        {
            get
            {
                return minimizeButton;
            }
        }

        /// <summary>
        /// Gets the Maximize button
        /// </summary>
        public RadButtonElement MaximizeButton
        {
            get
            {
                return maximizeButton;
            }
        }

        /// <summary>
        /// Gets the Close button
        /// </summary>
        public RadButtonElement CloseButton
        {
            get
            {
                return closeButton;
            }
        }

        /// <summary>
        /// Gets the ImagePrimitive representing the Icon
        /// of the currently maximized MDI child.
        /// </summary>
        public ImagePrimitive MaximizedMdiIcon
        {
            get
            {
                return this.mdiFormIcon;
            }
        }

        public StackLayoutPanel SystemButtonsLayout
        {
            get
            {
                return this.systemButtons;
            }
        }

        protected override void CreateChildElements()
        {
            base.CreateChildElements();

            this.AutoSizeMode = RadAutoSizeMode.WrapAroundChildren;

            this.fill = new FillPrimitive();
            this.fill.Class = "TitleFill";
            this.Children.Add(this.fill);

            this.layout = new DockLayoutPanel();
            this.layout.StretchVertically = true;
            this.layout.StretchHorizontally = true;
            this.layout.LastChildFill = false;
            this.Children.Add(this.layout);

            this.systemButtons = new StackLayoutPanel();
            this.systemButtons.ZIndex = 10;
            this.systemButtons.Alignment = ContentAlignment.MiddleRight;

            this.minimizeButton = new RadImageButtonElement();
            this.minimizeButton.StretchHorizontally = false;
            this.minimizeButton.StretchVertically = false;
            this.minimizeButton.Class = "MinimizeButton";
            this.minimizeButton.ButtonFillElement.Name = "MinimizeButtonFill";
            this.minimizeButton.CanFocus = false;
            this.minimizeButton.ThemeRole = "MDIStripMinimizeButton";
            this.systemButtons.Children.Add(minimizeButton);

            this.maximizeButton = new RadImageButtonElement();
            this.maximizeButton.StretchHorizontally = false;
            this.maximizeButton.StretchVertically = false;
            this.maximizeButton.Class = "MaximizeButton";
            this.maximizeButton.ButtonFillElement.Name = "MaximizeButtonFill";
            this.maximizeButton.CanFocus = false;
            this.maximizeButton.ThemeRole = "MDIStripMaximizeButton";
            this.systemButtons.Children.Add(maximizeButton);

            this.closeButton = new RadImageButtonElement();
            this.closeButton.StretchHorizontally = false;
            this.closeButton.StretchVertically = false;
            this.closeButton.Class = "CloseButton";
            this.closeButton.ButtonFillElement.Name = "CloseButtonFill";
            this.closeButton.CanFocus = false;
            this.closeButton.ThemeRole = "MDIStripCloseButton";
            this.systemButtons.Children.Add(closeButton);

            this.mdiFormIcon = new ImagePrimitive();
            this.mdiFormIcon.Class = "MdiStripIcon";
            this.mdiFormIcon.StretchHorizontally = false;
            this.mdiFormIcon.StretchVertically = false;
            this.mdiFormIcon.CanFocus = false;
            this.mdiFormIcon.Margin = new Padding(1, 1, 0, 0);
            this.mdiFormIcon.MaxSize = new Size(16, 16);
            this.mdiFormIcon.ImageLayout = ImageLayout.Stretch;
            this.layout.Children.Add(this.mdiFormIcon);


            this.layout.Children.Add(this.systemButtons);
            
            DockLayoutPanel.SetDock(this.systemButtons, Dock.Right);
            DockLayoutPanel.SetDock(this.mdiFormIcon, Dock.Left);
        }

        protected override void OnPropertyChanged(RadPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == RadItem.VisibilityProperty)
            {
                if (this.activeMDIForm != null)
                {
                    if (this.activeMDIForm.Icon != null && this.activeMDIForm.ShowIcon)
                    {
                        if (this.mdiFormIcon.Image != null)
                        {
                            this.mdiFormIcon.Image.Dispose();
                            this.mdiFormIcon.Image = null;
                        }
                        this.mdiFormIcon.Image = this.activeMDIForm.Icon.ToBitmap();
                    }
                    else
                    {
                        this.mdiFormIcon.Image = null;
                    }
                }
            }
        }

    }
}

using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Layouts;
using Telerik.WinControls.Primitives;
using Telerik.WinControls.UI;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Drawing;
using System.ComponentModel;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// This is the class that represents the element hierarchy which
    /// is painted in the non-client area of a RadForm.
    /// </summary>
    public class RadFormElement : RadItem
    {
        #region RadProperties

        public static RadProperty IsFormActiveProperty = RadProperty.Register(
            "IsFormActive",
            typeof(bool),
            typeof(RadFormElement),
            new RadElementPropertyMetadata(
                false,
                ElementPropertyOptions.AffectsDisplay | ElementPropertyOptions.CanInheritValue)
            );

        public static RadProperty FormWindowStateProperty = RadProperty.Register(
            "FormWindowState",
            typeof(FormWindowState),
            typeof(RadFormElement),
            new RadElementPropertyMetadata(
                FormWindowState.Normal, 
                ElementPropertyOptions.AffectsDisplay | ElementPropertyOptions.CanInheritValue | ElementPropertyOptions.AffectsLayout)
        );

        #endregion

        #region Fields

        #region Settings

        #endregion

        #region Children

        private DockLayoutPanel dockLayoutPanel;
        private DockLayoutPanel scrollBarsDockLayout;
        private FormBorderPrimitive borderPrimitive;
        private RadFormTitleBarElement radTitleBarElement;
        private RadFormMdiControlStripItem mdiControlStripItem;
        private FormImageBorderPrimitive imageBorder;
        private RadScrollBarElement horizontalScrollbar;
        private RadScrollBarElement verticalScrollbar;
        private DockLayoutPanel horizontalScrollBarAndSquareHolder;
        private DockLayoutPanel horizontalScrollbarDockPanel;
        private RadItem formSizingGrip;
        private FillPrimitive formSizingGripFill;
        private BorderPrimitive formSizingGripBorder;
        private StackLayoutPanel imageBorderAndScrollbarsDockLayout;

        #endregion

        #endregion

        #region Constructor

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.UseNewLayoutSystem = true;
        }

        static RadFormElement()
        {
            //new Themes.ControlDefault.Form().DeserializeTheme();
            new Themes.ControlDefault.ControlDefault_Telerik_WinControls_UI_RadForm().DeserializeTheme();
            new Themes.ControlDefault.ControlDefault_Telerik_WinControls_UI_RadTitleBar().DeserializeTheme();

            ItemStateManagerFactoryRegistry.AddStateManagerFactory(new RadFormElementStateManager(), typeof(RadFormElement));
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a boolean value to determine whether the form
        /// should appear as active or inactive.
        /// Usign this property, you can override the default theme styles
        /// which define different appearance of the form when in active/inactive state.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsFormActive
        {
            get
            {
                return (bool)this.GetValue(RadFormElement.IsFormActiveProperty);
            }
            set
            {
                this.SetValue(RadFormElement.IsFormActiveProperty, value);
            }
        }

        /// <summary>
        /// Gets the square element that appears at the end of the horizontal 
        /// scrollbar.
        /// </summary>
        public RadItem ScrollBarsFormSizingGrip
        {
            get
            {
                return this.formSizingGrip;
            }
        }

        /// <summary>
        /// Gets the composed width of the border
        /// built on the width of the line and image borders.
        /// </summary>
        public Padding BorderWidth
        {
            get
            {
                return Padding.Add(
                    this.imageBorder.BorderWidth,
                    new Padding((int)this.borderPrimitive.Width));
            }
        }

        /// <summary>
        /// Gets the MdiControlStrip item that should appear
        /// below the title bar when a MDI child is maximized.
        /// </summary>
        public RadFormMdiControlStripItem MdiControlStrip
        {
            get
            {
                return this.mdiControlStripItem;
            }
        }

        /// <summary>
        /// Gets the BorderPrimitive of the RadFormElement.
        /// </summary>
        public FormBorderPrimitive Border
        {
            get
            {
                return this.borderPrimitive;
            }
        }

        /// <summary>
        /// Gets the FormImageBorderPrimitive of the RadFormElement.
        /// </summary>
        public FormImageBorderPrimitive ImageBorder
        {
            get
            {
                return this.imageBorder;
            }
        }

        /// <summary>
        /// Gets the RadFormTitleBarElement of the RadFormElement.
        /// </summary>
        public RadFormTitleBarElement TitleBar
        {
            get
            {
                return this.radTitleBarElement;
            }
        }

        /// <summary>
        /// Gets the horizontal scrollbar element of the form element.
        /// </summary>
        public RadScrollBarElement HorizontalScrollbar
        {
            get
            {
                return this.horizontalScrollbar;
            }
        }

        /// <summary>
        /// Gets the vertical scrollbar element of the form element.
        /// </summary>
        public RadScrollBarElement VerticalScrollbar
        {
            get
            {
                return this.verticalScrollbar;
            }
        }

        #endregion

        #region Methods

        internal void SetIsFormActiveInternal(bool isActive)
        {
            this.SetDefaultValueOverride(RadFormElement.IsFormActiveProperty, isActive);
        }

        internal void SetFormElementIconInternal(Image icon)
        {
            this.TitleBar.IconPrimitive.SetDefaultValueOverride(ImagePrimitive.ImageProperty, icon);
            this.TitleBar.UpdateLayout();
        }

        #region Overriden


        protected override void CreateChildElements()
        {
            base.CreateChildElements();
            this.imageBorderAndScrollbarsDockLayout = new StackLayoutPanel();
            this.dockLayoutPanel = new DockLayoutPanel();
            this.scrollBarsDockLayout = new DockLayoutPanel();
            this.imageBorder = new FormImageBorderPrimitive();
            this.imageBorder.Class = "RadFormImageBorder";
            this.borderPrimitive = new FormBorderPrimitive();
            this.borderPrimitive.Class = "RadFormBorder";
            this.borderPrimitive.StretchVertically = true;
            this.borderPrimitive.StretchHorizontally = true;

            this.radTitleBarElement = new RadFormTitleBarElement();
            this.radTitleBarElement.Class = "TitleBar";
            this.radTitleBarElement.AutoSizeMode = RadAutoSizeMode.FitToAvailableSize;

            this.mdiControlStripItem = new RadFormMdiControlStripItem();
            this.mdiControlStripItem.Fill.SetDefaultValueOverride(FillPrimitive.GradientStyleProperty, GradientStyles.Solid);
            this.mdiControlStripItem.Visibility = ElementVisibility.Collapsed;
            this.mdiControlStripItem.MinSize = new Size(0, 18);
            this.mdiControlStripItem.MaxSize = new Size(0, 18);
            this.mdiControlStripItem.Class = "MdiControlStrip";

            this.dockLayoutPanel.Children.Add(this.radTitleBarElement);
            this.dockLayoutPanel.Children.Add(this.mdiControlStripItem);
            this.dockLayoutPanel.Children.Add(this.imageBorderAndScrollbarsDockLayout);

            this.Children.Add(this.imageBorder);
            this.Children.Add(this.scrollBarsDockLayout);

            this.horizontalScrollbar = new RadScrollBarElement();
            this.horizontalScrollbar.ScrollType = ScrollType.Horizontal;
            this.horizontalScrollbar.Class = "RadFormHScroll";
            this.horizontalScrollbar.MinSize = new Size(0, SystemInformation.HorizontalScrollBarHeight);

            this.verticalScrollbar = new RadScrollBarElement();
            this.verticalScrollbar.ScrollType = ScrollType.Vertical;
            this.verticalScrollbar.Class = "RadFormVScroll";
            this.verticalScrollbar.MinSize = new Size(SystemInformation.VerticalScrollBarWidth, 0);

            this.horizontalScrollBarAndSquareHolder = new DockLayoutPanel();

            this.formSizingGrip = new RadItem();
            this.formSizingGripFill = new FillPrimitive();
            this.formSizingGripFill.Class = "RadFormScrollSquareFill";
            this.formSizingGripBorder = new BorderPrimitive();
            this.formSizingGripBorder.Class = "RadFormScrollSquareBorder";
            this.formSizingGrip.Class = "RadFormElementScrollbarSquare";

            this.formSizingGrip.Children.Add(this.formSizingGripFill);
            this.formSizingGrip.Children.Add(this.formSizingGripBorder);
            
            this.formSizingGrip.MinSize = new Size(
                SystemInformation.VerticalScrollBarWidth,
                SystemInformation.HorizontalScrollBarHeight);

            this.horizontalScrollbarDockPanel = new DockLayoutPanel();
            this.horizontalScrollbarDockPanel.Children.Add(this.horizontalScrollbar);
            DockLayoutPanel.SetDock(this.horizontalScrollbar, Dock.Bottom);

            this.horizontalScrollBarAndSquareHolder.Children.Add(this.formSizingGrip);
            this.horizontalScrollBarAndSquareHolder.Children.Add(this.horizontalScrollbarDockPanel);
            this.horizontalScrollBarAndSquareHolder.LastChildFill = true;

            DockLayoutPanel.SetDock(this.horizontalScrollbarDockPanel, Dock.Right);
            DockLayoutPanel.SetDock(this.formSizingGrip, Dock.Right);

            DockLayoutPanel.SetDock(this.radTitleBarElement, Dock.Top);
            DockLayoutPanel.SetDock(this.mdiControlStripItem, Dock.Top);

            this.scrollBarsDockLayout.Children.Add(this.horizontalScrollBarAndSquareHolder);
            this.scrollBarsDockLayout.Children.Add(this.verticalScrollbar);
            this.scrollBarsDockLayout.LastChildFill = false;

            DockLayoutPanel.SetDock(this.horizontalScrollBarAndSquareHolder, Dock.Bottom);
            DockLayoutPanel.SetDock(this.verticalScrollbar, Dock.Right);

            this.Children.Add(this.dockLayoutPanel);
            this.Children.Add(this.borderPrimitive);
        }

        protected override SizeF MeasureOverride(SizeF availableSize)
        {
            availableSize.Width -= this.BorderWidth.Horizontal;
            availableSize.Width -= this.BorderThickness.Horizontal;

            return base.MeasureOverride(availableSize);
        }

        protected override SizeF ArrangeOverride(SizeF finalSize)
        {
            SizeF result = base.ArrangeOverride(finalSize);

            float imageBorderX = this.borderPrimitive.BorderThickness.Left;
            float imageBorderY = this.radTitleBarElement.ControlBoundingRectangle.Height + this.borderPrimitive.BorderThickness.Top;
            float imageBorderWidth = finalSize.Width - this.borderPrimitive.BorderThickness.Horizontal;
            float imageBorderHeight = finalSize.Height - (this.radTitleBarElement.DesiredSize.Height + this.borderPrimitive.BorderThickness.Vertical);
            RectangleF imageBorderRect = new RectangleF(imageBorderX, imageBorderY, imageBorderWidth, imageBorderHeight);
            this.imageBorder.Arrange(imageBorderRect);

            float scrollbarsX = this.imageBorder.BorderThickness.Left + this.borderPrimitive.BorderThickness.Left;
            float scrollbarsY = imageBorderY;
            float scrollbarsWidth = finalSize.Width - (this.imageBorder.BorderThickness.Horizontal + this.borderPrimitive.BorderThickness.Horizontal);
            float scrollbarsHeight = finalSize.Height - (this.borderPrimitive.BorderThickness.Vertical + this.imageBorder.BorderThickness.Bottom + this.radTitleBarElement.DesiredSize.Height);
            RectangleF scrollbarsLayoutRect = new RectangleF(scrollbarsX, scrollbarsY, scrollbarsWidth, scrollbarsHeight);

            this.scrollBarsDockLayout.Arrange(scrollbarsLayoutRect);

            if (this.borderPrimitive.Visibility == ElementVisibility.Collapsed &&
               this.imageBorder.Visibility == ElementVisibility.Collapsed)
            {
                this.borderPrimitive.Arrange(Rectangle.Empty);
                this.imageBorder.Arrange(Rectangle.Empty);

                scrollbarsLayoutRect = new RectangleF(0, this.radTitleBarElement.DesiredSize.Height, finalSize.Width, finalSize.Height - this.radTitleBarElement.DesiredSize.Height);
                this.scrollBarsDockLayout.Arrange(scrollbarsLayoutRect);
            }

            return result;
        }

        #endregion


        #endregion
    }
}


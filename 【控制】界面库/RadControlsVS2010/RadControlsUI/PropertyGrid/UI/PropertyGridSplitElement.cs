using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;

namespace Telerik.WinControls.UI
{
    public class PropertyGridSplitElement : LightVisualElement
    {
        #region Fields

        private bool isResizing;
        private float cachedHelpElementHeight;
        private bool isHelpHidden;

        private PropertyGridTableElement propertyTableElement;
        private PropertyGridSizeGripElement sizeGripElement;
        private PropertyGridHelpElement helpElement;
        
        #endregion

        #region Initialization & Constructor

        public PropertyGridSplitElement()
        {
            this.propertyTableElement.SelectedGridItemChanged += new RadPropertyGridEventHandler(propertyGridTableElement_SelectedItemChanged);
        }

        protected override void DisposeManagedResources()
        {
            this.propertyTableElement.SelectedGridItemChanged -= propertyGridTableElement_SelectedItemChanged;

            base.DisposeManagedResources();
        }

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.isHelpHidden = false;
            this.isResizing = false;
            this.cachedHelpElementHeight = -1;
        }

        protected override void CreateChildElements()
        {
            base.CreateChildElements();

            this.propertyTableElement = new PropertyGridTableElement();
            this.sizeGripElement = new PropertyGridSizeGripElement();
            this.helpElement = new PropertyGridHelpElement();

            this.Children.Add(this.propertyTableElement);
            this.Children.Add(this.sizeGripElement);
            this.Children.Add(this.helpElement);
        }

        #endregion

        #region Properties
        
        /// <summary>
        /// Gets the <see cref="PropertyTableElement"/>.
        /// </summary>
        public PropertyGridTableElement PropertyTableElement
        {
            get
            {
                return propertyTableElement;
            }
        }

        /// <summary>
        /// Gets the <see cref="PropertyGridSizeGripElement"/>.
        /// </summary>
        public PropertyGridSizeGripElement SizeGripElement
        {
            get
            {
                return sizeGripElement;
            }
        }

        /// <summary>
        /// Gets the <see cref="PropertyGridHelpElement"/>.
        /// </summary>
        public PropertyGridHelpElement HelpElement
        {
            get
            {
                return helpElement;
            }
        }

        /// <summary>
        /// Gets or sets the height of the <see cref="PropertyGridHelpElement"/>.
        /// </summary>
        public float HelpElementHeight
        {
            get
            {
                return this.helpElement.HelpElementHeight;
            }
            set
            {
                this.helpElement.HelpElementHeight = value;
                this.InvalidateMeasure(true);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="PropertyGridHelpElement"/> is visible.
        /// </summary>
        public bool HelpVisible
        {
            get
            {
                return this.helpElement.Visibility == ElementVisibility.Visible;
            }
            set
            {
                if (value)
                {
                    this.helpElement.Visibility = ElementVisibility.Visible;
                    this.sizeGripElement.Visibility = ElementVisibility.Visible;
                    if (this.cachedHelpElementHeight != -1)
                    {
                        this.helpElement.HelpElementHeight = this.cachedHelpElementHeight;
                    }
                }
                else
                {
                    this.cachedHelpElementHeight = this.helpElement.HelpElementHeight;
                    this.helpElement.Visibility = ElementVisibility.Collapsed;
                    this.sizeGripElement.Visibility = ElementVisibility.Collapsed;
                }

                this.InvalidateMeasure();

                this.OnNotifyPropertyChanged("HelpVisible");
            }
        }

        #endregion

        #region Methods

        private bool IsInResizeLocation(Point location)
        {
            if (this.ElementTree == null)
            {
                return false;
            }

            RadElement element = this.ElementTree.GetElementAtPoint(new Point(location.X, location.Y));

            if (element == null)
            {
                return false;
            }

            if (element is PropertyGridSizeGripElement)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Begins the resize of the description element.
        /// </summary>
        /// <param name="offset">The offset used to resize the description element.</param>
        public virtual void BeginResize(int offset)
        {
            this.Capture = true;
            this.isResizing = true;
        }

        public virtual void ShowHelp()
        {
            this.helpElement.HelpElementHeight = this.cachedHelpElementHeight;
            this.isHelpHidden = false;
            this.InvalidateMeasure(true);
        }

        public virtual void HideHelp()
        {
            this.cachedHelpElementHeight = this.helpElement.HelpElementHeight;
            this.helpElement.HelpElementHeight = 0;
            this.isHelpHidden = true;
            this.InvalidateMeasure(true);
        }

        #endregion

        #region Event handlers

        protected override void OnDoubleClick(EventArgs e)
        {
            base.OnDoubleClick(e);

            if (this.IsInResizeLocation(((MouseEventArgs)e).Location))
            {
                if (this.isHelpHidden)
                {
                    this.ShowHelp();
                }
                else
                {
                    this.HideHelp();
                }
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (IsInResizeLocation(e.Location))
            {
                this.BeginResize(this.sizeGripElement.ControlBoundingRectangle.Y - e.Y);
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (isResizing)
            {
                if (this.isHelpHidden)
                {
                    this.isHelpHidden = false;
                }

                float offset = this.BoundingRectangle.Bottom - e.Y;

                if (offset >= this.BoundingRectangle.Height - this.propertyTableElement.ItemHeight * 2)
                {
                    this.HelpElementHeight = this.BoundingRectangle.Height - this.propertyTableElement.ItemHeight * 2;
                }
                else if (offset > 0)
                {
                    this.HelpElementHeight = offset;
                }
                else
                {
                    this.HideHelp();
                }

                return;
            }

            if (this.IsInResizeLocation(e.Location))
            {
                this.ElementTree.Control.Cursor = Cursors.HSplit;
            }
            else
            {
                this.ElementTree.Control.Cursor = Cursors.Default;
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            this.isResizing = false;
            this.Capture = false;
        }

        void propertyGridTableElement_SelectedItemChanged(object sender, RadPropertyGridEventArgs e)
        {
            this.helpElement.TitleText = e.Item.Label;
            this.helpElement.ContentText = e.Item.Description;
        }

        #endregion

        #region Layout

        protected override SizeF MeasureOverride(SizeF availableSize)
        {
            Padding borderThickness = this.GetBorderThickness(true);
            availableSize.Width -= this.Padding.Horizontal - borderThickness.Horizontal;
            availableSize.Height -= this.Padding.Vertical - borderThickness.Vertical;

            SizeF desiredSize = SizeF.Empty;

            this.helpElement.Measure(availableSize);
            availableSize.Height -= this.helpElement.DesiredSize.Height;
            desiredSize.Height += this.helpElement.DesiredSize.Height;

            this.sizeGripElement.Measure(availableSize);
            availableSize.Height -= this.sizeGripElement.DesiredSize.Height;
            desiredSize.Height += this.sizeGripElement.DesiredSize.Height;

            this.propertyTableElement.Measure(availableSize);
            desiredSize.Height += this.propertyTableElement.DesiredSize.Height;

            desiredSize.Width = Math.Max(this.sizeGripElement.DesiredSize.Width, this.propertyTableElement.DesiredSize.Width);
            desiredSize.Width = Math.Max(this.helpElement.DesiredSize.Width, desiredSize.Width);
            
            desiredSize.Width = Math.Min(desiredSize.Width, availableSize.Width);
            desiredSize.Height = Math.Min(desiredSize.Height, availableSize.Height);

            return desiredSize;
        }

        protected override SizeF ArrangeOverride(SizeF finalSize)
        {
            RectangleF clientRect = this.GetClientRectangle(finalSize);
                        
            float y = clientRect.Y;
            float height = clientRect.Height - (this.helpElement.DesiredSize.Height + this.sizeGripElement.DesiredSize.Height);
            RectangleF propertyGridRect = new RectangleF(clientRect.X, y, clientRect.Width, height);
            this.propertyTableElement.Arrange(propertyGridRect);

            y = height;
            height = this.sizeGripElement.DesiredSize.Height;
            RectangleF sizeGripRect = new RectangleF(clientRect.X, y, clientRect.Width, height);
            this.sizeGripElement.Arrange(sizeGripRect);

            y = y + height;
            height = this.helpElement.DesiredSize.Height;
            RectangleF descriptionRect = new RectangleF(clientRect.X, y, clientRect.Width, height);
            this.helpElement.Arrange(descriptionRect);

            return finalSize;
        }

        #endregion
    }
}

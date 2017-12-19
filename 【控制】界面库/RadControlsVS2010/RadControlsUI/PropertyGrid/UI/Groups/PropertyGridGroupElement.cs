using System.Drawing;
using System.ComponentModel;
using System;
using System.Windows.Forms;

namespace Telerik.WinControls.UI
{
    public class PropertyGridGroupElement : PropertyGridItemElementBase
    {
        #region Fields

        private PropertyGridGroupItem item;

        protected PropertyGridGroupExpanderElement expanderElement;
        protected PropertyGridGroupTextElement textElement;

        #endregion

        #region Constructors & Initialization
        
        protected override void CreateChildElements()
        {
            this.textElement = new PropertyGridGroupTextElement();
            this.expanderElement = new PropertyGridGroupExpanderElement(); 

            this.SuspendLayout();
            this.Children.Add(expanderElement);
            this.Children.Add(textElement);
            this.ResumeLayout(true);

            this.textElement.DrawBorder = true;
            this.textElement.DrawFill = true;
            this.textElement.TextAlignment = ContentAlignment.MiddleLeft;

            this.expanderElement.ExpanderItem.Class = "PropertyGridGroupExpanderItem";
        }
        
        #endregion

        #region Properties

        /// <summary>
        /// Gets the property grid group item expander element.
        /// </summary>
        public PropertyGridGroupExpanderElement ExpanderElement
        {
            get
            {
                return this.expanderElement;
            }
        }

        /// <summary>
        /// Gets the property grid group item text element.
        /// </summary>
        public PropertyGridGroupTextElement TextElement
        {
        	get
            {
                return this.textElement;
            }
        }

        #endregion

        #region Layout

        protected override SizeF MeasureOverride(SizeF availableSize)
        {
            SizeF desiredSize = SizeF.Empty;

            this.expanderElement.Measure(availableSize);
            availableSize.Width -= this.expanderElement.DesiredSize.Width;

            this.textElement.Measure(availableSize);

            desiredSize.Width = this.expanderElement.DesiredSize.Width + this.textElement.DesiredSize.Width;
            desiredSize.Height = Math.Max(this.expanderElement.DesiredSize.Height, this.textElement.DesiredSize.Height);

            return desiredSize;
        }

        protected override SizeF ArrangeOverride(SizeF finalSize)
        {
            RectangleF clientRect = GetClientRectangle(finalSize);

            expanderElement.Arrange(new RectangleF(clientRect.X, clientRect.Y, expanderElement.DesiredSize.Width, expanderElement.DesiredSize.Height));
            textElement.Arrange(new RectangleF(clientRect.X + expanderElement.DesiredSize.Width, clientRect.Y,
                clientRect.Width - expanderElement.DesiredSize.Width, clientRect.Height));

            return finalSize;
        }

        #endregion

        #region Event Handlers

        protected override void OnMouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseMove(e);

            this.ElementTree.Control.Cursor = Cursors.Default;
        }

        #endregion

        #region IVirtualizedElement<PropertyGridDataItemBase> Members

        /// <summary>
        /// Gets the logical item currently attached to this visual element.
        /// </summary>
        public override PropertyGridItemBase Data
        {
            get
            {
                return this.item;
            }
        }

        /// <summary>
        /// Attaches a logical item to this visual element.
        /// </summary>
        /// <param name="data">The logical item.</param>
        /// <param name="context">The context.</param>
        public override void Attach(PropertyGridItemBase data, object context)
        {
            PropertyGridGroupItem dataItem = data as PropertyGridGroupItem;

            if (dataItem != null)
            {
                this.item = dataItem;
                this.textElement.Text = data.Label;

                this.item.PropertyChanged += new PropertyChangedEventHandler(item_PropertyChanged);
                this.Synchronize();
            }
        }

        /// <summary>
        /// Detaches the currently attached logical item.
        /// </summary>
        public override void Detach()
        {
            this.item.PropertyChanged -= new PropertyChangedEventHandler(item_PropertyChanged);
            this.item = null;
        }

        /// <summary>
        /// Syncronizes changes with other elements.
        /// </summary>
        public override void Synchronize()
        {
            this.IsSelected = this.Data.Selected;
            this.IsExpanded = this.Data.Expanded;
            this.ToolTipText = this.Data.ToolTipText;

            this.expanderElement.Synchronize();

            this.PropertyTableElement.OnItemFormatting(new PropertyGridItemFormattingEventArgs(this));
        }

        /// <summary>
        /// Determines if a logical item is compatible with this visual element.
        /// </summary>
        /// <param name="data">The logical item to be checked for compatibility.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public override bool IsCompatible(PropertyGridItemBase data, object context)
        {
            return (data is PropertyGridGroupItem);
        }

        #endregion
    }
}
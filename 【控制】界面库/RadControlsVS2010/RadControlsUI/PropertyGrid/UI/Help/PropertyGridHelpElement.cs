using System;
using System.Drawing;
using Telerik.WinControls.Styles;

namespace Telerik.WinControls.UI
{
    public class PropertyGridHelpElement : LightVisualElement
    {
        #region Fields

        PropertyGridHelpTitleElement titleElement;
        PropertyGridHelpContentElement contentElement;
        private float helpElementHeight;

        #endregion

        #region Initialization

        static PropertyGridHelpElement()
        {
            ItemStateManagerFactoryRegistry.AddStateManagerFactory(new ItemStateManagerFactory(), typeof(PropertyGridHelpElement));
        }

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.NotifyParentOnMouseInput = true;
            this.DrawFill = true;
            this.DrawBorder = true;
            this.StretchHorizontally = true;
            this.helpElementHeight = 80;
        }

        protected override void CreateChildElements()
        {
            base.CreateChildElements();

            this.titleElement = new PropertyGridHelpTitleElement();
            this.contentElement = new PropertyGridHelpContentElement();

            this.Children.Add(this.titleElement);
            this.Children.Add(this.contentElement);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the text of the <see cref="PropertyGridHelpElement"/> title.
        /// </summary>
        public string TitleText
        {
            get
            {
                return this.titleElement.Text;
            }
            set
            {
                this.titleElement.Text = value;
            }
        }

        /// <summary>
        /// Gets or sets the text of the <see cref="PropertyGridHelpElement"/> content.
        /// </summary>
        public string ContentText
        {
        	get
            {
                return this.contentElement.Text;
            }
            set
            {
                this.contentElement.Text = value;
            }
        }

        /// <summary>
        /// Gets the <see cref="PropertyGridHelpTitleElement"/>.
        /// </summary>
        public PropertyGridHelpTitleElement HelpTitleElement
        {
            get
            {
                return this.titleElement;
            }
        }

        /// <summary>
        /// Gets the <see cref="PropertyGridHelpContentElement"/>.
        /// </summary>
        public PropertyGridHelpContentElement HelpContentElement
        {
            get
            {
                return this.contentElement;
            }
        }

        /// <summary>
        /// Gets or sets the height of the <see cref="PropertyGridHelpElement"/>.
        /// </summary>
        public float HelpElementHeight
        {
            get
            {
                return this.helpElementHeight;
            }
            set
            {
                this.helpElementHeight = value;
                this.InvalidateMeasure(true);
                this.SplitElement.PropertyTableElement.Update(PropertyGridTableElement.UpdateActions.ExpandedChanged);
            }
        }
        
        /// <summary>
        /// Gets the parent <see cref="PropertyGridSplitElement"/> of this element.
        /// </summary>
        public PropertyGridSplitElement SplitElement
        {
            get
            {
                return this.FindAncestor<PropertyGridSplitElement>();
            }
        }

        #endregion

        #region Layout

        protected override SizeF MeasureOverride(SizeF availableSize)
        {
            float originalAvailableHeight = availableSize.Height;

            availableSize.Height = Math.Min(helpElementHeight, availableSize.Height);

            System.Windows.Forms.Padding borderThickness = GetBorderThickness(false);
            availableSize.Width -= this.Padding.Horizontal + borderThickness.Horizontal;
            availableSize.Height -= this.Padding.Vertical + borderThickness.Vertical;

            SizeF desiredSize = SizeF.Empty;

            this.titleElement.Measure(availableSize);
            availableSize.Height -= this.titleElement.DesiredSize.Height;

            this.contentElement.Measure(availableSize);

            desiredSize.Width = Math.Max(this.titleElement.DesiredSize.Width, this.contentElement.DesiredSize.Width);
            desiredSize.Height = Math.Min(helpElementHeight, originalAvailableHeight);
            
            return desiredSize;
        }

        protected override SizeF ArrangeOverride(SizeF finalSize)
        {
            RectangleF clientRect = this.GetClientRectangle(finalSize);

            RectangleF titleRect = new RectangleF(clientRect.X, clientRect.Y, clientRect.Width, this.titleElement.DesiredSize.Height);
            this.titleElement.Arrange(titleRect);

            RectangleF contentRect = new RectangleF(clientRect.X, clientRect.Y + this.titleElement.DesiredSize.Height, 
                clientRect.Width, clientRect.Height - this.titleElement.DesiredSize.Height);
            this.contentElement.Arrange(contentRect);

            return finalSize;
        }

        #endregion
    }
}

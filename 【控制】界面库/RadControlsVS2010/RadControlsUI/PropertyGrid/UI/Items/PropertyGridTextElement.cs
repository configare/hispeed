using System.Drawing;

namespace Telerik.WinControls.UI
{
    public class PropertyGridTextElement : PropertyGridContentElement
    {
        private StackLayoutElement buttonsLayout;
        private PropertyValueButtonElement propertyValueButton;
        private PropertyGridErrorIndicatorElement errorIndicator;

        public StackLayoutElement ButtonsLayout
        {
            get { return buttonsLayout; }
        }

        public PropertyValueButtonElement PropertyValueButton
        {
            get { return propertyValueButton; }
        }

        public PropertyGridErrorIndicatorElement ErrorIndicator
        {
            get { return errorIndicator; }
        }

        static PropertyGridTextElement()
        {
            ItemStateManagerFactoryRegistry.AddStateManagerFactory(new PropertyGridItemElementStateManager(), typeof(PropertyGridTextElement));
        }

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.ShouldHandleMouseInput = false;
            this.NotifyParentOnMouseInput = true;
            this.ClipDrawing = true;
            this.ClipText = true;
            this.AutoEllipsis = true;
            this.TextAlignment = ContentAlignment.MiddleLeft;
            this.StretchHorizontally = true;
            this.StretchVertically = true;
            this.DrawBorder = false;
            this.DrawFill = false;
            this.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.ImageLayout = System.Windows.Forms.ImageLayout.None;
            this.ImageAlignment = ContentAlignment.MiddleLeft;
        }

        protected override void CreateChildElements()
        {
            base.CreateChildElements();

            buttonsLayout = new StackLayoutElement();
            buttonsLayout.Alignment = ContentAlignment.MiddleRight;
            buttonsLayout.FitInAvailableSize = true;

            errorIndicator = new PropertyGridErrorIndicatorElement();
            buttonsLayout.Children.Add(errorIndicator);

            propertyValueButton = new PropertyValueButtonElement();
            propertyValueButton.Click += new System.EventHandler(propertyValueButton_Click);
            buttonsLayout.Children.Add(propertyValueButton);
            this.Children.Add(buttonsLayout);
        }

        public virtual void Synchronize()
        {
            this.Text = VisualItem.Data.Label;
            this.Image = VisualItem.Data.Image;                      
        }

        protected override SizeF MeasureOverride(SizeF availableSize)
        {
            base.MeasureOverride(availableSize);
            return SizeF.Empty;
        }

        void propertyValueButton_Click(object sender, System.EventArgs e)
        {
            RadContextMenu menu = PropertyGridTableElement.GetElementContextMenu(this.VisualItem);
            if (menu != null)
            {
                if (this.PropertyGridTableElement.IsEditing)
                {
                    if (!this.PropertyGridTableElement.EndEdit())
                    {
                        return;
                    }
                }
                menu.Show(this.ElementTree.Control, 
                    this.propertyValueButton.ControlBoundingRectangle.X, 
                    this.propertyValueButton.ControlBoundingRectangle.Bottom);
            }
        }
    }
}

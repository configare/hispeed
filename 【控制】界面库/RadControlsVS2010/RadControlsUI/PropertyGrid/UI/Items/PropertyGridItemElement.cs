using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Telerik.WinControls.UI
{
    public class PropertyGridItemElement : PropertyGridItemElementBase
    {
        #region Dependency properties

        public static RadProperty IsChildItemProperty = RadProperty.Register(
            "IsChildItem", typeof(bool), typeof(PropertyGridItemElement), new RadElementPropertyMetadata(
                false, ElementPropertyOptions.AffectsDisplay | ElementPropertyOptions.CanInheritValue));

        public static RadProperty IsModifiedProperty = RadProperty.Register(
            "IsModified", typeof(bool), typeof(PropertyGridItemElementBase), new RadElementPropertyMetadata(
                false, ElementPropertyOptions.AffectsDisplay | ElementPropertyOptions.CanInheritValue));

        public static RadProperty IsInEditModeProperty = RadProperty.Register(
            "IsInEditMode", typeof(bool), typeof(PropertyGridItemElement), new RadElementPropertyMetadata(
                false, ElementPropertyOptions.AffectsDisplay));

        public static RadProperty HasChildrenProperty = RadProperty.Register(
            "HasChildren", typeof(bool), typeof(PropertyGridItemElement), new RadElementPropertyMetadata(
                false, ElementPropertyOptions.AffectsDisplay));

        #endregion

        #region Fields

        private const int resizePointerOffset = 3;

        private StackLayoutElement stack;
        private PropertyGridItem item;
        private PropertyGridRowHeaderElement headerElement;
        private PropertyGridIndentElement indentElement;
        private PropertyGridExpanderElement expanderElement;
        private PropertyGridTextElement textElement;
        private PropertyGridValueElement valueElement;
        private RadItem editorElement;
        private IInputEditor editor;
        private bool isResizing;
        private Point downLocation;
        private int downWidth;

        #endregion

        #region Constructors & Initialization

        static PropertyGridItemElement()
        {
            ItemStateManagerFactoryRegistry.AddStateManagerFactory(new PropertyGridItemElementStateManager(), typeof(PropertyGridItemElement));
        }

        protected override void CreateChildElements()
        {
            this.stack = new StackLayoutElement();
            this.stack.FitInAvailableSize = true;
            this.stack.StretchHorizontally = true;
            this.stack.StretchVertically = true;
            this.stack.NotifyParentOnMouseInput = true;
            this.stack.ShouldHandleMouseInput = false;
            this.stack.FitToSizeMode = RadFitToSizeMode.FitToParentBounds;

            this.headerElement = new PropertyGridRowHeaderElement();
            this.indentElement = new PropertyGridIndentElement();
            this.expanderElement = new PropertyGridExpanderElement();
            this.textElement = new PropertyGridTextElement();
            this.valueElement = new PropertyGridValueElement();

            this.stack.Children.Add(this.headerElement);
            this.stack.Children.Add(this.indentElement);
            this.stack.Children.Add(this.expanderElement);
            this.stack.Children.Add(this.textElement);
            this.stack.Children.Add(this.valueElement);
            
            this.Children.Add(this.stack);
        }

        protected override void InitializeFields()
        {
            base.InitializeFields();
            this.NotifyParentOnMouseInput = true;
            this.StretchHorizontally = true;
            this.StretchVertically = true;
            this.DrawBorder = false;
            this.DrawFill = false;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether this item has a parent or not.
        /// </summary>
        public bool IsChildItem
        {
            get
            {
                return (bool)this.GetValue(IsChildItemProperty);
            }
            set
            {
                this.SetValue(IsChildItemProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this item has changed its value or not.
        /// </summary>
        public bool IsModified
        {
            get { return (bool)GetValue(IsModifiedProperty); }
            set { SetValue(IsModifiedProperty, value); }
        }

        /// <summary>
        /// Gets the header element of the <see cref="PropertyGridItemElement"/>.
        /// </summary>
        public PropertyGridRowHeaderElement HeaderElement
        {
            get
            {
                return this.headerElement;
            }
        }

        /// <summary>
        /// Gets the property grid item indent element
        /// </summary>
        public PropertyGridIndentElement IndentElement
        {
            get
            {
                return this.indentElement;
            }
        }

        /// <summary>
        /// Gets the property grid item expander element.
        /// </summary>
        public PropertyGridExpanderElement ExpanderElement
        {
            get
            {
                return this.expanderElement;
            }
        }

        /// <summary>
        /// Gets the property grid item text element.
        /// </summary>
        public PropertyGridTextElement TextElement
        {
            get
            {
                return this.textElement;
            }
        }

        /// <summary>
        /// Gets the property grid item value element
        /// </summary>
        public PropertyGridValueElement ValueElement
        {
            get
            {
                return this.valueElement;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets a value indicating whether a given point is in a location where resize should be initialized when the left mouse button is pressed.
        /// </summary>
        /// <param name="location">The point to check for.</param>
        /// <returns>true if point is in location for resize otherwise false.</returns>
        public bool IsInResizeLocation(Point location)
        {
            return (location.X >= this.ControlBoundingRectangle.Width - this.PropertyTableElement.ValueColumnWidth - resizePointerOffset &&
                    location.X <= this.ControlBoundingRectangle.Width - this.PropertyTableElement.ValueColumnWidth + resizePointerOffset);
        }

        #endregion

        #region Event handlers

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (IsInResizeLocation(e.Location))
            {
                if (this.PropertyTableElement.IsEditing)
                {
                    this.PropertyTableElement.EndEdit();
                }

                this.Capture = true;
                this.isResizing = true;
                this.downLocation = e.Location;
                this.downWidth = this.PropertyTableElement.ValueColumnWidth;
            }

            base.OnMouseDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (this.isResizing)
            {
                int delta = e.Location.X - downLocation.X;
                this.PropertyTableElement.ValueColumnWidth = downWidth - delta;
                return;
            }

            if (this.IsInResizeLocation(e.Location))
            {
                this.ElementTree.Control.Cursor = Cursors.VSplit;
            }
            else
            {
                this.ElementTree.Control.Cursor = Cursors.Default;
            }

            base.OnMouseMove(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (this.isResizing)
            {
                this.isResizing = false;
                this.Capture = false;
            }

            base.OnMouseUp(e);
        }
        
        #endregion

        #region IVirtualizedElement<PropertyGridDataItemBase> Memebers

        /// <summary>
        /// Gets the logical item attached to this visual element.
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
            PropertyGridItem dataItem = data as PropertyGridItem;

            if (dataItem != null)
            {
                this.item = dataItem;
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
            this.IsChildItem = this.Data.Level > 0;
            this.ToolTipText = this.Data.ToolTipText;
            this.Enabled = this.Data.Enabled;
            this.SetValue(HasChildrenProperty, this.Data.GridItems.Count > 0);

            bool modified = ((PropertyGridItem)this.Data).IsModified;
            this.IsModified = modified;
            this.textElement.PropertyValueButton.SetValue(PropertyValueButtonElement.IsModifiedProperty, modified);
            this.valueElement.SetValue(PropertyGridValueElement.IsModifiedProperty, modified);

            string errorMessage = ((PropertyGridItem)this.Data).ErrorMessage;
            this.textElement.ErrorIndicator.ToolTipText = item.ErrorMessage;
            bool containsError = !string.IsNullOrEmpty(errorMessage);
            if (containsError)
            {
                this.textElement.ErrorIndicator.Visibility = ElementVisibility.Visible;
            }
            else
            {
                this.textElement.ErrorIndicator.Visibility = ElementVisibility.Collapsed;
            }

            this.valueElement.SetValue(PropertyGridValueElement.ContainsErrorProperty, containsError);

            this.headerElement.Synchronize();
            this.expanderElement.Synchronize();
            this.indentElement.Synchronize();
            this.valueElement.Synchronize();
            this.textElement.Synchronize();

            if (IsSelected)
            {
                this.ZIndex = 100;
            }
            else
            {
                ResetValue(ZIndexProperty, ValueResetFlags.Local);
            }

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
            return (data is PropertyGridItem);
        }

        #endregion

        #region Editing

        public bool IsInEditMode
        {
            get
            {
                return this.editor != null;
            }
        }

        public IInputEditor Editor
        {
            get
            {
                return this.editor;
            }
        }

        public virtual void AddEditor(IInputEditor editor)
        {
            if (editor != null && this.editor != editor)
            {
                this.editor = editor;
                this.editorElement = GetEditorElement(editor);

                if (this.editorElement != null && !this.valueElement.Children.Contains(this.editorElement))
                {
                    this.valueElement.Children.Add(this.editorElement);
                    SetValue(IsInEditModeProperty, true);
                    this.Synchronize();
                    this.UpdateLayout();
                }
            }
        }

        public virtual void RemoveEditor(IInputEditor editor)
        {
            if (editor != null && this.editor == editor)
            {
                this.editorElement = GetEditorElement(editor);

                if (this.editorElement != null && this.valueElement.Children.Contains(this.editorElement))
                {
                    this.valueElement.Children.Remove(this.editorElement);
                    this.editorElement = null;
                }

                this.editor = null;
                SetValue(IsInEditModeProperty, false);
                this.Synchronize();
            }
        }

        protected RadItem GetEditorElement(IValueEditor editor)
        {
            BaseInputEditor baseInputEditor = this.editor as BaseInputEditor;
            if (baseInputEditor != null)
            {
                return baseInputEditor.EditorElement as RadItem;
            }
            return editor as RadItem;
        }

        #endregion
    }
}

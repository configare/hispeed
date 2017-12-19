using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Telerik.WinControls.UI.StateManagers;

namespace Telerik.WinControls.UI
{
    public class TreeNodeElement : StackLayoutElement, IVirtualizedElement<RadTreeNode>
    {
        #region Dependency properties

        public static RadProperty IsSelectedProperty = RadProperty.Register(
            "IsSelected", typeof(bool), typeof(TreeNodeElement), new RadElementPropertyMetadata(
                false, ElementPropertyOptions.AffectsDisplay | ElementPropertyOptions.CanInheritValue));

        public static RadProperty IsCurrentProperty = RadProperty.Register(
            "IsCurrent", typeof(bool), typeof(TreeNodeElement), new RadElementPropertyMetadata(
                false, ElementPropertyOptions.AffectsDisplay | ElementPropertyOptions.CanInheritValue));

        public static RadProperty IsExpandedProperty = RadProperty.Register(
            "IsExpanded", typeof(bool), typeof(TreeNodeElement), new RadElementPropertyMetadata(
                false, ElementPropertyOptions.AffectsDisplay | ElementPropertyOptions.CanInheritValue));

        public static RadProperty IsControlInactiveProperty = RadProperty.Register(
            "IsControlInactive", typeof(bool), typeof(TreeNodeElement), new RadElementPropertyMetadata(
                false, ElementPropertyOptions.AffectsDisplay | ElementPropertyOptions.CanInheritValue));

        public static RadProperty IsRootNodeProperty = RadProperty.Register(
            "IsRootNode", typeof(bool), typeof(TreeNodeElement), new RadElementPropertyMetadata(
                false, ElementPropertyOptions.AffectsDisplay | ElementPropertyOptions.CanInheritValue));

        public static RadProperty HotTrackingProperty = RadProperty.Register(
            "HotTracking", typeof(bool), typeof(TreeNodeElement), new RadElementPropertyMetadata(
                false, ElementPropertyOptions.AffectsDisplay | ElementPropertyOptions.CanInheritValue));

        public static RadProperty FullRowSelectProperty = RadProperty.Register(
            "FullRowSelect", typeof(bool), typeof(TreeNodeElement), new RadElementPropertyMetadata(
                false, ElementPropertyOptions.AffectsDisplay | ElementPropertyOptions.CanInheritValue));

        public static RadProperty HasChildrenProperty = RadProperty.Register(
            "HasChildren", typeof(bool), typeof(TreeNodeElement), new RadElementPropertyMetadata(
                false, ElementPropertyOptions.AffectsDisplay));

        public static RadProperty ItemHeightProperty = RadProperty.Register(
    "ItemHeight", typeof(int), typeof(TreeNodeElement), new RadElementPropertyMetadata(
        -1, ElementPropertyOptions.AffectsDisplay));


        #endregion

        #region Constants

        protected const int AlrternatingColorSetState = 1;
        protected const int FullRowSelectState = AlrternatingColorSetState << 1;
        protected const int UpdateScrollRangeIfNeeded = FullRowSelectState << 1;

        #endregion

        #region Fields

        private RadTreeNode node;
        private TreeNodeLinesContainer linesElement;
        private TreeNodeExpanderItem expanderElement;
        private RadToggleButtonElement toggleElement;
        private TreeNodeImageElement imageElement;
        private TreeNodeContentElement contentElement;
        private RadBitVector64 states = new RadBitVector64(0);

        #endregion

        #region Constructors & Initialization

        static TreeNodeElement()
        {
            ItemStateManagerFactoryRegistry.AddStateManagerFactory(new TreeNodeElementStateManager(), typeof(TreeNodeElement));
        }

        protected override void CreateChildElements()
        {
            base.CreateChildElements();

            this.linesElement = new TreeNodeLinesContainer();
            this.Children.Add(this.linesElement);

            this.expanderElement = new TreeNodeExpanderItem();
            this.expanderElement.BindProperty(TreeNodeExpanderItem.IsSelectedProperty, this, IsSelectedProperty, PropertyBindingOptions.OneWay);
            this.expanderElement.BindProperty(TreeNodeExpanderItem.HotTrackingProperty, this, HotTrackingProperty, PropertyBindingOptions.OneWay);
            this.Children.Add(this.expanderElement);

            this.imageElement = new TreeNodeImageElement();
            this.imageElement.ClipDrawing = true;
            this.imageElement.StretchHorizontally = false;
            this.Children.Add(this.imageElement);

            this.contentElement = CreateContentElement();
            this.contentElement.BindProperty(TreeNodeContentElement.IsRootNodeProperty, this, IsRootNodeProperty, PropertyBindingOptions.OneWay);
            this.contentElement.BindProperty(TreeNodeContentElement.HasChildrenProperty, this, HasChildrenProperty, PropertyBindingOptions.OneWay);
            this.contentElement.BindProperty(TreeNodeContentElement.IsControlInactiveProperty, this, IsControlInactiveProperty, PropertyBindingOptions.OneWay);
            this.contentElement.BindProperty(TreeNodeContentElement.FullRowSelectProperty, this, FullRowSelectProperty, PropertyBindingOptions.OneWay);
            this.contentElement.BindProperty(TreeNodeContentElement.IsSelectedProperty, this, IsSelectedProperty, PropertyBindingOptions.OneWay);
            this.contentElement.BindProperty(TreeNodeContentElement.IsCurrentProperty, this, IsCurrentProperty, PropertyBindingOptions.OneWay);
            this.contentElement.BindProperty(TreeNodeContentElement.HotTrackingProperty, this, HotTrackingProperty, PropertyBindingOptions.OneWay);
            this.contentElement.BindProperty(TreeNodeContentElement.IsExpandedProperty, this, IsExpandedProperty, PropertyBindingOptions.OneWay);
            this.Children.Add(this.contentElement);
        }

        protected virtual RadToggleButtonElement CreateToggleElement()
        {
            RadToggleButtonElement toggleButton = null;

            CheckType checkType = this.node.CheckType;

            if (checkType == CheckType.CheckBox)
            {
                toggleButton = new TreeNodeCheckBoxElement();
            }
            else if (checkType == CheckType.RadioButton)
            {
                toggleButton = new RadRadioButtonElement();
            }

            return toggleButton;
        }

        protected override void InitializeFields()
        {
            base.InitializeFields();
            this.AllowDrag = true;
            this.AllowDrop = true;
            this.RightToLeftMode = StackLayoutElement.RightToLeftModes.ReverseOffset;
            this.Orientation = Orientation.Horizontal;
            this.StretchHorizontally = true;
            this.FitInAvailableSize = true;
            this.states[TreeNodeElement.UpdateScrollRangeIfNeeded] = false;
            this.states[TreeNodeElement.AlrternatingColorSetState] = false;
            this.states[TreeNodeElement.FullRowSelectState] = false;
        }

        protected virtual TreeNodeContentElement CreateContentElement()
        {
            TreeNodeContentElement contentElement = new TreeNodeContentElement();
            contentElement.ShouldHandleMouseInput = false;
            return contentElement;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether the node is selected.
        /// </summary>
        [Category(RadDesignCategory.AppearanceCategory)]
        [Description("Gets a value indicating that the node is selected")]
        public virtual bool IsSelected
        {
            get
            {
                return (bool)this.GetValue(TreeNodeElement.IsSelectedProperty);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating that this is the current node.
        /// </summary>
        [Category(RadDesignCategory.AppearanceCategory)]
        [Description("Gets a value indicating that this is the current node.")]
        public virtual bool IsCurrent
        {
            get
            {
                return (bool)this.GetValue(TreeNodeElement.IsCurrentProperty);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the node is expanded.
        /// </summary>
        [Category(RadDesignCategory.AppearanceCategory)]
        [Description("Gets a value indicating that the node is expanded")]
        public virtual bool IsExpanded
        {
            get
            {
                return (bool)this.GetValue(TreeNodeElement.IsExpandedProperty);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the control contains the focus.
        /// </summary>
        [Category(RadDesignCategory.AppearanceCategory)]
        [Description("Gets a value indicating that the node is expanded")]
        public virtual bool IsControlFocused
        {
            get
            {
                return !(bool)this.GetValue(TreeNodeElement.IsControlInactiveProperty);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the node is currently at root level.
        /// </summary>
        [Category(RadDesignCategory.AppearanceCategory)]
        [Description("Gets a value indicating whether the node is currently at root level.")]
        public virtual bool IsRootNode
        {
            get { return (bool)GetValue(IsRootNodeProperty); }
        }

        /// <summary>
        /// Gets or sets the arbitrary height for this particular node.
        /// Valid when the owning RadTreeViewElement's AllowArbitraryHeight property is set to true.
        /// </summary>
        [Category(RadDesignCategory.AppearanceCategory)]
        [Description("Gets or sets the arbitrary height for this particular node. Valid when the owning RadTreeViewElement's AllowArbitraryHeight property is set to true.")]
        public virtual int ItemHeight
        {
            get
            {
                RadTreeViewElement treeViewElement = this.TreeViewElement;

                if (this.Data != null && treeViewElement != null)
                {
                    if (this.Data.ItemHeight != -1 && (treeViewElement.AllowArbitraryItemHeight || treeViewElement.AutoSizeItems))
                    {
                        return this.Data.ItemHeight;
                    }

                    if (this.GetValueSource(TreeNodeElement.ItemHeightProperty) == ValueSource.DefaultValue)
                    {
                        return treeViewElement.ItemHeight;
                    }
                }

                return (int)this.GetValue(TreeNodeElement.ItemHeightProperty);
            }
            set
            {
                if (this.Data != null)
                {
                    this.Data.ItemHeight = value;
                }
            }
        }

        /// <summary>
        /// Gets a value indicating that this is the hot tracking node.
        /// </summary>
        [Category(RadDesignCategory.AppearanceCategory)]
        [Description("Gets a value indicating that this is the hot tracking node.")]
        public virtual bool HotTracking
        {
            get { return (bool)GetValue(HotTrackingProperty); }
        }

        /// <summary>
        /// Gets a value indicating whether this node contains child nodes.
        /// </summary>
        [Category(RadDesignCategory.AppearanceCategory)]
        [Description("Gets a value indicating whether this node contains child nodes.")]
        public virtual bool HasChildren
        {
            get { return (bool)GetValue(HasChildrenProperty); }
        }

        public TreeNodeLinesContainer LinesContainerElement
        {
            get { return this.linesElement; }
        }

        public TreeNodeExpanderItem ExpanderElement
        {
            get { return this.expanderElement; }
        }

        public RadToggleButtonElement ToggleElement
        {
            get { return this.toggleElement; }
        }

        public TreeNodeImageElement ImageElement
        {
            get { return this.imageElement; }
        }

        public TreeNodeContentElement ContentElement
        {
            get { return this.contentElement; }
        }

        public RadTreeViewElement TreeViewElement
        {
            get
            {
                return this.FindAncestor<RadTreeViewElement>();
            }
        }

        #endregion

        #region IVirtualizedElement<TreeNode> Members

        public RadTreeNode Data
        {
            get
            {
                return this.node;
            }
        }

        public virtual void Attach(RadTreeNode data, object context)
        {
            this.node = data;
            this.node.PropertyChanged += new PropertyChangedEventHandler(node_PropertyChanged);
            this.Synchronize();
        }

        public virtual void Detach()
        {
            this.states[TreeNodeElement.UpdateScrollRangeIfNeeded] = false;

            if (this.node.TreeViewElement != null &&
                this.node.TreeViewElement.SelectedNode == this.node &&
                this.node.TreeViewElement.ActiveEditor != null)
            {
                this.node.TreeViewElement.EndEdit();
            }

            this.SetValue(HotTrackingProperty, false);
            this.ResetValue(LightVisualElement.EnabledProperty, ValueResetFlags.Local);

            LightVisualElement applyElement = this;

            if (!this.states[TreeNodeElement.FullRowSelectState])
            {
                applyElement = this.ContentElement;
            }

            this.ImageElement.ResetValue(LightVisualElement.ImageProperty, ValueResetFlags.Local);
            this.ContentElement.ResetValue(LightVisualElement.EnabledProperty, ValueResetFlags.Local);
            this.ContentElement.ResetValue(LightVisualElement.FontProperty, ValueResetFlags.Local);
            this.ContentElement.ResetValue(LightVisualElement.ForeColorProperty, ValueResetFlags.Local);

            applyElement.ResetValue(LightVisualElement.OpacityProperty);
            applyElement.ResetValue(LightVisualElement.TextAlignmentProperty, ValueResetFlags.Local);
            applyElement.ResetValue(LightVisualElement.DrawFillProperty, ValueResetFlags.Local);
            applyElement.ResetValue(LightVisualElement.BackColorProperty, ValueResetFlags.Local);
            applyElement.ResetValue(LightVisualElement.BackColor2Property, ValueResetFlags.Local);
            applyElement.ResetValue(LightVisualElement.BackColor3Property, ValueResetFlags.Local);
            applyElement.ResetValue(LightVisualElement.BackColor4Property, ValueResetFlags.Local);
            applyElement.ResetValue(LightVisualElement.DrawBorderProperty, ValueResetFlags.Local);
            applyElement.ResetValue(LightVisualElement.BorderColorProperty, ValueResetFlags.Local);
            applyElement.ResetValue(LightVisualElement.GradientAngleProperty, ValueResetFlags.Local);
            applyElement.ResetValue(LightVisualElement.GradientPercentageProperty, ValueResetFlags.Local);
            applyElement.ResetValue(LightVisualElement.GradientPercentage2Property, ValueResetFlags.Local);
            applyElement.ResetValue(LightVisualElement.GradientStyleProperty, ValueResetFlags.Local);
            applyElement.ResetValue(LightVisualElement.NumberOfColorsProperty, ValueResetFlags.Local);

            this.node.PropertyChanged -= new PropertyChangedEventHandler(node_PropertyChanged);
            this.node = null;
        }

        protected virtual void DisposeToggleElement()
        {
            if (this.toggleElement != null)
            {
                this.toggleElement.ToggleStateChanging -= new StateChangingEventHandler(ToggleElement_ToggleStateChanging);
                this.Children.Remove(this.toggleElement);
                this.toggleElement.Dispose();
                this.toggleElement = null;
            }
        }

        public virtual void Synchronize()
        {
            this.states[TreeNodeElement.FullRowSelectState] = TreeViewElement.FullRowSelect;
            this.SetValue(IsSelectedProperty, node.Selected);
            this.SetValue(IsCurrentProperty, node.Current);
            this.SetValue(IsExpandedProperty, node.Expanded);
            this.SetValue(IsRootNodeProperty, node.IsRootNode);
            this.SetValue(FullRowSelectProperty, this.TreeViewElement.FullRowSelect);
            this.SetValue(HasChildrenProperty, node.Nodes.Count > 0);
            this.SetValue(EnabledProperty, node.Enabled);

            if (IsInValidState(true))
            {
                bool containsFocus = (this.ElementTree.Control.Focused || this.ElementTree.Control.ContainsFocus);
                this.SetValue(IsControlInactiveProperty, !containsFocus);
            }

            this.ToolTipText = node.ToolTipText;
            this.ContentElement.Synchronize();
            this.ExpanderElement.Synchronize();
            this.LinesContainerElement.Synchronize();
            this.SynchronizeToggleElement();
            this.ImageElement.Synchronize();
            this.ApplyStyle();
            this.OnFormatting();
        }

        protected virtual void SynchronizeToggleElement()
        {
            CheckType checkType = this.node.CheckType;

            if (checkType == CheckType.None)
            {
                this.DisposeToggleElement();
                return;
            }

            if ((checkType == CheckType.CheckBox && this.toggleElement is RadCheckBoxElement) ||
                (checkType == CheckType.RadioButton && this.toggleElement is RadRadioButtonElement))
            {
                this.toggleElement.ToggleState = this.node.CheckState;
                return;
            }

            this.DisposeToggleElement();
            this.toggleElement = this.CreateToggleElement();

            if (this.toggleElement == null)
            {
                return;
            }

            int index = this.Children.IndexOf(this.ExpanderElement);
            this.Children.Insert(index + 1, this.toggleElement);
            this.toggleElement.StretchHorizontally = false;
            this.toggleElement.ToggleState = this.node.CheckState;
            this.toggleElement.ShouldHandleMouseInput = true;
            this.toggleElement.NotifyParentOnMouseInput = false;
            this.toggleElement.ToggleStateChanging += new StateChangingEventHandler(ToggleElement_ToggleStateChanging);
        }

        protected virtual void ApplyStyle()
        {
            if (!this.node.HasStyle)
            {
                return;
            }

            TreeNodeStyle nodeStyle = this.node.Style;

            if (nodeStyle.ForeColor != Color.Empty && this.ContentElement.ForeColor != nodeStyle.ForeColor)
            {
                this.ContentElement.ForeColor = nodeStyle.ForeColor;
            }

            if (nodeStyle.TextAlignment != ContentAlignment.MiddleLeft &&
                this.ContentElement.TextAlignment != nodeStyle.TextAlignment)
            {
                this.ContentElement.TextAlignment = nodeStyle.TextAlignment;
            }

            LightVisualElement applyElement = this;
            if (!TreeViewElement.FullRowSelect)
            {
                applyElement = this.ContentElement;
            }

            if (nodeStyle.BackColor != Color.Empty && applyElement.BackColor != nodeStyle.BackColor)
            {
                applyElement.DrawFill = true;
                applyElement.BackColor = nodeStyle.BackColor;
            }

            if (nodeStyle.BackColor2 != Color.Empty && applyElement.BackColor2 != nodeStyle.BackColor2)
            {
                this.EnsureDrawFill(applyElement, 2);
                applyElement.BackColor2 = nodeStyle.BackColor2;
            }

            if (nodeStyle.BackColor3 != Color.Empty && applyElement.BackColor3 != nodeStyle.BackColor3)
            {
                this.EnsureDrawFill(applyElement, 3);
                applyElement.BackColor3 = nodeStyle.BackColor3;
            }

            if (nodeStyle.BackColor4 != Color.Empty && applyElement.BackColor4 != nodeStyle.BackColor4)
            {
                this.EnsureDrawFill(applyElement, 4);
                applyElement.BackColor4 = nodeStyle.BackColor4;
            }

            if (nodeStyle.BorderColor != Color.Empty && applyElement.BorderColor != nodeStyle.BorderColor)
            {
                applyElement.DrawBorder = true;
                applyElement.BorderColor = nodeStyle.BorderColor;
            }

            if (nodeStyle.GradientAngle != 90.0f && applyElement.GradientAngle != nodeStyle.GradientAngle)
            {
                applyElement.GradientAngle = nodeStyle.GradientAngle;
            }

            if (nodeStyle.GradientPercentage != 0.5f && applyElement.GradientPercentage != nodeStyle.GradientPercentage)
            {
                applyElement.GradientPercentage = nodeStyle.GradientPercentage;
            }

            if (nodeStyle.GradientPercentage2 != 0.5f && applyElement.GradientPercentage2 != nodeStyle.GradientPercentage2)
            {
                applyElement.GradientPercentage2 = nodeStyle.GradientPercentage2;
            }

            if (nodeStyle.GradientStyle != GradientStyles.Linear && applyElement.GradientStyle != nodeStyle.GradientStyle)
            {
                applyElement.GradientStyle = nodeStyle.GradientStyle;
            }

            if (nodeStyle.NumberOfColors != 4 && applyElement.NumberOfColors != nodeStyle.NumberOfColors)
            {
                applyElement.NumberOfColors = nodeStyle.NumberOfColors;
            }

            if (nodeStyle.Font != null && this.ContentElement.Font != nodeStyle.Font)
            {
                this.ContentElement.Font = nodeStyle.Font;
            }
        }

        private void EnsureDrawFill(LightVisualElement element, int backColorNo)
        {
            if (this.node.Style.GradientStyle != GradientStyles.Solid && this.node.Style.NumberOfColors >= backColorNo)
            {
                element.DrawFill = true;
            }
        }

        public virtual bool IsCompatible(RadTreeNode data, object context)
        {
            if (this.Data != null)
            {
                return data.Level == this.Data.Level;
            }

            return true;
        }

        #endregion

        #region Event handlers

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (this.TreeViewElement != null)
            {
                this.TreeViewElement.OnNodeMouseDown(new RadTreeViewMouseEventArgs(this.Data, e));
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (this.TreeViewElement != null)
            {
                this.TreeViewElement.OnNodeMouseUp(new RadTreeViewMouseEventArgs(this.Data, e));
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (this.TreeViewElement != null)
            {
                this.TreeViewElement.OnNodeMouseMove(new RadTreeViewMouseEventArgs(this.Data, e));
            }
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);

            if (this.TreeViewElement != null)
            {
                this.TreeViewElement.OnNodeMouseEnter(new RadTreeViewEventArgs(this.Data));
            }
        }

        protected override void OnMouseHover(EventArgs e)
        {
            base.OnMouseHover(e);

            if (this.TreeViewElement != null)
            {
                this.TreeViewElement.OnNodeMouseHover(new RadTreeViewEventArgs(this.Data));
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            if (this.TreeViewElement != null)
            {
                this.TreeViewElement.OnNodeMouseLeave(new RadTreeViewEventArgs(this.Data));
            }
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            if (this.TreeViewElement != null)
            {
                this.TreeViewElement.OnNodeMouseClick(new RadTreeViewEventArgs(this.Data));
            }
        }

        protected override void OnDoubleClick(EventArgs e)
        {
            base.OnDoubleClick(e);
            if (this.TreeViewElement != null)
            {
                this.TreeViewElement.OnNodeMouseDoubleClick(new RadTreeViewEventArgs(this.Data));
            }
        }

        private void ToggleElement_ToggleStateChanging(object sender, StateChangingEventArgs args)
        {
            if (this.IsInValidState(true))
            {
                this.node.CheckState = args.NewValue;
                args.Cancel = this.node.CheckState != args.NewValue;
            }
        }

        private void node_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.IsInValidState(true))
            {
                OnNodePropertyChanged(e);
            }
        }

        protected override void OnPropertyChanged(RadPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == IsMouseOverElementProperty && this.IsInValidState(true))
            {
                if (TreeViewElement.HotTracking)
                {
                    this.SetValue(HotTrackingProperty, e.NewValue);
                }
                else
                {
                    this.SetValue(HotTrackingProperty, false);
                }
                OnFormatting();
            }
        }

        protected virtual void OnNodePropertyChanged(PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsSelected")
            {
                this.SetValue(IsSelectedProperty, this.node.Selected);
                OnFormatting();
            }
            else if (e.PropertyName == "Current")
            {
                this.SetValue(IsCurrentProperty, this.node.Current);
                OnFormatting();
            }
            else if (e.PropertyName == "Text")
            {
                this.ContentElement.Text = this.node.Text;
                OnFormatting();
            }
            else if (e.PropertyName == "Expanded")
            {
                this.SetValue(IsExpandedProperty, this.node.Expanded);
                this.ExpanderElement.Synchronize();
                OnFormatting();
            }
            else if (e.PropertyName == "CheckState" && this.toggleElement != null)
            {
                this.toggleElement.ToggleState = this.node.CheckState;
            }
            else if (e.PropertyName == "Style")
            {
                ApplyStyle();
                OnFormatting();
            }
        }

        protected virtual void OnFormatting()
        {
            RadTreeViewElement treeViewElement = TreeViewElement;

            if (treeViewElement != null && treeViewElement.AllowAlternatingRowColor)
            {
                bool alternatingAllowed = !treeViewElement.FullRowSelect || (!Data.Current && !Data.Selected && !HotTracking);
                int index = treeViewElement.FirstVisibleIndex + this.Parent.Children.IndexOf(this);
                if (alternatingAllowed && index % 2 != 0)
                {
                    DrawFill = true;
                    GradientStyle = GradientStyles.Solid;
                    BackColor = treeViewElement.AlternatingRowColor;
                    StretchHorizontally = true;
                    this.states[TreeNodeElement.AlrternatingColorSetState] = true;
                    TreeViewElement.OnNodeFormatting(new TreeNodeFormattingEventArgs(this));
                    return;
                }
            }

            if (this.states[TreeNodeElement.AlrternatingColorSetState])
            {
                ResetValue(DrawFillProperty, ValueResetFlags.Local);
                ResetValue(GradientStyleProperty, ValueResetFlags.Local);
                ResetValue(BackColorProperty, ValueResetFlags.Local);
                ResetValue(StretchHorizontallyProperty, ValueResetFlags.Local);
            }

            TreeViewElement.OnNodeFormatting(new TreeNodeFormattingEventArgs(this));
        }

        #endregion

        #region Drag & Drop

        protected override bool CanDragCore(Point dragStartPoint)
        {
            return base.CanDragCore(dragStartPoint) && this.Enabled;
        }

        protected override Image GetDragHintCore()
        {
            ISupportDrag draggableImage = imageElement as ISupportDrag;
            ISupportDrag draggableContent = this.ContentElement as ISupportDrag;
            Image hint = draggableContent.GetDragHint();

            if (draggableImage == null)
            {
                return hint;
            }

            Image image = draggableImage.GetDragHint();

            if (image == null)
            {
                return hint;
            }

            if (hint == null)
            {
                return image;
            }

            Bitmap newHint = new Bitmap(image.Width + hint.Width, Math.Max(image.Height, hint.Height));

            using (Graphics graphics = Graphics.FromImage(newHint))
            {
                graphics.DrawImage(image, Point.Empty);
                graphics.DrawImage(hint, new Point(image.Width, 0));
            }

            return newHint;
        }

        #endregion

        #region Layout

        protected override SizeF MeasureOverride(SizeF availableSize)
        {
            RadTreeViewElement treeViewElement = this.TreeViewElement;

            SizeF desiredSize = base.MeasureOverride(availableSize);

            int oldHeight = this.Data.ActualSize.Height;
            int newHeight = this.ItemHeight;

            if (treeViewElement != null && ((this.Editor != null && newHeight < desiredSize.Height) || treeViewElement.AutoSizeItems))
            {
                newHeight = (int)desiredSize.Height;
                this.states[TreeNodeElement.UpdateScrollRangeIfNeeded] = true;
            }
            else
            {
                desiredSize.Height = newHeight;
            }

            if (treeViewElement != null && oldHeight != newHeight &&
                (this.states[TreeNodeElement.UpdateScrollRangeIfNeeded] ||
                 treeViewElement.AllowArbitraryItemHeight || treeViewElement.AutoSizeItems))
            {
                ItemScroller<RadTreeNode> scroller = treeViewElement.Scroller;
                this.Data.ActualSize = new Size(this.Data.ActualSize.Width, newHeight);
                scroller.UpdateScrollRange(scroller.Scrollbar.Maximum + (newHeight - oldHeight), false);
            }

            if (this.Editor == null)
            {
                this.states[TreeNodeElement.UpdateScrollRangeIfNeeded] = false;
            }

            if (!float.IsInfinity(availableSize.Width))
            {
                Size size = desiredSize.ToSize();
                this.Data.ActualSize = new Size(this.Data.ActualSize.Width, size.Height);
                desiredSize.Width = availableSize.Width;
            }
            else if (!ContentElement.TextWrap && this.Editor == null)
            {
                this.Data.ActualSize = desiredSize.ToSize();
            }

            return desiredSize;
        }

        protected override SizeF ArrangeOverride(SizeF finalSize)
        {
            this.PositionOffset = new SizeF(((RadTreeViewVirtualizedContainer)this.Parent).ScrollOffset.Width, 0);
            base.ArrangeOverride(finalSize);
            return finalSize;
        }

        #endregion

        #region Editing

        IInputEditor editor;

        public bool IsInEditMode
        {
            get { return this.editor != null; }
        }

        public IInputEditor Editor
        {
            get { return this.editor; }
        }

        public virtual void AddEditor(IInputEditor editor)
        {
            if (editor != null && this.editor != editor)
            {
                this.editor = editor;
                RadItem element = GetEditorElement(editor);

                if (element != null && !this.ContentElement.Children.Contains(element))
                {
                    this.ContentElement.Children.Add(element);
                    //this.ContentElement.Text = "";
                    //this.ContentElement.Image = null;
                    this.TreeViewElement.ViewElement.UpdateItems();
                    this.UpdateLayout();
                }
            }
        }

        public virtual void RemoveEditor(IInputEditor editor)
        {
            if (editor != null && this.editor == editor)
            {
                RadItem element = GetEditorElement(editor);

                if (element != null && this.ContentElement.Children.Contains(element))
                {
                    this.ContentElement.Children.Remove(element);
                }

                this.editor = null;
                this.Synchronize();
            }
        }

        private RadItem GetEditorElement(IValueEditor editor)
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

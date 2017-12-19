using System.Drawing;
using System.Drawing.Drawing2D;
using Telerik.WinControls.UI.StateManagers;


namespace Telerik.WinControls.UI
{
    public class TreeNodeExpanderItem : ExpanderItem
    {
        #region Dependancy Properties

        public static RadProperty IsSelectedProperty = RadProperty.Register(
            "IsSelected", typeof(bool), typeof(TreeNodeExpanderItem),
            new RadElementPropertyMetadata(false, ElementPropertyOptions.AffectsDisplay));

        public static RadProperty HotTrackingProperty = RadProperty.Register(
            "HotTracking", typeof(bool), typeof(TreeNodeExpanderItem),
                new RadElementPropertyMetadata(false, ElementPropertyOptions.AffectsDisplay));

        #endregion

        #region Fields

        private bool signImageSet;

        #endregion

        #region Initialization

        static TreeNodeExpanderItem()
        {
            ItemStateManagerFactoryRegistry.AddStateManagerFactory(new TreeNodeExpanderItemStateManager(), typeof(TreeNodeExpanderItem));
        }

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.FitToSizeMode = RadFitToSizeMode.FitToParentBounds;
            this.LinkOrientation = ExpanderItem.LinkLineOrientation.Bottom;
            this.LinkLineStyle = DashStyle.Dot;
            this.DrawSignBorder = true;
            this.DrawSignFill = false;
            this.SignSize = new System.Drawing.Size(9, 9);
            this.SignPadding = new System.Windows.Forms.Padding(1, 1, 1, 1);
            this.SignBorderColor = Color.LightGray;
            this.SignBorderWidth = 1;
            this.DrawSignBorder = true;
            this.ForeColor = Color.Black;
            this.SignStyle = SignStyles.PlusMinus;
            this.SignBorderColor = Color.Gray;
            this.LinkLineColor = Color.Gray;
        }

        #endregion

        #region Properties

        public TreeNodeElement NodeElement
        {
            get
            {
                return this.FindAncestor<TreeNodeElement>();
            }
        }

        public RadTreeViewElement TreeViewElement
        {
            get
            {
                return this.FindAncestor<RadTreeViewElement>();
            }
        }

        #endregion

        #region Methods

        public virtual void Synchronize()
        {
            TreeNodeElement nodeElement = NodeElement;
            if (nodeElement != null)
            {
                this.Expanded = nodeElement.Data.Expanded;
                RadTreeViewElement treeViewElement = nodeElement.TreeViewElement;

                if (treeViewElement.FullLazyMode)
                {
                    this.Visibility = ElementVisibility.Visible;
                }
                else
                {
                    if (!treeViewElement.ShowExpandCollapse || nodeElement.Data.Nodes.Count == 0 ||
                        (nodeElement.Data.Parent == null && !treeViewElement.ShowRootLines))
                    {
                        this.Visibility = ElementVisibility.Collapsed;
                    }
                    else
                    {
                        this.Visibility = ElementVisibility.Visible;
                    }
                }

                this.LinkLineColor = treeViewElement.LineColor;
                this.LinkOrientation = ExpanderItem.LinkLineOrientation.None;
                this.LinkLineStyle = (DashStyle)treeViewElement.LineStyle;

                if (treeViewElement.ShowLines)
                {
                    if (nodeElement.Data.PrevNode != null || nodeElement.Data.Parent != null)
                    {
                        this.LinkOrientation |= ExpanderItem.LinkLineOrientation.Top | LinkLineOrientation.Horizontal;
                    }
                    if (nodeElement.Data.NextNode != null)
                    {
                        this.LinkOrientation |= LinkLineOrientation.Bottom | LinkLineOrientation.Horizontal;
                    }
                }

                if (this.TreeViewElement.AllowPlusMinusAnimation)
                {
                    this.Opacity = this.TreeViewElement.ContainsMouse ? 1 : 0;
                }
                else
                {
                    this.Opacity = 1;
                }

                UpdateSignImage();
            }
        }

        #endregion

        #region Layout

        protected override SizeF MeasureOverride(SizeF availableSize)
        {
            SizeF desiredSize = base.MeasureOverride(availableSize);
            TreeNodeElement nodeElement = NodeElement;
            if (nodeElement != null)
            {
                desiredSize.Width = nodeElement.TreeViewElement.TreeIndent;
            }
            return desiredSize;
        }

        #endregion

        #region Event handlers

        protected override void ToggleExpanded()
        {
            RadTreeNode node = this.NodeElement.Data;
            bool expanded = !node.Expanded;
            node.Expanded = expanded;

            if (node.Expanded == expanded)
            {
                this.Expanded = expanded;
            }
        }

        protected override void OnPropertyChanged(RadPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property == ExpandedProperty || e.Property == TreeNodeElement.HotTrackingProperty)
            {
                UpdateSignImage();
            }
        }

        #endregion

        protected virtual void UpdateSignImage()
        {
            RadTreeViewElement treeViewElement = TreeViewElement;
            if (treeViewElement == null)
                return;

            bool hotTracking = (bool)GetValue(TreeNodeElement.HotTrackingProperty);

            if (Expanded)
            {
                if (hotTracking && SetSignImage(treeViewElement, RadTreeViewElement.HoveredExpandImageProperty))
                    return;

                if (SetSignImage(treeViewElement, RadTreeViewElement.ExpandImageProperty))
                    return;
            }

            if (hotTracking && SetSignImage(treeViewElement, RadTreeViewElement.HoveredCollapseImageProperty))
                return;

            if (SetSignImage(treeViewElement, RadTreeViewElement.CollapseImageProperty))
                return;

            if (signImageSet)
            {
                signImageSet = false;
                ResetValue(SignImageProperty, ValueResetFlags.Local);
                ResetValue(DrawSignBorderProperty, ValueResetFlags.Local);
                ResetValue(DrawSignFillProperty, ValueResetFlags.Local);
            }
        }

        protected virtual bool SetSignImage(RadElement element, RadProperty property)
        {
            ValueSource valueSource = element.GetValueSource(property);
            if (valueSource != ValueSource.DefaultValue)
            {
                this.SignImage = (Image)element.GetValue(property);
                this.SignStyle = SignStyles.Image;
                this.DrawSignBorder = false;
                this.DrawSignFill = false;
                this.signImageSet = true;
                return true;
            }
            return false;
        }
    }
}

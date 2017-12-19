using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing.Design;
using Telerik.WinControls.Layouts;
using Telerik.WinControls.UI.RibbonBar;
using System.Drawing.Drawing2D;
using System.Drawing;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Represents a ribbon tab. Ribbon tabs are used to manage between different
    /// groups of related operations, for example, in a text editor application between 
    /// write and insert functionality.
    /// </summary>
    public class RibbonTab : RadPageViewStripItem
    {
        #region Rad properties

        public static RadProperty RightShadowInnerColor1Property = RadProperty.Register(
            "RightShadowInnerColor1",
            typeof(Color),
            typeof(RibbonTab),
            new RadElementPropertyMetadata(Color.FromArgb(10, Color.Black), ElementPropertyOptions.AffectsDisplay)
            );

        public static RadProperty RightShadowInnerColor2Property = RadProperty.Register(
          "RightShadowInnerColor2",
          typeof(Color),
          typeof(RibbonTab),
          new RadElementPropertyMetadata(Color.FromArgb(20, Color.Black), ElementPropertyOptions.AffectsDisplay)
          );

        public static RadProperty RightShadowOuterColor1Property = RadProperty.Register(
          "RightShadowOuterColor1",
          typeof(Color),
          typeof(RibbonTab),
          new RadElementPropertyMetadata(Color.FromArgb(0, Color.Black), ElementPropertyOptions.AffectsDisplay)
          );

        public static RadProperty RightShadowOuterColor2Property = RadProperty.Register(
          "RightShadowOuterColor2",
          typeof(Color),
          typeof(RibbonTab),
          new RadElementPropertyMetadata(Color.FromArgb(20, Color.Black), ElementPropertyOptions.AffectsDisplay)
          );

        public static RadProperty LeftShadowInnerColor1Property = RadProperty.Register(
            "LeftShadowInnerColor1",
            typeof(Color),
            typeof(RibbonTab),
            new RadElementPropertyMetadata(Color.FromArgb(10, Color.Black), ElementPropertyOptions.AffectsDisplay)
            );

        public static RadProperty LeftShadowInnerColor2Property = RadProperty.Register(
          "LeftShadowInnerColor2",
          typeof(Color),
          typeof(RibbonTab),
          new RadElementPropertyMetadata(Color.FromArgb(20, Color.Black), ElementPropertyOptions.AffectsDisplay)
          );

        public static RadProperty LeftShadowOuterColor1Property = RadProperty.Register(
          "LeftShadowOuterColor1",
          typeof(Color),
          typeof(RibbonTab),
          new RadElementPropertyMetadata(Color.FromArgb(0, Color.Black), ElementPropertyOptions.AffectsDisplay)
          );

        public static RadProperty LeftShadowOuterColor2Property = RadProperty.Register(
          "LeftShadowOuterColor2",
          typeof(Color),
          typeof(RibbonTab),
          new RadElementPropertyMetadata(Color.FromArgb(20, Color.Black), ElementPropertyOptions.AffectsDisplay)
          );

        #endregion

        #region Fields

        private RadItemOwnerCollection items;
        private RadRibbonBarCommandTabCollection parentCollection;
        private ContextualTabGroup contextualTabGroup;
        internal RadPageViewItem obsoleteTab = null;
        private ExpandableStackLayout tabContentLayout;
        private RadTabStripContentPanel contentPanel;
        private RibbonTabStripElement owner;

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the RadRibbonBarCommandTab class.
        /// </summary>
        public RibbonTab()
        {
            this.Class = "RibbonTab";
        }

        public RibbonTab(string text)
            : this()
        {
            this.Text = text;
        }

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.items = new RadItemOwnerCollection();
            this.items.ItemTypes = new Type[] { typeof(RadRibbonBarGroup) };
#pragma warning disable 0618
            this.items.ExcludedTypes = new Type[] { typeof(RadRibbonBarCommandTab) };
#pragma warning restore 0618
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the first right inner color of the RibbonTab's shadow.
        /// </summary>
        [Description("Gets or sets first right inner color of the RibbonTab's shadow."), Category(RadDesignCategory.AppearanceCategory)]
        public Color RightShadowInnerColor1
        {
            get
            {
                return (Color)this.GetValue(RibbonTab.RightShadowInnerColor1Property);
            }
            set
            {
                this.SetValue(RibbonTab.RightShadowInnerColor1Property, value);
            }
        }

        /// <summary>
        /// Gets or sets the second right inner color of the RibbonTab's shadow.
        /// </summary>
        [Description("Gets or sets second right inner color of the RibbonTab's shadow."), Category(RadDesignCategory.AppearanceCategory)]
        public Color RightShadowInnerColor2
        {
            get
            {
                return (Color)this.GetValue(RibbonTab.RightShadowInnerColor2Property);
            }
            set
            {
                this.SetValue(RibbonTab.RightShadowInnerColor2Property, value);
            }
        }

        /// <summary>
        /// Gets or sets the first right outer color of the RibbonTab's shadow.
        /// </summary>
        [Description("Gets or sets first right outer color of the RibbonTab's shadow."), Category(RadDesignCategory.AppearanceCategory)]
        public Color RightShadowOuterColor1
        {
            get
            {
                return (Color)this.GetValue(RibbonTab.RightShadowOuterColor1Property);
            }
            set
            {
                this.SetValue(RibbonTab.RightShadowOuterColor1Property, value);
            }
        }

        /// <summary>
        /// Gets or sets the second right outer color of the RibbonTab's shadow.
        /// </summary>
        [Description("Gets or sets second right outer color of the RibbonTab's shadow."), Category(RadDesignCategory.AppearanceCategory)]
        public Color RightShadowOuterColor2
        {
            get
            {
                return (Color)this.GetValue(RibbonTab.RightShadowOuterColor2Property);
            }
            set
            {
                this.SetValue(RibbonTab.RightShadowOuterColor2Property, value);
            }
        }

        /// <summary>
        /// Gets or sets the first left inner color of the RibbonTab's shadow.
        /// </summary>
        [Description("Gets or sets the first left inner color of the RibbonTab's shadow."), Category(RadDesignCategory.AppearanceCategory)]
        public Color LeftShadowInnerColor1
        {
            get
            {
                return (Color)this.GetValue(RibbonTab.LeftShadowInnerColor1Property);
            }
            set
            {
                this.SetValue(RibbonTab.LeftShadowInnerColor1Property, value);
            }
        }

        /// <summary>
        /// Gets or sets the second left inner color of the RibbonTab's shadow.
        /// </summary>
        [Description("Gets or sets the second left inner color of the RibbonTab's shadow."), Category(RadDesignCategory.AppearanceCategory)]
        public Color LeftShadowInnerColor2
        {
            get
            {
                return (Color)this.GetValue(RibbonTab.LeftShadowInnerColor2Property);
            }
            set
            {
                this.SetValue(RibbonTab.LeftShadowInnerColor2Property, value);
            }
        }

        /// <summary>
        /// Gets or sets the first left outer color of the RibbonTab's shadow.
        /// </summary>
        [Description("Gets or sets the first left outer color of the RibbonTab's shadow."), Category(RadDesignCategory.AppearanceCategory)]
        public Color LeftShadowOuterColor1
        {
            get
            {
                return (Color)this.GetValue(RibbonTab.LeftShadowOuterColor1Property);
            }
            set
            {
                this.SetValue(RibbonTab.LeftShadowOuterColor1Property, value);
            }
        }

        /// <summary>
        /// Gets or sets the second left outer color of the RibbonTab's shadow.
        /// </summary>
        [Description("Gets or sets the second left outer color of the RibbonTab's shadow."), Category(RadDesignCategory.AppearanceCategory)]
        public Color LeftShadowOuterColor2
        {
            get
            {
                return (Color)this.GetValue(RibbonTab.LeftShadowOuterColor2Property);
            }
            set
            {
                this.SetValue(RibbonTab.LeftShadowOuterColor2Property, value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        //[Obsolete("Please use this instead.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RadPageViewItem Tab
        {
            get
            {
                return this;
            }
            set
            {
                if (value != null)
                {
                    //TODO: copy other properties for backward compat.

                    this.obsoleteTab = (RadPageViewItem)value;
                    this.Visibility = value.Visibility;
                    this.Text = value.Text;

                    this.AssureTabAdded();
                }
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        //[Obsolete()]
        public RadTabStripContentPanel ContentPanel
        {
            get
            {
                if (this.contentPanel == null)
                {
                    this.contentPanel = new RadTabStripContentPanel();
                }
                return this.contentPanel;
            }
        }

        /// <summary>
        /// Gets an instance of the <see cref="ExpandableStackLayout"/>class
        /// that represents the content layout of the tab. In this layout all
        /// chunks visible to the end user are put.
        /// </summary>
        [Browsable(false)]
        public ExpandableStackLayout ContentLayout
        {
            get
            {
                return this.tabContentLayout;
            }
        }

        /// <summary>
        /// Gets or sets the ContextualTabGroup of this CommandTab.
        /// </summary>
        [Editor(typeof(ContextualTabGroupsEditor), typeof(UITypeEditor)), DefaultValue(null)]
        [Browsable(false)]
        public ContextualTabGroup ContextualTabGroup
        {
            get
            {
                return this.contextualTabGroup;
            }
            set
            {
                this.contextualTabGroup = value;
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        internal RadRibbonBarCommandTabCollection ParentCollection
        {
            get
            {
                return this.parentCollection;
            }
            set
            {
                this.parentCollection = value;
                SetItems();
            }
        }

        /// <summary>
        /// Gets the nested items.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Editor(DesignerConsts.RadItemCollectionEditorString, typeof(UITypeEditor))]
        [RadNewItem("Add New Group...", true, false, false)]
        public RadItemOwnerCollection Items
        {
            get
            {
                return this.items;
            }
        }

        [Browsable(false)]
        [DefaultValue(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public override bool IsSelected
        {
            get { return base.IsSelected; }
            set { base.IsSelected = value; }
        }

        #endregion

        #region methods

        private void AssureTabAdded()
        {
            if (this.ParentCollection == null || this.ParentCollection.Owner == null || !(this.ParentCollection.Owner is RadTabStripElement))
                return;

            RadPageViewElement tabStrip = (RadPageViewElement)this.ParentCollection.Owner;

            int index = this.ParentCollection.IndexOf(this);

            if (index >= 0)
            {
                tabStrip.RemoveItem(this);
            }

            tabStrip.InsertItem(index, this);
        }

        private void SetItems()
        {
            if (this.parentCollection == null || this.parentCollection.Owner == null)
            {
                if (this.tabContentLayout != null && this.tabContentLayout.Parent != null)
                {
                    this.tabContentLayout.Parent.Children.Remove(this.tabContentLayout);
                }
                return;
            }

            RadRibbonBar parentRibbon = (RadRibbonBar)this.parentCollection.Owner.ElementTree.Control;
            RadElement baseElement = parentRibbon.RibbonBarElement.TabStripElement.ContentArea;

            if (this.tabContentLayout == null)
            {
                this.tabContentLayout = new ExpandableStackLayout();
                this.Items.Owner = this.tabContentLayout;
            }

            this.tabContentLayout.CollapseElementsOnResize = true;
            this.tabContentLayout.UseParentSizeAsAvailableSize = true;
            this.tabContentLayout.IsInStripMode = true;
            this.tabContentLayout.Visibility = ElementVisibility.Collapsed;
            if (!baseElement.Children.Contains(this.tabContentLayout))
            {
                baseElement.Children.Add(this.tabContentLayout);
            }
        }

        #endregion

        #region Overrides

        protected override void OnPropertyChanging(RadPropertyChangingEventArgs e)
        {
            base.OnPropertyChanging(e);

            if (e.Property == RadItem.VisibilityProperty &&
                ((ElementVisibility)e.NewValue) == ElementVisibility.Collapsed)
            {

                int indexOfCurrentTab = this.Owner.Items.IndexOf(this);
                bool selectedTabChanged = false;

                for (int i = indexOfCurrentTab + 1; i < this.Owner.Items.Count; i++)
                {
                    RadPageViewItem currentItem = this.Owner.Items[i] as RadPageViewItem;
                    if (currentItem.Visibility == ElementVisibility.Visible)
                    {
                        this.Owner.SelectedItem = currentItem;
                        selectedTabChanged = true;
                        break;
                    }
                }

                if (!selectedTabChanged)
                {
                    for (int i = indexOfCurrentTab - 1; i > -1; i--)
                    {
                        RadPageViewItem currentItem = this.Owner.Items[i] as RadPageViewItem;

                        if (currentItem.Visibility == ElementVisibility.Visible)
                        {
                            this.Owner.SelectedItem = currentItem;
                            selectedTabChanged = true;
                            break;
                        }
                    }
                }


                e.Cancel = !selectedTabChanged;

                if (!e.Cancel)
                {
                    RadRibbonBarElement ribbonBar = this.Owner.Parent as RadRibbonBarElement;

                    if (ribbonBar != null)
                    {
                        CommandTabEventArgs args = new CommandTabEventArgs(this);
                        ribbonBar.CallOnCommandTabCollapsed(args);
                    }

                }
                this.Owner.InvalidateMeasure();
                this.Owner.InvalidateArrange();
                this.Owner.UpdateLayout();
            }
            else if (e.Property == RadItem.VisibilityProperty
                && ((ElementVisibility)e.NewValue) == ElementVisibility.Visible)
            {
                RadRibbonBarElement ribbonBar = this.Owner.Parent as RadRibbonBarElement;

                if (ribbonBar != null)
                {
                    CommandTabEventArgs args = new CommandTabEventArgs(this);
                    ribbonBar.CallOnCommandTabExpanded(args);

                    ribbonBar.TabStripElement.SelectedItem = this;
                }
            }
        }

        protected override void OnPropertyChanged(RadPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property.Name == "IsSelected" && (bool)e.NewValue && this.Owner != null)
            {
                foreach (RadPageViewItem item in this.Owner.Items)
                {
                    if (item != this && item.IsSelected)
                    {
                        RadPageViewItemSelectingEventArgs selectingEventArgs = new RadPageViewItemSelectingEventArgs(item, this);
                        this.Owner.OnItemSelecting(this, selectingEventArgs);

                        item.IsSelected = false;
                    }
                }

                this.Owner.selectedItem = this;

                RadPageViewItemSelectedEventArgs selectedEventArgs = new RadPageViewItemSelectedEventArgs(null, this);
                this.Owner.OnItemSelected(this, selectedEventArgs);
            }
        }

        protected override void PaintElement(Telerik.WinControls.Paint.IGraphics graphics, float angle, SizeF scale)
        {
            if (this.owner == null)
            {
                this.owner = base.Owner as RibbonTabStripElement;
            }
            if (this.owner.PaintTabShadows)
            {
                this.PaintRightTabShadow(graphics);
                this.PaintLeftTabShadow(graphics);
            }

            base.PaintElement(graphics, angle, scale);
        }

        /// <summary>
        /// This method paints the left RibbonTab shadow that appears on the right of the tab.
        /// The method paints two 1 pixel wide vertical linear gradient lines that
        /// create a shadow effect. The colors of the shadow can be styled by
        /// the Visual Style Builder.
        /// </summary>
        private void PaintLeftTabShadow(Telerik.WinControls.Paint.IGraphics graphics)
        {
            int shapeCausedOffset = 3;
            int shadowLineWidth = 1;
            GraphicsPath shadowPath1 = new GraphicsPath();

            Rectangle shadowBounds1 = new Rectangle(
                shapeCausedOffset - shadowLineWidth,
                shapeCausedOffset + 1,
                shadowLineWidth,
                this.Bounds.Height - shapeCausedOffset);

            shadowPath1.AddLine(
                shapeCausedOffset - shadowLineWidth,
                shapeCausedOffset + 1,
                shapeCausedOffset - shadowLineWidth,
                this.Bounds.Height - shapeCausedOffset);

            graphics.DrawLinearGradientPath(shadowPath1,
                shadowBounds1,
                new Color[] {
                    this.LeftShadowInnerColor1,
                    this.LeftShadowInnerColor2},
                    PenAlignment.Left,
                    shadowLineWidth,
                    90);


            GraphicsPath shadowPath2 = new GraphicsPath();
            Rectangle shadowBounds2 = new Rectangle(
                shapeCausedOffset - 2 * shadowLineWidth,
                shapeCausedOffset + 1,
                shadowLineWidth,
                this.Bounds.Height - shapeCausedOffset);

            shadowPath2.AddLine(
                shapeCausedOffset - 2 * shadowLineWidth,
                shapeCausedOffset + 1,
                shapeCausedOffset - 2 * shadowLineWidth,
                this.Bounds.Height - shapeCausedOffset);

            graphics.DrawLinearGradientPath(shadowPath2,
                shadowBounds2,
                new Color[] {
                    this.LeftShadowOuterColor1,
                    this.LeftShadowOuterColor2},
                    PenAlignment.Left,
                    shadowLineWidth,
                    90);
        }

        /// <summary>
        /// This method paints the right RibbonTab shadow that appears on the right of the tab.
        /// The method paints two 1 pixel wide vertical linear gradient lines that
        /// create a shadow effect. The colors of the shadow can be styled by
        /// the Visual Style Builder.
        /// </summary>
        private void PaintRightTabShadow(Telerik.WinControls.Paint.IGraphics graphics)
        {
            int shapeCausedOffset = 3;
            int shadowLineWidth = 1;

            GraphicsPath shadowPath1 = new GraphicsPath();

            Rectangle shadowBounds1 = new Rectangle(
                this.Bounds.Width - shapeCausedOffset,
                shapeCausedOffset + 1,
                shadowLineWidth,
                this.Bounds.Height - shapeCausedOffset);

            shadowPath1.AddLine(
                this.Bounds.Width - shapeCausedOffset,
                shapeCausedOffset + 1,
                this.Bounds.Width - shapeCausedOffset,
                this.Bounds.Height - shapeCausedOffset);

            graphics.DrawLinearGradientPath(shadowPath1,
                shadowBounds1,
                new Color[] {
                    this.RightShadowInnerColor1,
                    this.RightShadowInnerColor2},
                    PenAlignment.Right,
                   shadowLineWidth,
                    90);


            GraphicsPath shadowPath2 = new GraphicsPath();
            Rectangle shadowBounds2 = new Rectangle(
                this.Bounds.Width - shapeCausedOffset + shadowLineWidth,
                shapeCausedOffset + 1,
                1,
                this.Bounds.Height - shapeCausedOffset);

            shadowPath2.AddLine(
                this.Bounds.Width - shapeCausedOffset + shadowLineWidth,
                shapeCausedOffset + 1,
                this.Bounds.Width - shapeCausedOffset + shadowLineWidth,
                this.Bounds.Height - shapeCausedOffset + 1);

            graphics.DrawLinearGradientPath(shadowPath2,
                shadowBounds2,
                new Color[] { this.RightShadowOuterColor1, this.RightShadowOuterColor2 },
                PenAlignment.Right,
                shadowLineWidth,
                90);
        }

        #endregion
    }
}

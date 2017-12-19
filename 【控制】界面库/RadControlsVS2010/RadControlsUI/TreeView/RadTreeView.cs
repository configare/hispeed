using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls.Commands;
using Telerik.WinControls.Data;
using Telerik.WinControls.Design;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Displays a hierarchical collection of labeled items, each represented by a <see cref="RadTreeNode">RadTreeNode</see>. 
    /// </summary>
    [TelerikToolboxCategory(ToolboxGroupStrings.DataControlsGroup)]
    [ToolboxItem(true)]
    [Designer("Telerik.WinControls.UI.Design.RadTreeViewDesigner, Telerik.WinControls.UI.Design, Version=" + VersionNumber.Number + ", Culture=neutral, PublicKeyToken=5bb2a467cbec794e")]
    [Description("Displays a hierarchical collection of labelled items to the user that can optionally contain an image")]
    [DefaultProperty("Nodes"), DefaultEvent("SelectedNodeChanged")]
    [ComplexBindingProperties("DataSource", "DataMember")]
    [LookupBindingProperties("DataSource", "DisplayMember", "ValueMember", "SelectedNode")]
    [Docking(DockingBehavior.Ask)]
    public class RadTreeView : RadControl
    {
        #region Fields

        private RadTreeViewElement treeElement;

        #endregion

        #region Constructors & Initialization & Disposal

        public RadTreeView()
        {
            WireEvents();
        }

        protected override void Dispose(bool disposing)
        {
            UnwireEvents();

            if (this.RadContextMenu is TreeViewDefaultContextMenu)
            {
                this.RadContextMenu.Dispose();
            }

            base.Dispose(disposing);
        }

        protected override void CreateChildItems(RadElement parent)
        {
            this.treeElement = CreateTreeViewElement();
            parent.Children.Add(this.treeElement);
        }

        protected virtual RadTreeViewElement CreateTreeViewElement()
        {
            return new RadTreeViewElement();
        }

        protected virtual void WireEvents()
        {
        }

        protected virtual void UnwireEvents()
        {

        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether the TreeView load child Nodes collection in NodesNeeded event only when Parend nodes expanded.
        /// </summary>
        /// <value><c>true</c> if [lazy mode]; otherwise, <c>false</c>.</value>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [DefaultValue(false)]
        public bool LazyMode
        {
            get
            {
                return this.treeElement.LazyMode;
            }
            set
            {
                if (this.treeElement.LazyMode != value)
                {
                    this.treeElement.LazyMode = value;
                    this.OnNotifyPropertyChanged("LazyMode");
                }
            }
        }

        /// <summary>
        /// Gets or sets the color of the drop hint.
        /// </summary>
        /// <remarks>
        /// The drop feedback is a visual cue that assists the user with information where to
        /// drop during the drag and drop operation.
        /// </remarks>
        /// <seealso cref="ShowDropHint">ShowDropHint Property</seealso>
        /// <value>
        /// The default value is
        /// <a href="ms-help://MS.VSCC.v80/MS.MSDN.v80/MS.NETDEVFX.v20.en/cpref8/html/P_System_Drawing_Color_Black.htm">
        /// black</a>.
        /// </value>
        [Category("Behavior"), DefaultValue(typeof(Color), "")]
        [Description("Gets or sets the color of the drop hint.")]
        public Color DropHintColor
        {
            get { return this.treeElement.DragDropService.DropHintColor; }
            set { this.treeElement.DragDropService.DropHintColor = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show drop feedback].
        /// </summary>
        /// <value><c>true</c> if [show drop feedback]; otherwise, <c>false</c>.</value>
        [Category("Behavior"), DefaultValue(true)]
        [Description(" Gets or sets a value indicating whether [show drop hint]")]
        public bool ShowDropHint
        {
            get { return this.treeElement.DragDropService.ShowDropHint; }
            set { this.treeElement.DragDropService.ShowDropHint = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show drop feedback].
        /// </summary>
        /// <value><c>true</c> if [show drop feedback]; otherwise, <c>false</c>.</value>
        [Category("Behavior"), DefaultValue(true)]
        [Description(" Gets or sets a value indicating whether [show drop feedback]")]
        public bool ShowDragHint
        {
            get { return this.treeElement.DragDropService.ShowDragHint; }
            set { this.treeElement.DragDropService.ShowDragHint = value; }
        }

        /// <summary>
        /// Contains data binding settings for related data.
        /// </summary>
        [Browsable(true)]
        [Description("Contains data binding settings for related data.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category("Data")]
        public RelationBindingCollection RelationBindings
        {
            get
            {
                return this.treeElement.RelationBindings;
            }
        }

        [DefaultValue(false), Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool EnableDeferredScrolling
        {
            get
            {
                return this.treeElement.EnableDeferredScrolling;
            }
            set
            {
                this.treeElement.EnableDeferredScrolling = value;
            }
        }

        /// <summary>Gets or sets the type of the <see cref="ExpandAnimation">expand animation enumeration</see>.</summary>
        /// <seealso cref="AllowPlusMinusAnimation">AllowPlusMinusAnimation enumeration</seealso>
        /// <seealso cref="PlusMinusAnimationStep">PlusMinusAnimationStep Property</seealso>
        /// <seealso cref="ExpandAnimation">ExpandAnimation Enumeration</seealso>
        /// <value>
        ///     The default value is ExpandAnimation.Opacity.
        /// </value>
        [Category("Behavior")]
        [Description("Gets or sets the type of expand animation.")]
        [DefaultValue(ExpandAnimation.Opacity)]
        public ExpandAnimation ExpandAnimation
        {
            get
            {
                return this.treeElement.ExpandAnimation;
            }
            set
            {
                this.treeElement.ExpandAnimation = value;
            }
        }

        /// <summary>Gets or sets the opacity animation step for expand/collapse animation.</summary>
        /// <value>
        /// Returns a double value from double.Epsilon to 1 representing the opacity changing step with
        /// which the plus minus buttons are animated. The default value is 0.025.
        /// </value>
        [Browsable(false)]
        [DefaultValue(0.025)]
        [Description("Gets or sets the animation step for expand/collapse animation.")]
        public double PlusMinusAnimationStep
        {
            get
            {
                return this.treeElement.PlusMinusAnimationStep;
            }

            set
            {
                this.treeElement.PlusMinusAnimationStep = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether animation of collapse/expand images is enabled.
        ///  </summary>
        /// <seealso cref="ShowExpandCollapse">ShowExpanCollapse Property</seealso>
        /// <seealso cref="PlusMinusAnimationStep">PlusMinusAnimationStep Property</seealso>
        /// <value>The default value is false.</value>
        [Category("Behavior"), DefaultValue(false)]
        [Description("Gets or sets a value indicating whether animation of collapse/expand images is enabled.")]
        public bool AllowPlusMinusAnimation
        {
            get
            {
                return this.treeElement.AllowPlusMinusAnimation;
            }
            set
            {
                this.treeElement.AllowPlusMinusAnimation = value;
            }
        }

        /// <summary>
        /// The default image index for nodes.
        /// </summary>
        /// <value>The index of the image.</value>
        [Category(RadDesignCategory.AppearanceCategory)]
        [RelatedImageList("ImageList")]
        [Editor(DesignerConsts.ImageIndexEditorString, typeof(UITypeEditor))]
        [TypeConverter(DesignerConsts.NoneExcludedImageIndexConverterString)]
        [Description("The default image index for nodes.")]
        [DefaultValue(-1)]
        public int ImageIndex
        {
            get { return this.treeElement.ImageIndex; }
            set
            {
                this.treeElement.ImageIndex = value;
            }
        }

        /// <summary>
        /// The default image key for nodes.
        /// </summary>
        /// <value>The image key.</value>
        [Category(RadDesignCategory.AppearanceCategory)]
        [RelatedImageList("ImageList"), Localizable(true), TypeConverter(typeof(ImageKeyConverter))]
        [Editor("System.Windows.Forms.Design.ImageIndexEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        [DefaultValue(""), RefreshProperties(RefreshProperties.Repaint)]
        [Description("The default image key for nodes.")]
        public string ImageKey
        {
            get { return this.treeElement.ImageKey; }
            set
            {
                this.treeElement.ImageKey = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether drag and drop operation with treeview
        /// nodes is enabled.
        /// </summary>
        /// <seealso cref="AllowDragDropBetweenTreeViews">AllowDragDropBetweenTreeViews Property</seealso>
        /// <seealso cref="RadTreeNode.AllowDrop">AllowDrop Property (Telerik.WinControls.UI.RadTreeNode)</seealso>
        /// <value>The default value is false.</value>
        [Category("Behavior"), DefaultValue(false)]
        [Description("Gets or sets a value indicating whether drag and drop is enabled.")]
        public bool AllowDragDrop
        {
            get { return this.treeElement.AllowDragDrop; }
            set
            {
                this.treeElement.AllowDragDrop = value;
                if (value)
                {
                    this.AllowDrop = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the user is allowed to select more than one tree node at time
        /// </summary>
        /// <value><c>true</c> if [multi select]; otherwise, <c>false</c>.</value>
        [Description("Gets or sets a value indicating whether the user is allowed to select more than one tree node at time")]
        [DefaultValue(false)]
        [Category("Behavior")]
        public bool MultiSelect
        {
            get { return this.treeElement.MultiSelect; }
            set { this.treeElement.MultiSelect = value; }
        }

        /// <summary>
        /// Gets or sets the shortcut menu associated with the control.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// A <see cref="T:System.Windows.Forms.ContextMenu"/> that represents the shortcut menu associated with the control.
        /// </returns>
        [Category("Behavior")]
        [DefaultValue(null)]
        [Description("Gets or sets the shortcut menu associated to the RadTreeView.")]
        public virtual RadContextMenu RadContextMenu
        {
            get { return this.treeElement.ContextMenu; }
            set
            {
                if (this.treeElement.ContextMenu != value)
                {
                    if (this.treeElement.ContextMenu is TreeViewDefaultContextMenu)
                    {
                        this.treeElement.ContextMenu.Dispose();
                    }

                    this.treeElement.ContextMenu = value;
                    if (this.treeElement.ContextMenu != null)
                    {
                        this.treeElement.ContextMenu.ThemeName = this.ThemeName;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="T:System.Windows.Forms.ContextMenuStrip"/> associated with this control.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The <see cref="T:System.Windows.Forms.ContextMenuStrip"/> for this control, or null if there is no <see cref="T:System.Windows.Forms.ContextMenuStrip"/>. The default is null.
        /// </returns>
        [Browsable(false)]
        public override ContextMenuStrip ContextMenuStrip
        {
            get
            {
                return base.ContextMenuStrip;
            }
            set
            {
                base.ContextMenuStrip = value;
            }
        }

        /// <summary>
        /// Gets or sets the filter.
        /// </summary>
        /// <value>The filter.</value>
        [Browsable(false)]
        [DefaultValue(null)]
        public object Filter
        {
            get
            {
                return this.TreeViewElement.Filter;
            }
            set
            {
                this.TreeViewElement.Filter = value;
            }
        }

        /// <summary>
        /// Gets or sets the sort order of Nodes.
        /// </summary>
        /// <value>The sort order.</value>
        [Browsable(true)]
        [DefaultValue(SortOrder.None)]
        public SortOrder SortOrder
        {
            get
            {
                return this.TreeViewElement.SortOrder;
            }
            set
            {
                this.TreeViewElement.SortOrder = value;
            }
        }

        /// <summary>
        /// Gets the filter descriptors.
        /// </summary>
        /// <value>The filter descriptors.</value>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public FilterDescriptorCollection FilterDescriptors
        {
            get
            {
                return this.TreeViewElement.FilterDescriptors;
            }
        }

        /// <summary>
        /// Gets the sort descriptors.
        /// </summary>
        /// <value>The sort descriptors.</value>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SortDescriptorCollection SortDescriptors
        {
            get
            {
                return this.TreeViewElement.SortDescriptors;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether checkboxes are displayed beside the nodes.
        ///  </summary>
        /// <value>The default value is false.</value>
        [Category("Behavior"), DefaultValue(false)]
        [Description("Gets or sets a value indicating whether checkboxes are displayed beside the nodes.")]
        public bool CheckBoxes
        {
            get { return this.TreeViewElement.CheckBoxes; }
            set { this.TreeViewElement.CheckBoxes = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the child nodes should be auto checked when RadTreeView is in tri state mode
        ///  </summary>
        /// <value>The default value is false.</value>
        [Category("Behavior"), DefaultValue(true)]
        [Description("Gets or sets a value indicating whether the child nodes should be auto checked when RadTreeView is in tri state mode")]
        public bool AutoCheckChildNodes
        {
            get { return this.treeElement.AutoCheckChildNodes; }
            set { this.treeElement.AutoCheckChildNodes = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the highlight spans the width of the tree
        /// view.
        /// </summary>
        /// <value>The default value is false.</value>
        [Category("Behavior"), DefaultValue(true)]
        [Description("Gets or sets a value indicating whether the highlight spans the width of the tree view.")]
        public bool FullRowSelect
        {
            get { return this.TreeViewElement.FullRowSelect; }
            set { this.TreeViewElement.FullRowSelect = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [hide selection].
        /// </summary>
        /// <value><c>true</c> if [hide selection]; otherwise, <c>false</c>.</value>
        [Category(RadDesignCategory.BehaviorCategory)]
        [Description("TreeViewHideSelectionDescr"), DefaultValue(true)]
        public bool HideSelection
        {
            get { return this.TreeViewElement.HideSelection; }
            set { this.TreeViewElement.HideSelection = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [hot tracking].
        /// </summary>
        /// <value><c>true</c> if [hot tracking]; otherwise, <c>false</c>.</value>
        [DefaultValue(true), Category("Behavior"), Description("TreeViewHotTrackingDescr")]
        public bool HotTracking
        {
            get { return this.TreeViewElement.HotTracking; }
            set { this.TreeViewElement.HotTracking = value; }
        }

        /// <summary>
        /// Gets or sets the indent.
        /// </summary>
        /// <value>The indent.</value>
        [Category(RadDesignCategory.AppearanceCategory)]
        [Localizable(true), Description("TreeViewIndentDescr")]
        [DefaultValue(20)]
        public int TreeIndent
        {
            get { return this.TreeViewElement.TreeIndent; }
            set { this.TreeViewElement.TreeIndent = value; }
        }

        /// <summary>
        /// Gets or sets the height of the item.
        /// </summary>
        /// <value>The height of the item.</value>
        [Description("TreeViewItemHeightDescr"), Category("CatAppearance")]
        [DefaultValue(20)]
        [Browsable(false)]
        public int ItemHeight
        {
            get { return this.TreeViewElement.ItemHeight; }
            set { this.TreeViewElement.ItemHeight = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether nodes can have different height.
        /// </summary>
        /// <value>The default value is false.</value>
        [Browsable(true), DefaultValue(false)]
        [Description("Gets or sets a value indicating whether nodes can have different height.")]
        public bool AllowArbitraryItemHeight
        {
            get
            {
                return this.treeElement.AllowArbitraryItemHeight;
            }
            set
            {
                if (this.treeElement.AllowArbitraryItemHeight != value)
                {
                    this.treeElement.AllowArbitraryItemHeight = value;
                    OnNotifyPropertyChanged("AllowArbitraryItemHeight");
                }
            }
        }

        /// <summary>Gets or sets the spacing in pixels between nodes.</summary>
        /// <value>The default value is 0.</value>
        [Category(RadDesignCategory.AppearanceCategory)]
        [DefaultValue(0)]
        [Description("Gets or sets the spacing between nodes")]
        public int SpacingBetweenNodes
        {
            get
            {
                return this.treeElement.ViewElement.ItemSpacing;
            }
            set
            {
                if (this.treeElement.NodeSpacing != value)
                {
                    this.treeElement.NodeSpacing = value;
                    OnNotifyPropertyChanged("SpacingBetweenNodes");
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether editing is allowed.
        /// </summary>
        /// <value><c>true</c> if [allow edit]; otherwise, <c>false</c>.</value>
        [DefaultValue(false), Category("Behavior"), Description("Gets or sets a value indicating whether editing is allowed.")]
        public bool AllowEdit
        {
            get { return this.TreeViewElement.AllowEdit; }
            set
            {
                if (this.TreeViewElement.AllowEdit != value)
                {
                    this.TreeViewElement.AllowEdit = value;
                    base.OnNotifyPropertyChanged("AllowEdit");
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether adding new nodes is allowed.
        /// </summary>
        /// <value><c>true</c> if [allow edit]; otherwise, <c>false</c>.</value>
        [DefaultValue(false)]
        [Category("Behavior"), Description("Gets or sets a value indicating whether adding new nodes is allowed.")]
        public bool AllowAdd
        {
            get
            {
                return this.TreeViewElement.AllowAdd;
            }
            set
            {
                if (this.TreeViewElement.AllowAdd != value)
                {
                    this.TreeViewElement.AllowAdd = value;
                    base.OnNotifyPropertyChanged("AllowAdd");
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether removing nodes is allowed.
        /// </summary>
        /// <value><c>true</c> if [allow edit]; otherwise, <c>false</c>.</value>
        [DefaultValue(false), Category("Behavior"), Description("Gets or sets a value indicating whether removing nodes is allowed.")]
        public bool AllowRemove
        {
            get
            {
                return this.TreeViewElement.AllowRemove;
            }
            set
            {
                if (this.TreeViewElement.AllowRemove != value)
                {
                    this.TreeViewElement.AllowRemove = value;
                    base.OnNotifyPropertyChanged("AllowRemove");
                }
            }
        }

        /// <summary>Gets a value indicating whether there is an open editor in the tree view.</summary>
        [Description("Gets a value indicating whether there is an open editor in the tree view.")]
        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsEditing
        {
            get { return this.treeElement.IsEditing; }
        }

        /// <summary>Gets the active editor in the tree.</summary>
        /// <value>
        ///     The <see cref="IValueEditor">IValueEditor Interface</see> if any. 
        /// </value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IInputEditor ActiveEditor
        {
            get
            {
                return this.treeElement.ActiveEditor;
            }
        }

        /// <summary>
        /// Gets or sets the color of the line.
        /// </summary>
        /// <value>The color of the line.</value>
        [Category(RadDesignCategory.AppearanceCategory)]
        [Description("TreeViewLineColorDescr"), DefaultValue(typeof(Color), "Gray")]
        public Color LineColor
        {
            get { return this.TreeViewElement.LineColor; }
            set { this.TreeViewElement.LineColor = value; }
        }

        /// <summary>
        /// Gets or sets the path separator.
        /// </summary>
        /// <value>The path separator.</value>
        [DefaultValue(@"\"), Category("Behavior")]
        public string PathSeparator
        {
            get { return this.TreeViewElement.PathSeparator; }
            set { this.TreeViewElement.PathSeparator = value; }
        }

        /// <summary>
        /// Gets or sets the selected node.
        /// </summary>
        /// <value>The selected node.</value>
        [Category("CatAppearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), Description("TreeViewSelectedNodeDescr")]
        public RadTreeNode SelectedNode
        {
            get { return this.TreeViewElement.SelectedNode; }
            set { this.TreeViewElement.SelectedNode = value; }
        }

        [Category("CatAppearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), Description("TreeViewSelectedNodeDescr")]
        public SelectedTreeNodeCollection SelectedNodes
        {
            get { return this.TreeViewElement.SelectedNodes; }
        }

        /// <summary>
        /// Gets the checked nodes.
        /// </summary>
        /// <value>The checked nodes.</value>
        [Browsable(false)]
        public CheckedTreeNodeCollection CheckedNodes
        {
            get { return this.TreeViewElement.CheckedNodes; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show lines].
        /// </summary>
        /// <value><c>true</c> if [show lines]; otherwise, <c>false</c>.</value>
        [Description("TreeViewShowLinesDescr"), Category("Behavior"), DefaultValue(false)]
        public bool ShowLines
        {
            get { return this.TreeViewElement.ShowLines; }
            set { this.TreeViewElement.ShowLines = value; }
        }

        [Category("Behavior"), Description("TreeViewShowShowNodeToolTipsDescr"), DefaultValue(false)]
        public bool ShowNodeToolTips
        {
            get { return this.TreeViewElement.ShowNodeToolTips; }
            set { this.TreeViewElement.ShowNodeToolTips = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether expand/collapse (plus-minus) buttons are
        /// shown next to nodes with children.
        /// </summary>
        /// <value>The default value is true.</value>
        [Category("Behavior"), Description("Gets or sets a value indicating whether expand/collapse buttons are shown next to nodes with children."), DefaultValue(true)]
        public bool ShowExpandCollapse
        {
            get { return this.TreeViewElement.ShowExpandCollapse; }
            set { this.TreeViewElement.ShowExpandCollapse = value; }
        }

        [Description("TreeViewShowRootLinesDescr"), Category("Behavior"), DefaultValue(true)]
        public bool ShowRootLines
        {
            get { return this.TreeViewElement.ShowRootLines; }
            set { this.TreeViewElement.ShowRootLines = value; }
        }

        /// <summary>
        /// Gets the top node.
        /// </summary>
        /// <value>The top node.</value>
        [Browsable(false), Category("CatAppearance"), Description("TreeViewTopNodeDescr"), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RadTreeNode TopNode
        {
            get { return this.TreeViewElement.TopNode; }
        }

        /// <summary>
        /// Gets the visible count.
        /// </summary>
        /// <value>The visible count.</value>
        [Category("CatAppearance"), Description("TreeViewVisibleCountDescr"), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int VisibleCount
        {
            get
            {
                return this.TreeViewElement.VisibleCount;
            }
        }

        /// <summary>
        /// Gets or sets the name of the list or table in the data source for which the <see cref="GridViewTemplate"/> is displaying data. 
        /// </summary>
        [Browsable(true), Category("Data"), DefaultValue("")]
        [Editor("System.Windows.Forms.Design.DataMemberListEditor, System.Design", "System.Drawing.Design.UITypeEditor, System.Drawing")]
        [Description("Gets or sets the name of the list or table in the data source for which the RadTreeView is displaying data. ")]
        public string DataMember
        {
            get { return this.TreeViewElement.DataMember; }
            set
            {
                if (this.TreeViewElement.DataMember != value)
                {
                    this.TreeViewElement.DataMember = value;
                    this.OnNotifyPropertyChanged("DataMember");
                }
            }
        }

        /// <summary>
        /// Gets or sets the data source that the <see cref="RadTreeView"/> is displaying data for.
        /// </summary>
        [Category("Data")]
        [Description("Gets or sets the data source that the RadTreeView is displaying data for.")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [AttributeProvider(typeof(IListSource))]
        [DefaultValue((string)null)]
        public object DataSource
        {
            get { return this.TreeViewElement.DataSource; }
            set
            {
                if (this.TreeViewElement.DataSource != value)
                {
                    this.TreeViewElement.DataSource = value;
                    this.OnNotifyPropertyChanged("DataSource");
                }
            }
        }

        /// <summary>
        /// Gets or sets the display member.
        /// </summary>
        /// <value>The display member.</value>
        [Description("Gets or sets the display member.")]
        [Editor("System.Windows.Forms.Design.DataMemberFieldEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        [Category("Data")]
        [DefaultValue("")]
        public string DisplayMember
        {
            get
            {
                return this.TreeViewElement.DisplayMember;
            }
            set
            {
                this.TreeViewElement.DisplayMember = value;
            }
        }

        /// <summary>
        /// Gets or sets the value member.
        /// </summary>
        /// <value>The value member.</value>
        [Editor("System.Windows.Forms.Design.DataMemberFieldEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        [Description("Gets or sets the value member.")]
        [Category("Data")]
        [DefaultValue("")]
        public string ValueMember
        {
            get
            {
                return this.TreeViewElement.ValueMember;
            }
            set
            {
                this.TreeViewElement.ValueMember = value;
            }
        }

        /// <summary>
        /// Gets or sets the child member.
        /// </summary>
        /// <value>The child member.</value>
        [Editor("System.Windows.Forms.Design.DataMemberFieldEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        [Description("Gets or sets the value member.")]
        [Category("Data")]
        [DefaultValue("")]
        public string ChildMember
        {
            get
            {
                return this.TreeViewElement.ChildMember;
            }
            set
            {
                this.TreeViewElement.ChildMember = value;
            }
        }

        /// <summary>
        /// Gets or sets the parent member.
        /// </summary>
        /// <value>The parent member.</value>
        [Editor("System.Windows.Forms.Design.DataMemberFieldEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        [Description("Gets or sets the parent member.")]
        [Category("Data")]
        [DefaultValue("")]
        public string ParentMember
        {
            get
            {
                return this.TreeViewElement.ParentMember;
            }
            set
            {
                this.TreeViewElement.ParentMember = value;
            }
        }

        /// <summary>
        ///  Gets the collection of tree nodes that are assigned to the tree view control.
        /// </summary>
        /// <value>A System.Windows.Forms.TreeNodeCollection that represents the tree nodes assigned to the tree view control.</value>
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Editor("Telerik.WinControls.UI.Design.RadTreeViewEditor, Telerik.WinControls.UI.Design, Version=" + VersionNumber.Number + ", Culture=neutral, PublicKeyToken=5bb2a467cbec794e", typeof(UITypeEditor))]
        [Category("Data")]
        public RadTreeNodeCollection Nodes
        {
            get { return this.TreeViewElement.Nodes; }
        }

        /// <summary>
        /// Gets the tree view element.
        /// </summary>
        /// <value>The tree view element.</value>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual RadTreeViewElement TreeViewElement
        {
            get { return treeElement; }
        }

        /// <summary>
        /// Gets the Horizontal scroll bar.
        /// </summary>
        /// <value>The Horizontal scroll bar.</value>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RadScrollBarElement HScrollBar
        {
            get
            {
                return this.treeElement.HScrollBar;
            }
        }

        /// <summary>
        /// Gets the Vertical scroll bar.
        /// </summary>
        /// <value>The Vertical scroll bar.</value>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RadScrollBarElement VScrollBar
        {
            get
            {
                return this.treeElement.VScrollBar;
            }
        }

        /// <summary>
        /// Gets or sets the line style.
        /// </summary>
        /// <seealso cref="TreeLineStyle">TreeLineStyle enumeration</seealso>
        /// <value>
        ///     A <see cref="TreeLineStyle">TreeLineStyle</see> that represents the style used for
        ///     the lines beteen the nodes. The default is
        ///     <see cref="TreeLineStyle">TreeLineStyle.Dot</see>.
        /// </value>
        [Category(RadDesignCategory.AppearanceCategory)]
        [DefaultValue(TreeLineStyle.Dot)]
        [Description("Gets or sets the line style.")]
        public TreeLineStyle LineStyle
        {
            get
            {
                return this.TreeViewElement.LineStyle;
            }
            set
            {
                if (this.TreeViewElement.LineStyle != value)
                {
                    this.TreeViewElement.LineStyle = value;
                    base.OnNotifyPropertyChanged("LineStyle");
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether tri state mode is enabled.
        ///  </summary>
        /// <value>The default value is false.</value>
        [Category("Behavior"), DefaultValue(false)]
        [Description("Gets or sets a value indicating whether tri state mode is enabled.")]
        public bool TriStateMode
        {
            get { return this.TreeViewElement.TriStateMode; }
            set
            {
                if (this.TreeViewElement.TriStateMode != value)
                {
                    this.TreeViewElement.TriStateMode = value;
                    base.OnNotifyPropertyChanged("TriStateMode");
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating the default tree view toggle mode.
        /// </summary>
        [DefaultValue(ToggleMode.DoubleClick), Category(RadDesignCategory.BehaviorCategory)]
        public ToggleMode ToggleMode
        {
            get { return this.TreeViewElement.ToggleMode; }
            set
            {
                if (this.TreeViewElement.ToggleMode != value)
                {
                    this.TreeViewElement.ToggleMode = value;
                    base.OnNotifyPropertyChanged("ToggleMode");
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether a search is performed as the user types
        /// symbols.
        /// </summary>
        /// <value>The default value is false.</value>
        [Category("Behavior"), DefaultValue(false)]
        [Description("Gets or sets a value indicating whether incremental search is enabled.")]
        [Obsolete("This property will be removed in the next version. You can define your own search behavior by setting the Find method.")]
        [Browsable(false)]
        public bool AllowIncrementalSearch
        {
            get { return false; }
            set { }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), DefaultValue(""),
         Editor("System.ComponentModel.Design.MultilineStringEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        public string TreeViewXml
        {
            get
            {
                if (this.Nodes.Count > 0)
                {
                    StringBuilder builder = new StringBuilder();
                    using (StringWriter writer = new StringWriter(builder))
                    {
                        this.SaveXMLWithWriter(writer);
                        return builder.ToString();
                    }
                }

                return String.Empty;
            }
            set
            {
                using (StringReader reader = new StringReader(value))
                {
                    this.LoadXMLWithReader(reader);
                }

                this.OnNotifyPropertyChanged("TreeViewXml");
            }
        }

        #endregion

        #region Events

        public delegate void RadTreeViewEventHandler(object sender, RadTreeViewEventArgs e);
        public delegate void TreeViewEventHandler(object sender, RadTreeViewEventArgs e);
        public delegate void RadTreeViewCancelEventHandler(object sender, RadTreeViewCancelEventArgs e);
        public delegate void TreeViewMouseEventHandler(object sender, RadTreeViewMouseEventArgs e);
        public delegate void ItemDragHandler(object sender, RadTreeViewEventArgs e);
        public delegate void EditorRequiredHandler(object sender, TreeNodeEditorRequiredEventArgs e);
        public delegate void DragStartedHandler(object sender, RadTreeViewDragEventArgs e);
        public delegate void DragStartingHandler(object sender, RadTreeViewDragCancelEventArgs e);
        public delegate void DragEndingHandler(object sender, RadTreeViewDragCancelEventArgs e);
        public delegate void DragEndedHandler(object sender, RadTreeViewDragEventArgs e);

        /// <summary>
        /// Occurs when the RadTreeView report the data error.
        /// </summary>
        [Category("Behavior")]
        [Description("Occurs when the RadTreeView report the data error.")]
        public event TreeNodeDataErrorEventHandler DataError
        {
            add { this.treeElement.DataError += value; }
            remove { this.treeElement.DataError -= value; }
        }
        
        /// <summary>
        /// Occurs when the user begins dragging an item.
        /// </summary>
        [Category("Behavior")]
        [Description("Occurs when the user begins dragging an item.")]
        public event ItemDragHandler ItemDrag
        {
            add { this.treeElement.ItemDrag += value; }
            remove { this.treeElement.ItemDrag -= value; }
        }

        /// <summary>
        /// Occurs when TreeView required editor.
        /// </summary>
        [Browsable(true),
        Category(RadDesignCategory.ActionCategory),
        Description("Occurs when TreeView required editor.")]
        public event EditorRequiredHandler EditorRequired
        {
            add { this.treeElement.EditorRequired += value; }
            remove { this.treeElement.EditorRequired -= value; }
        }

        /// <summary>
        /// Occurs before the tree node label text is edited.
        /// </summary>
        [Category("Behavior")]
        [Description("Occurs before the tree node label text is edited.")]
        public event TreeNodeEditingEventHandler Editing
        {
            add { this.treeElement.Editing += value; }
            remove { this.treeElement.Editing -= value; }
        }

        /// <summary>
        /// Occurs when initializing the active editor.
        /// </summary>
        [Category("Behavior")]
        [Description("Occurs when initializing the active editor.")]
        public event TreeNodeEditorInitializedEventHandler EditorInitialized
        {
            add { this.treeElement.EditorInitialized += value; }
            remove { this.treeElement.EditorInitialized -= value; }
        }

        /// <summary>
        /// Occurs before the tree node label text is edited.
        /// </summary>
        [Category("Behavior")]
        [Description("Occurs after the tree node label text is edited.")]
        public event TreeNodeEditedEventHandler Edited
        {
            add { this.treeElement.Edited += value; }
            remove { this.treeElement.Edited -= value; }
        }

        /// <summary>
        /// Occurs when the editor is changing the value during the editing proccess.
        /// </summary>
        [Category(RadDesignCategory.ActionCategory),
         Description("Occurs when the editor is changing the value during the editing proccess.")]
        public event TreeNodeValueChangingEventHandler ValueChanging
        {
            add { this.treeElement.ValueChanging += value; }
            remove { this.treeElement.ValueChanging -= value; }
        }

        /// <summary>
        /// Occurs when the editor finished the value editing.
        /// </summary>
        [Browsable(true),
         Category(RadDesignCategory.ActionCategory),
         Description("Occurs when the editor finished the value editing.")]
        public event TreeNodeValueChangedEventHandler ValueChanged
        {
            add { this.treeElement.ValueChanged += value; }
            remove { this.treeElement.ValueChanged -= value; }
        }

        /// <summary>
        /// Occurs when the editor changed the value edting.
        /// </summary>
        [Browsable(true),
       Category(RadDesignCategory.ActionCategory),
       Description("Occurs when the editor changed the value edting.")]
        public event TreeNodeValidatingEventHandler ValueValidating
        {
            add { this.treeElement.ValueValidating += value; }
            remove { this.treeElement.ValueValidating -= value; }
        }

        /// <summary>
        /// Occurs when editor validating fails.
        /// </summary>
        [Browsable(true),
       Category(RadDesignCategory.ActionCategory),
       Description("Occurs when editor validating fails.")]
        public event EventHandler ValidationError
        {
            add { this.treeElement.ValidationError += value; }
            remove { this.treeElement.ValidationError -= value; }
        }

        /// <summary>
        /// Occurs when a drag is ending 
        /// </summary>
        [Category("Behavior")]
        [Description("Occurs when a drag is ending ")]
        public event DragEndingHandler DragEnding
        {
            add { this.treeElement.DragEnding += value; }
            remove { this.treeElement.DragEnding -= value; }
        }

        /// <summary>
        /// Occurs when a drag has ended
        /// </summary>
        [Category("Behavior")]
        [Description("Occurs when a drag has ended ")]
        public event DragEndedHandler DragEnded
        {
            add { this.treeElement.DragEnded += value; }
            remove { this.treeElement.DragEnded -= value; }
        }

        /// <summary>
        /// Occurs when a drag is starting
        /// </summary>
        [Category("Behavior")]
        [Description("Occurs when a drag is starting")]
        public event DragStartingHandler DragStarting
        {
            add { this.treeElement.DragStarting += value; }
            remove { this.treeElement.DragStarting -= value; }
        }

        /// <summary>
        /// Occurs when a drag has started
        /// </summary>
        [Category("Behavior")]
        [Description("Occurs when a drag has started")]
        public event DragStartedHandler DragStarted
        {
            add { this.treeElement.DragStarted += value; }
            remove { this.treeElement.DragStarted -= value; }
        }

        /// <summary>
        /// Occurs when drag feedback is needed for a node.
        /// </summary>
        [Category("Behavior")]
        [Description("Occurs when drag feedback is needed for a node.")]
        public event EventHandler<RadTreeViewDragCancelEventArgs> DragOverNode
        {
            add { this.treeElement.DragOverNode += value; }
            remove { this.treeElement.DragOverNode -= value; }
        }

        /// <summary>
        /// Occurs before a tree node is selected.
        /// </summary>
        [Category("Behavior")]
        [Description("Occurs before a node is selected.")]
        public event RadTreeViewCancelEventHandler SelectedNodeChanging
        {
            add { this.treeElement.SelectedNodeChanging += value; }
            remove { this.treeElement.SelectedNodeChanging -= value; }
        }

        /// <summary>
        /// Occurs after the tree node is selected.
        /// <remarks>
        /// For more information about handling events, see also SelectedNodeChanging.
        /// </remarks>
        /// </summary>
        [Category("Behavior")]
        [Description("Occurs after a node is selected.")]
        public event RadTreeViewEventHandler SelectedNodeChanged
        {
            add { this.treeElement.SelectedNodeChanged += value; }
            remove { this.treeElement.SelectedNodeChanged -= value; }
        }

        /// <summary>
        /// Occurs when the user presses a mouse button over a RadTreeNode.
        /// </summary>
        [Description("Occurs when the user presses a mouse button over a RadTreeNode.")]
        [Category(RadDesignCategory.BehaviorCategory)]
        public event RadTreeView.TreeViewMouseEventHandler NodeMouseDown
        {
            add { this.treeElement.NodeMouseDown += value; }
            remove { this.treeElement.NodeMouseDown -= value; }
        }

        /// <summary>
        /// Occurs when the user releases a mouse button over a RadTreeNode.
        /// </summary>
        [Description("Occurs when the user releases a mouse button over a RadTreeNode.")]
        [Category(RadDesignCategory.BehaviorCategory)]
        public event TreeViewMouseEventHandler NodeMouseUp
        {
            add { this.treeElement.NodeMouseUp += value; }
            remove { this.treeElement.NodeMouseUp -= value; }
        }


        /// <summary>
        /// Occurs when the user moves the mouse in the area of a RadTreeNode.
        /// </summary>
        [Description("Occurs when the user moves the mouse in the area of a RadTreeNode.")]
        [Category(RadDesignCategory.BehaviorCategory)]
        public event TreeViewMouseEventHandler NodeMouseMove
        {
            add { this.treeElement.NodeMouseMove += value; }
            remove { this.treeElement.NodeMouseMove -= value; }
        }

        /// <summary>
        /// Occurs when the mouse enters the area of a RadTreeNode.
        /// </summary>
        [Description("Occurs when the mouse enters the area of a RadTreeNode.")]
        [Category(RadDesignCategory.BehaviorCategory)]
        public event TreeViewEventHandler NodeMouseEnter
        {
            add { this.treeElement.NodeMouseEnter += value; }
            remove { this.treeElement.NodeMouseEnter -= value; }
        }

        /// <summary>
        /// Occurs when the mouse leaves the area of a RadTreeNode.
        /// </summary>
        [Description("Occurs when the mouse leaves the area of a RadTreeNode.")]
        [Category(RadDesignCategory.BehaviorCategory)]
        public event TreeViewEventHandler NodeMouseLeave
        {
            add { this.treeElement.NodeMouseLeave += value; }
            remove { this.treeElement.NodeMouseLeave -= value; }
        }

        /// <summary>
        /// Occurs when the mouse hovers over a RadTreeNode.
        /// </summary>
        [Description("Occurs when the mouse hovers over a RadTreeNode.")]
        [Category(RadDesignCategory.BehaviorCategory)]
        public event TreeViewEventHandler NodeMouseHover
        {
            add { this.treeElement.NodeMouseHover += value; }
            remove { this.treeElement.NodeMouseHover -= value; }
        }

        /// <summary>
        ///  Occurs when a mouse button is clicked inside a <see cref="TreeNodeElement"/>
        /// </summary>
        [Description("Occurs when a mouse button is clicked inside a TreeNodeElement")]
        [Category(RadDesignCategory.BehaviorCategory)]
        public event TreeViewEventHandler NodeMouseClick
        {
            add { this.treeElement.NodeMouseClick += value; }
            remove { this.treeElement.NodeMouseClick -= value; }
        }

        /// <summary>
        /// Occurs when a mouse button is double clicked inside a <see cref="TreeNodeElement"/>
        /// </summary>
        [Description("Occurs when a mouse button is double clicked inside a TreeNodeElement")]
        [Category(RadDesignCategory.BehaviorCategory)]
        public event TreeViewEventHandler NodeMouseDoubleClick
        {
            add { this.treeElement.NodeMouseDoubleClick += value; }
            remove { this.treeElement.NodeMouseDoubleClick -= value; }
        }

        /// <summary>
        /// Occurs when the value of the Checked property of a RadTreeNode is changing.
        /// </summary>
        [Description("Occurs when the value of the Checked property of a RadTreeNode is changing.")]
        [Category(RadDesignCategory.BehaviorCategory)]
        public event RadTreeViewCancelEventHandler NodeCheckedChanging
        {
            add { this.treeElement.NodeCheckedChanging += value; }
            remove { this.treeElement.NodeExpandedChanging -= value; }
        }

        /// <summary>
        /// Occurs when the value of the Checked property of a RadTreeNode is changed.
        /// </summary>
        [Description("Occurs when the value of the Checked property of a RadTreeNode is changed.")]
        [Category(RadDesignCategory.BehaviorCategory)]
        public event TreeViewEventHandler NodeCheckedChanged
        {
            add { this.treeElement.NodeCheckedChanged += value; }
            remove { this.treeElement.NodeCheckedChanged -= value; }
        }

        /// <summary>
        /// Occurs before the value of the Expanded property of a tree node is changed.
        /// </summary>
        [Browsable(true), Category(RadDesignCategory.ActionCategory),
        Description("Occurs before the value of the Expanded property of a tree node is changed.")]
        public event RadTreeViewCancelEventHandler NodeExpandedChanging
        {
            add { this.treeElement.NodeExpandedChanging += value; }
            remove { this.treeElement.NodeExpandedChanging -= value; }
        }

        /// <summary>
        /// Occurs after the value of the Expanded property of a tree node is changed.
        /// </summary>
        [Browsable(true), Category(RadDesignCategory.ActionCategory),
        Description("Occurs after the value of the Expanded property of a tree node is changed.")]
        public event TreeViewEventHandler NodeExpandedChanged
        {
            add { this.treeElement.NodeExpandedChanged += value; }
            remove { this.treeElement.NodeExpandedChanged -= value; }
        }

        /// <summary>
        /// Occurs when the Nodes collection requires to be populated in Load-On-Demand mode using LazyTreeNodeProvider.
        /// </summary>
        [Browsable(true), Category(RadDesignCategory.ActionCategory),
        Description(" Occurs when the Nodes collection requires to be populated in Load-On-Demand mode using LazyTreeNodeProvider.")]
        public event NodesNeededEventHandler NodesNeeded
        {
            add { this.treeElement.NodesNeeded += value; }
            remove { this.treeElement.NodesNeeded -= value; }
        }

        /// <summary>
        /// Occurs when the node changes its state and needs to be formatted.
        /// </summary>
        [Category(RadDesignCategory.ActionCategory),
         Description("Occurs when the node changes its state and needs to be formatted.")]
        public event TreeNodeFormattingEventHandler NodeFormatting
        {
            add { this.treeElement.NodeFormatting += value; }
            remove { this.treeElement.NodeFormatting -= value; }
        }

        /// <summary>
        /// Occurs when a new node element is going to be created.
        /// </summary>
        [Category(RadDesignCategory.ActionCategory),
         Description("Occurs when a new node is going to be created.")]
        public event CreateTreeNodeEventHandler CreateNode
        {
            add { this.treeElement.CreateNode += value; }
            remove { this.treeElement.CreateNode -= value; }
        }

        /// <summary>
        /// Occurs when a new node element is going to be created.
        /// </summary>
        [Category(RadDesignCategory.ActionCategory),
         Description("Occurs when a new node element is going to be created.")]
        public event CreateTreeNodeElementEventHandler CreateNodeElement
        {
            add { this.treeElement.CreateNodeElement += value; }
            remove { this.treeElement.CreateNodeElement -= value; }
        }

        /// <summary>
        /// Occurs when opening the context menu.
        /// </summary>
        [Category(RadDesignCategory.ActionCategory),
         Description("Occurs when opening the context menu.")]
        public event TreeViewContextMenuOpeningEventHandler ContextMenuOpening
        {
            add { this.treeElement.ContextMenuOpening += value; }
            remove { this.treeElement.ContextMenuOpening -= value; }
        }

        /// <summary>
        /// Occurs after a node is removed.
        /// </summary>
        [Browsable(true), Category(RadDesignCategory.ActionCategory),
        Description("Occurs after a node is removed.")]
        public event RadTreeViewEventHandler NodeRemoved
        {
            add { this.treeElement.NodeRemoved += value; }
            remove { this.treeElement.NodeRemoved -= value; }
        }

        /// <summary>
        /// Occurs after a node is being added.
        /// </summary>
        [Browsable(true), Category(RadDesignCategory.ActionCategory),
        Description("Occurs after the value of the Expanded property of a tree node is changed.")]
        public event RadTreeViewEventHandler NodeAdded
        {
            add { this.treeElement.NodeAdded += value; }
            remove { this.treeElement.NodeAdded -= value; }
        }

        #endregion

        #region Methods
        /// <summary>
        /// Sets the error.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="radTreeNode">The RAD tree node.</param>
        public void SetError(string text, RadTreeNode radTreeNode)
        {
            this.treeElement.SetError(text, radTreeNode);
        }


        /// <summary>
        /// Creates a new node and adds a node by path. The label of the new node will be the text after the last separator.
        /// </summary>
        /// <param name="path">Where the node should be added.</param>
        /// <returns>The new node if the operation is successful.</returns>
        public RadTreeNode AddNodeByPath(string path)
        {
            return this.treeElement.AddNodeByPath(path);
        }

        /// <summary>
        /// Creates a new node and adds a node by path. The label of the new node will be the text after the last separator.
        /// </summary>
        /// <param name="path">Where the node should be added.</param>
        /// <param name="pathSeparator">The path separator.</param>
        /// <returns>The new node if the operation is successful.</returns>
        public RadTreeNode AddNodeByPath(string path, string pathSeparator)
        {
            return this.treeElement.AddNodeByPath(path, pathSeparator);
        }

        /// <summary>
        /// Gets a node by specifying a path to it.
        /// </summary>
        /// <param name="path">The path to the node.</param>
        /// <param name="pathSeparator">The path separator.</param>
        /// <returns>The node if found.</returns>
        public RadTreeNode GetNodeByPath(string path)
        {
            return this.treeElement.GetNodeByPath(path);
        }


        /// <summary>
        /// Gets a node by specifying a path to it.
        /// </summary>
        /// <param name="path">The path to the node.</param>
        /// <param name="pathSeparator">The path separator.</param>
        /// <returns>The node if found.</returns>
        public RadTreeNode GetNodeByPath(string path, string pathSeparator)
        {
            return this.treeElement.GetNodeByPath(path, pathSeparator);
        }

        /// <summary>
        /// Brings the into view.
        /// </summary>
        /// <param name="node">The node.</param>
        public void BringIntoView(RadTreeNode node)
        {
            this.treeElement.BringIntoView(node);
        }

        /// <summary>
        /// Finds the specified match.
        /// </summary>
        /// <param name="match">The match.</param>
        /// <returns></returns>
        public RadTreeNode Find(Predicate<RadTreeNode> match)
        {
            return this.treeElement.Find(match);
        }

        /// <summary>
        /// Finds the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public RadTreeNode Find(string text)
        {
            return this.treeElement.Find(text);
        }

        /// <summary>
        /// Finds the nodes.
        /// </summary>
        /// <param name="match">The match.</param>
        /// <returns></returns>
        public RadTreeNode[] FindNodes(Predicate<RadTreeNode> match)
        {
            return this.treeElement.FindNodes(match);
        }

        /// <summary>
        /// Finds the nodes.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public RadTreeNode[] FindNodes(string text)
        {
            return this.treeElement.FindNodes(text);
        }

        /// <summary>
        /// Executes the specified command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="settings">The settings.</param>
        /// <returns></returns>
        public object Execute(ICommand command, params object[] settings)
        {
            return this.treeElement.Execute(true, command, settings);
        }


        /// <summary>
        /// Executes the specified command include sub trees.
        /// </summary>
        /// <param name="includeSubTrees">if set to <c>true</c> [include sub trees].</param>
        /// <param name="command">The command.</param>
        /// <param name="settings">The settings.</param>
        /// <returns></returns>
        public object Execute(bool includeSubTrees, ICommand command, params object[] settings)
        {
            return this.treeElement.Execute(command, includeSubTrees, settings);
        }

        /// <summary>
        /// Begins the edit.
        /// </summary>
        public bool BeginEdit()
        {
            return this.treeElement.BeginEdit();
        }

        /// <summary>
        ///  Commits any changes and ends the edit operation on the current cell.
        /// </summary>
        /// <returns></returns>
        public bool EndEdit()
        {
            return this.treeElement.EndEdit();
        }

        /// <summary>
        /// Close the currently active editor and discard changes.
        /// </summary>
        /// <returns></returns>
        public void CancelEdit()
        {
            this.treeElement.CancelEdit();
        }

        /// <summary>
        /// Loads the XML.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="extraTypes">The extra types that will be load</param>
        public void LoadXML(string fileName, params Type[] extraTypes)
        {
            using (StreamReader reader = new StreamReader(fileName))
            {
                this.LoadXMLWithReader(reader, extraTypes);
            }
        }

        /// <summary>
        /// Loads the XML.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="extraTypes">The extra types that will be load</param>
        public void LoadXML(Stream stream, params Type[] extraTypes)
        {
            using (StreamReader reader = new StreamReader(stream))
            {
                this.LoadXMLWithReader(reader, extraTypes);
            }
        }

        /// <summary>
        /// Saves the XML.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="extraTypes">The extra types that will be saved</param>
        public void SaveXML(string fileName, params Type[] extraTypes)
        {
            using (StreamWriter writer = new StreamWriter(fileName))
            {
                this.SaveXMLWithWriter(writer, extraTypes);
            }
        }

        /// <summary>
        /// Saves the XML.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="extraTypes">The extra types that will be saved</param>
        public void SaveXML(Stream stream, params Type[] extraTypes)
        {
            using (StreamWriter writer = new StreamWriter(stream, Encoding.UTF8))
            {
                this.SaveXMLWithWriter(writer, extraTypes);
            }
        }

        /// <summary>
        /// Disables any update of the tree view.
        /// </summary>
        public void BeginUpdate()
        {
            this.TreeViewElement.BeginUpdate();
        }

        /// <summary>
        /// Ends the update.
        /// </summary>
        public void EndUpdate()
        {
            this.TreeViewElement.EndUpdate();
        }

        /// <summary>
        /// Defers the refresh.
        /// </summary>
        /// <returns></returns>
        public virtual IDisposable DeferRefresh()
        {
            return this.TreeViewElement.DeferRefresh();
        }

        /// <summary>
        /// Collapses all the tree nodes.
        /// </summary>
        public void CollapseAll()
        {
            this.TreeViewElement.CollapseAll();
        }

        /// <summary>
        /// Collapses all nodes in a given collection.
        /// </summary>
        /// <param name="nodes">The collection of nodes to be collapsed.</param>
        public void CollapseAll(RadTreeNodeCollection nodes)
        {
            foreach (RadTreeNode node in nodes)
            {
                node.Collapse();
            }
        }

        /// <summary>
        /// Expands all the tree nodes.
        /// </summary>
        public void ExpandAll()
        {
            this.TreeViewElement.ExpandAll();
        }

        /// <summary>
        /// Expands all nodes in a given collection.
        /// </summary>
        /// <param name="nodes">The collection of nodes to be expanded.</param>
        public virtual void ExpandAll(RadTreeNodeCollection nodes)
        {
            foreach (RadTreeNode node in nodes)
            {
                node.Expand();
            }
        }

        /// <summary>
        ///  Retrieves the tree node that is at the specified point.
        /// </summary>
        /// <param name="pt">The System.Drawing.Point to evaluate and retrieve the node from.</param>
        /// <returns>The System.Windows.Forms.TreeNode at the specified point, in tree view (client) coordinates, or null if there is no node at that location.</returns>
        public RadTreeNode GetNodeAt(Point pt)
        {
            return this.GetNodeAt(pt.X, pt.Y);
        }

        /// <summary>
        /// Retrieves the tree node at the point with the specified coordinates.
        /// </summary>
        /// <param name="x">The System.Drawing.Point.X position to evaluate and retrieve the node from.</param>
        /// <param name="y">The System.Drawing.Point.Y position to evaluate and retrieve the node from.</param>
        /// <returns>The System.Windows.Forms.TreeNode at the specified location, in tree view (client) coordinates, or null if there is no node at that location.</returns>
        public RadTreeNode GetNodeAt(int x, int y)
        {
            return this.TreeViewElement.GetNodeAt(x, y);
        }

        /// <summary>
        /// Retrieves the number of tree nodes, optionally including those in all subtrees,  assigned to the tree view control.
        /// </summary>
        /// <param name="includeSubTrees">The number of tree nodes, optionally including those in all subtrees, assigned to the tree view control.</param>
        /// <returns></returns>
        public int GetNodeCount(bool includeSubTrees)
        {
            return this.TreeViewElement.GetNodeCount(includeSubTrees);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            string str = base.ToString();
            if (this.Nodes != null)
            {
                str = str + ", Nodes.Count: " + this.Nodes.Count.ToString(CultureInfo.CurrentCulture);
                if (this.Nodes.Count > 0)
                {
                    str = str + ", Nodes[0]: " + this.Nodes[0].ToString();
                }
            }

            return str;
        }

        public void SelectAll()
        {
            this.treeElement.SelectAll();
        }

        public void ClearSelection()
        {
            this.treeElement.ClearSelection();
        }

        #endregion

        #region Internal

        private void LoadXMLWithReader(TextReader reader, params Type[] extraTypes)
        {
            XmlTreeSerializer serializer = null;

            if (extraTypes == null || extraTypes.Length == 0)
            {
                serializer = new XmlTreeSerializer(typeof(XmlTreeView));
            }
            else
            {
                serializer = new XmlTreeSerializer(typeof(XmlTreeView), extraTypes);
            }

            XmlTreeView xmlTreeView = serializer.Deserialize(reader) as XmlTreeView;
            xmlTreeView.Deserialize(this);
        }

        private void SaveXMLWithWriter(TextWriter writer, params Type[] extraTypes)
        {
            XmlTreeView xmlTreeView = new XmlTreeView(this);
            XmlTreeSerializer serializer = null;

            if (extraTypes == null || extraTypes.Length == 0)
            {
                serializer = new XmlTreeSerializer(typeof(XmlTreeView));
            }
            else
            {
                serializer = new XmlTreeSerializer(typeof(XmlTreeView), extraTypes);
            }

            serializer.Serialize(writer, xmlTreeView);
        }

        protected override Size DefaultSize
        {
            get
            {
                return new Size(150, 250);
            }
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case 0x7b:
                    this.WmContextMenu(ref m);
                    return;
            }

            base.WndProc(ref m);
        }

        private void WmContextMenu(ref Message m)
        {
            Point point;
            int x = NativeMethods.Util.SignedLOWORD(m.LParam);
            int y = NativeMethods.Util.SignedHIWORD(m.LParam);
            if (((int)((long)m.LParam)) == -1)
            {
                point = new Point(this.Width / 2, this.Height / 2);
            }
            else
            {
                point = this.PointToClient(new Point(x, y));
            }

            if (!treeElement.ProcessContextMenu(point))
            {
                ContextMenu menu = this.ContextMenu;
                ContextMenuStrip strip = (menu != null) ? null : this.ContextMenuStrip;
                if ((menu != null) || (strip != null))
                {
                    if (this.ClientRectangle.Contains(point))
                    {
                        if (menu != null)
                        {
                            menu.Show(this, point);
                        }
                        else if (strip != null)
                        {
                            strip.Show(this, point);
                        }

                        return;
                    }
                }
            }

            this.DefWndProc(ref m);
        }

        protected override void OnBindingContextChanged(EventArgs e)
        {
            if (this.BindingContext != null)
            {
                this.TreeViewElement.BindingContext = this.BindingContext;
            }

            base.OnBindingContextChanged(e);
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            this.treeElement.Update(RadTreeViewElement.UpdateActions.StateChanged);
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);

            if (this.ContainsFocus && this.IsEditing)
            {
                return;
            }

            if (!this.ContainsFocus)
            {
                this.TreeViewElement.EndEdit();
            }

            this.treeElement.Update(RadTreeViewElement.UpdateActions.StateChanged);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (this.TreeViewElement.ProcessMouseDown(e))
            {
                return;
            }
            base.OnMouseDown(e);
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            if (this.TreeViewElement.ProcessMouseClick(e))
            {
                return;
            }
            base.OnMouseClick(e);
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            if (this.TreeViewElement.ProcessMouseDoubleClick(e))
            {
                return;
            }
            base.OnMouseDoubleClick(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (this.TreeViewElement.ProcessKeyDown(e))
            {
                return;
            }
            base.OnKeyDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (this.TreeViewElement.ProcessMouseMove(e))
            {
                return;
            }
            base.OnMouseMove(e);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            if (this.TreeViewElement.ProecessMouseEnter(e))
            {
                return;
            }

            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            if (this.TreeViewElement.ProecessMouseLeave(e))
            {
                return;
            }

            base.OnMouseLeave(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (this.TreeViewElement.ProcessMouseUp(e))
            {
                return;
            }
            base.OnMouseUp(e);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            if (this.TreeViewElement.ProcessMouseWheel(e))
            {
                return;
            }
            base.OnMouseWheel(e);
        }

        protected override bool IsInputKey(Keys keyData)
        {
            switch (keyData)
            {
                case Keys.End:
                case Keys.Home:
                case Keys.Left:
                case Keys.Right:
                case Keys.Up:
                case Keys.Down:
                case Keys.Right | Keys.Shift:
                case Keys.Up | Keys.Shift:
                case Keys.Left | Keys.Shift:
                case Keys.Down | Keys.Shift:
                    return true;
            }

            return base.IsInputKey(keyData);
        }

        protected internal override void OnThemeNameChanged(ThemeNameChangedEventArgs e)
        {
            base.OnThemeNameChanged(e);
            this.treeElement.Update(RadTreeViewElement.UpdateActions.Reset);
        }

        #endregion

        #region Legacy

        /// <summary>
        /// Sorts the RdaTreeView Nodes alphabetically.
        /// </summary>
        [Obsolete("This method will be removed in the next version. Please use the SortOrder property instead.")]
        public void Sort()
        {
            this.SortOrder = System.Windows.Forms.SortOrder.Ascending;
        }

        /// <summary>
        /// Gets or sets the color of the drop hint.
        /// </summary>
        /// <remarks>
        /// The drop feedback is a visual cue that assists the user with information where to
        /// drop during the drag and drop operation.
        /// </remarks>
        /// <seealso cref="ShowDropHint">ShowDropHint Property</seealso>
        /// <value>
        /// The default value is
        /// <a href="ms-help://MS.VSCC.v80/MS.MSDN.v80/MS.NETDEVFX.v20.en/cpref8/html/P_System_Drawing_Color_Black.htm">
        /// black</a>.
        /// </value>
        [Category("Behavior"), DefaultValue(typeof(Color), "")]
        [Description("Gets or sets the color of the drop hint.")]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Obsolete("This property will be removed in the next version.")]
        public Color DropFeedbackColor
        {
            get { return this.DropHintColor; }
            set { this.DropHintColor = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether single node expand is enabled.
        /// </summary>
        [Category(RadDesignCategory.BehaviorCategory)]
        [DefaultValue(typeof(ExpandMode), "Multiple")]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ExpandMode ExpandMode
        {
            get { return this.treeElement.ExpandMode; }
            set { this.treeElement.ExpandMode = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether loading on demand is enabled.
        ///  </summary>
        /// <value>The default value is false.</value>
        [Category("Behavior"), DefaultValue(false)]
        [Description("Gets or sets a value indicating whether loading on demand is enabled.")]
        [Obsolete("This property will be removed in the next version. Please use the TreeNodeCollection providers instead.")]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool LoadOnDemand
        {
            get { return false; }
            set { }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the user can select multiple
        /// nodes.
        /// </summary>
        /// <remarks>
        /// To select multiple elements, the user can hold down the CTRL key while clicking
        /// the nodes to select. Consecutive nodes can be selected by clicking the first node to
        /// select and then, while holding down the SHIFT key, clicking the last node to
        /// select.
        /// </remarks>
        /// <value>The default value is false.</value>
        [Category("Behavior"), DefaultValue(false)]
        [Description("Gets or sets a value indicating whether multiple selection is allowed.")]
        [Obsolete("This property will be removed in the next version. Please use the MultiSelect instead.")]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool AllowMultiselect
        {
            get { return this.MultiSelect; }
            set { this.MultiSelect = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether drag and drop operations between
        /// RadTreeView controls is allowed.
        /// </summary>
        /// <seealso cref="AllowDragDrop">AllowDragDrop Property</seealso>
        /// <requirements>
        ///     To allow drag and drop operations the <see cref="AllowDragDrop">AllowDragDrop
        ///     Property</see> must be true.
        /// </requirements>
        /// <value>The default value is true.</value>
        [Category("Behavior"), DefaultValue(false)]
        [Description("Gets or sets a value indicating whether drag and drop between RadTreeView controls is allowed.")]
        [Obsolete("This property will be removed in the next version. Please use the AllowDragDrop instead.")]
        [Browsable(false)]
        public bool AllowDragDropBetweenTreeViews
        {
            get { return this.AllowDragDrop; }
            set { this.AllowDragDrop = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the default context menu is enabled.
        /// </summary>
        /// <value>The default value is false.</value>
        [DefaultValue(false)]
        [Description("Gets or sets a value indicating whether the default context menu is enabled.")]
        public bool AllowDefaultContextMenu
        {
            get { return this.RadContextMenu is TreeViewDefaultContextMenu; }
            set
            {
                if (value && this.RadContextMenu is TreeViewDefaultContextMenu)
                {
                    return;
                }

                if (!value && this.RadContextMenu is TreeViewDefaultContextMenu)
                {
                    this.RadContextMenu = null;
                    return;
                }

                this.RadContextMenu = new TreeViewDefaultContextMenu(this.TreeViewElement);
                base.OnNotifyPropertyChanged("AllowDefaultContextMenu");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether it is allowed to add new nodes from the default context menu.
        /// </summary>
        /// <seealso cref="AllowDeleteInContextMenu">AllowDeleteInContextMenu Property</seealso>
        /// <value>The default value is false.</value>
        [DefaultValue(true)]
        [Description("Gets or sets a value indicating whether it is allowed to add new nodes from the default context menu.")]
        [Obsolete("This property will be removed in the next version. Use the AllowAdd property instead.")]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool AllowAddNewInContextMenu
        {
            get
            {
                return this.AllowAdd;
            }
            set
            {
                this.AllowAdd = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether it is allowed to delete nodes from the default context menu.
        /// </summary>
        /// <seealso cref="AllowAddNewInContextMenu">AllowAddNewInContextMenu Property</seealso>
        /// <value>The default value is false.</value>
        [DefaultValue(false)]
        [Description("Gets or sets a value indicating whether it is allowed to delete nodes from the default context menu.")]
        [Obsolete("This property will be removed in the next version. Use the AllowRemove property instead.")]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool AllowDeleteInContextMenu
        {
            get
            {
                return AllowRemove;
            }
            set
            {
                this.AllowRemove = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether different visual style will be applied to
        /// nodes in the selected path.
        /// </summary>
        /// <remarks>
        /// 	<para>
        ///         If true a different style is applied to the nodes that take part in the value
        ///         of the <see cref="RadTreeNode.FullPath">FullPath Property
        ///         (Telerik.WinControls.UI.RadTreeNode)</see>.
        ///     </para>
        /// 	<para>The style could be set up and modified through the themes.</para>
        /// </remarks>
        /// <value>The default value is false.</value>
        [DefaultValue(false)]
        [Description("Gets or sets a value indicating whether different style can be applied to nodes in the selected path.")]
        [Obsolete("This property will be removed in the next version.")]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool AllowSelectedPath
        {
            get { return false; }
            set { }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the default selection style is used.
        /// </summary>
        /// <value>The default value is false.</value>
        [Category("Appearance"), DefaultValue(false)]
        [Description("Gets or sets a value indicating whether the default selection style is used.")]
        [Obsolete("This property will be removed in the next version.")]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool UseDefaultSelection
        {
            get { return false; }
            set { }
        }

        /// <summary>
        /// Gets or sets a value indicating whether drop feedback is shown while dragging.
        /// </summary>
        /// <seealso cref="DropHintColor">DropFeedbackColor Property</seealso>
        /// <value>The default value is true.</value>
        [Category("Behavior"), DefaultValue(true)]
        [Description("Gets or sets a value indicating whether drag feedback is shown while dragging.")]
        [Obsolete("This property will be removed in the next version.")]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool ShowFeedbackForm
        {
            get { return this.ShowDragHint; }
            set { this.ShowDragHint = value; }
        }


        /// <summary>
        /// Gets or sets the text of the root node when using relations. 
        /// </summary>
        /// <seealso cref="RelationBindings">RelationBindings Property</seealso>
        /// <value>The default value is null.</value>
        [Description("Gets or sets the text of the root node when using relations.")]
        [Category(RadDesignCategory.DataCategory), DefaultValue(null)]
        [Obsolete("This property will be removed in the next version.")]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string RootRelationDisplayName
        {
            get { return null; }
            set { }
        }

        /// <summary>
        /// Gets or sets the parent ID field when binding to a self referencing table.
        /// </summary>
        /// <value>The default value is null.</value>
        [Description("Gets or sets the parent ID field when binding to a self referencing table.")]
        [Category(RadDesignCategory.DataCategory), DefaultValue(null)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        [Obsolete("This property will be removed in the next version. Use the ParentMember property instead.")]
        public string ParentIDMember
        {
            get { return this.ParentMember; }
            set { this.ParentMember = value; }
        }



        /// <summary>
        /// Executes a command over an entire subtree starting with the specified nodes.
        /// </summary>
        /// <param name="nodes">The nodes form which the execuition starts.</param>
        /// <param name="level">The level of nodes over which to execute the command. If -1 the entire subtree is traversed.</param>
        /// <param name="command">The command to execute.</param>
        /// <param name="settings">Parameters to pass the command prior to execution.</param>
        /// <returns>The first result from the batch execution.</returns>
        [Obsolete("This method will be removed in the next version. Please use Execute method of RadTreeView or RadTreeNode instead.")]
        public static object ExecuteScalarCommand(RadTreeNodeCollection nodes, int level, ICommand command, params object[] settings)
        {
            if (nodes == null || nodes.Count == 0)
            {
                return null;
            }

            object result = null;
            for (int i = 0; i < nodes.Count; i++)
            {
                result = ExecuteScalarCommand(nodes[i], level, command, settings);
                if (result != null)
                {
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// Executes a command over an entire subtree starting with the specified nodes.
        /// </summary>
        /// <param name="nodes">The nodes form which the execuition starts.</param>
        /// <param name="level">The level of nodes over which to execute the command. If -1 the entire subtree is traversed.</param>
        /// <param name="command">The command to execute.</param>
        /// <param name="settings">Parameters to pass the command prior to execution.</param>
        /// <returns>The results from the batch execution.</returns>
        public static List<object> ExecuteBatchCommand(RadTreeNodeCollection nodes, int level, ICommand command, params object[] settings)
        {
            if (nodes == null || nodes.Count == 0)
            {
                return new List<object>();
            }

            List<object> result = new List<object>();
            for (int i = 0; i < nodes.Count; i++)
            {
                result.AddRange(ExecuteBatchCommand(nodes[i], level, command, settings));
            }

            return result;
        }

        /// <summary>
        /// Executes a command over an entire subtree starting with the specified node.
        /// </summary>
        /// <param name="node">The node form which the execuition starts.</param>
        /// <param name="level">The level of nodes over which to execute the command. If -1 the entire subtree is traversed.</param>
        /// <param name="command">The command to execute.</param>
        /// <param name="settings">Parameters to pass the command prior to execution.</param>
        /// <returns>The first result from the batch execution.</returns>
        public static object ExecuteScalarCommand(RadTreeNode node, int level, ICommand command, params object[] settings)
        {
            if (node == null)
            {
                return null;
            }

            object result = null;
            Queue<RadTreeNode> nodeQueue = new Queue<RadTreeNode>();

            nodeQueue.Enqueue(node);
            while (nodeQueue.Count > 0)
            {
                RadTreeNode temp = nodeQueue.Dequeue();
                if (level == -1 || temp.Level == level)
                {
                    result = command.Execute(temp, settings);
                    if (result != null)
                    {
                        break;
                    }
                    if (temp.Level == level)
                    {
                        continue;
                    }
                }
                for (int i = 0; i < temp.Nodes.Count; i++)
                {
                    RadTreeNode childNode = temp.Nodes[i];
                    nodeQueue.Enqueue(childNode);
                }
            }

            return result;
        }

        /// <summary>
        /// Executes a command over an entire subtree starting with the specified node.
        /// </summary>
        /// <param name="node">The node form which the execuition starts.</param>
        /// <param name="level">The level of nodes over which to execute the command. If -1 the entire subtree is traversed.</param>
        /// <param name="command">The command to execute.</param>
        /// <param name="settings">Parameters to pass the command prior to execution.</param>
        /// <returns>The results from the batch execution.</returns>
        public static List<object> ExecuteBatchCommand(RadTreeNode node, int level, ICommand command, params object[] settings)
        {
            if (node == null)
            {
                return new List<object>();
            }

            List<object> results = new List<object>();
            Queue<RadTreeNode> nodeQueue = new Queue<RadTreeNode>();

            nodeQueue.Enqueue(node);
            while (nodeQueue.Count > 0)
            {
                RadTreeNode temp = nodeQueue.Dequeue();
                if (level == -1 || temp.Level == level)
                {
                    object result = command.Execute(temp, settings);
                    if (result != null)
                    {
                        results.Add(result);
                    }
                    if (temp.Level == level)
                    {
                        continue;
                    }
                }

                for (int i = 0; i < temp.Nodes.Count; i++)
                {
                    RadTreeNode childNode = temp.Nodes[i];
                    nodeQueue.Enqueue(childNode);
                }
            }

            return results;
        }

        /// <summary>
        /// Sets the ImageInFill of all nodes.
        /// </summary>
        /// <param name="value">The new value of the ImageInFill property of nodes.</param>
        [Obsolete("This method will be removed in the next version. Please use Execute method of RadTreeView or RadTreeNode instead.")]
        public void SetImagesInFill(bool value)
        {
            this.BeginUpdate();
            ExecuteBatchCommand(this.Nodes, -1, RadElement.SetPropertyValueCommand, value, "ImageInFill");
            this.EndUpdate();
        }

        /// <summary>
        /// Checks or uncheks nodes at a given level.
        /// </summary>
        /// <param name="value">A boolean value indicating wether nodes should be checked or unchecked.</param>
        /// <param name="level">The level of nodes.</param>
        [Obsolete("This method will be removed in the next version. Please use Execute method of RadTreeView or RadTreeNode instead.")]
        public void SetCheckBoxesVisibilityOnLevel(bool value, int level)
        {
            this.BeginUpdate();
            ExecuteBatchCommand(this.Nodes, level, new SetCheckBoxCommand(), value, level);
            this.EndUpdate();
        }

        #endregion

        #region MSAA

        protected override AccessibleObject CreateAccessibilityInstance()
        {
            return new RadTreeViewAccessibleObject(this);
        }

        #endregion
    }
}

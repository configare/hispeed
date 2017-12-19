using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Globalization;
using System.Windows.Forms;
using Telerik.Data.Expressions;
using Telerik.WinControls.Commands;
using Telerik.WinControls.Data;
using Telerik.WinControls.Enumerations;
using Telerik.WinControls.Themes;
using Telerik.WinControls.UI.StateManagers;
using System.Diagnostics;

namespace Telerik.WinControls.UI
{
    public class RadTreeViewElement : VirtualizedScrollPanel<RadTreeNode, TreeNodeElement>, IDataItemSource
    {
        #region Nested

        public enum UpdateActions
        {
            Reset,
            Resume,
            ItemAdded,
            ItemRemoved,
            ItemMoved,
            ItemEdited,
            ExpandedChanged,
            ExpandedChangedUsingAnimation,
            StateChanged,
            SortChanged
        }

        internal class RootTreeNode : RadTreeNode
        {
            public RootTreeNode(RadTreeViewElement treeView)
            {
                this.Text = "RootNode";
                this.TreeViewElement = treeView;
                this.Expanded = true;
            }
        }

        internal TreeNodeDescriptor RootDescriptor
        {
            get
            {
                if (this.boundDescriptors.Count == 0)
                {
                    this.boundDescriptors.Add(new TreeNodeDescriptor());
                }

                return this.boundDescriptors[0];
            }
        }

        private class DeferHelper : IDisposable
        {
            private RadTreeViewElement treeView;

            public DeferHelper(RadTreeViewElement treeView)
            {
                this.treeView = treeView;
            }

            public void Dispose()
            {
                if (this.treeView != null)
                {
                    this.treeView.EndUpdate();
                    this.treeView = null;
                }
            }
        }

        #endregion

        #region Fields

        private int updateSuspendedCount = 0;
        private int updateCurrentNodeChanged = 0;
        private int updateSelectionChanged = 0;

        private bool multiSelect = false;
        private bool hideSelection = true;
        private bool hotTracking = true;
        private bool checkBoxes = false;
        private bool allowEdit = false;
        private bool allowAdd = false;
        private bool allowRemove = false;
        private bool showNodeToolTips = false;
        private bool allowDragDrop = false;
        private bool triStateMode = false;
        private bool allowAlternatingRowColor = false;
        private bool lazyMode = true;

        private int firstVisibleIndex = -1;
        private int defaultImageIndex = -1;

        private string valueMember = string.Empty;
        private string displayMember = string.Empty;
        private string childMember = string.Empty;
        private string parentMember = string.Empty;
        private string defaultImageKey = string.Empty;
        private string pathSeparator;
        private object search;

        private IInputEditor activeEditor;
        private RadTreeNode root, selected, anchorPosition;
        private RadContextMenu contextMenu;
        private TreeViewTraverser traverser;
        private RadListSource<RadTreeNode> listSource;
        private BindingContext bindingContext;
        private FilterDescriptorCollection filterDescriptors;
        private TreeNodeProvider treeNodeProvider;
        private SelectedTreeNodeCollection selectedNodeCollection;
        private TreeViewDragDropService dragDropService;
        private RelationBindingCollection relationBinding;
        private List<TreeNodeDescriptor> boundDescriptors;
        private ToggleMode toggleMode = ToggleMode.DoubleClick;
        private bool pendingScrollerUpdates;
        private int calculatedHScrollbarMaximum = -1;
        private Telerik.WinControls.UI.ExpandMode expandMode = ExpandMode.Multiple;
        private Dictionary<Type, IInputEditor> cachedEditors = new Dictionary<Type, IInputEditor>();
        private object cachedOldValue;
        private int anchorIndex = -1;
        private Point mouseDownLocation;
        private Point mouseMoveLocation;
        private bool enterEditMode = false;
        private bool enableAutoExpand = true;
        private bool performSelectionOnMouseUp = false;
        internal RadTreeNode draggedNode = null;
        private bool resetSizes = false;
        private bool autoCheckChildNodes = true;
        private UpdateActions resumeAction = UpdateActions.Resume;
        private ScrollState horizontalScrollState;
        private Timer scrollTimer;
        private Timer expandTimer;
        private bool doNotStartEditingOnMouseUp;
        private Timer mouseUpTimer = new Timer();
        private RadTreeNode lastClickedNode;
        private CheckedTreeNodeCollection checkedNodeCollection;

        #endregion

        #region Constructors & Initialization

        static RadTreeViewElement()
        {
            ItemStateManagerFactoryRegistry.AddStateManagerFactory(new TreeViewElementStateManager(), typeof(RadTreeViewElement));
            new Themes.ControlDefault.TreeView().DeserializeTheme();
        }

        public RadTreeViewElement()
        {
            this.boundDescriptors = new List<TreeNodeDescriptor>();
            this.boundDescriptors.Add(new TreeNodeDescriptor());

            this.listSource = new RadListSource<RadTreeNode>(this, null);
            this.listSource.CollectionView.CanFilter = true;
            this.listSource.CollectionView.CanSort = true;
            this.listSource.CollectionView.CanGroup = false;
            this.listSource.CollectionView.Comparer = new TreeNodeComparer(this);
            this.listSource.CollectionChanged += new NotifyCollectionChangedEventHandler(listSource_CollectionChanged);
            this.listSource.CollectionView.SortDescriptors.CollectionChanged += new NotifyCollectionChangedEventHandler(SortDescriptors_CollectionChanged);
            this.listSource.CollectionView.Filter = this.PerformExpressionFilter;
            this.filterDescriptors = new TreeViewFilterDescriptorCollection();
            this.filterDescriptors.CollectionChanged += new NotifyCollectionChangedEventHandler(filterDescriptors_CollectionChanged);


            this.root = new RootTreeNode(this);

            this.traverser = new TreeViewTraverser(this);
            this.Scroller.ScrollMode = ItemScrollerScrollModes.Smooth;
            this.Scroller.Traverser = this.traverser;

            this.NotifyParentOnMouseInput = true;
            this.pathSeparator = @"\";

            this.selectedNodeCollection = new SelectedTreeNodeCollection(this);
            this.checkedNodeCollection = new CheckedTreeNodeCollection(this.root);
            this.dragDropService = this.CreateDragDropService();

            this.expandTimer = new Timer();
            this.expandTimer.Interval = 500;
            this.expandTimer.Tick += new EventHandler(ExpandTimer_Tick);

            this.scrollTimer = new Timer();
            this.scrollTimer.Interval = 20;
            this.scrollTimer.Tick += new EventHandler(ScrollTimer_Tick);

            this.mouseUpTimer = new Timer();
            this.mouseUpTimer.Interval = SystemInformation.DoubleClickTime + 10;
            this.mouseUpTimer.Tick += new EventHandler(mouseUpTimer_Tick);

            this.Scroller.ScrollerUpdated += new EventHandler(Scroller_ScrollerUpdated);
        }

        private void SortDescriptors_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            //Stopwatch watch = Stopwatch.StartNew();

            this.Update(UpdateActions.SortChanged);
            
            //watch.Stop();
            //Console.WriteLine("Elapsed time: {0}", watch.ElapsedMilliseconds);
        }

        protected override void InitializeFields()
        {
            base.InitializeFields();
            this.BackColor = Color.White;
            this.GradientStyle = GradientStyles.Solid;
            this.DrawFill = true;
            this.AllowDrop = true;
        }

        protected override void CreateChildElements()
        {
            base.CreateChildElements();
            this.FitItemsToSize = this.FullRowSelect;
        }

        protected override VirtualizedStackContainer<RadTreeNode> CreateViewElement()
        {
            return new RadTreeViewVirtualizedContainer();
        }

        protected override void DisposeManagedResources()
        {
            if (this.expandTimer != null)
            {
                this.expandTimer.Stop();
                this.expandTimer.Tick -= new EventHandler(ExpandTimer_Tick);
                this.expandTimer.Dispose();
                this.expandTimer = null;
            }

            if (this.mouseUpTimer != null)
            {
                this.mouseUpTimer.Stop();
                this.mouseUpTimer.Tick -= new EventHandler(mouseUpTimer_Tick);
                this.mouseUpTimer.Dispose();
                this.mouseUpTimer = null;
            }

            if (this.scrollTimer != null)
            {
                this.scrollTimer.Stop();
                this.scrollTimer.Tick -= new EventHandler(ScrollTimer_Tick);
                this.scrollTimer.Dispose();
                this.scrollTimer = null;

            }

            this.Scroller.ScrollerUpdated -= new EventHandler(Scroller_ScrollerUpdated);

            foreach (KeyValuePair<Type, IInputEditor> keyValuePairEditor in this.cachedEditors)
            {
                IInputEditor editor = keyValuePairEditor.Value;
                IDisposable disposable = null;
                BaseInputEditor baseInputEditor = editor as BaseInputEditor;
                if (baseInputEditor != null)
                {
                    disposable = baseInputEditor.EditorElement as IDisposable;
                    if (disposable != null)
                    {
                        disposable.Dispose();
                    }
                }

                disposable = editor as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }
            this.cachedEditors.Clear();

            base.DisposeManagedResources();
        }

        #endregion

        #region Events


        /// <summary>
        /// Occurs when [data error].
        /// </summary>
        public event TreeNodeDataErrorEventHandler DataError;

        /// <summary>
        /// Raises the <see cref="E:DataError"/> event.
        /// </summary>
        /// <param name="e">The <see cref="Telerik.WinControls.UI.TreeNodeDataErrorEventArgs"/> instance containing the event data.</param>
        protected internal virtual void OnDataError(TreeNodeDataErrorEventArgs e)
        {
            TreeNodeDataErrorEventHandler handler = DataError;
            if (handler != null)
            {
                handler(this, e);
            }
            else
            {
                RadMessageBox.Show(e.ErrorText, "Data Error", MessageBoxButtons.OK, RadMessageIcon.Error);
            }
        }

        /// <summary>
        /// Occurs when [binding context changed].
        /// </summary>
        public event EventHandler BindingContextChanged;

        protected virtual void OnBindingContextChanged(EventArgs e)
        {
            EventHandler bindingContextChanged = this.BindingContextChanged;

            if (bindingContextChanged != null)
            {
                bindingContextChanged(this, e);
            }
        }

        /// <summary>
        /// Occurs when <see cref="TreeNodeElement"/> is formatting
        /// </summary>
        public event TreeNodeFormattingEventHandler NodeFormatting;

        protected internal virtual void OnNodeFormatting(TreeNodeFormattingEventArgs e)
        {
            TreeNodeFormattingEventHandler handler = NodeFormatting;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Occurs when <see cref="TreeNodeElement"/> is created.
        /// </summary>
        public event CreateTreeNodeElementEventHandler CreateNodeElement;

        protected internal virtual void OnCreateNodeElement(CreateTreeNodeElementEventArgs e)
        {
            CreateTreeNodeElementEventHandler handler = CreateNodeElement;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Occurs when <see cref="RadTreeNode"/> is created.
        /// </summary>
        public event CreateTreeNodeEventHandler CreateNode;

        protected virtual void OnCreateNode(CreateTreeNodeEventArgs e)
        {
            CreateTreeNodeEventHandler handler = CreateNode;
            if (handler != null)
            {
                handler(this, e);
            }
        }


        /// <summary>
        /// Occurs when <see cref="TreeNodeElement"/> is mouse down.
        /// </summary>
        public event RadTreeView.TreeViewMouseEventHandler NodeMouseDown;

        protected internal virtual void OnNodeMouseDown(RadTreeViewMouseEventArgs e)
        {
            RadTreeView.TreeViewMouseEventHandler handler = this.NodeMouseDown;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Occurs when <see cref="TreeNodeElement"/> is mouse up.
        /// </summary>
        public event RadTreeView.TreeViewMouseEventHandler NodeMouseUp;

        protected internal virtual void OnNodeMouseUp(RadTreeViewMouseEventArgs e)
        {
            RadTreeView.TreeViewMouseEventHandler handler = this.NodeMouseUp;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Occurs when mouse is move over a <see cref="TreeNodeElement"/>.
        /// </summary>
        public event RadTreeView.TreeViewMouseEventHandler NodeMouseMove;

        protected internal virtual void OnNodeMouseMove(RadTreeViewMouseEventArgs e)
        {
            RadTreeView.TreeViewMouseEventHandler handler = this.NodeMouseMove;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Occurs when mouse enters a <see cref="TreeNodeElement"/>
        /// </summary>
        public event RadTreeView.TreeViewEventHandler NodeMouseEnter;

        protected internal virtual void OnNodeMouseEnter(RadTreeViewEventArgs e)
        {
            RadTreeView.TreeViewEventHandler handler = this.NodeMouseEnter;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Occurs when mouse leaves a <see cref="TreeNodeElement"/>
        /// </summary>
        public event RadTreeView.TreeViewEventHandler NodeMouseLeave;

        protected internal virtual void OnNodeMouseLeave(RadTreeViewEventArgs e)
        {
            RadTreeView.TreeViewEventHandler handler = this.NodeMouseLeave;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Occurs when a mouse button is clicked inside a <see cref="TreeNodeElement"/>
        /// </summary>
        public event RadTreeView.TreeViewEventHandler NodeMouseClick;

        protected internal virtual void OnNodeMouseClick(RadTreeViewEventArgs e)
        {
            RadTreeView.TreeViewEventHandler handler = this.NodeMouseClick;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Occurs when a mouse button is double clicked inside a <see cref="TreeNodeElement"/>
        /// </summary>
        public event RadTreeView.TreeViewEventHandler NodeMouseDoubleClick;

        protected internal virtual void OnNodeMouseDoubleClick(RadTreeViewEventArgs e)
        {
            RadTreeView.TreeViewEventHandler handler = this.NodeMouseDoubleClick;

            if (handler != null)
            {
                handler(this, e);
            }
        }


        /// <summary>
        /// Occurs when <see cref="TreeNodeElement"/> is hovered.
        /// </summary>
        public event RadTreeView.TreeViewEventHandler NodeMouseHover;

        protected internal virtual void OnNodeMouseHover(RadTreeViewEventArgs e)
        {
            RadTreeView.TreeViewEventHandler handler = this.NodeMouseHover;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Occurs when node's checked state is changing.
        /// </summary>
        public event RadTreeView.RadTreeViewCancelEventHandler NodeCheckedChanging;

        protected virtual void OnNodeCheckedChanging(RadTreeViewCancelEventArgs e)
        {
            RadTreeView.RadTreeViewCancelEventHandler handler = this.NodeCheckedChanging;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected internal bool OnNodeCheckedChanging(RadTreeNode node)
        {
            RadTreeViewCancelEventArgs e = new RadTreeViewCancelEventArgs(node);
            this.OnNodeCheckedChanging(e);
            return !e.Cancel;
        }

        /// <summary>
        /// Occurs when node's checked state is changed.
        /// </summary>
        public event RadTreeView.TreeViewEventHandler NodeCheckedChanged;

        protected virtual void OnNodeCheckedChanged(RadTreeViewEventArgs e)
        {
            RadTreeView.TreeViewEventHandler handler = this.NodeCheckedChanged;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected internal void OnNodeCheckedChanged(RadTreeNode node)
        {
            RadTreeViewEventArgs e = new RadTreeViewEventArgs(node);
            this.OnNodeCheckedChanged(e);
        }

        /// <summary>
        /// Occurs when node is expanding.
        /// </summary>
        public event RadTreeView.RadTreeViewCancelEventHandler NodeExpandedChanging;

        protected internal bool OnNodeExpandedChanging(RadTreeNode node)
        {
            RadTreeViewCancelEventArgs args = new RadTreeViewCancelEventArgs(node);
            //args.Cancel = !node.Expanded && node.Nodes.Count == 0;
            this.OnNodeExpandedChanging(args);
            return !args.Cancel;
        }

        protected virtual void OnNodeExpandedChanging(RadTreeViewCancelEventArgs e)
        {
            RadTreeView.RadTreeViewCancelEventHandler handler = this.NodeExpandedChanging;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Occurs when node has been expanded.
        /// </summary>
        public event RadTreeView.TreeViewEventHandler NodeExpandedChanged;

        protected internal virtual void OnNodeExpandedChanged(RadTreeViewEventArgs e)
        {
            RadTreeView.TreeViewEventHandler handler = this.NodeExpandedChanged;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Occurs when the selected node is changing
        /// </summary>
        public event RadTreeView.RadTreeViewCancelEventHandler SelectedNodeChanging;

        protected virtual void OnSelectedNodeChanging(RadTreeViewCancelEventArgs args)
        {
            if (IsEditing)
            {
                EndEdit();
            }
            RadTreeView.RadTreeViewCancelEventHandler handler = SelectedNodeChanging;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        /// <summary>
        /// Occurs when selected node has been changed.
        /// </summary>
        public event RadTreeView.RadTreeViewEventHandler SelectedNodeChanged;

        protected virtual void OnSelectedNodeChanged(RadTreeViewEventArgs args)
        {
            RadTreeView.RadTreeViewEventHandler handler = SelectedNodeChanged;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        /// <summary>
        /// Occurs when editor is required.
        /// </summary>
        public event RadTreeView.EditorRequiredHandler EditorRequired;

        protected virtual void OnEditorRequired(TreeNodeEditorRequiredEventArgs e)
        {
            RadTreeView.EditorRequiredHandler handler = EditorRequired;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Occurs when editing is started.
        /// </summary>
        public event TreeNodeEditingEventHandler Editing;

        protected virtual void OnEditing(TreeNodeEditingEventArgs e)
        {
            TreeNodeEditingEventHandler handler = Editing;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Occurs when editor is initialized.
        /// </summary>
        public event TreeNodeEditorInitializedEventHandler EditorInitialized;

        protected virtual void OnEditorInitialized(TreeNodeEditorInitializedEventArgs e)
        {
            TreeNodeEditorInitializedEventHandler handler = EditorInitialized;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Occurs when editing has been finished.
        /// </summary>
        public event TreeNodeEditedEventHandler Edited;

        protected virtual void OnEdited(TreeNodeEditedEventArgs e)
        {
            TreeNodeEditedEventHandler handler = Edited;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Occurs when node's value is changing.
        /// </summary>
        public event TreeNodeValueChangingEventHandler ValueChanging;

        protected virtual void OnValueChanging(TreeNodeValueChangingEventArgs e)
        {
            TreeNodeValueChangingEventHandler handler = ValueChanging;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Occurs when node's value has been changed.
        /// </summary>
        public event TreeNodeValueChangedEventHandler ValueChanged;

        protected virtual void OnValueChanged(TreeNodeValueChangedEventArgs e)
        {
            TreeNodeValueChangedEventHandler handler = ValueChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Occurs when node's value is validating.
        /// </summary>
        public event TreeNodeValidatingEventHandler ValueValidating;

        protected virtual void OnValueValidating(TreeNodeValidatingEventArgs e)
        {
            TreeNodeValidatingEventHandler handler = ValueValidating;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Occurs when validation error occurs by canceling the ValueValidating event.
        /// </summary>
        public event EventHandler ValidationError;

        protected virtual void OnValidationError(EventArgs e)
        {
            EventHandler handler = this.ValidationError;

            if (handler != null)
            {
                handler(this, e);
            }
        }


        /// <summary>
        /// Occurs when the user begins dragging an item.
        /// </summary>
        public event RadTreeView.ItemDragHandler ItemDrag;

        protected internal virtual void OnItemDrag(RadTreeViewEventArgs e)
        {
            RadTreeView.ItemDragHandler handler = this.ItemDrag;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Occurs when a drag is starting
        /// </summary>
        public event RadTreeView.DragStartingHandler DragStarting;

        protected internal virtual void OnDragStarting(RadTreeViewDragCancelEventArgs e)
        {
            RadTreeView.DragStartingHandler handler = this.DragStarting;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Occurs when a drag has started
        /// </summary>
        public event RadTreeView.DragStartedHandler DragStarted;

        protected internal virtual void OnDragStarted(RadTreeViewDragEventArgs e)
        {
            doNotStartEditingOnMouseUp = true;
            mouseUpTimer.Stop();

            RadTreeView.DragStartedHandler handler = this.DragStarted;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Occurs when a drag is ending 
        /// </summary>
        public event RadTreeView.DragEndingHandler DragEnding;

        protected internal virtual void OnDragEnding(RadTreeViewDragCancelEventArgs e)
        {
            RadTreeView.DragEndingHandler handler = this.DragEnding;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Occurs when a drag has ended
        /// </summary>
        public event RadTreeView.DragEndedHandler DragEnded;

        protected internal virtual void OnDragEnded(RadTreeViewDragEventArgs e)
        {
            RadTreeView.DragEndedHandler handler = this.DragEnded;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Occurs when drag feedback is needed for a node.
        /// </summary>
        public event EventHandler<RadTreeViewDragCancelEventArgs> DragOverNode;

        protected internal virtual void OnDragOverNode(RadTreeViewDragCancelEventArgs e)
        {
            EventHandler<RadTreeViewDragCancelEventArgs> handler = this.DragOverNode;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Occurs when nodes are needed in load on demand hierarchy
        /// </summary>
        public event NodesNeededEventHandler NodesNeeded;

        protected internal virtual void OnNodesNeeded(NodesNeededEventArgs e)
        {
            if (this.NodesNeeded != null)
            {
                this.NodesNeeded(this, e);
            }
        }

        public event TreeViewContextMenuOpeningEventHandler ContextMenuOpening;

        protected virtual void OnContextMenuOpening(TreeViewContextMenuOpeningEventArgs e)
        {
            TreeViewContextMenuOpeningEventHandler handler = ContextMenuOpening;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Occurs after a node is removed.
        /// </summary>
        public event RadTreeView.RadTreeViewEventHandler NodeRemoved;

        protected internal virtual void OnNodeRemoved(RadTreeViewEventArgs e)
        {
            if (this.NodeRemoved != null)
            {
                this.NodeRemoved(this, e);
            }
        }

        /// <summary>
        /// Occurs after a node is being added.
        /// </summary>
        public event RadTreeView.RadTreeViewEventHandler NodeAdded;

        protected internal virtual void OnNodeAdded(RadTreeViewEventArgs e)
        {
            if (this.NodeAdded != null)
            {
                this.NodeAdded(this, e);
            }
        }

        #endregion

        #region Dependency properties

        public static RadProperty ItemDropHintProperty = RadProperty.Register(
            "ItemDropHint", typeof(RadImageShape), typeof(RadTreeViewElement),
                new RadElementPropertyMetadata(null, ElementPropertyOptions.AffectsDisplay));

        public static RadProperty ShowLinesProperty = RadProperty.Register(
            "ShowLines", typeof(bool), typeof(RadTreeViewElement),
                new RadElementPropertyMetadata(false, ElementPropertyOptions.AffectsDisplay));

        public static RadProperty ShowRootLinesProperty = RadProperty.Register(
            "ShowRootLines", typeof(bool), typeof(RadTreeViewElement),
                new RadElementPropertyMetadata(true, ElementPropertyOptions.AffectsDisplay));

        public static RadProperty ShowExpandCollapseProperty = RadProperty.Register(
            "ShowExpandCollapse", typeof(bool), typeof(RadTreeViewElement),
                new RadElementPropertyMetadata(true, ElementPropertyOptions.AffectsDisplay));

        public static RadProperty LineColorProperty = RadProperty.Register(
            "LineColor", typeof(Color), typeof(RadTreeViewElement),
                new RadElementPropertyMetadata(Color.Gray, ElementPropertyOptions.AffectsDisplay));

        public static RadProperty ExpandImageProperty = RadProperty.Register(
            "ExpandImage", typeof(Image), typeof(RadTreeViewElement), new RadElementPropertyMetadata(
                null, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

        public static RadProperty CollapseImageProperty = RadProperty.Register(
            "CollapseImage", typeof(Image), typeof(RadTreeViewElement), new RadElementPropertyMetadata(
                null, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

        public static RadProperty HoveredExpandImageProperty = RadProperty.Register(
            "HoveredExpandImage", typeof(Image), typeof(RadTreeViewElement), new RadElementPropertyMetadata(
                null, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

        public static RadProperty HoveredCollapseImageProperty = RadProperty.Register(
            "HoveredCollapseImage", typeof(Image), typeof(RadTreeViewElement), new RadElementPropertyMetadata(
                null, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

        public static RadProperty LineStyleProperty = RadProperty.Register(
            "LineStyle", typeof(TreeLineStyle), typeof(RadTreeViewElement), new RadElementPropertyMetadata(
                TreeLineStyle.Dot, ElementPropertyOptions.AffectsDisplay));

        public static RadProperty NodeSpacingProperty = RadProperty.Register(
            "NodeSpacing", typeof(int), typeof(RadTreeViewElement), new RadElementPropertyMetadata(
                0, ElementPropertyOptions.AffectsDisplay | ElementPropertyOptions.AffectsArrange));

        public static RadProperty FullRowSelectProperty = RadProperty.Register(
            "FullRowSelect", typeof(bool), typeof(RadTreeViewElement), new RadElementPropertyMetadata(
                true, ElementPropertyOptions.AffectsDisplay));

        public static RadProperty AllowArbitraryItemHeightProperty = RadProperty.Register(
            "AllowArbitraryItemHeight", typeof(bool), typeof(RadTreeViewElement), new RadElementPropertyMetadata(
                false, ElementPropertyOptions.None));

        public static RadProperty ItemHeightProperty = RadProperty.Register(
                "ItemHeight", typeof(int), typeof(RadTreeViewElement),
                new RadElementPropertyMetadata(20, ElementPropertyOptions.AffectsDisplay | ElementPropertyOptions.AffectsMeasure));

        public static RadProperty TreeIndentProperty = RadProperty.Register(
            "TreeIndent", typeof(int), typeof(RadTreeViewElement), new RadElementPropertyMetadata(
                20, ElementPropertyOptions.AffectsDisplay | ElementPropertyOptions.AffectsArrange));

        public static RadProperty AlternatingRowColorProperty = RadProperty.Register(
            "AlternatingRowColor", typeof(Color), typeof(RadTreeViewElement), new RadElementPropertyMetadata(
                Color.FromArgb(246, 251, 255), ElementPropertyOptions.AffectsDisplay));

        public static RadProperty ExpandAnimationProperty = RadProperty.Register(
            "ExpandAnimation", typeof(ExpandAnimation), typeof(RadTreeViewElement), new RadElementPropertyMetadata(ExpandAnimation.Opacity));

        public static RadProperty PlusMinusAnimationStepProperty = RadProperty.Register(
            "PlusMinusAnimationStepProperty", typeof(double), typeof(RadTreeViewElement), new RadElementPropertyMetadata((double)0.025));

        public static RadProperty AllowPlusMinusAnimationProperty = RadProperty.Register(
            "AllowPlusMinusAnimation", typeof(bool), typeof(RadTreeViewElement), new RadElementPropertyMetadata(false));



        #endregion

        #region Properties


        /// <summary>
        /// Gets or sets a value indicating whether the TreeView load child Nodes collection in NodesNeeded event only when Parend nodes expanded.
        /// </summary>
        /// <value><c>true</c> if [lazy mode]; otherwise, <c>false</c>.</value>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [DefaultValue(true)]
        public bool LazyMode
        {
            get
            {
                return this.lazyMode;
            }
            set
            {
                if (this.lazyMode != value)
                {
                    this.lazyMode = value;
                    this.OnNotifyPropertyChanged("LazyMode");
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the child nodes should be auto checked when RadTreeView is in tri state mode
        ///  </summary>
        /// <value>The default value is false.</value>
        [Category("Behavior"), DefaultValue(true)]
        [Description("Gets or sets a value indicating whether the child nodes should be auto checked when RadTreeView is in tri state mode")]
        public bool AutoCheckChildNodes
        {
            get
            {
                return this.autoCheckChildNodes;
            }
            set
            {
                if (value != this.autoCheckChildNodes)
                {
                    this.autoCheckChildNodes = value;
                    this.OnNotifyPropertyChanged("AutoCheckChildNodes");
                }
            }
        }

        /// <summary>
        /// Contains data binding settings for related data.
        /// </summary>
        [Browsable(true)]
        [Description("Contains data binding settings for related data.")]
        public RelationBindingCollection RelationBindings
        {
            get
            {
                if (this.relationBinding == null)
                {
                    this.relationBinding = new RelationBindingCollection();
                }

                return relationBinding;
            }
        }

        [DefaultValue(false), Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool EnableDeferredScrolling
        {
            get
            {
                return this.Scroller.ScrollMode == ItemScrollerScrollModes.Deferred;
            }
            set
            {
                if (value)
                {
                    this.Scroller.ScrollMode = ItemScrollerScrollModes.Deferred;
                    this.OnNotifyPropertyChanged("EnableDeferedScrolling");
                }
            }
        }

        /// <summary>Gets or sets the type of the <see cref="ExpandAnimation">expand animation enumeration</see>.</summary>
        /// <seealso cref="AllowPlusMinusAnimation">AllowPlusMinusAnimation enumeration</seealso>
        /// <seealso cref="PlusMinusAnimationStep">PlusMinusAnimationStep Property</seealso>
        /// <seealso cref="ExpandAnimation">ExpandAnimation Enumeration</seealso>
        /// <value>
        /// 
        ///     The default value is ExpandAnimation.Opacity.
        /// </value>
        [Category("Behavior")]
        [Description("Gets or sets the type of expand animation.")]
        [DefaultValue(ExpandAnimation.Opacity)]
        public ExpandAnimation ExpandAnimation
        {
            get
            {
                return (ExpandAnimation)this.GetValue(ExpandAnimationProperty);
            }
            set
            {
                this.SetValue(ExpandAnimationProperty, value);
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
                return (double)this.GetValue(PlusMinusAnimationStepProperty);
            }

            set
            {
                if (this.PlusMinusAnimationStep != value)
                {
                    double newValue = value;

                    if (value < 0)
                    {
                        newValue = 0;
                    }
                    else if (value > 1)
                    {
                        newValue = 1;
                    }

                    this.SetValue(PlusMinusAnimationStepProperty, newValue);
                }
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
                return (bool)this.GetValue(AllowPlusMinusAnimationProperty);
            }
            set
            {
                this.SetValue(AllowPlusMinusAnimationProperty, value);
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
        public override int ImageIndex
        {
            get { return this.defaultImageIndex; }
            set
            {
                if (this.defaultImageIndex != value)
                {
                    this.defaultImageKey = string.Empty;
                    this.defaultImageIndex = value;
                    this.Update(UpdateActions.StateChanged);
                }
            }
        }

        /// <summary>
        /// The default image key for nodes.
        /// </summary>
        /// <value>The image key.</value>
        /// 
        [Category("Behavior"), RelatedImageList("ImageList"), Localizable(true), TypeConverter(typeof(ImageKeyConverter))]
        [Editor("System.Windows.Forms.Design.ImageIndexEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        [DefaultValue(""), RefreshProperties(RefreshProperties.Repaint)]
        [Description("The default image key for nodes.")]
        public override string ImageKey
        {
            get { return this.defaultImageKey; }
            set
            {
                if (this.defaultImageKey != value)
                {
                    this.defaultImageKey = value;
                    this.defaultImageIndex = -1;
                    this.Update(UpdateActions.StateChanged);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [tri state mode].
        /// </summary>
        /// <value><c>true</c> if [tri state mode]; otherwise, <c>false</c>.</value>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [DefaultValue(false)]
        public bool TriStateMode
        {
            get { return this.triStateMode; }
            set
            {
                if (this.triStateMode != value)
                {
                    this.triStateMode = value;

                    if (value && !this.CheckBoxes)
                    {
                        this.CheckBoxes = value;
                    }

                    this.OnNotifyPropertyChanged("TriStateMode");
                }
            }
        }

        /// <summary>
        /// Gets or sets the toggle mode.
        /// </summary>
        /// <value>The toggle mode.</value>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [DefaultValue(ToggleMode.DoubleClick)]
        public ToggleMode ToggleMode
        {
            get { return this.toggleMode; }
            set
            {
                if (this.toggleMode != value)
                {
                    this.toggleMode = value;
                    this.OnNotifyPropertyChanged("ToggleMode");
                }
            }
        }

        /// <summary>
        /// Gets the drag drop service.
        /// </summary>
        /// <value>The drag drop service.</value>
        public TreeViewDragDropService DragDropService
        {
            get { return dragDropService; }
        }

        /// <summary>
        /// Gets or sets the <see cref="RadImageShape">RadImageShape</see> instance which describes the hint that indicates where an item will be dropped after a drag operation.
        /// </summary>
        [Browsable(false)]
        [VsbBrowsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RadImageShape ItemDropHint
        {
            get
            {
                return (RadImageShape)this.GetValue(ItemDropHintProperty);
            }
            set
            {
                this.SetValue(ItemDropHintProperty, value);
            }
        }

        /// <summary>
        /// Gets the last node.
        /// </summary>
        /// <value>The last node.</value>
        public RadTreeNode LastNode
        {
            get
            {
                if (root.Nodes.Count > 0)
                {
                    RadTreeNode node = root.Nodes[root.Nodes.Count - 1];
                    while (node != null)
                    {
                        if ((!node.Visible && !IsInDesignMode))
                        {
                            node = node.PrevVisibleNode;
                        }
                        if (node != null)
                        {
                            if (node.Expanded && node.Nodes.Count > 0)
                            {
                                node = node.Nodes[node.Nodes.Count - 1];
                            }
                            else
                            {
                                return node;
                            }
                        }
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [allow drag drop].
        /// </summary>
        /// <value><c>true</c> if [allow drag drop]; otherwise, <c>false</c>.</value>
        public bool AllowDragDrop
        {
            get
            {
                return this.allowDragDrop;
            }
            set
            {
                if (this.allowDragDrop != value)
                {
                    this.allowDragDrop = value;

                    Control control = this.ElementTree.Control;

                    if (control != null)
                    {
                        control.AllowDrop = true;
                    }

                    this.OnNotifyPropertyChanged("AllowDragDrop");
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [multi select].
        /// </summary>
        /// <value><c>true</c> if [multi select]; otherwise, <c>false</c>.</value>
        public bool MultiSelect
        {
            get { return this.multiSelect; }
            set
            {
                if (this.multiSelect != value)
                {
                    this.ClearSelection();

                    if (this.SelectedNode != null)
                    {
                        this.SelectedNode.Selected = true;
                        this.Update(UpdateActions.StateChanged, this.SelectedNode);
                    }

                    this.multiSelect = value;
                    this.OnNotifyPropertyChanged("MultiSelect");
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show expander].
        /// </summary>
        /// <value><c>true</c> if [show expander]; otherwise, <c>false</c>.</value>
        public bool ShowExpandCollapse
        {
            get { return (bool)GetValue(ShowExpandCollapseProperty); }
            set { SetValue(ShowExpandCollapseProperty, value); }
        }

        /// <summary>
        /// Gets the selected nodes.
        /// </summary>
        /// <value>The selected nodes.</value>
        public SelectedTreeNodeCollection SelectedNodes
        {
            get
            {
                return this.selectedNodeCollection;
            }
        }

        /// <summary>
        /// Gets the checked nodes.
        /// </summary>
        /// <value>The checked nodes.</value>
        public CheckedTreeNodeCollection CheckedNodes
        {
            get
            {
                return this.checkedNodeCollection;
            }
        }

        /// <summary>
        /// Gets or sets the context menu.
        /// </summary>
        /// <value>The context menu.</value>
        public virtual RadContextMenu ContextMenu
        {
            get { return this.contextMenu; }
            set
            {
                if (this.contextMenu != value)
                {
                    this.contextMenu = value;
                    this.OnNotifyPropertyChanged("ContextMenu");
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [check boxes].
        /// </summary>
        /// <value><c>true</c> if [check boxes]; otherwise, <c>false</c>.</value>
        public bool CheckBoxes
        {
            get { return this.checkBoxes; }
            set
            {
                if (this.checkBoxes != value)
                {
                    this.checkBoxes = value;

                    if (!value)
                    {
                        this.DisableCheckBoxes();
                    }

                    this.OnNotifyPropertyChanged("CheckBoxes");
                }
            }
        }


        /// <summary>
        /// Gets or sets a value indicating whether [hide selection].
        /// </summary>
        /// <value><c>true</c> if [hide selection]; otherwise, <c>false</c>.</value>
        public bool HideSelection
        {
            get { return this.hideSelection; }
            set
            {
                if (this.hideSelection != value)
                {
                    this.hideSelection = value;
                    this.OnNotifyPropertyChanged("HideSelection");
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [hot tracking].
        /// </summary>
        /// <value><c>true</c> if [hot tracking]; otherwise, <c>false</c>.</value>
        public bool HotTracking
        {
            get { return this.hotTracking; }
            set
            {
                if (this.hotTracking != value)
                {
                    this.hotTracking = value;
                    this.OnNotifyPropertyChanged("HotTracking");
                }
            }
        }

        /// <summary>
        /// Gets or sets the height of the item.
        /// </summary>
        /// <value>The height of the item.</value>
        public int ItemHeight
        {
            get { return (int)GetValue(ItemHeightProperty); }
            set { SetValue(ItemHeightProperty, value); }
        }

        /// <summary>
        /// Gets or sets the active editor.
        /// </summary>
        /// <value>The active editor.</value>
        public IInputEditor ActiveEditor
        {
            get
            {
                return this.activeEditor;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [allow edit].
        /// </summary>
        /// <value><c>true</c> if [allow edit]; otherwise, <c>false</c>.</value>
        public bool AllowEdit
        {
            get { return this.allowEdit; }
            set
            {
                if (this.allowEdit != value)
                {
                    this.allowEdit = value;
                    this.OnNotifyPropertyChanged("AllowEdit");
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [allow edit].
        /// </summary>
        /// <value><c>true</c> if [allow edit]; otherwise, <c>false</c>.</value>
        public bool AllowAdd
        {
            get { return this.allowAdd; }
            set
            {
                if (this.allowAdd != value)
                {
                    this.allowAdd = value;
                    this.OnNotifyPropertyChanged("AllowAdd");
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [allow edit].
        /// </summary>
        /// <value><c>true</c> if [allow edit]; otherwise, <c>false</c>.</value>
        public bool AllowRemove
        {
            get { return this.allowRemove; }
            set
            {
                if (this.allowRemove != value)
                {
                    this.allowRemove = value;
                    this.OnNotifyPropertyChanged("AllowRemove");
                }
            }
        }
        /// <summary>
        /// Gets a value indicating whether there is an open editor in the tree view.
        /// </summary>
        public bool IsEditing
        {
            get
            {
                return this.ActiveEditor != null;
            }
        }

        /// <summary>
        /// Gets or sets the selected node.
        /// </summary>
        /// <value>The selected node.</value>
        public RadTreeNode SelectedNode
        {
            get { return this.selected; }
            set
            {
                if (this.selected != value)
                {
                    if (value != null)
                    {
                        this.ProcessSelection(value, Keys.None, false);
                    }
                    else
                    {
                        this.ProcessCurrentNode(value, true);
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show lines].
        /// </summary>
        /// <value><c>true</c> if [show lines]; otherwise, <c>false</c>.</value>
        public bool ShowLines
        {
            get { return (bool)GetValue(ShowLinesProperty); }
            set { SetValue(ShowLinesProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show root lines].
        /// </summary>
        /// <value><c>true</c> if [show root lines]; otherwise, <c>false</c>.</value>
        public bool ShowRootLines
        {
            get { return (bool)GetValue(ShowRootLinesProperty); }
            set { SetValue(ShowRootLinesProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show node tool tips].
        /// </summary>
        /// <value><c>true</c> if [show node tool tips]; otherwise, <c>false</c>.</value>
        public bool ShowNodeToolTips
        {
            get { return this.showNodeToolTips; }
            set
            {
                if (this.showNodeToolTips != value)
                {
                    this.showNodeToolTips = value;
                    this.OnNotifyPropertyChanged("ShowNodeToolTips");
                }
            }
        }

        /// <summary>
        /// Gets the first visible tree node in the tree view.
        /// </summary>
        public RadTreeNode TopNode
        {
            get
            {
                RadTreeNode node = null;

                if (root.Nodes.Count > 0)
                {
                    node = root.Nodes[0];
                }

                while (node != null)
                {
                    if (node.Visible || IsInDesignMode)
                    {
                        break;
                    }

                    node = node.NextVisibleNode;
                }

                return node;
            }
        }

        /// <summary>
        /// Gets or sets the color of the lines connecting the nodes in the tree view.
        /// </summary>
        public Color LineColor
        {
            get { return (Color)GetValue(LineColorProperty); }
            set { SetValue(LineColorProperty, value); }
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
        public virtual TreeLineStyle LineStyle
        {
            get
            {
                return (TreeLineStyle)this.GetValue(LineStyleProperty);
            }
            set
            {
                this.SetValue(LineStyleProperty, value);
            }
        }

        /// <summary>
        ///  Gets the number of tree nodes that are visible in the tree view
        /// </summary>
        public int VisibleCount
        {
            get
            {
                if (this.ViewElement != null)
                {
                    return this.ViewElement.Children.Count;
                }

                return 0;
            }
        }

        /// <summary>
        /// Gets or sets the path separator.
        /// </summary>
        /// <value>The path separator.</value>
        [Browsable(false), DefaultValue(@"\")]
        [Description("Gets or sets the value of the path separator.")]
        public string PathSeparator
        {
            get
            {
                return this.pathSeparator;
            }
            set
            {
                if (this.pathSeparator != value)
                {
                    this.pathSeparator = value;
                    this.OnNotifyPropertyChanged("PathSeparator");
                }
            }
        }

        /// <summary>
        /// Gets or sets the tree node provider.
        /// </summary>
        /// <value>The tree node provider.</value>
        public virtual TreeNodeProvider TreeNodeProvider
        {
            get { return treeNodeProvider; }
            set
            {
                if (treeNodeProvider != value)
                {
                    treeNodeProvider = value;
                    EnsureNodeProvider();
                    this.OnNotifyPropertyChanged("TreeNodeProvider");
                }
            }
        }

        /// <summary>
        /// Gets or sets the binding context.
        /// </summary>
        /// <value>The binding context.</value>
        public override BindingContext BindingContext
        {
            get
            {
                return this.bindingContext;
            }
            set
            {
                if (this.bindingContext != value)
                {
                    this.bindingContext = value;
                    this.OnBindingContextChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or sets the data source that the <see cref="GridViewTemplate"/> is displaying data for.
        /// </summary>
        public object DataSource
        {
            get { return this.ListSource.DataSource; }
            set
            {
                if (this.ListSource.DataSource != value)
                {
                    this.ListSource.DataSource = value;
                }
            }
        }

        /// <summary>
        /// Gets the nodes.
        /// </summary>
        /// <value>The nodes.</value>
        public RadTreeNodeCollection Nodes
        {
            get
            {
                return this.root.Nodes;
            }
        }

        /// <summary>
        /// Gets or sets the indent of nodes, applied to each tree level.
        /// </summary>
        [Description("Gets or sets the indent of nodes, applied to each tree level.")]
        public virtual int TreeIndent
        {
            get
            {
                return (int)this.GetValue(TreeIndentProperty);
            }

            set
            {
                this.SetValue(TreeIndentProperty, value);
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
                if (this.filterDescriptors.Count > 0)
                {
                    return this.filterDescriptors[0].Value;
                }

                return null;
            }
            set
            {
                if (value != null)
                {
                    if (this.filterDescriptors.Count == 0)
                    {
                        this.filterDescriptors.Add(new FilterDescriptor("Text", FilterOperator.Contains, value));
                        return;
                    }

                    this.filterDescriptors[0].Value = value;
                }

                if ((value == null || value == "") && this.filterDescriptors.Count > 0)
                {
                    this.filterDescriptors.RemoveAt(0);
                }
            }
        }

        /// <summary>
        /// Gets or sets the sort order of Nodes.
        /// </summary>
        /// <value>The sort order.</value>
        [Browsable(false)]
        [DefaultValue(SortOrder.None)]
        public SortOrder SortOrder
        {
            get
            {
                if (this.SortDescriptors.Count > 0)
                {
                    ListSortDirection direction = this.SortDescriptors[0].Direction;
                    switch (direction)
                    {
                        case ListSortDirection.Ascending:
                            return System.Windows.Forms.SortOrder.Ascending;
                        case ListSortDirection.Descending:
                            return System.Windows.Forms.SortOrder.Descending;
                    }
                }

                return System.Windows.Forms.SortOrder.None;
            }
            set
            {
                if (value == System.Windows.Forms.SortOrder.None)
                {
                    if (this.SortDescriptors.Count > 0)
                    {
                        this.SortDescriptors.RemoveAt(0);
                    }

                    return;
                }

                ListSortDirection direction = ListSortDirection.Ascending;
                if (value == System.Windows.Forms.SortOrder.Descending)
                {
                    direction = ListSortDirection.Descending;
                }

                if (this.SortDescriptors.Count == 0)
                {
                    this.SortDescriptors.Add("Text", direction);
                    return;
                }

                this.SortDescriptors[0].Direction = direction;
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
                return this.filterDescriptors;
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
                return this.listSource.CollectionView.SortDescriptors;
            }
        }

        /// <summary>
        /// Gets or sets the name of the list or table in the data source for which the <see cref="RadTreeViewElement"/> is displaying data. 
        /// </summary>
        public string DataMember
        {
            get { return this.ListSource.DataMember; }
            set
            {
                if (this.ListSource.DataMember != value)
                {
                    this.ListSource.DataMember = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a property name which will be used to extract a value from the data items. The value of the property with
        /// this name will be available via the Value property of every RadTreeNode.
        /// </summary>
        public string ValueMember
        {
            get
            {
                return this.valueMember;
            }
            set
            {
                if (value == this.valueMember)
                {
                    return;
                }

                this.valueMember = value;
                if (this.listSource.IsDataBound && !string.IsNullOrEmpty(this.valueMember))
                {
                    string valuePath = this.valueMember.Split('\\')[0];
                    string[] names = valuePath.Split('.');

                    PropertyDescriptor pd = this.listSource.BoundProperties.Find(names[0], true);
                    if (pd != null)
                    {
                        if (names.Length > 1)
                        {
                            this.RootDescriptor.SetValueDescriptor(pd, valuePath);
                        }
                        else
                        {
                            this.RootDescriptor.ValueDescriptor = pd;
                        }
                    }

                    EnsureNodeProvider();
                }
            }
        }

        /// <summary>
        /// Gets or sets a property name which will be used to define a relation of the data items. 
        /// </summary>
        public string ChildMember
        {
            get
            {
                return this.childMember;
            }
            set
            {
                if (value == this.childMember)
                {
                    return;
                }

                this.childMember = value;
                if (this.listSource.IsDataBound && !string.IsNullOrEmpty(this.childMember))
                {
                    string childPath = this.childMember.Split('\\')[0];
                    string[] names = childPath.Split('.');

                    PropertyDescriptor pd = this.listSource.BoundProperties.Find(names[0], true);
                    if (pd != null)
                    {
                        if (names.Length > 1)
                        {
                            this.RootDescriptor.SetChildDescriptor(pd, childPath);
                        }
                        else
                        {
                            this.RootDescriptor.ChildDescriptor = pd;
                        }
                    }

                    EnsureNodeProvider();
                }
            }
        }

        /// <summary>
        /// Gets or sets the name of the list or table in the data source for which the <see cref="RadListSource&lt;T&gt;"/> is bound. 
        /// </summary>
        public string DisplayMember
        {
            get
            {
                return this.displayMember;
            }
            set
            {
                if (value == this.displayMember)
                {
                    return;
                }

                this.displayMember = value;
                if (this.listSource.IsDataBound && !string.IsNullOrEmpty(this.displayMember))
                {
                    string displayPath = this.displayMember.Split('\\')[0];
                    string[] names = displayPath.Split('.');

                    PropertyDescriptor pd = this.listSource.BoundProperties.Find(names[0], true);
                    if (pd != null)
                    {
                        if (names.Length > 1)
                        {
                            this.RootDescriptor.SetDisplaytDescriptor(pd, displayPath);
                        }
                        else
                        {
                            this.RootDescriptor.DisplayDescriptor = pd;
                        }
                    }

                    EnsureNodeProvider();
                }
            }
        }

        /// <summary>
        /// Gets or sets a property name which will be used to extract a value from the data items. The value of the property with
        /// this name will be available via the Value property of every RadListDataItem in the Items collection.
        /// </summary>
        public string ParentMember
        {
            get
            {
                return this.parentMember;
            }
            set
            {
                if (value == this.parentMember)
                {
                    return;
                }

                this.parentMember = value;
                if (this.listSource.IsDataBound && !string.IsNullOrEmpty(this.parentMember))
                {
                    string parentPath = this.parentMember.Split('\\')[0];
                    string[] names = parentPath.Split('.');

                    PropertyDescriptor pd = this.listSource.BoundProperties.Find(names[0], true);
                    if (pd != null)
                    {
                        if (names.Length > 1)
                        {
                            this.RootDescriptor.SetParentDescriptor(pd, parentPath);
                        }
                        else
                        {
                            this.RootDescriptor.ParentDescriptor = pd;
                        }
                    }

                    EnsureNodeProvider();
                }
            }
        }

        ///<summary>
        /// Gets or sets the expand image 
        /// </summary>
        [Description("Gets or sets the expand image "),
        Category(RadDesignCategory.AppearanceCategory),
        Localizable(true),
        TypeConverter(typeof(Telerik.WinControls.Primitives.ImageTypeConverter)),
        RefreshProperties(RefreshProperties.All)]
        [Editor(DesignerConsts.RadImageTypeEditorString, typeof(UITypeEditor))]
        public Image ExpandImage
        {
            get
            {
                return (Image)this.GetValue(ExpandImageProperty);
            }
            set
            {
                this.SetValue(ExpandImageProperty, value);
            }
        }

        ///<summary>
        /// Gets or sets the expand image 
        /// </summary>
        [Description("Gets or sets the expand image "),
        Category(RadDesignCategory.AppearanceCategory),
        Localizable(true),
        TypeConverter(typeof(Telerik.WinControls.Primitives.ImageTypeConverter)),
        RefreshProperties(RefreshProperties.All)]
        [Editor(DesignerConsts.RadImageTypeEditorString, typeof(UITypeEditor))]
        public Image CollapseImage
        {
            get
            {
                return (Image)this.GetValue(CollapseImageProperty);
            }
            set
            {
                this.SetValue(CollapseImageProperty, value);
            }
        }

        ///<summary>
        /// Gets or sets the hovered expand image 
        /// </summary>
        [Description("Gets or sets the hovered expand image "),
        Category(RadDesignCategory.AppearanceCategory),
        Localizable(true),
        TypeConverter(typeof(Telerik.WinControls.Primitives.ImageTypeConverter)),
        RefreshProperties(RefreshProperties.All)]
        [Editor(DesignerConsts.RadImageTypeEditorString, typeof(UITypeEditor))]

        public Image HoveredExpandImage
        {
            get
            {
                return (Image)this.GetValue(HoveredExpandImageProperty);
            }
            set
            {
                this.SetValue(HoveredExpandImageProperty, value);
            }
        }

        ///<summary>
        /// Gets or sets the hovered collapse image 
        /// </summary>
        [Description("Gets or sets the expand image "),
        Category(RadDesignCategory.AppearanceCategory),
        Localizable(true),
        TypeConverter(typeof(Telerik.WinControls.Primitives.ImageTypeConverter)),
        RefreshProperties(RefreshProperties.All)]
        [Editor(DesignerConsts.RadImageTypeEditorString, typeof(UITypeEditor))]
        public Image HoveredCollapseImage
        {
            get
            {
                return (Image)this.GetValue(HoveredCollapseImageProperty);
            }
            set
            {
                this.SetValue(HoveredCollapseImageProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether nodes can have different height.
        /// </summary>
        /// <value>The default value is false.</value>
        [Browsable(true), DefaultValue(false)]
        [Description("Gets or sets a value indicating whether nodes can have different height.")]
        public bool AllowArbitraryItemHeight
        {
            get { return (bool)GetValue(AllowArbitraryItemHeightProperty); }
            set { SetValue(AllowArbitraryItemHeightProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to select the full row.
        /// </summary>
        /// <value>The default value is false.</value>
        [Browsable(true), DefaultValue(false)]
        [Description("Gets or sets a value indicating whether to select the full row.")]
        public bool FullRowSelect
        {
            get
            {
                return (bool)this.GetValue(FullRowSelectProperty);
            }
            set
            {
                this.SetValue(FullRowSelectProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the vertical spacing among nodes.
        /// </summary>
        [Description("Gets or sets the vertical spacing among nodes.")]
        public virtual int NodeSpacing
        {
            get
            {
                return (int)this.GetValue(NodeSpacingProperty);
            }

            set
            {
                this.SetValue(NodeSpacingProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the alternating row color.
        /// </summary>
        [Description("Gets or sets the alternating row color.")]
        public Color AlternatingRowColor
        {
            get { return (Color)GetValue(AlternatingRowColorProperty); }
            set { SetValue(AlternatingRowColorProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to show rows with alternating row colors.
        /// </summary>
        [Description("Gets or sets a value indicating whether to show rows with alternating row colors.")]
        public bool AllowAlternatingRowColor
        {
            get
            {
                return this.allowAlternatingRowColor;
            }
            set
            {
                if (allowAlternatingRowColor != value)
                {
                    firstVisibleIndex = -1;
                    allowAlternatingRowColor = value;
                    OnNotifyPropertyChanged("AllowAlternatingRowColor");
                    Update(UpdateActions.StateChanged);
                }
            }
        }

        /// <summary>
        /// Gets the index of the first visible node.
        /// </summary>
        [Description("Gets the index of the first visible node.")]
        public int FirstVisibleIndex
        {
            get
            {
                if (firstVisibleIndex == -1)
                {
                    UpdateFirstVisibleIndex();
                }
                return this.firstVisibleIndex;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether single node expand is enabled.
        /// </summary>
        [Category(RadDesignCategory.BehaviorCategory)]
        [DefaultValue(ExpandMode.Multiple)]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ExpandMode ExpandMode
        {
            get { return this.expandMode; }
            set { this.expandMode = value; }
        }

        /// <summary>
        /// Gets or sets a property that controls the visibility of the horizontal scrollbar.
        /// </summary>
        [Browsable(true), DefaultValue(ScrollState.AutoHide)]
        [Description("Gets or sets a property that controls the visibility of the horizontal scrollbar.")]
        public ScrollState HorizontalScrollState
        {
            get
            {
                return this.horizontalScrollState;
            }
            set
            {
                if (this.horizontalScrollState != value)
                {
                    this.horizontalScrollState = value;
                    InvalidateMeasure();
                }
            }
        }

        /// <summary>
        /// Gets or sets a property that controls the visibility of the vertical scrollbar.
        /// </summary>
        [Browsable(true), DefaultValue(ScrollState.AutoHide)]
        [Description("Gets or sets a property that controls the visibility of the vertical scrollbar.")]
        public ScrollState VerticalScrollState
        {
            get
            {
                return this.Scroller.ScrollState;
            }
            set
            {
                this.Scroller.ScrollState = value;
            }
        }

        /// <summary>
        /// Gets or  a value indicating whether the control is in design mode.
        /// </summary>
        [Browsable(false)]
        [Description("Gets or  a value indicating whether the control is in design mode.")]
        public bool IsInDesignMode
        {
            get
            {
                if (this.ElementTree != null &&
                    this.ElementTree.Control != null)
                {
                    return this.ElementTree.Control.Site != null || this.ElementTree.Control.GetType().Name == "DesignTimeTreeView";
                }
                return false;
            }
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
            OnDataError(new TreeNodeDataErrorEventArgs(text, radTreeNode));
        }

        /// <summary>
        /// Creates a new node and adds a node by path. The label of the new node will be the text after the last separator.
        /// </summary>
        /// <param name="path">Where the node should be added.</param>
        /// <returns>The new node if the operation is successful.</returns>
        public RadTreeNode AddNodeByPath(string path)
        {
            return this.AddNodeByPath(path, this.PathSeparator);
        }

        /// <summary>
        /// Creates a new node and adds a node by path. The label of the new node will be the text after the last separator.
        /// </summary>
        /// <param name="path">Where the node should be added.</param>
        /// <param name="pathSeparator">The path separator.</param>
        /// <returns>The new node if the operation is successful.</returns>
        public RadTreeNode AddNodeByPath(string path, string pathSeparator)
        {
            if (String.IsNullOrEmpty(path) || String.IsNullOrEmpty(pathSeparator))
            {
                return null;
            }

            RadTreeNodeCollection nodes = this.Nodes;
            int lastIndex = path.LastIndexOf(pathSeparator);

            if (lastIndex != -1)
            {
                RadTreeNode parent = this.GetNodeByPath(path.Substring(0, lastIndex), pathSeparator);

                if (parent != null)
                {
                    nodes = parent.Nodes;
                }
            }

            lastIndex++;
            string nodeText = path.Substring(lastIndex, path.Length - lastIndex);
            return nodes.Add(nodeText);
        }

        /// <summary>
        /// Gets a node by specifying a path to it.
        /// </summary>
        /// <param name="path">The path to the node.</param>
        /// <returns>The node if found.</returns>
        public RadTreeNode GetNodeByPath(string path)
        {
            return this.GetNodeByPath(path, this.PathSeparator);
        }

        /// <summary>
        /// Gets a node by specifying a path to it.
        /// </summary>
        /// <param name="path">The path to the node.</param>
        /// <param name="pathSeparator">The path separator.</param>
        /// <returns>The node if found.</returns>
        public RadTreeNode GetNodeByPath(string path, string pathSeparator)
        {
            if (string.IsNullOrEmpty(path) || string.IsNullOrEmpty(pathSeparator))
            {
                return null;
            }

            string[] nodeTexts = path.Split(pathSeparator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            RadTreeNodeCollection nodes = this.Nodes;
            RadTreeNode node = null;

            foreach (string nodeText in nodeTexts)
            {
                node = nodes[nodeText];

                if (node == null)
                {
                    break;
                }

                nodes = node.Nodes;
            }

            return node;
        }

        /// <summary>
        /// Puts the current node in edit mode.
        /// </summary>
        /// <returns></returns>
        public bool BeginEdit()
        {
            if (!this.allowEdit || this.activeEditor != null || this.SelectedNodes.Count > 1 || this.SelectedNode == null)
            {
                return false;
            }

            TreeNodeEditorRequiredEventArgs editorRequiredEventArgs = new TreeNodeEditorRequiredEventArgs(this.SelectedNode, typeof(TreeViewTextBoxEditor));
            OnEditorRequired(editorRequiredEventArgs);

            IInputEditor editor = editorRequiredEventArgs.Editor as IInputEditor;
            if (editor == null)
            {
                editor = GetEditor(editorRequiredEventArgs.EditorType);
            }
            if (editor == null)
            {
                return false;
            }

            this.activeEditor = editor;

            TreeNodeEditingEventArgs editingEventArgs = new TreeNodeEditingEventArgs(this.SelectedNode, editor);
            this.OnEditing(editingEventArgs);

            if (editingEventArgs.Cancel)
            {
                this.activeEditor = null;
                return false;
            }

            this.SelectedNode.EnsureVisible();
            TreeNodeElement nodeElement = GetElement(this.SelectedNode);

            if (nodeElement == null)
            {
                this.activeEditor = null;
                return false;
            }

            RadScrollBarElement scrollbar = this.Scroller.Scrollbar;
            bool isLastScrollbarPosition = scrollbar.Value == (scrollbar.Maximum - scrollbar.LargeChange + 1);
            nodeElement.AddEditor(editor);

            ISupportInitialize initializable = this.activeEditor as ISupportInitialize;
            if (initializable != null)
            {
                initializable.BeginInit();
            }

            activeEditor.Initialize(nodeElement, this.SelectedNode.Value);

            if (initializable != null)
            {
                initializable.EndInit();
            }

            OnEditorInitialized(new TreeNodeEditorInitializedEventArgs(nodeElement, editor));

            this.activeEditor.BeginEdit();

            this.cachedOldValue = this.SelectedNode.Value;

            // 28.March.2011
            // Svetlin
            // We need to perform it for second time because when the vertical scrollbar is 
            // on last position and the editor require more height
            // the scroll bar should be synchronized

            if (isLastScrollbarPosition)
            {
                this.SelectedNode.EnsureVisible();
            }

            return true;
        }

        /// <summary>
        ///  Commits any changes and ends the edit operation on the current cell.
        /// </summary>
        /// <returns></returns>
        public bool EndEdit()
        {
            return EndEditCore(true);
        }

        /// <summary>
        /// Close the currently active editor and discard changes.
        /// </summary>
        /// <returns></returns>
        public void CancelEdit()
        {
            EndEditCore(false);
        }

        /// <summary>
        /// Updates the visual items in the three view
        /// </summary>
        /// <param name="updateAction">Indicated the update action</param>
        public void Update(UpdateActions updateAction)
        {
            if (this.updateSuspendedCount > 0)
            {
                if (updateAction == UpdateActions.Reset)
                {
                    resumeAction = UpdateActions.Reset;
                }
                return;
            }

            if (updateAction == UpdateActions.Reset)
            {
                this.HScrollBar.Value = this.HScrollBar.Minimum;
                this.VScrollBar.Value = this.VScrollBar.Minimum;
                this.ViewElement.ElementProvider.ClearCache();
                this.ViewElement.DisposeChildren();
                this.root.ChildrenSize = Size.Empty;
                resetSizes = true;
            }

            if (updateAction == UpdateActions.Reset ||
                updateAction == UpdateActions.Resume ||
                updateAction == UpdateActions.SortChanged)
            {
                UpdateScrollers(null, updateAction);
            }

            this.ViewElement.UpdateItems();

            if (updateAction == UpdateActions.StateChanged ||
                updateAction == UpdateActions.SortChanged ||
                updateAction == UpdateActions.Resume)
            {
                this.SynchronizeNodeElements();
            }
        }

        /// <summary>
        /// Updates the visual items in the three view
        /// </summary>
        /// <param name="updateAction">Indicated the update action</param>
        /// <param name="node">Indicated the updated node</param>
        public void Update(UpdateActions updateAction, params RadTreeNode[] nodes)
        {
            if (this.updateSuspendedCount > 0)
            {
                return;
            }

            if (updateAction == UpdateActions.ExpandedChanged ||
                updateAction == UpdateActions.ExpandedChangedUsingAnimation)
            {
                if (this.ElementState != WinControls.ElementState.Loaded)
                {
                    pendingScrollerUpdates = true;
                    return;
                }

                if (!this.UpdateOnExpandedChanged(updateAction, nodes[0]))
                {
                    return;
                }
            }

            if (updateAction == UpdateActions.ItemAdded)
            {
                if (this.ElementState != WinControls.ElementState.Loaded)
                {
                    pendingScrollerUpdates = true;
                    return;
                }
                UpdateScrollersOnAdd(nodes[0]);
            }

            if (updateAction == UpdateActions.ItemRemoved ||
                updateAction == UpdateActions.ItemMoved)
            {
                if (this.ElementState != WinControls.ElementState.Loaded)
                {
                    pendingScrollerUpdates = true;
                    return;
                }
                UpdateScrollers(null, updateAction);
            }

            bool found = false;

            foreach (TreeNodeElement nodeElement in this.ViewElement.Children)
            {
                if (nodeElement.Data == nodes[0])
                {
                    found = true;
                    break;
                }
            }

            if (found)
            {
                Update(updateAction);
            }
        }

        protected virtual bool UpdateOnExpandedChanged(UpdateActions updateAction, RadTreeNode node)
        {
            if (updateAction == UpdateActions.ExpandedChangedUsingAnimation &&
                this.ExpandAnimation != UI.ExpandAnimation.None)
            {
                TreeExpandAnimation animation = this.CreateExpandAnimation();
                TreeNodeElement nodeElement = this.GetElement(node);

                if (animation != null && nodeElement != null)
                {
                    if (node.Expanded)
                    {
                        animation.Expand(node);
                    }
                    else
                    {
                        animation.Collapse(node);
                    }

                    return false;
                }
            }

            this.UpdateOnExpandedChangedCore(node);
            return true;
        }

        protected virtual TreeExpandAnimation CreateExpandAnimation()
        {
            if (this.ExpandAnimation == UI.ExpandAnimation.Opacity)
            {
                return new TreeExpandAnimationOpacity(this);
            }

            return null;
        }

        protected virtual void UpdateOnExpandedChangedCore(RadTreeNode node)
        {
            if (node.Expanded)
            {
                this.UpdateScrollersOnExpand(node);

                if (ExpandMode == ExpandMode.Single)
                {
                    RadTreeNode parentNode = node.Parent;

                    if (parentNode == null)
                    {
                        parentNode = root;
                    }
                    foreach (RadTreeNode child in parentNode.Nodes)
                    {
                        if (child != node)
                        {
                            child.Expanded = false;
                        }
                    }
                }
            }
            else
            {
                this.UpdateScrollersOnCollapse(node);
            }
        }

        /// <summary>
        /// Begins the update.
        /// </summary>
        public void BeginUpdate()
        {
            this.updateSuspendedCount++;
            //if (this.listSource.IsDataBound)
            //{
            //    this.listSource.BeginUpdate();
            //}
        }

        /// <summary>
        /// Ends the update.
        /// </summary>
        public void EndUpdate()
        {
            EndUpdate(true, resumeAction);
            resumeAction = UpdateActions.Resume;
        }

        /// <summary>
        /// Ends the update.
        /// </summary>
        /// <param name="performUpdate">Tells the view whether an update is required or not.</param>
        /// <param name="action">Indicates the update action</param>
        public void EndUpdate(bool performUpdate, UpdateActions action)
        {
            //RefreshSource(action);

            if (this.updateSuspendedCount == 0)
            {
                return;
            }

            this.updateSuspendedCount--;
            if (this.updateSuspendedCount == 0 && performUpdate)
            {
                Update(action);
                return;
            }
        }

        private void RefreshSource(UpdateActions action)
        {
            if (action == UpdateActions.ExpandedChanged || action == UpdateActions.ExpandedChangedUsingAnimation ||
                action == UpdateActions.Resume || action == UpdateActions.StateChanged)
            {
                return;
            }

            if (this.listSource.IsDataBound)
            {
                this.listSource.EndUpdate();
            }
        }

        /// <summary>
        /// Defers the refresh.
        /// </summary>
        /// <returns></returns>
        public virtual IDisposable DeferRefresh()
        {
            this.BeginUpdate();
            return new DeferHelper(this);
        }

        /// <summary>
        /// Collapses all.
        /// </summary>
        public void CollapseAll()
        {
            BeginUpdate();
            for (int i = 0; i < this.Nodes.Count; i++)
            {
                this.Nodes[i].Collapse();
            }
            EndUpdate();
        }

        /// <summary>
        /// Expands all.
        /// </summary>
        public void ExpandAll()
        {
            BeginUpdate();
            for (int i = 0; i < this.Nodes.Count; i++)
            {
                this.Nodes[i].ExpandAll();
            }
            EndUpdate();
        }

        /// <summary>
        /// Gets the node at.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns></returns>
        public RadTreeNode GetNodeAt(int x, int y)
        {
            TreeNodeElement nodeElement = this.GetNodeElementAt(x, y);
            if (nodeElement != null)
            {
                return nodeElement.Data;
            }

            return null;
        }

        /// <summary>
        /// Gets the node at.
        /// </summary>
        /// <param name="pt">The pt.</param>
        /// <returns></returns>
        public RadTreeNode GetNodeAt(Point pt)
        {
            return this.GetNodeAt(pt.X, pt.Y);
        }

        /// <summary>
        /// Gets the node element at.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns></returns>
        public TreeNodeElement GetNodeElementAt(int x, int y)
        {
            if (this.ElementTree == null)
            {
                return null;
            }

            RadElement element = this.ElementTree.GetElementAtPoint(new Point(x, y));

            if (element == null)
            {
                return null;
            }

            if (element is TreeNodeElement)
            {
                return ((TreeNodeElement)element);
            }

            TreeNodeElement nodeElement = element.FindAncestor<TreeNodeElement>();
            if (nodeElement != null)
            {
                return nodeElement;
            }

            return null;
        }

        /// <summary>
        /// Gets the node element at.
        /// </summary>
        /// <param name="pt">The pt.</param>
        /// <returns></returns>
        public TreeNodeElement GetNodeElementAt(Point pt)
        {
            return this.GetNodeElementAt(pt.X, pt.Y);
        }

        /// <summary>
        /// Gets the node count.
        /// </summary>
        /// <param name="includeSubTrees">if set to <c>true</c> [include sub trees].</param>
        /// <returns></returns>
        public int GetNodeCount(bool includeSubTrees)
        {
            int count = 0;

            if (includeSubTrees)
            {
                for (int i = 0; i < this.Nodes.Count; i++)
                {
                    count += this.Nodes[i].GetNodeCount(true);
                }
            }

            return count + this.Nodes.Count;
        }

        /// <summary>
        /// Finds the specified match.
        /// </summary>
        /// <param name="match">The match.</param>
        /// <returns></returns>
        public RadTreeNode Find(Predicate<RadTreeNode> match)
        {
            return this.Root.Find(match);
        }

        /// <summary>
        /// Finds the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public RadTreeNode Find(string text)
        {
            this.search = text;
            return this.Root.Find(this.FindByText);
        }

        /// <summary>
        /// Finds the nodes.
        /// </summary>
        /// <param name="match">The match.</param>
        /// <returns></returns>
        public RadTreeNode[] FindNodes(Predicate<RadTreeNode> match)
        {
            return this.Root.FindNodes(match);
        }

        /// <summary>
        /// Finds the nodes.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public RadTreeNode[] FindNodes(string text)
        {
            this.search = text;
            return this.Root.FindNodes(this.FindByText);
        }

        /// <summary>
        /// Executes the specified command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="settings">The settings.</param>
        /// <returns></returns>
        public object Execute(ICommand command, params object[] settings)
        {
            return this.Execute(true, command, settings);
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
            return this.Root.Execute(command, includeSubTrees, settings);
        }

        /// <summary>
        /// Scrolls to.
        /// </summary>
        /// <param name="delta">The delta.</param>
        public void ScrollTo(int delta)
        {
            int result = this.VScrollBar.Value - delta * this.VScrollBar.SmallChange;
            if (result > this.VScrollBar.Maximum - this.VScrollBar.LargeChange + 1)
            {
                result = this.VScrollBar.Maximum - this.VScrollBar.LargeChange + 1;
            }

            if (result < this.VScrollBar.Minimum)
            {
                result = 0;
            }
            else if (result > this.VScrollBar.Maximum)
            {
                result = this.VScrollBar.Maximum;
            }

            this.VScrollBar.Value = result;
        }

        /// <summary>
        /// Ensures that the specified tree node is visible within the tree view element, scrolling the contents of the element if necessary.
        /// </summary>
        /// <param name="node">The node to scroll into view</param>
        public void EnsureVisible(RadTreeNode node)
        {
            TreeNodeElement nodeElement = GetElement(node);

            nodeElement = EnsureNodeVisibleVertical(node, nodeElement);
            EnsureNodeVisibleHorizontal(node, nodeElement);

            this.UpdateLayout();
        }

        /// <summary>
        /// Ensures that the specified tree node is visible within the tree view element, scrolling the contents of the element if necessary. 
        /// This method expands parent items when necessary.
        /// </summary>
        /// <param name="node">The node to bring into view</param>
        public void BringIntoView(RadTreeNode node)
        {
            if (node != null)
            {
                this.BeginUpdate();

                RadTreeNode parent = node.Parent;
                while (parent != null && !parent.Expanded)
                {
                    parent.Expand();
                    parent = parent.Parent;
                }

                this.EndUpdate(true, RadTreeViewElement.UpdateActions.Resume);
                this.EnsureVisible(node);
            }
        }

        public void ClearSelection()
        {
            this.selectedNodeCollection.Clear();
        }

        public void SelectAll()
        {
            if (MultiSelect && this.root.Nodes.Count > 0)
            {
                this.BeginUpdate();
                RadTreeNode node = this.root.Nodes[0];
                while (node != null)
                {
                    node.Selected = true;
                    node = node.NextVisibleNode;
                }
                this.EndUpdate(true, UpdateActions.StateChanged);
            }
        }

        #endregion

        #region Private & protected

        internal bool FullLazyMode
        {
            get
            {
                return (this.LazyMode && this.NodesNeeded != null);
            }
        }

        internal bool IsLazyLoading
        {
            get
            {
                return (this.NodesNeeded != null);
            }
        }

        private RadTreeNode GetNodeAt(Point location, bool fullRowSelect)
        {
            if (fullRowSelect)
            {
                return this.GetNodeAt(location);
            }

            TreeNodeElement nodeElement = this.GetNodeElementAt(location);

            if (nodeElement == null)
            {
                return null;
            }

            Rectangle nodeRect = Rectangle.Empty;

            foreach (RadElement element in nodeElement.Children)
            {
                nodeRect = Rectangle.Union(nodeRect, element.ControlBoundingRectangle);
            }

            if (nodeRect.Contains(location))
            {
                return nodeElement.Data;
            }

            return null;
        }

        private bool FindByText(RadTreeNode node)
        {
            string text = (this.search != null) ? this.search.ToString() : null;

            if ((text == null) || (node.Text == null))
            {
                return false;
            }

            if (text.Length != node.Text.Length)
            {
                return false;
            }

            return (string.Compare(text, node.Text, true, CultureInfo.InvariantCulture) == 0);
        }

        private void DisableCheckBoxes()
        {
            if (this.Nodes.Count == 0)
            {
                return;
            }

            Queue<RadTreeNodeCollection> collectionQueue = new Queue<RadTreeNodeCollection>();
            collectionQueue.Enqueue(this.Nodes);

            while (collectionQueue.Count > 0)
            {
                RadTreeNodeCollection nodeCollection = collectionQueue.Dequeue();

                foreach (RadTreeNode node in nodeCollection)
                {
                    if (node.CheckType == CheckType.CheckBox)
                    {
                        node.CheckType = CheckType.None;
                    }

                    if (node.Nodes.Count > 0)
                    {
                        collectionQueue.Enqueue(node.Nodes);
                    }
                }
            }
        }

        private void ResetDescriptors()
        {
            if (this.listSource.IsDataBound)
            {
                if (!string.IsNullOrEmpty(this.displayMember))
                {
                    string displayPath = this.displayMember.Split('\\')[0];
                    string[] names = displayPath.Split('.');

                    PropertyDescriptor pd = this.listSource.BoundProperties.Find(names[0], true);
                    if (pd != null)
                    {
                        if (names.Length > 1)
                        {
                            this.RootDescriptor.SetDisplaytDescriptor(pd, displayPath);
                        }
                        else
                        {
                            this.RootDescriptor.DisplayDescriptor = pd;
                        }
                    }
                }

                if (!string.IsNullOrEmpty(this.childMember))
                {
                    string childPath = this.childMember.Split('\\')[0];
                    string[] names = childPath.Split('.');

                    PropertyDescriptor pd = this.listSource.BoundProperties.Find(names[0], true);
                    if (pd != null)
                    {
                        if (names.Length > 1)
                        {
                            this.RootDescriptor.SetChildDescriptor(pd, childPath);
                        }
                        else
                        {
                            this.RootDescriptor.ChildDescriptor = pd;
                        }
                    }
                }

                if (!string.IsNullOrEmpty(this.parentMember))
                {
                    string parentPath = this.parentMember.Split('\\')[0];
                    string[] names = parentPath.Split('.');

                    PropertyDescriptor pd = this.listSource.BoundProperties.Find(names[0], true);
                    if (pd != null)
                    {
                        if (names.Length > 1)
                        {
                            this.RootDescriptor.SetParentDescriptor(pd, parentPath);
                        }
                        else
                        {
                            this.RootDescriptor.ParentDescriptor = pd;
                        }
                    }
                }

                if (!string.IsNullOrEmpty(this.valueMember))
                {
                    string valuePath = this.valueMember.Split('\\')[0];
                    string[] names = valuePath.Split('.');

                    PropertyDescriptor pd = this.listSource.BoundProperties.Find(names[0], true);
                    if (pd != null)
                    {
                        if (names.Length > 1)
                        {
                            this.RootDescriptor.SetValueDescriptor(pd, valuePath);
                        }
                        else
                        {
                            this.RootDescriptor.ValueDescriptor = pd;
                        }
                    }
                }

                return;
            }

            this.RootDescriptor.DisplayDescriptor = null;
            this.RootDescriptor.ChildDescriptor = null;
            this.RootDescriptor.ParentDescriptor = null;
            this.RootDescriptor.ValueDescriptor = null;
        }

        private bool IsEditorHint(Point point)
        {
            BaseInputEditor inputEditor = this.activeEditor as BaseInputEditor;

            if (inputEditor != null)
            {
                return inputEditor.EditorElement.ControlBoundingRectangle.Contains(point);
            }

            return false;
        }

        private bool IsExpander(Point point)
        {
            if (this.ElementTree == null)
            {
                return false;
            }

            return this.ElementTree.GetElementAtPoint(point) is ExpanderItem;
        }

        private bool IsToggle(Point point)
        {
            if (this.ElementTree == null)
            {
                return false;
            }

            RadElement elementUnderMouse = this.ElementTree.GetElementAtPoint(point);

            while (elementUnderMouse != null)
            {
                RadToggleButtonElement toggleButton = elementUnderMouse as RadToggleButtonElement;

                if (toggleButton != null)
                {
                    return true;
                }

                elementUnderMouse = elementUnderMouse.Parent;
            }

            return false;
        }

        protected override IVirtualizedElementProvider<RadTreeNode> CreateElementProvider()
        {
            return new TreeViewElementProvider(this);
        }

        protected virtual void UpdateScrollers(RadTreeNode skipNode, UpdateActions updateAction)
        {
            if (this.root.Nodes.Count > 0)
            {
                resetSizes = true;
                this.root.ChildrenSize = Size.Empty;
                this.root.ActualSize = Size.Empty;

                // Svetlin
                // 29.March.2011
                // This is a temporary solution which resolved scrollbar's visibility
                // issue caused bye sort ascending and then descending (Work Item: 111351)
                // If we access node.Nodes[0] the data view may not be loaded by the .Load() method
                // the GetEnumerator()'s logic of TreeNodeDataView invoked .Load() 
                // If we invoke the .Load() in this[index] of TreeNodeDataView we will decrease the performance
                foreach (RadTreeNode node in this.root.Nodes)
                {
                    this.UpdateActualSize(node, false, skipNode);
                    break;
                }

                this.UpdateHScrollbarMaximum(this.root.ChildrenSize.Width);
                resetSizes = false;
                this.Scroller.UpdateScrollRange(root.ChildrenSize.Height, false);

                if (updateAction == UpdateActions.SortChanged)
                {
                    this.Scroller.ScrollToItem(this.Scroller.Traverser.Current);
                }
            }
            else
            {
                this.HScrollBar.Value = this.HScrollBar.Minimum;
                this.HScrollBar.Maximum = this.HScrollBar.Minimum;
                this.VScrollBar.Value = this.VScrollBar.Minimum;
                this.VScrollBar.Maximum = this.VScrollBar.Minimum;
                UpdateHScrollbarVisibility();
                if (VerticalScrollState == ScrollState.AlwaysShow)
                {
                    this.VScrollBar.Visibility = ElementVisibility.Visible;
                }
                else
                {
                    this.VScrollBar.Visibility = ElementVisibility.Collapsed;
                }
            }
        }

        protected virtual void UpdateActualSize(RadTreeNode node, bool stopOnSameLevel, RadTreeNode skipNode)
        {
            RadTreeNode startNode = node;
            bool performLayout = false; 

            TreeViewElementProvider elementProvider = (TreeViewElementProvider)ViewElement.ElementProvider;
            //TreeViewEnumerator e = new TreeViewEnumerator(startNode);
            while (node != null)
            {
                if (node == skipNode)
                {
                    node = node.NextNode;
                    continue;
                }
                               
                if (resetSizes)
                {
                    node.SuspendPropertyNotifications();
                    node.ActualSize = Size.Empty;
                    node.ChildrenSize = Size.Empty;
                    node.ResumePropertyNotifications();
                }

                if ((node.ActualSize != Size.Empty || (!node.Visible && !IsInDesignMode)))
                {
                    node = node.NextVisibleNode;
                    //node = null;
                    //if (e.MoveNext())
                    //{
                    //    node = e.Current;
                    //}

                    if (node != null && node.Level == startNode.Level && stopOnSameLevel)
                    {
                        break;
                    }

                    continue;
                }

                if (!performLayout)
                {
                    performLayout = true;
                    this.ViewElement.SuspendLayout();
                    ((RadControl)this.ElementTree.Control).SuspendUpdate();
                }

                TreeNodeElement element = (TreeNodeElement)elementProvider.GetElementFromCache(node, null);
                if (element != null)
                {
                    element.SuspendThemeRefresh();
                    this.ViewElement.Children.Add(element);
                    element.ResumeThemeRefresh();
                }
                else
                {
                    element = (TreeNodeElement)elementProvider.CreateElement(node, null);
                    this.ViewElement.Children.Add(element);
                }

                element.Attach(node, null);

                element.Measure(new SizeF(element.ContentElement.TextWrap ? this.ViewElement.Size.Width : float.PositiveInfinity, float.PositiveInfinity));

                element.SuspendThemeRefresh();
                ViewElement.ElementProvider.CacheElement(element);
                this.ViewElement.Children.Remove(element);
                element.Detach();
                element.ResumeThemeRefresh();

                if (resetSizes && !node.Expanded)
                {
                    ResetChildrenSize(node.Nodes);
                }

                node = node.NextVisibleNode;
                //node = null;
                //if (e.MoveNext())
                //{
                //    node = e.Current;
                //}
                
                if (node != null && node.Level == startNode.Level && stopOnSameLevel)
                {
                    break;
                }
            }

            if (performLayout)
            {
                this.ViewElement.ResumeLayout(false, true);
                ((RadControl)this.ElementTree.Control).ResumeUpdate();
            }
        }

        protected virtual void UpdateScrollersOnAdd(RadTreeNode node)
        {
            if (!IsNodeVisible(node))
            {
                SynchronizeNodeElements();
                return;
            }

            UpdateActualSize(node, true, null);

            this.Scroller.UpdateScrollRange(root.ChildrenSize.Height, false);
            UpdateHScrollbarMaximum(root.ChildrenSize.Width);
            SynchronizeNodeElements();
        }

        protected virtual void UpdateScrollersOnExpand(RadTreeNode node)
        {
            if (node.ChildrenSize.Width == 0)
            {
                UpdateActualSize(node, true, null);
            }

            this.Scroller.UpdateScrollRange(root.ChildrenSize.Height, false);
            UpdateHScrollbarMaximum(root.ChildrenSize.Width);
        }

        protected virtual void UpdateScrollersOnCollapse(RadTreeNode node)
        {
            this.Scroller.UpdateScrollRange(this.VScrollBar.Maximum - node.ChildrenSize.Height, false);
            UpdateHScrollbarMaximum(root.ChildrenSize.Width);

            int value = Scroller.Scrollbar.Value;
            Scroller.Scrollbar.Value = 0;
            SetScrollValue(Scroller.Scrollbar, value);
        }

        internal void UpdateHScrollbarMaximum(int newMaximum)
        {
            if (this.HScrollBar.Maximum != newMaximum)
            {
                this.HScrollBar.Maximum = newMaximum;
                UpdateHScrollbarVisibility();
            }
        }

        private void CalculateHScrollbarMaximum(object state)
        {
            RadTreeNode skipNode = (RadTreeNode)state;
            int currentMaximum = this.HScrollBar.Maximum;
            int maxWidth = 0;
            RadTreeNode node = this.root.Nodes[0];
            while (node != null)
            {
                bool skip = false;
                RadTreeNode n = node;
                while (n != null)
                {
                    if (n == skipNode)
                    {
                        skip = true;
                        break;
                    }
                    n = n.Parent;
                }

                if (!skip && node.ActualSize.Width > maxWidth)
                {
                    maxWidth = node.ActualSize.Width;
                }
                node = node.NextVisibleNode;
            }
            calculatedHScrollbarMaximum = maxWidth;
        }

        protected virtual bool EndEditCore(bool commitChanges)
        {
            if (!IsEditing)
            {
                return false;
            }

            TreeNodeElement nodeElement = GetElement(this.SelectedNode);
            if (nodeElement == null)
            {
                return false;
            }

            if (commitChanges && this.ActiveEditor.IsModified)
            {
                SaveEditorValue(nodeElement, this.ActiveEditor.Value);
            }

            this.activeEditor.EndEdit();
            nodeElement.RemoveEditor(this.activeEditor);

            this.InvalidateMeasure();
            UpdateLayout();

            OnEdited(new TreeNodeEditedEventArgs(nodeElement, activeEditor, !commitChanges));

            this.activeEditor = null;

            return false;
        }

        protected virtual IInputEditor GetEditor(Type editorType)
        {
            IInputEditor editor = null;

            if (!this.cachedEditors.TryGetValue(editorType, out editor))
            {
                editor = Activator.CreateInstance(editorType) as IInputEditor;
            }

            if (editor != null && !this.cachedEditors.ContainsValue(editor))
            {
                this.cachedEditors.Add(editorType, editor);
            }

            return editor;
        }

        protected virtual void SaveEditorValue(TreeNodeElement nodeElement, object newValue)
        {
            // We consider Empty string as null value
            if (Object.Equals(newValue, String.Empty))
            {
                newValue = null;
            }

            TreeNodeValidatingEventArgs validatingArgs = new TreeNodeValidatingEventArgs(nodeElement, cachedOldValue, newValue);
            OnValueValidating(validatingArgs);
            if (validatingArgs.Cancel)
            {
                this.OnValidationError(EventArgs.Empty);
                return;
            }

            newValue = validatingArgs.NewValue;

            TreeNodeValueChangingEventArgs valueChangingArgs = new TreeNodeValueChangingEventArgs(this.SelectedNode, newValue, cachedOldValue);
            OnValueChanging(valueChangingArgs);

            if (!valueChangingArgs.Cancel)
            {
                SuspendProvider();
                this.SelectedNode.SuspendPropertyNotifications();
                this.SelectedNode.Value = valueChangingArgs.NewValue;
                this.SelectedNode.ResumePropertyNotifications();
                OnValueChanged(new TreeNodeValueChangedEventArgs(this.SelectedNode));
                ResumeProvider();
            }
        }

        protected internal void ResumeProvider()
        {
            if (this.treeNodeProvider != null)
            {
                this.treeNodeProvider.ResumeUpdate();
            }
        }

        protected internal void SuspendProvider()
        {
            if (this.treeNodeProvider != null)
            {
                this.treeNodeProvider.SuspendUpdate();
            }
        }

        private void SetScrollValue(RadScrollBarElement scrollbar, int newValue)
        {
            if (newValue > scrollbar.Maximum - scrollbar.LargeChange + 1)
            {
                newValue = scrollbar.Maximum - scrollbar.LargeChange + 1;
            }
            if (newValue < scrollbar.Minimum)
            {
                newValue = scrollbar.Minimum;
            }
            scrollbar.Value = newValue;
        }

        private Rectangle CreateSelectionRectangle(Point currentMouseLocation)
        {
            int width = Math.Abs(this.mouseDownLocation.X - currentMouseLocation.X);
            int height = Math.Abs(this.mouseDownLocation.Y - currentMouseLocation.Y);

            if (width == 0)
            {
                width = 1;
            }
            if (height == 0)
            {
                height = 1;
            }

            int x = Math.Min(this.mouseDownLocation.X, currentMouseLocation.X);
            int y = Math.Min(this.mouseDownLocation.Y, currentMouseLocation.Y);
            return new Rectangle(x, y, width, height);
        }

        private TreeNodeElement GetLastFullVisibleElement()
        {
            for (int i = ViewElement.Children.Count - 1; i >= 0; i--)
            {
                TreeNodeElement temp = (TreeNodeElement)ViewElement.Children[i];
                if (temp.ControlBoundingRectangle.Bottom <= ViewElement.ControlBoundingRectangle.Bottom)
                {
                    return temp;
                }
            }
            return null;
        }

        private TreeNodeElement GetLastPartialVisibleElement()
        {
            for (int i = ViewElement.Children.Count - 1; i >= 0; i--)
            {
                TreeNodeElement temp = (TreeNodeElement)ViewElement.Children[i];
                if (temp.ControlBoundingRectangle.Top < ViewElement.ControlBoundingRectangle.Bottom)
                {
                    return temp;
                }
            }
            return null;
        }

        private TreeNodeElement GetFirstFullVisibleElement()
        {
            for (int i = 0; i < ViewElement.Children.Count - 1; i++)
            {
                TreeNodeElement temp = (TreeNodeElement)ViewElement.Children[i];
                if (temp.ControlBoundingRectangle.Top >= ViewElement.ControlBoundingRectangle.Top)
                {
                    return temp;
                }
            }
            return null;
        }

        private TreeNodeElement GetFirstPartialVisibleElement()
        {
            for (int i = 0; i < ViewElement.Children.Count - 1; i++)
            {
                TreeNodeElement temp = (TreeNodeElement)ViewElement.Children[i];
                if (temp.ControlBoundingRectangle.Bottom > ViewElement.ControlBoundingRectangle.Top)
                {
                    return temp;
                }
            }
            return null;
        }

        private bool IsNodeVisible(RadTreeNode node)
        {
            if (!node.Visible && !IsInDesignMode)
            {
                return false;
            }
            node = node.Parent;
            while (node != null)
            {
                if (!node.Expanded || (!node.Visible && !IsInDesignMode))
                {
                    return false;
                }
                node = node.Parent;
            }
            return true;
        }

        private void ResetChildrenSize(RadTreeNodeCollection nodes)
        {
            if (nodes != null)
            {
                foreach (RadTreeNode node in nodes)
                {
                    node.SuspendPropertyNotifications();
                    node.ActualSize = Size.Empty;
                    node.ChildrenSize = Size.Empty;
                    node.ResumePropertyNotifications();
                    ResetChildrenSize(node.Nodes);
                }
            }
        }

        protected virtual TreeNodeElement EnsureNodeVisibleVertical(RadTreeNode node, TreeNodeElement nodeElement)
        {
            if (nodeElement == null)
            {
                // This performs any pending layouts
                // hence we have a valid element tree
                this.UpdateLayout();

                if (this.ViewElement.Children.Count > 0)
                {

                    int nodeIndex = GetNodeIndex(node);
                    int firstVisibleIndex = GetNodeIndex(((TreeNodeElement)this.ViewElement.Children[0]).Data);

                    if (nodeIndex <= firstVisibleIndex)
                    {
                        this.Scroller.ScrollToItem(node);
                    }
                    else
                    {
                        nodeElement = EnsureNodeVisibleVerticalCore(node);
                    }
                }
            }
            else
            {
                if (nodeElement.ControlBoundingRectangle.Bottom > this.ViewElement.ControlBoundingRectangle.Bottom)
                {
                    int offset = nodeElement.ControlBoundingRectangle.Bottom - this.ViewElement.ControlBoundingRectangle.Bottom;
                    this.SetScrollValue(this.VScrollBar, this.VScrollBar.Value + offset);
                }
                else if (nodeElement.ControlBoundingRectangle.Top < this.ViewElement.ControlBoundingRectangle.Top)
                {
                    int offset = this.ViewElement.ControlBoundingRectangle.Top - nodeElement.ControlBoundingRectangle.Top;
                    this.SetScrollValue(this.VScrollBar, this.VScrollBar.Value - offset);
                }
            }

            return nodeElement;
        }

        protected virtual TreeNodeElement EnsureNodeVisibleVerticalCore(RadTreeNode node)
        {
            bool start = false;
            int offset = 0;
            RadTreeNode lastVisible = ((TreeNodeElement)this.ViewElement.Children[this.ViewElement.Children.Count - 1]).Data;
            TreeViewTraverser traverser = (TreeViewTraverser)this.Scroller.Traverser.GetEnumerator();
            TreeNodeElement nodeElement = null;

            while (traverser.MoveNext())
            {
                if (traverser.Current == node)
                {
                    int oldMaximum = this.VScrollBar.Maximum;
                    this.SetScrollValue(this.VScrollBar, this.VScrollBar.Value + offset);
                    this.UpdateLayout();
                    nodeElement = this.GetElement(node);

                    if (nodeElement != null &&
                        nodeElement.ControlBoundingRectangle.Bottom > this.ViewElement.ControlBoundingRectangle.Bottom)
                    {
                        this.EnsureVisible(node);
                    }
                    break;
                }
                if (traverser.Current == lastVisible)
                {
                    start = true;
                }
                if (start)
                {
                    offset += (int)ViewElement.ElementProvider.GetElementSize(traverser.Current).Height + this.NodeSpacing;
                }
            }

            return nodeElement;
        }

        protected virtual void EnsureNodeVisibleHorizontal(RadTreeNode node, TreeNodeElement nodeElement)
        {
            if (nodeElement != null &&
                nodeElement.ContentElement.ControlBoundingRectangle.Left > this.ViewElement.ControlBoundingRectangle.Left &&
                nodeElement.ContentElement.ControlBoundingRectangle.Right < this.ViewElement.ControlBoundingRectangle.Right)
            {
                return;
            }

            if (nodeElement != null && this.HScrollBar.Value + this.HScrollBar.LargeChange < nodeElement.ContentElement.ControlBoundingRectangle.Right)
            {
                int offset = node.ActualSize.Width - (this.HScrollBar.Value + this.HScrollBar.LargeChange);
                this.SetScrollValue(this.HScrollBar, this.HScrollBar.Value + offset);
            }
            else
            {
                int level = node.Level;
                int width = node.ActualSize.Width - TreeIndent * level;
                this.SetScrollValue(this.HScrollBar, node.ActualSize.Width - width);
            }
        }

        #endregion

        #region Selection

        private void ProcessSelection(RadTreeNode node, Keys modifierKeys, bool isMouseSelection)
        {
            if (node == null || this.updateSelectionChanged > 0)
            {
                return;
            }

            this.updateSelectionChanged++;
            this.BeginUpdate();

            bool isShiftPressed = (modifierKeys & Keys.Shift) == Keys.Shift;
            bool isControlPressed = (modifierKeys & Keys.Control) == Keys.Control;
            bool clearSelection = this.MultiSelect && (isShiftPressed || !isControlPressed ||
                                  (!isMouseSelection && !isShiftPressed && !isControlPressed));

            if (!node.Current)
            {
                if (!this.ProcessCurrentNode(node, clearSelection))
                {
                    this.EndUpdate(false, UpdateActions.StateChanged);
                    this.updateSelectionChanged--;
                    return;
                }
            }
            else if (clearSelection)
            {
                this.ClearSelection();
                node.Selected = true;
                anchorPosition = node;
                anchorIndex = -1;
            }

            if (this.MultiSelect)
            {
                if (!isShiftPressed)
                {
                    this.anchorPosition = node;
                    this.anchorIndex = -1;

                    if (isMouseSelection)
                    {
                        node.Selected = isControlPressed ? !node.Selected : true;
                    }
                    else if (!isControlPressed)
                    {
                        node.Selected = true;
                    }
                }
                else
                {
                    if (this.anchorPosition == null)
                    {
                        this.anchorPosition = node;
                    }

                    if (this.anchorIndex == -1)
                    {
                        this.anchorIndex = this.GetNodeIndex(this.anchorPosition);
                    }

                    bool forward = anchorIndex < GetNodeIndex(node);
                    RadTreeNode temp = anchorPosition;
                    node.Selected = true;

                    while (temp != node)
                    {
                        temp.Selected = true;
                        temp = forward ? temp.NextVisibleNode : temp.PrevVisibleNode;
                    }
                }
            }
            else
            {
                this.anchorPosition = node;
                this.anchorIndex = -1;
            }

            this.EndUpdate(true, UpdateActions.StateChanged);
            this.updateSelectionChanged--;
        }

        internal bool ProcessCurrentNode(RadTreeNode node, bool clearSelection)
        {
            if (this.updateCurrentNodeChanged > 0)
            {
                return true;
            }

            this.updateCurrentNodeChanged++;
            RadTreeViewCancelEventArgs args = new RadTreeViewCancelEventArgs(node);
            this.OnSelectedNodeChanging(args);

            if (args.Cancel)
            {
                this.updateCurrentNodeChanged--;
                return false;
            }

            if (clearSelection)
            {
                this.ClearSelection();
            }

            if (this.selected != null)
            {
                this.selected.Current = false;
            }

            this.selected = node;

            if (this.updateSelectionChanged == 0)
            {
                this.anchorPosition = this.selected;
                this.anchorIndex = -1;
            }

            if (this.TreeNodeProvider != null)
            {
                this.TreeNodeProvider.SetCurrent(this.selected);
            }

            if (this.selected != null)
            {
                this.selected.Current = true;
                this.BringIntoView(this.selected);
            }

            this.OnNotifyPropertyChanged("SelectedNode");
            this.OnSelectedNodeChanged(new RadTreeViewEventArgs(node));
            this.updateCurrentNodeChanged--;
            this.Update(UpdateActions.StateChanged);
            return true;
        }

        private bool ExtendSelectionUp(Point location)
        {
            if (location.Y >= this.ViewElement.ControlBoundingRectangle.Top)
            {
                return false;
            }

            RadTreeNode node = this.GetNodeAt(this.mouseDownLocation);

            if (node == null || this.VScrollBar.Value == this.VScrollBar.Minimum)
            {
                return false;
            }

            TreeViewTraverser treeTraverser = new TreeViewTraverser(this);
            treeTraverser.Position = node;

            Rectangle containerBounds = this.ViewElement.ControlBoundingRectangle;
            int delta = this.ViewElement.ControlBoundingRectangle.Top - location.Y;

            this.VScrollBar.Value = this.ClampValue(this.VScrollBar.Value - delta,
                                                    this.VScrollBar.Minimum,
                                                    this.VScrollBar.Maximum - this.VScrollBar.LargeChange + 1);

            this.mouseDownLocation.Y = this.ClampValue(this.mouseDownLocation.Y + delta, containerBounds.Y, containerBounds.Bottom - 2);
            this.UpdateLayout();

            TreeNodeElement firstNode = this.GetFirstPartialVisibleElement();

            if (firstNode != null)
            {
                do
                {
                    if (treeTraverser.Current == null)
                    {
                        break;
                    }

                    treeTraverser.Current.Selected = true;

                    if (treeTraverser.Current == firstNode.Data)
                    {
                        break;
                    }
                }
                while (treeTraverser.MovePrevious());
            }

            return true;
        }

        private bool ExtendSelectionDown(Point location)
        {
            if (location.Y <= this.ViewElement.ControlBoundingRectangle.Bottom)
            {
                return false;
            }

            RadTreeNode node = this.GetNodeAt(this.mouseDownLocation);

            if (node == null || this.VScrollBar.Value >= this.VScrollBar.Maximum - this.VScrollBar.LargeChange + 1)
            {
                return false;
            }

            TreeViewTraverser treeTraverser = new TreeViewTraverser(this);
            treeTraverser.Position = node;

            Rectangle containerBounds = this.ViewElement.ControlBoundingRectangle;
            int delta = location.Y - containerBounds.Bottom;

            this.VScrollBar.Value = this.ClampValue(this.VScrollBar.Value + delta,
                                                    this.VScrollBar.Minimum,
                                                    this.VScrollBar.Maximum - this.VScrollBar.LargeChange + 1);

            this.mouseDownLocation.Y = this.ClampValue(mouseDownLocation.Y - delta,
                                                       containerBounds.Y + 1, containerBounds.Bottom);
            this.UpdateLayout();

            TreeNodeElement lastNode = this.GetLastPartialVisibleElement();

            if (lastNode != null)
            {
                do
                {
                    treeTraverser.Current.Selected = true;

                    if (treeTraverser.Current == lastNode.Data)
                    {
                        break;
                    }
                }
                while (treeTraverser.MoveNext());
            }

            return true;
        }

        private int ClampValue(int value, int minimum, int maximum)
        {
            if (value < minimum)
            {
                return minimum;
            }
            if (maximum > 0 && value > maximum)
            {
                return maximum;
            }
            return value;
        }

        #endregion

        #region Drag & drop

        protected virtual TreeViewDragDropService CreateDragDropService()
        {
            return new TreeViewDragDropService(this);
        }

        protected virtual internal void AutoExpand(RadTreeNode node)
        {
            if (!node.Expanded)
            {
                this.expandTimer.Tag = node;
                this.expandTimer.Start();
            }
        }

        protected virtual internal void AutoScroll(TreeNodeElement hitItem)
        {
            if (this.ViewElement.Children.Count <= 0)
            {
                return;
            }

            if (this.ViewElement.Children[0] == hitItem)
            {
                this.VScrollBar.PerformSmallDecrement(1);
                return;
            }

            if (this.ViewElement.Children[this.ViewElement.Children.Count - 1] == hitItem)
            {
                this.VScrollBar.PerformSmallIncrement(1);
            }
        }

        #endregion

        #region Event handlers

        private void Scroller_ScrollerUpdated(object sender, EventArgs e)
        {
            firstVisibleIndex = -1;
        }

        private void ExpandTimer_Tick(object sender, EventArgs e)
        {
            this.expandTimer.Stop();

            if (this.expandTimer.Tag != null)
            {
                ((RadTreeNode)this.expandTimer.Tag).Expand();
            }

            this.expandTimer.Tag = null;
        }

        private void ScrollTimer_Tick(object sender, EventArgs e)
        {
            Point mousePos = this.ElementTree.Control.PointToClient(Control.MousePosition);

            if (this.ControlBoundingRectangle.Contains(mousePos))
            {
                scrollTimer.Enabled = false;
                return;
            }

            if (this.MultiSelect)
            {
                if (!this.ExtendSelectionDown(this.mouseMoveLocation))
                {
                    this.ExtendSelectionUp(this.mouseMoveLocation);
                }
            }
        }

        private void mouseUpTimer_Tick(object sender, EventArgs e)
        {
            mouseUpTimer.Stop();
            if (lastClickedNode != null && lastClickedNode.Current)
            {
                lastClickedNode = null;
                BeginEdit();
            }
        }

        private bool PerformExpressionFilter(RadTreeNode node)
        {
            if (this.filterDescriptors.Count == 0)
            {
                return true;
            }

            try
            {
                int count = node.Nodes.Count;   //needed for recursive lazy processing
                if (node.Nodes.Count > 0)
                {
                    return true;
                }

                ExpressionNode expressionNode = ExpressionParser.Parse(this.filterDescriptors.Expression, false);
                ExpressionContext context = ExpressionContext.Context;
                context.Clear();
                foreach (FilterDescriptor descriptor in this.filterDescriptors)
                {
                    if (context.ContainsKey(descriptor.PropertyName))
                    {
                        context[descriptor.PropertyName] = node.Text;
                    }
                    else
                    {
                        context.Add(descriptor.PropertyName, node.Text);
                    }
                }

                object result = expressionNode.Eval(null, context);
                if (result is bool)
                {
                    return (bool)result;
                }
            }
            catch (Exception ex)
            {
                throw new FilterExpressionException("Invalid filter expression.", ex);
            }

            return false;
        }

        private void listSource_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                if (this.treeNodeProvider != null)
                {
                    this.treeNodeProvider.Reset();
                }

                this.root.Nodes.Reset();
                this.Update(UpdateActions.Reset);
            }
        }

        private void filterDescriptors_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.listSource.CollectionView.FilterExpression = this.filterDescriptors.Expression;
            this.Update(UpdateActions.Resume);
        }

        protected override void OnPropertyChanged(RadPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property == ShowLinesProperty ||
                e.Property == ShowRootLinesProperty ||
                e.Property == ShowExpandCollapseProperty ||
                e.Property == LineColorProperty ||
                e.Property == AlternatingRowColorProperty ||
                e.Property == LineStyleProperty)
            {
                this.SynchronizeNodeElements();
            }
            else if (e.Property == AllowPlusMinusAnimationProperty)
            {
                this.Update(UpdateActions.Reset);
            }
            else if (e.Property == TreeIndentProperty)
            {
                this.ViewElement.UpdateItems();
            }
            else if (e.Property == NodeSpacingProperty)
            {
                this.ViewElement.ItemSpacing = (int)e.NewValue;
                this.Scroller.ItemSpacing = this.ViewElement.ItemSpacing;
            }
            else if (e.Property == AllowArbitraryItemHeightProperty)
            {
                this.ViewElement.UpdateItems();
                this.UpdateLayout();
                this.Scroller.UpdateScrollRange();
            }
            else if (e.Property == ItemHeightProperty)
            {
                this.Scroller.ItemHeight = this.ItemHeight;
                this.ViewElement.UpdateItems();
                this.UpdateLayout();
                this.Scroller.UpdateScrollRange();

            }
            else if (e.Property == FullRowSelectProperty)
            {
                this.FitItemsToSize = (bool)e.NewValue;
                Update(UpdateActions.Reset);
            }
        }

        protected virtual void SynchronizeNodeElements()
        {
            foreach (TreeNodeElement element in this.ViewElement.Children)
            {
                element.Synchronize();
            }
            this.Invalidate();
        }

        protected override void OnNotifyPropertyChanged(PropertyChangedEventArgs e)
        {
            base.OnNotifyPropertyChanged(e);

            if (e.PropertyName == "TriStateMode")
            {
                this.SynchronizeNodeElements();

                if (!this.TriStateMode)
                {
                    TreeViewTraverser traverser = new TreeViewTraverser(this);

                    foreach (RadTreeNode node in traverser)
                    {
                        if (node.CheckState == ToggleState.Indeterminate)
                        {
                            node.CheckState = ToggleState.Off;
                        }
                    }
                }
            }
            else if (e.PropertyName == "CheckBoxes")
            {
                this.Update(UpdateActions.StateChanged);
            }
        }

        protected override void OnStyleChanged(RadPropertyChangedEventArgs e)
        {
            base.OnStyleChanged(e);
            this.ViewElement.ItemSpacing = this.NodeSpacing;
            this.Scroller.ItemSpacing = this.NodeSpacing;
        }

        protected override void OnAutoSizeChanged()
        {
            if (!AutoSizeItems && root.Nodes.Count > 0)
            {
                RadTreeNode node = root.Nodes[0];
                while (node != null)
                {
                    node.ItemHeight = -1;
                    node = node.NextVisibleNode;
                }
            }

            base.OnAutoSizeChanged();
        }

        protected internal virtual bool ProcessMouseDown(MouseEventArgs e)
        {
            this.doNotStartEditingOnMouseUp = false;
            this.mouseDownLocation = e.Location;
            this.enterEditMode = false;
            this.enableAutoExpand = true;
            this.performSelectionOnMouseUp = false;
            this.draggedNode = null;
            bool isToggleHinted = this.IsToggle(mouseDownLocation);

            lastClickedNode = null;

            if (this.IsScrollBar(mouseDownLocation) ||
                this.IsExpander(mouseDownLocation) ||
                this.IsEditorHint(mouseDownLocation))
            {
                return false;
            }

            RadTreeNode node = this.GetNodeAt(mouseDownLocation, this.FullRowSelect);

            if (node == null)
            {
                if (this.activeEditor != null)
                {
                    this.EndEdit();
                }

                return false;
            }


            if (e.Button == MouseButtons.Right)
            {
                this.SelectedNode = node;
                return false;
            }

            if (e.Button == MouseButtons.Left)
            {
                this.draggedNode = node;

                if (node.Selected && !isToggleHinted)
                {
                    if (this.activeEditor != null && !this.IsEditorHint(e.Location))
                    {
                        this.EndEdit();
                        return false;
                    }

                    this.enterEditMode = this.activeEditor == null && this.allowEdit &&
                                         node.Current && this.SelectedNodes.Count <= 1;


                    if (this.AllowDragDrop)
                    {
                        this.performSelectionOnMouseUp = true;
                        this.DragDropService.Start(this.GetElement(node));
                        return false;
                    }
                }

                if (this.activeEditor != null)
                {
                    this.EndEdit();
                    this.enterEditMode = false;
                }

                this.ProcessSelection(node, Control.ModifierKeys, true);

                // We may start drag drop immediately after the new node is selected
                if (this.SelectedNode == node && !isToggleHinted && !this.MultiSelect)
                {
                    this.DragDropService.Start(this.GetElement(node));
                }
            }

            return false;
        }

        protected internal virtual bool ProcessMouseUp(MouseEventArgs e)
        {
            RadTreeNode node = this.GetNodeAt(e.Location, this.FullRowSelect);

            if (this.scrollTimer.Enabled)
            {
                this.scrollTimer.Enabled = false;
            }

            if (this.IsScrollBar(e.Location) || this.IsExpander(e.Location) ||
                this.IsEditorHint(e.Location) || e.Button != MouseButtons.Left || this.IsToggle(mouseDownLocation) || node == null)
            {
                return false;
            }

            if (this.performSelectionOnMouseUp)
            {
                this.ProcessSelection(node, Control.ModifierKeys, true);
            }

            if (this.enterEditMode && !doNotStartEditingOnMouseUp)
            {
                lastClickedNode = node;
                mouseUpTimer.Start();
                return false;
            }

            if (this.enableAutoExpand && node == this.SelectedNode && this.ExpandMode == UI.ExpandMode.Single)
            {
                node.Expanded = !node.Expanded;
            }

            return false;
        }

        protected internal virtual bool ProcessMouseMove(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && this.MultiSelect &&
                this.draggedNode != null && this.DragDropService.State != RadServiceState.Working)
            {
                this.performSelectionOnMouseUp = false;
                RadTreeNode nodeUnderMouse = this.GetNodeAt(e.Location);

                if (nodeUnderMouse != null)
                {
                    this.scrollTimer.Stop();
                    this.ProcessMouseSelection(e.Location);
                }
                else
                {
                    this.mouseMoveLocation = e.Location;

                    if (!scrollTimer.Enabled)
                    {
                        scrollTimer.Enabled = true;
                    }
                }
            }

            return false;
        }

        private void ProcessMouseSelection(Point location)
        {
            Rectangle rect = this.CreateSelectionRectangle(location);
            Dictionary<int, RadTreeNode> selectedList = new Dictionary<int, RadTreeNode>();

            this.BeginUpdate();

            this.draggedNode.Current = true;

            foreach (TreeNodeElement element in this.ViewElement.Children)
            {
                if (element.ControlBoundingRectangle.IntersectsWith(rect))
                {
                    element.Data.Selected = true;
                    selectedList.Add(element.Data.GetHashCode(), element.Data);
                }
            }

            if (Control.ModifierKeys != Keys.Control)
            {
                List<RadTreeNode> nodesToExclude = new List<RadTreeNode>();
                foreach (RadTreeNode node in this.SelectedNodes)
                {
                    if (!selectedList.ContainsKey(node.GetHashCode()))
                    {
                        nodesToExclude.Add(node);
                    }
                }
                for (int i = 0; i < nodesToExclude.Count; i++)
                {
                    nodesToExclude[i].Selected = false;
                }
            }

            this.EndUpdate(true, UpdateActions.StateChanged);
        }

        protected internal virtual bool ProecessMouseEnter(EventArgs e)
        {
            if (this.AllowPlusMinusAnimation)
            {
                foreach (TreeNodeElement node in this.ViewElement.Children)
                {
                    AnimatedPropertySetting animatedExpander = new AnimatedPropertySetting(RadItem.OpacityProperty,
                        10, 1, (object)(this.PlusMinusAnimationStep));

                    animatedExpander.StartValue = (double)0.0;
                    animatedExpander.EndValue = (double)1.0;
                    animatedExpander.ApplyEasingType = RadEasingType.Linear;
                    animatedExpander.UnapplyEasingType = RadEasingType.Linear;
                    animatedExpander.RemoveAfterApply = false;
                    animatedExpander.ApplyValue(node.ExpanderElement);
                }
            }

            return false;
        }

        protected internal virtual bool ProecessMouseLeave(EventArgs e)
        {
            if (this.AllowPlusMinusAnimation)
            {
                foreach (TreeNodeElement node in this.ViewElement.Children)
                {
                    AnimatedPropertySetting animatedExpander = new AnimatedPropertySetting(RadItem.OpacityProperty,
                        10, 1, (object)(this.PlusMinusAnimationStep));

                    animatedExpander.StartValue = (double)1.0;
                    animatedExpander.EndValue = (double)0.0;
                    animatedExpander.ApplyEasingType = RadEasingType.Linear;
                    animatedExpander.UnapplyEasingType = RadEasingType.Linear;
                    animatedExpander.RemoveAfterApply = false;
                    animatedExpander.ApplyValue(node.ExpanderElement);
                }
            }

            return false;
        }

        protected internal virtual bool ProcessMouseClick(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && !this.IsExpander(e.Location) && !this.IsToggle(e.Location) && ExpandMode == UI.ExpandMode.Multiple)
            {
                RadTreeNode node = GetNodeAt(e.Location);
                if (node != null && !this.AllowEdit && ToggleMode == UI.ToggleMode.SingleClick)
                {
                    node.Expanded = !node.Expanded;
                }
            }
            return false;
        }

        protected internal virtual bool ProcessMouseDoubleClick(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && !this.IsExpander(e.Location) && !this.IsToggle(e.Location) && ExpandMode == UI.ExpandMode.Multiple)
            {
                RadTreeNode node = GetNodeAt(e.Location);

                if (node != null && ToggleMode == UI.ToggleMode.DoubleClick)
                {
                    node.Expanded = !node.Expanded;
                }
            }

            lastClickedNode = null;
            mouseUpTimer.Stop();
            doNotStartEditingOnMouseUp = true;

            return false;
        }

        protected internal virtual bool ProcessMouseWheel(MouseEventArgs e)
        {
            int step = Math.Max(1, e.Delta / SystemInformation.MouseWheelScrollDelta);
            int delta = Math.Sign(e.Delta) * step * SystemInformation.MouseWheelScrollLines;

            ScrollTo(delta);
            return false;
        }

        protected internal virtual bool ProcessKeyDown(KeyEventArgs e)
        {
            if (this.IsEditing)
            {
                return false;
            }

            switch (e.KeyCode)
            {
                case Keys.Prior:
                    this.HandlePageUp(e);
                    break;
                case Keys.Next:
                    this.HandlePageDown(e);
                    break;
                case Keys.Left:
                    HandleLeftKey(e);
                    break;
                case Keys.Right:
                    HandleRightKey(e);
                    break;
                case Keys.Up:
                    HandleUpKey(e);
                    break;
                case Keys.Down:
                    HandleDownKey(e);
                    break;
                case Keys.End:
                    HandleEndKey(e);
                    break;
                case Keys.Home:
                    HandleHomeKey(e);
                    break;
                case Keys.Return:
                    HandleReturnKey(e);
                    break;
                case Keys.Multiply:
                    HandleMultiplyKey(e);
                    break;
                case Keys.Divide:
                    HandleDivideKey(e);
                    break;
                case Keys.Add:
                    HandleAddKey(e);
                    break;
                case Keys.Subtract:
                    HandleSubtractKey(e);
                    break;
                case Keys.Space:
                    HandleSpaceKey(e);
                    break;
                case Keys.Escape:
                    HandleEscapeKey(e);
                    break;
                case Keys.F2:
                    HandleF2Key(e);
                    break;
                case Keys.Delete:
                    HandleDelKey(e);
                    break;
            }

            return false;
        }

        protected internal virtual bool ProcessContextMenu(Point location)
        {
            if (IsScrollBar(location))
            {
                return false;
            }

            if (this.activeEditor != null)
            {
                this.EndEdit();
            }

            RadContextMenu menu = this.ContextMenu;
            RadTreeNode node = this.GetNodeAt(location);

            if (node != null && node.ContextMenu != null)
            {
                menu = node.ContextMenu;
            }

            if (node == null && menu is TreeViewDefaultContextMenu)
            {
                menu = null;
            }

            if (menu != null)
            {
                TreeViewDefaultContextMenu defaultMenu = menu as TreeViewDefaultContextMenu;

                if (defaultMenu != null)
                {
                    if (node.Expanded && node.Nodes.Count > 0)
                    {
                        defaultMenu.ExpandCollapseMenuItem.Text = TreeViewLocalizationProvider.CurrentProvider.GetLocalizedString(TreeViewStringId.ContextMenuCollapse);
                    }
                    else
                    {
                        defaultMenu.ExpandCollapseMenuItem.Text = TreeViewLocalizationProvider.CurrentProvider.GetLocalizedString(TreeViewStringId.ContextMenuExpand);
                    }
                    defaultMenu.ExpandCollapseMenuItem.Enabled = node.Nodes.Count > 0;
                    defaultMenu.EditMenuItem.Enabled = AllowEdit;
                    RadTreeView treeView = this.ElementTree.Control as RadTreeView;
                    if (treeView != null)
                    {
                        defaultMenu.AddMenuItem.Enabled = AllowAdd;
                        defaultMenu.DeleteMenuItem.Enabled = AllowRemove;
                    }
                }

                if (node != null)
                {
                    this.SelectedNode = node;
                }

                TreeViewContextMenuOpeningEventArgs args = new TreeViewContextMenuOpeningEventArgs(node, menu);
                OnContextMenuOpening(args);

                if (!args.Cancel)
                {
                    menu.Show(this.ElementTree.Control, location);
                    return true;
                }
            }

            return false;
        }

        protected override void OnLoaded()
        {
            base.OnLoaded();
            if (this.pendingScrollerUpdates)
            {
                this.pendingScrollerUpdates = false;
                this.Update(UpdateActions.Resume);
            }
        }

        #endregion

        #region Keyboard handling

        private void HandleF2Key(KeyEventArgs e)
        {
            if (this.IsEditing)
            {
                return;
            }

            e.Handled = this.BeginEdit();
        }

        private void HandleDelKey(KeyEventArgs e)
        {
            if (this.IsEditing || !this.AllowRemove)
            {
                return;
            }

            RadTreeNode node = this.SelectedNode;
            if (node == null)
            {
                return;
            }

            RadTreeNodeCollection nodes = this.Nodes;
            RadTreeNode parent = node.Parent;
            if (parent != null)
            {
                nodes = parent.Nodes;
            }

            nodes.Remove(node);
        }

        private void HandleEscapeKey(KeyEventArgs e)
        {
            if (dragDropService.State == RadServiceState.Working)
            {
                this.dragDropService.Stop(false);
                doNotStartEditingOnMouseUp = true;
            }
        }

        private void HandleSpaceKey(KeyEventArgs e)
        {
            RadTreeNode selectedNode = this.SelectedNode;

            if (selectedNode != null)
            {
                TreeNodeElement nodeElement = this.GetElement(selectedNode);

                if (nodeElement != null)
                {
                    if (nodeElement.ToggleElement != null)
                    {
                        nodeElement.ToggleElement.Focus();
                    }
                    else
                    {
                        nodeElement.Focus();
                    }
                }

                if (MultiSelect && e.Modifiers == Keys.Control)
                {
                    selectedNode.Selected = !selectedNode.Selected;
                }
            }
        }

        private void HandleSubtractKey(KeyEventArgs e)
        {
            RadTreeNode selectedNode = this.SelectedNode;
            if (selectedNode != null)
            {
                selectedNode.Collapse(true);
            }
        }

        private void HandleAddKey(KeyEventArgs e)
        {
            RadTreeNode selectedNode = this.SelectedNode;
            if (selectedNode != null)
            {
                selectedNode.Expand();
            }
        }

        private void HandleDivideKey(KeyEventArgs e)
        {
            RadTreeNode selectedNode = this.SelectedNode;
            if (selectedNode != null)
            {
                selectedNode.Collapse();
            }
        }

        private void HandleMultiplyKey(KeyEventArgs e)
        {
            RadTreeNode selectedNode = this.SelectedNode;
            if (selectedNode != null)
            {
                selectedNode.ExpandAll();
            }
        }

        private void HandleReturnKey(KeyEventArgs e)
        {
            RadTreeNode selectedNode = this.SelectedNode;
            if (selectedNode != null)
            {
                selectedNode.Toggle();
            }
        }

        private void HandleHomeKey(KeyEventArgs e)
        {
            RadTreeNode selectedNode = this.TopNode;
            if (selectedNode != null)
            {
                this.SelectedNode = selectedNode;
                this.SelectedNode.EnsureVisible();
            }
        }

        private void HandleEndKey(KeyEventArgs e)
        {
            RadTreeNode selectedNode = this.LastNode;
            if (selectedNode != null)
            {
                this.SelectedNode = selectedNode;
                this.SelectedNode.EnsureVisible();
            }
        }

        private void HandleLeftKey(KeyEventArgs e)
        {
            if (e.Modifiers == Keys.Control && this.HScrollBar != null)     //explorer mode / implement focus mode like VS
            {
                this.HScrollBar.PerformSmallDecrement(1);
                return;
            }

            RadTreeNode selectedNode = this.SelectedNode;

            if (selectedNode == null)
            {
                return;
            }

            bool isRightToLeft = this.ElementTree.Control.RightToLeft == System.Windows.Forms.RightToLeft.Yes;

            if (selectedNode.Nodes.Count > 0)
            {
                if (isRightToLeft)
                {
                    if (!selectedNode.Expanded)
                    {
                        selectedNode.Expand();
                        return;
                    }
                }
                else if (selectedNode.Expanded)
                {
                    selectedNode.Collapse(true);
                    return;
                }
            }

            if (e.Modifiers == Keys.Shift)
            {
                return;
            }

            if (isRightToLeft)
            {
                if (selectedNode.Nodes.Count > 0)
                {
                    this.SelectedNode = selectedNode.Nodes[0];
                    EnsureVisible(this.SelectedNode);
                }
            }
            else if (selectedNode.Parent != null)
            {
                this.SelectedNode = selectedNode.Parent;
                EnsureVisible(this.SelectedNode);
            }
        }

        private void HandleRightKey(KeyEventArgs e)
        {
            if (e.Modifiers == Keys.Control && this.HScrollBar != null)     //explorer mode / implement focus mode like VS
            {
                this.HScrollBar.PerformSmallIncrement(1);
                return;
            }

            RadTreeNode selectedNode = this.SelectedNode;

            if (selectedNode == null)
            {
                return;
            }

            bool isRighToLeft = this.ElementTree.Control.RightToLeft == System.Windows.Forms.RightToLeft.Yes;

            if (selectedNode.Nodes.Count > 0)
            {
                if (isRighToLeft)
                {
                    if (selectedNode.Expanded)
                    {
                        selectedNode.Collapse(true);
                        return;
                    }
                }
                else if (!selectedNode.Expanded)
                {
                    selectedNode.Expand();
                    return;
                }
            }

            if (e.Modifiers == Keys.Shift)
            {
                return;
            }

            if (isRighToLeft)
            {
                if (selectedNode.Parent != null)
                {
                    this.SelectedNode = selectedNode.Parent;
                    EnsureVisible(this.SelectedNode);
                }
            }
            else if (selectedNode.Nodes.Count > 0)
            {
                this.SelectedNode = selectedNode.Nodes[0];
                EnsureVisible(this.SelectedNode);
            }
        }

        private void HandleUpKey(KeyEventArgs e)
        {
            RadTreeNode node = this.SelectedNode;

            if (node == null || node.PrevVisibleNode == null)
            {
                return;
            }

            this.ProcessSelection(node.PrevVisibleNode, Control.ModifierKeys, false);
        }

        private void HandleDownKey(KeyEventArgs e)
        {
            RadTreeNode node = this.SelectedNode;

            if (node == null && this.Nodes.Count > 0)
            {
                node = this.Nodes[0];
            }

            if (node == null || node.NextVisibleNode == null)
            {
                return;
            }

            this.ProcessSelection(node.NextVisibleNode, Control.ModifierKeys, false);
        }

        private void HandlePageDown(KeyEventArgs e)
        {
            TreeNodeElement treeNodeElement = GetLastFullVisibleElement();

            if (treeNodeElement != null)
            {
                if (!treeNodeElement.Data.Current)
                {
                    this.SelectedNode = treeNodeElement.Data;
                    this.SelectedNode.Selected = true;
                    return;
                }
                else
                {
                    int delta = treeNodeElement.ControlBoundingRectangle.Top;
                    this.SetScrollValue(this.VScrollBar, this.VScrollBar.Value + delta);
                    this.ViewElement.UpdateItems();
                    this.UpdateLayout();
                    treeNodeElement = GetLastFullVisibleElement();

                    if (treeNodeElement != null)
                    {
                        this.SelectedNode = treeNodeElement.Data;
                        this.SelectedNode.Selected = true;
                    }
                }
            }
        }

        private void HandlePageUp(KeyEventArgs e)
        {
            TreeNodeElement treeNodeElement = GetFirstFullVisibleElement();

            if (treeNodeElement != null)
            {
                if (!treeNodeElement.Data.Current)
                {
                    this.SelectedNode = treeNodeElement.Data;
                    this.SelectedNode.Selected = true;
                    return;
                }
                else
                {
                    int delta = ViewElement.ControlBoundingRectangle.Height - treeNodeElement.ControlBoundingRectangle.Bottom;
                    this.SetScrollValue(this.VScrollBar, this.VScrollBar.Value - delta);
                    this.ViewElement.UpdateItems();
                    this.UpdateLayout();
                    treeNodeElement = GetFirstFullVisibleElement();

                    if (treeNodeElement != null)
                    {
                        this.SelectedNode = treeNodeElement.Data;
                        this.SelectedNode.Selected = true;
                    }
                }
            }
        }

        private int GetNodeIndex(RadTreeNode position)
        {
            if (root.Nodes.Count == 0)
            {
                return -1;
            }

            int index = 0;

            RadTreeNode temp = root.Nodes[0];

            while (temp != null)
            {
                if (temp == position)
                {
                    break;
                }
                temp = temp.NextVisibleNode;
                index++;
            }
            return index;
        }

        #endregion

        #region IDataItemSource Members

        IDataItem IDataItemSource.NewItem()
        {
            return CreateNewNode();
        }

        void IDataItemSource.BindingComplete()
        {
        }

        protected internal virtual RadTreeNode CreateNewNode()
        {
            CreateTreeNodeEventArgs args = new CreateTreeNodeEventArgs();
            OnCreateNode(args);

            if (args.Node != null)
            {
                return args.Node;
            }

            RadTreeNode node = new RadTreeNode();
            if (this.listSource.IsDataBound)
            {
                node.Parent = this.root;
            }

            return node;
        }

        void IDataItemSource.Initialize()
        {
            ResetDescriptors();

            TreeNodeComparer comparer = this.listSource.CollectionView.Comparer as TreeNodeComparer;
            if (comparer != null)
            {
                comparer.Update();
            }

            EnsureNodeProvider();
        }

        private bool ContainsObjectRelation
        {
            get
            {
                if (string.IsNullOrEmpty(this.childMember))
                {
                    return false;
                }

                string[] childMembers = this.childMember.Split('\\');
                if (childMembers.Length > 1)
                {
                    return true;
                }

                return false;
            }
        }

        private void EnsureNodeProvider()
        {
            if (this.listSource.IsDataBound)
            {
                if (this.ContainsObjectRelation)
                {
                    if (!(this.treeNodeProvider is ObjectRelationalProvider))
                    {
                        this.treeNodeProvider = new ObjectRelationalProvider(this);
                    }
                }
                else if (!(this.treeNodeProvider is BindingProvider))
                {
                    this.treeNodeProvider = new BindingProvider(this);
                }

                this.TreeNodeProvider.Reset();
                this.Nodes.Reset();
                return;
            }

            this.treeNodeProvider = null;
            this.Nodes.Reset();
        }

        internal RadListSource<RadTreeNode> ListSource
        {
            get { return listSource; }
        }

        internal bool IsScrollBar(Point point)
        {
            if (this.ElementTree == null)
            {
                return false;
            }

            RadElement scrollBar = this.ElementTree.GetElementAtPoint(point);

            if (scrollBar != null)
            {
                return scrollBar.FindAncestor<RadScrollBarElement>() != null;
            }

            return false;
        }

        internal RadTreeNode Root
        {
            get { return this.root; }
        }

        internal List<TreeNodeDescriptor> BoundDescriptors
        {
            get { return this.boundDescriptors; }
        }

        #endregion

        #region Layout

        protected override bool UpdateOnMeasure(SizeF availableSize)
        {
            if (this.AllowAlternatingRowColor)
            {
                firstVisibleIndex = -1;
            }

            RectangleF clientRect = GetClientRectangle(availableSize);
            SizeF clientSize = clientRect.Size;
            ElementVisibility visibility = this.HScrollBar.Visibility;
            RadTreeViewVirtualizedContainer container = (RadTreeViewVirtualizedContainer)this.ViewElement;

            this.HScrollBar.Maximum = this.root.ChildrenSize.Width;
            int scrollbarOffset = this.VScrollBar.Visibility == ElementVisibility.Visible ? (int)this.VScrollBar.DesiredSize.Width : 0;
            this.HScrollBar.LargeChange = (int)(clientRect.Width - scrollbarOffset - this.ViewElement.Margin.Horizontal);
            if (this.HScrollBar.Value > this.HScrollBar.Maximum - this.HScrollBar.LargeChange + 1)
            {
                SetScrollValue(this.HScrollBar, this.HScrollBar.Maximum - this.HScrollBar.LargeChange + 1);
            }
            this.HScrollBar.SmallChange = this.HScrollBar.LargeChange / 10;
            UpdateHScrollbarVisibility();

            if (this.HScrollBar.Visibility == ElementVisibility.Visible)
            {
                clientSize.Height -= HScrollBar.DesiredSize.Height;
            }
            else
            {
                this.HScrollBar.Value = this.HScrollBar.Minimum;
            }

            bool update = false;
            ElementVisibility visibility2 = VScrollBar.Visibility;
            this.Scroller.ClientSize = clientSize;
            if (visibility2 != VScrollBar.Visibility)
            {
                scrollbarOffset = this.VScrollBar.Visibility == ElementVisibility.Visible ? (int)this.VScrollBar.DesiredSize.Width : 0;
                int largeChange = (int)(clientRect.Width - scrollbarOffset - this.ViewElement.Margin.Horizontal);
                this.HScrollBar.LargeChange = largeChange >= 0 ? largeChange : 0;
                UpdateHScrollbarVisibility();
                update = true;
            }

            if (VScrollBar.Visibility == ElementVisibility.Collapsed && VScrollBar.Value != VScrollBar.Minimum)
            {
                VScrollBar.Value = VScrollBar.Minimum;
            }

            return update || visibility != this.HScrollBar.Visibility;
        }

        protected virtual void UpdateHScrollbarVisibility()
        {
            if (this.HorizontalScrollState == ScrollState.AlwaysShow)
            {
                this.HScrollBar.Visibility = ElementVisibility.Visible;
            }
            else if (this.HorizontalScrollState == ScrollState.AlwaysHide)
            {
                this.HScrollBar.Visibility = ElementVisibility.Collapsed;
            }
            else
            {
                this.HScrollBar.Visibility = this.HScrollBar.LargeChange < this.HScrollBar.Maximum ? ElementVisibility.Visible : ElementVisibility.Collapsed;
            }
        }

        protected override void UpdateFitToSizeMode()
        {
        }

        private void UpdateFirstVisibleIndex()
        {
            if (this.ViewElement.Children.Count == 0)
            {
                return;
            }

            RadTreeNode node = ((TreeNodeElement)this.ViewElement.Children[0]).Data;
            node = node.PrevVisibleNode;
            firstVisibleIndex = 0;
            while (node != null)
            {
                firstVisibleIndex++;
                node = node.PrevVisibleNode;
            }
        }

        #endregion

        
    }
}

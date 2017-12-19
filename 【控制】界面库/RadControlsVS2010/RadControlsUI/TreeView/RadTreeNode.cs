using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls.Commands;
using Telerik.WinControls.Data;
using Telerik.WinControls.Enumerations;

namespace Telerik.WinControls.UI
{
    public class RadTreeNode : IDataItem, ICloneable, INotifyPropertyChanged
    {
        #region Fields

        internal const int DefaultItemHeight = -1;

        protected const int SuspendNotificationsState = 1;
        protected const int IsExpandedState = SuspendNotificationsState << 1;
        protected const int IsSelectedState = IsExpandedState << 1;
        protected const int IsCurrentState = IsSelectedState << 1;
        protected const int IsVisibleState = IsCurrentState << 1;
        protected const int IsEnableState = IsVisibleState << 1;
        protected const int IsAllowDropState = IsEnableState << 1;
        protected const int UpdateParentSizeOnExpandedChangedState = IsAllowDropState << 1;
        protected const int ChildNodesLoadedState = UpdateParentSizeOnExpandedChangedState << 1;

        protected BitVector32 state = new BitVector32();

        private string toolTipText, name, imageKey;
        private ToggleState checkState = ToggleState.Off;
        private CheckType? checkType = null;
        private object dataBoundItem, tag;
        private RadContextMenu contextMenu;
        internal RadTreeNode parent;
        private RadTreeNodeCollection nodes;
        internal RadTreeViewElement treeView;
        private TreeNodeStyle style;
        private Image image;
        private int itemHeight = -1;
        private int imageIndex = -1;
        private int boundIndex = 0;
        private Size actualSize, childrenSize;
        private object value;
        private WeakReference matches;
        private bool? isInDesignMode;

        #endregion

        #region Constructors

        public RadTreeNode()
        {
            state[IsVisibleState] = true;
            state[IsEnableState] = true;
            state[IsAllowDropState] = true;
        }

        public RadTreeNode(string text)
            : this()
        {
            this.Text = text;
            this.name = text;
        }

        public RadTreeNode(string text, RadTreeNode[] children)
            : this()
        {
            this.Text = text;
            this.name = text;
            this.Nodes.AddRange(children);
        }

        /// <summary>
        /// Initializes new instance of the RadTreeNode class.
        /// </summary>
        /// <param name="text">The text to be used as label text.</param>
        /// <param name="expanded">A boolean value indicating whether the node is expanded.</param>
        public RadTreeNode(string text, bool expanded)
            : this(text)
        {
            this.Expanded = expanded;
        }

        public RadTreeNode(string text, Image image)
            : this(text)
        {
            this.Image = image;
        }

        public RadTreeNode(string text, Image image, bool expanded)
            : this(text, expanded)
        {
            this.Image = image;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the last matches using Find method.
        /// </summary>
        /// <value>Gets the last matches using Find method.</value>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IEnumerator<RadTreeNode> Matches
        {
            get
            {
                if (this.matches != null && this.matches.IsAlive)
                {
                    List<RadTreeNode> nodes = this.matches.Target as List<RadTreeNode>;
                    if (nodes != null)
                    {
                        return nodes.GetEnumerator();
                    }
                }

                return new List<RadTreeNode>().GetEnumerator();
            }
        }

        /// <summary>
        /// Gets the style.
        /// </summary>
        /// <value>The style.</value>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [DefaultValue(null)]
        public TreeNodeStyle Style
        {
            get
            {
                if (this.style == null)
                {
                    this.style = new TreeNodeStyle();
                    this.style.PropertyChanged += new PropertyChangedEventHandler(Style_PropertyChanged);
                }

                return style;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has style.
        /// </summary>
        /// <value><c>true</c> if this instance has style; otherwise, <c>false</c>.</value>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [DefaultValue(false)]
        public bool HasStyle
        {
            get
            {
                return this.style != null;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the node can respond to user interaction.
        /// </summary>
        /// <value>The default value is true.</value>
        [DefaultValue(true)]
        [Description("Gets or sets a value indicating whether the node can respond to user interaction.")]
        [Category("Behavior")]
        public bool Enabled
        {
            get
            {
                return state[IsEnableState];
            }
            set
            {
                if (value != this.state[IsEnableState])
                {
                    this.state[IsEnableState] = value;
                    Update(RadTreeViewElement.UpdateActions.StateChanged);
                }
            }
        }

        /// <summary>
        /// Gets the root parent node for this RadTreeView.
        /// </summary>
        /// <value>The default value is null.</value>
        [Browsable(false), DefaultValue(null),
        Description("Gets the root parent node for this instance."),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Category("Behavior")]
        public RadTreeNode RootNode
        {
            get
            {
                RadTreeNode node = this;
                while (node.Parent != null)
                {
                    node = node.Parent;
                }

                return node;
            }
        }

        /// <summary>
        /// Gets the parent tree view that the tree node is assigned to. 
        /// </summary>
        [Browsable(false), DefaultValue(null),
        Description("Gets the parent tree view that the tree node is assigned to."),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RadTreeView TreeView
        {
            get
            {
                if (this.treeView != null)
                {
                    return this.treeView.ElementTree.Control as RadTreeView;
                }

                return null;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="RadTreeNode"/> is checked.
        /// </summary>
        /// <value><c>true</c> if checked; otherwise, <c>false</c>.</value>
        [NotifyParentProperty(true)]
        [Category("Behavior"), DefaultValue(false)]
        [Description("Gets or set a value indicating whether the check box displayed by the tree node is in the checked state.")]
        public bool Checked
        {
            get { return this.checkState == ToggleState.On; }
            set
            {
                if (value == this.Checked)
                {
                    return;
                }

                if (value)
                {
                    this.CheckState = ToggleState.On; 
                }
                else
                {
                    this.CheckState = ToggleState.Off;
                }

                this.OnNotifyPropertyChanged("Checked");
            }
        }

        /// <summary>
        /// Gets or sets the state of the check element.
        /// </summary>
        /// <value>The state of the check.</value>
        [Category("Behavior")]
        [DefaultValue(ToggleState.Off)]
        [Description("Gets or sets the check state of the RadTreeNode.")]
        public ToggleState CheckState
        {
            get { return this.checkState; }
            set
            {
                if (this.SetCheckStateCore(value))
                {
                    if (this.TreeViewElement != null)
                    {
                        this.TreeViewElement.BeginUpdate();
                    }

                    this.UpdateChildrenCheckState();
                    this.UpdateParentCheckState();
                    this.OnCheckStateChanged();

                    if (this.TreeViewElement != null)
                    {
                        this.TreeViewElement.EndUpdate(true, RadTreeViewElement.UpdateActions.StateChanged);
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the type of the check element.
        /// </summary>
        /// <value>The type of the check.</value>
        [DefaultValue(CheckType.None)]
        [Category("Behavior")]
        [Description("Gets or sets the check type of the RadTreeNode.")]
        public CheckType CheckType
        {
            get
            {
                if (this.checkType == null)
                {
                    if (this.TreeViewElement != null && this.TreeViewElement.CheckBoxes)
                    {
                        return CheckType.CheckBox;
                    }

                    return CheckType.None;
                }

                return checkType.Value;
            }
            set
            {
                if (this.checkType != value)
                {
                    this.checkType = value;
                    this.OnNotifyPropertyChanged("CheckType");
                }
            }
        }

        /// <summary>Gets or sets the context menu associated to the node.</summary>
        /// <value>
        ///     Returns an instance of <see cref="RadDropDownMenu">RadDropDownMenu Class</see> that
        ///     is associated with the node. The default value is null.
        /// </value>
        /// <remarks>
        /// This property could be used to associate a custom menu and replace the treeview's
        /// default. If the context menu is invoked by right-clicking a node, the treeview's menu
        /// will not be shown and the context menu assigned to this node will be shown
        /// instead.
        /// </remarks>
        /// <seealso cref="RadTreeView.RadContextMenu">RadContextMenu Property (Telerik.WinControls.UI.RadTreeView)</seealso>
        [Category("Behavior")]
        [DefaultValue(null)]
        [Description("Gets or sets the shortcut menu associated to the node.")]
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
        /// Gets or sets a value indicating whether this instance is visible.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is visible; otherwise, <c>false</c>.
        /// </value>
        [DefaultValue(true)]
        [Description("Gets or sets a value indicating whether the tree node is visible.")]
        [Category("Behavior")]
        public bool Visible
        {
            get { return state[IsVisibleState]; }
            set { SetBooleanProperty("Visible", IsVisibleState, value); }
        }

        /// <summary>
        /// Gets the index.
        /// </summary>
        /// <value>The index.</value>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int Index
        {
            get
            {
                if (this.parent == null)
                {
                    return -1;
                }

                return this.parent.Nodes.IndexOf(this);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is editing.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is editing; otherwise, <c>false</c>.
        /// </value>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsEditing
        {
            get
            {
                return this.TreeViewElement != null && this.TreeViewElement.IsEditing && this.TreeViewElement.SelectedNode == this;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is selected.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is selected; otherwise, <c>false</c>.
        /// </value>
        [Description("Gets or sets a value indicating whether the tree node is in the selected state.")]
        [DefaultValue(false)]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool Selected
        {
            get
            {
                return state[IsSelectedState];
            }
            set
            {
                this.SetBooleanProperty("Selected", IsSelectedState, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is current.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is current; otherwise, <c>false</c>.
        /// </value>
        [Description("Gets or sets a value indicating whether the tree node is in the current state.")]
        [DefaultValue(false)]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool Current
        {
            get { return state[IsCurrentState]; }
            set
            {
                this.SetBooleanProperty("Current", IsCurrentState, value);
            }
        }

        /// <summary>
        /// Gets or sets the tree view element.
        /// </summary>
        /// <value>The tree view element.</value>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RadTreeViewElement TreeViewElement
        {
            get
            {
                if (this.treeView == null)
                {
                    this.treeView = this.FindTreeView();
                }

                return this.treeView;
            }
            internal set
            {
                if (value != this.treeView)
                {
                    this.treeView = value;

                    //foreach (RadTreeNode node in this.Nodes)
                    //{
                    //    if (value == null)
                    //    {
                    //        node.Selected = false;
                    //        node.Current = false;
                    //    }

                    //    node.TreeViewElement = value;
                    //}
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is expanded.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is expanded; otherwise, <c>false</c>.
        /// </value>
        [Browsable(false)]
        [DefaultValue(false)]
        [Description("Gets or sets a value indicating whether this instance is expanded.")]
        public bool Expanded
        {
            get { return state[IsExpandedState]; }
            set
            {
                if (Expanded != value)
                {
                    if (this.TreeViewElement != null && !this.TreeViewElement.OnNodeExpandedChanging(this))
                    {
                        return;
                    }

                    this.OnExpandedChanged(value);
                    this.SetBooleanProperty("Expanded", IsExpandedState, value);

                    if (this.TreeViewElement != null)
                    {
                        this.TreeViewElement.OnNodeExpandedChanged(new RadTreeViewEventArgs(this));
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the parent.
        /// </summary>
        /// <value>The parent.</value>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RadTreeNode Parent
        {
            get
            {
                if (this.parent is RadTreeViewElement.RootTreeNode)
                {
                    return null;
                }

                return parent;
            }
            internal set
            {
                if (this.parent != value)
                {
                    if (value == null)
                    {
                        this.Current = false;
                        this.Selected = false;
                    }

                    this.parent = value;

                    if (this.parent == null)
                    {
                        this.ClearActualAndChildrenSize();
                        return;
                    }

                    if (this.parent is RadTreeViewElement.RootTreeNode || this.TreeViewElement != null && this.TreeViewElement.IsLazyLoading)
                    {
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        [Category("Appearance")]
        [DefaultValue(null)]
        [Description("Gets or sets the text displayed in the label of the tree node.")]
        public string Text
        {
            get
            {
                if (this.dataBoundItem != null && this.parent != null)
                {
                    if (this.boundIndex < this.TreeViewElement.BoundDescriptors.Count)
                    {
                        TreeNodeDescriptor current = this.TreeViewElement.BoundDescriptors[this.boundIndex];
                        if (current.DisplayDescriptor != null)
                        {
                            return Convert.ToString(current.GetDisplay(this));//.DisplayDescriptor.GetValue(this.dataBoundItem).ToString();
                        }
                    }
                }

                object value = this.Value;

                if (value != null)
                {
                    return value.ToString();
                }

                return null;
            }
            set
            {
                if (this.Text != value)
                {
                    this.Value = value;
                    this.OnNotifyPropertyChanged("Text");
                    Update(RadTreeViewElement.UpdateActions.ItemEdited);
                }
            }
        }

        /// <summary>
        /// Gets or sets the node value.
        /// </summary>
        /// <value>The text.</value>
        [Category("Data")]
        [DefaultValue(null)]
        [Description("Gets or sets the node value.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public object Value
        {
            get
            {
                if (this.dataBoundItem != null && this.parent != null)
                {
                    if (this.boundIndex < this.TreeViewElement.BoundDescriptors.Count)
                    {
                        TreeNodeDescriptor current = this.TreeViewElement.BoundDescriptors[this.boundIndex];
                        if (current.ValueDescriptor != null)
                        {
                            return current.GetValue(this);// ValueDescriptor.GetValue(this.dataBoundItem);
                        }

                        if (current.DisplayDescriptor != null)
                        {
                            return current.GetDisplay(this);//.DisplayDescriptor.GetValue(this.dataBoundItem);
                        }
                    }

                    return this.dataBoundItem;
                }

                if (this.value != null)
                {
                    return value;
                }

                return null;
            }
            set
            {
                if (this.dataBoundItem != null && this.parent != null)
                {
                    if (this.boundIndex < this.TreeViewElement.BoundDescriptors.Count)
                    {
                        this.TreeViewElement.SuspendProvider();
                        TreeNodeDescriptor current = this.TreeViewElement.BoundDescriptors[this.boundIndex];
                        if (current.ValueDescriptor != null)
                        {
                            current.SetValue(this, value);//.ValueDescriptor.SetValue(this.dataBoundItem, value);
                            this.OnNotifyPropertyChanged("Value");
                        }
                        else if (current.DisplayDescriptor != null)
                        {
                            current.SetDisplay(this, value);//.DisplayDescriptor.SetValue(this.dataBoundItem, value);
                            this.OnNotifyPropertyChanged("Value");
                            this.OnNotifyPropertyChanged("Text");
                        }
                        this.TreeViewElement.ResumeProvider();
                    }

                    return;
                }

                this.value = value;
                this.OnNotifyPropertyChanged("Value");
            }
        }

        /// <summary>
        /// Gets the nodes.
        /// </summary>
        /// <value>The nodes.</value>
        [ListBindable(false)]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public RadTreeNodeCollection Nodes
        {
            get
            {
                if (this.nodes == null)
                {
                    this.nodes = new RadTreeNodeCollection(this);
                }

                return this.nodes;
            }
        }

        /// <summary>
        /// Gets the level.
        /// </summary>
        /// <value>The level.</value>
        /// 
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int Level
        {
            get
            {
                //if (this.Parent == null)
                //{
                //    return 0;
                //}

                //return (this.Parent.Level + 1);

                int level = 0;
                RadTreeNode parent = this.Parent;
                while (parent != null)
                {
                    level++;
                    parent = parent.Parent;
                }

                return level;
            }
        }

        /// <summary>
        /// Gets or sets the name of the RadTreeNode.
        /// </summary>
        /// <value>A <a href="ms-help://MS.VSCC.v80/MS.MSDN.v80/MS.NETDEVFX.v20.en/cpref2/html/T_System_String.htm">String</a> that represents the name of the tree node.</value> 
        /// <remarks>
        ///     The Name of a TreeNode is also the node's key, when the node is part of a
        ///     <see cref="RadTreeNodeCollection">TreeNodeCollection</see>. If the node does not
        ///     have a name, Name returns an empty string ("").
        /// </remarks>
        [NotifyParentProperty(true)]
        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        [Description("Gets or sets the name of the RadTreeNode.")]
        public string Name
        {
            get
            {
                if (this.name != null)
                {
                    return this.name;
                }

                if (this.dataBoundItem != null && this.parent != null)
                {
                    if (this.boundIndex < this.TreeViewElement.BoundDescriptors.Count)
                    {
                        TreeNodeDescriptor current = this.TreeViewElement.BoundDescriptors[this.boundIndex];
                        if (current.DisplayDescriptor != null)
                        {
                            return current.DisplayDescriptor.GetValue(this.dataBoundItem).ToString();
                        }
                    }
                }

                return "";
            }
            set
            {
                if (this.name != value)
                {
                    this.name = value;
                    this.OnNotifyPropertyChanged("Name");
                }
            }
        }

        /// <summary>
        /// Gets the first node.
        /// </summary>
        /// <value>The first node.</value>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RadTreeNode FirstNode
        {
            get
            {
                if (this.Nodes.Count == 0)
                {
                    return null;
                }

                return this.Nodes[0];
            }
        }

        /// <summary>
        /// Gets the last node.
        /// </summary>
        /// <value>The last node.</value>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RadTreeNode LastNode
        {
            get
            {
                if (this.Nodes.Count == 0)
                {
                    return null;
                }

                return this.Nodes[this.Nodes.Count - 1];
            }
        }

        /// <summary>
        /// Gets the next node.
        /// </summary>
        /// <value>The next node.</value>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RadTreeNode NextNode
        {
            get
            {
                if (this.parent == null)
                {
                    return null;
                }

                int index = this.parent.Nodes.IndexOf(this);
                if (index < 0)
                {
                    return null;
                }

                index++;
                if (index < this.parent.Nodes.Count)
                {
                    return this.parent.Nodes[index];
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the next visible node.
        /// </summary>
        /// <value>The next visible node.</value>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RadTreeNode NextVisibleNode
        {
            get
            {
                int index = 0;
                if (this.Expanded) 
                {
                    while (index < this.Nodes.Count)
                    {
                        if (this.Nodes[index].Visible || IsInDesignMode)
                        {
                            return this.Nodes[index];
                        }
                        index++;
                    }
                }

                RadTreeNode current = this;
                RadTreeNode parentNode = this.parent;
                while (parentNode != null)
                {
                    index = parentNode.Nodes.IndexOf(current);
                    if (index < 0) //fix for 
                    {
                        return null;
                    }

                    index++;
                    while (index >= 0 && index < parentNode.Nodes.Count)
                    {
                        if (parentNode.Nodes[index].Visible || IsInDesignMode)
                        {
                            return parentNode.Nodes[index];
                        }
                        index++;
                    }

                    current = parentNode;
                    parentNode = parentNode.parent;
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the prev node.
        /// </summary>
        /// <value>The prev node.</value>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RadTreeNode PrevNode
        {
            get
            {
                if (this.parent == null)
                {
                    return null;
                }

                int index = this.parent.Nodes.IndexOf(this) - 1;
                if (index >= 0 && index < this.parent.Nodes.Count)
                {
                    return this.parent.Nodes[index];
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the prev visible node.
        /// </summary>
        /// <value>The prev visible node.</value>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RadTreeNode PrevVisibleNode
        {
            get
            {
                if (this.parent == null)
                {
                    return null;
                }

                int index = this.parent.Nodes.IndexOf(this) - 1;
                while (index >= 0)
                {
                    RadTreeNode current = this.parent.Nodes[index];
                    if (current.Visible || IsInDesignMode)
                    {
                        if (current.Expanded)
                        {
                            RadTreeNode prevNode = GetLastVisibleNode(current);
                            if (prevNode != null)
                            {
                                return prevNode;
                            }
                        }

                        return current;
                    }

                    index--;
                }

                if (this.parent == this.TreeViewElement.Root)
                {
                    return null;
                }

                return this.parent;
            }
        }

        /// <summary>
        /// Gets or sets the tag object that can be used to store user data, corresponding to the tree node.
        /// </summary>
        /// <value>The tag.</value>
        [Category("Data"),
        Localizable(false),
        Bindable(true),
        DefaultValue((string)null),
        TypeConverter(typeof(StringConverter)),
        Description("Tag object that can be used to store user data, corresponding to the tree node")]
        public object Tag
        {
            get { return this.tag; }
            set
            {
                if (this.tag != value)
                {
                    this.tag = value;
                    this.OnNotifyPropertyChanged("Tag");
                }
            }
        }

        /// <summary>
        /// Gets or sets the text that appears when the mouse pointer hovers over a tree node.
        /// </summary>
        /// <value>The default value is "".</value>
        [DefaultValue(null)]
        [Category("Appearance")]
        [Description("Gets or sets the text that appears when the mouse pointer hovers over a tree node.")]
        public string ToolTipText
        {
            get { return this.toolTipText; }
            set
            {
                if (this.toolTipText != value)
                {
                    this.toolTipText = value;
                    this.OnNotifyPropertyChanged("ToolTipText");
                }
            }
        }

        /// <summary>
        /// Gets the full path.
        /// </summary>
        /// <value>The full path.</value>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string FullPath
        {
            get
            {
                RadTreeViewElement treeView = this.TreeViewElement;
                if (treeView == null)
                {
                    throw new InvalidOperationException(SR.GetString("TreeNodeNoParent"));
                }

                StringBuilder path = new StringBuilder();
                this.GetFullPath(path, treeView.PathSeparator);
                return path.ToString();
            }
        }

        /// <summary>
        /// Gets or sets the image of the node.
        /// </summary>		
        /// <seealso cref="ImageIndex">ImageIndex Property</seealso>		
        /// <seealso cref="ImageKey">ImageKey Property</seealso>		
        /// <seealso cref="SelectedImage">SelectedImage Property</seealso>		
        /// <seealso cref="StateImage">StateImage Property</seealso>		
        [NotifyParentProperty(true)]
        [Category("Appearance"), DefaultValue(null)]
        [Description("Gets or sets the image of the node.")]
        [TypeConverter(typeof(Telerik.WinControls.Primitives.ImageTypeConverter))]
        public virtual Image Image
        {
            get
            {
                if (this.image != null)
                {
                    return this.image;
                }

                if (this.TreeViewElement == null)
                {
                    return null;
                }

                int index = (this.ImageIndex >= 0) ? this.ImageIndex : this.TreeViewElement.ImageIndex;
                if (index >= 0 && string.IsNullOrEmpty(this.ImageKey))
                {
                    RadControl control = this.TreeViewElement.ElementTree.Control as RadControl;
                    if (control != null)
                    {
                        ImageList imageList = control.ImageList;
                        if (imageList != null && index < imageList.Images.Count)
                        {
                            return imageList.Images[index];
                        }
                    }
                }

                string treeKey = null;
                if (this.TreeViewElement != null)
                {
                    treeKey = this.TreeViewElement.ImageKey;
                }

                string key = (string.IsNullOrEmpty(this.ImageKey)) ? treeKey : this.ImageKey;
                if (!string.IsNullOrEmpty(key))
                {
                    RadControl control = this.TreeViewElement.ElementTree.Control as RadControl;
                    if (control != null)
                    {
                        ImageList imageList = control.ImageList;
                        if (imageList != null && imageList.Images.Count > 0 && imageList.Images.ContainsKey(key))
                        {
                            return imageList.Images[key];
                        }
                    }
                }

                return null;
            }
            set
            {
                if (this.image != value)
                {
                    this.image = value;
                    this.OnNotifyPropertyChanged("Image");
                    Update(RadTreeViewElement.UpdateActions.StateChanged);
                }
            }
        }

        /// <summary>
        /// Gets or sets the left image list index value of the image displayed when the tree
        /// node is not selected.
        /// </summary>
        /// <seealso cref="Image">Image Property</seealso>
        /// <seealso cref="ImageKey">ImageKey Property</seealso>
        [NotifyParentProperty(true)]
        [Category("Appearance"), DefaultValue(-1)]
        [Description("Gets or sets the left image list index value of the image displayed when the tree node is in the unselected state.")]
        [RelatedImageList("TreeView.ImageList"),
        Editor(DesignerConsts.ImageIndexEditorString, typeof(UITypeEditor)),
        TypeConverter(DesignerConsts.NoneExcludedImageIndexConverterString)]
        public virtual int ImageIndex
        {
            get
            {
                return this.imageIndex;
            }
            set
            {
                if (this.imageIndex != value)
                {
                    this.imageIndex = value;
                    this.OnNotifyPropertyChanged("ImageIndex");
                    Update(RadTreeViewElement.UpdateActions.StateChanged);
                }
            }
        }

        /// <summary>
        /// 	<see cref="SelectedImageKey">SelectedImageKey Property</see>Gets or sets the key
        ///     for the left image associated with this tree node when the node is not selected.
        /// </summary>
        /// <seealso cref="Image">Image Property</seealso>
        /// <seealso cref="ImageIndex">ImageIndex Property</seealso>
        [NotifyParentProperty(true)]
        [Category("Appearance"), DefaultValue(null)]
        [Description("Gets or sets the key for the left image associated with this tree node when the node is in an unselected state.")]
        [RelatedImageList("TreeView.ImageList"),
        Editor(DesignerConsts.ImageKeyEditorString, typeof(UITypeEditor)),
        TypeConverter(DesignerConsts.RadImageKeyConverterString)]
        public string ImageKey
        {
            get
            {
                return this.imageKey;
            }
            set
            {
                if (this.imageKey != value)
                {
                    this.imageKey = value;
                    this.imageIndex = -1;
                    this.OnNotifyPropertyChanged("ImageKey");
                    Update(RadTreeViewElement.UpdateActions.StateChanged);
                }
            }
        }

        /// <summary>
        /// Gets or sets the height of the tree node in the tree view control.
        /// </summary>
        /// <value>The default value is 20.</value>
        [NotifyParentProperty(true)]
        [Browsable(true),
        DefaultValue(DefaultItemHeight)]
        [Category("Layout")]
        [Description("Gets or sets the height of the tree node in the tree view control.")]
        public int ItemHeight
        {
            get
            {
                return this.itemHeight;
            }
            set
            {
                if (this.itemHeight != value)
                {
                    this.itemHeight = value;
                    this.OnNotifyPropertyChanged("ItemHeight");
                    Update(RadTreeViewElement.UpdateActions.Resume);
                }
            }
        }

        /// <summary>
        /// Gets or sets the measured desired width for this node.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public Size ActualSize
        {
            get
            {
                return this.actualSize;
            }
            internal set
            {
                if (this.actualSize != value)
                {
                    this.OnActualSizeChanged(value);
                }
            }
        }

        /// <summary>
        /// Gets or sets the measured desired width for this node.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public Size ChildrenSize
        {
            get
            {
                return this.childrenSize;
            }
            internal set
            {
                if (this.childrenSize != value)
                {
                    OnChildrenSizeChanged(value);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [allow drop].
        /// </summary>
        /// <value><c>true</c> if [allow drop]; otherwise, <c>false</c>.</value>
        [Browsable(false)]
        [Description("Gets or sets a value indicating whether [allow drop].")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool AllowDrop
        {
            get { return state[IsAllowDropState]; }
            set
            {
                if (state[IsAllowDropState] != value)
                {
                    state[IsAllowDropState] = value;
                    this.OnNotifyPropertyChanged("AllowDrop");
                }
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
                if (isInDesignMode.HasValue)
                {
                    return isInDesignMode.Value                        ;
                }
                isInDesignMode = this.TreeViewElement != null && this.TreeViewElement.IsInDesignMode;  
                return isInDesignMode.Value;
            }
        }

        #endregion

        #region Methods


        /// <summary>
        /// Finds the specified match.
        /// </summary>
        /// <param name="match">The match.</param>
        /// <returns></returns>
        public RadTreeNode Find(Predicate<RadTreeNode> match)
        {
            List<RadTreeNode> nodes = this.FindAll(match);
            if (nodes.Count == 0)
            {
                return null;
            }

            if (nodes.Count == 1)
            {
                return nodes[0];
            }

            RadTreeNode first = nodes[0];
            nodes.RemoveAt(0);
            first.CacheLastFind(nodes);
            return first;
        }

        /// <summary>
        /// Finds the nodes.
        /// </summary>
        /// <param name="match">The match.</param>
        /// <returns></returns>
        public RadTreeNode[] FindNodes(Predicate<RadTreeNode> match)
        {
            return this.FindAll(match).ToArray();
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
            if (includeSubTrees)
            {
                for (int i = 0; i < this.Nodes.Count; i++)
                {
                    this.Nodes[i].Execute(command, true, settings);
                }
            }

            if (command.CanExecute(this))
            {
                return command.Execute(this, settings);
            }

            return null;
        }

        /// <summary>
        /// Initiates the editing of the tree node.
        /// </summary>
        /// <returns></returns>
        public bool BeginEdit()
        {
            if (this.TreeViewElement != null)
            {
                this.TreeViewElement.SelectedNode = this;
                return this.TreeViewElement.BeginEdit();
            }
            return false;
        }

        /// <summary>
        /// Ends the edit.
        /// </summary>
        /// <returns></returns>
        public bool EndEdit()
        {
            if (this.TreeViewElement != null && this.TreeViewElement.IsEditing && this.TreeViewElement.SelectedNode == this)
            {
                return this.TreeViewElement.EndEdit();
            }
            return false;
        }

        /// <summary>
        /// Cancels the edit.
        /// </summary>
        /// <returns></returns>
        public bool CancelEdit()
        {
            if (this.TreeViewElement != null && this.TreeViewElement.IsEditing && this.TreeViewElement.SelectedNode == this)
            {
                this.TreeViewElement.CancelEdit();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Collapses the tree node.
        /// </summary>
        public void Collapse()
        {
            this.Collapse(false);
        }

        /// <summary>
        /// Collapses the System.Windows.Forms.TreeNode and optionally collapses its children.
        /// </summary>
        /// <param name="ignoreChildren">if set to <c>true</c> [ignore children].</param>
        public void Collapse(bool ignoreChildren)
        {
            this.Expanded = false;

            if (!ignoreChildren)
            {
                for (int i = 0; i < this.Nodes.Count; i++)
                {
                    this.Nodes[i].Collapse(ignoreChildren);
                }
            }

            this.NotifyExpandedChanged(null);
        }

        /// <summary>
        ///  Ensures that the tree node is visible, expanding tree nodes and scrolling the tree view control as necessary.
        /// </summary>
        public void EnsureVisible()
        {
            this.TreeViewElement.EnsureVisible(this);
        }

        /// <summary>
        /// Expands the tree node.
        /// </summary>
        public void Expand()
        {
            this.Expanded = true;

            //TODO: optimize update
            this.NotifyExpandedChanged(null);
        }

        /// <summary>
        /// Expands all the child tree nodes.
        /// </summary>
        public void ExpandAll()
        {
            this.Expand();
            for (int i = 0; i < this.Nodes.Count; i++)
            {
                this.Nodes[i].ExpandAll();
            }

            //TODO: optimize update
            this.NotifyExpandedChanged(null);
        }

        /// <summary>
        /// Returns the number of child tree nodes.
        /// </summary>
        /// <param name="includeSubTrees">if set to <c>true</c> [include sub trees].</param>
        /// <returns></returns>
        public int GetNodeCount(bool includeSubTrees)
        {
            int count = this.nodes.Count;

            if (includeSubTrees)
            {
                for (int i = 0; i < this.nodes.Count; i++)
                {
                    count += this.nodes[i].GetNodeCount(true);
                }
            }

            return count;
        }

        /// <summary>
        /// Removes the current tree node from the tree view control.
        /// </summary>
        public virtual void Remove()
        {
            if (this.Parent != null)
            {
                this.Parent.Nodes.Remove(this);
            }
            else if (this.TreeViewElement != null)
            {
                this.TreeViewElement.Nodes.Remove(this);
            }
        }

        /// <summary>
        /// Toggles the tree node to either the expanded or collapsed state.
        /// </summary>
        public void Toggle()
        {
            if (this.Expanded)
            {
                this.Collapse(true);
            }
            else
            {
                this.Expand();
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents the tree node.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents the tree node.
        /// </returns>
        public override string ToString()
        {
            return ("RadTreeNode: " + ((this.Text == null) ? "" : this.Text));
        }

        #endregion

        #region Internal

        private void ClearActualAndChildrenSize()
        {
            Queue<RadTreeNodeCollection> nodeLevels = new Queue<RadTreeNodeCollection>();
            nodeLevels.Enqueue(this.Nodes);

            this.ActualSize = Size.Empty;
            this.ChildrenSize = Size.Empty;

            while (nodeLevels.Count > 0)
            {
                RadTreeNodeCollection collection = nodeLevels.Dequeue();

                foreach (RadTreeNode node in collection)
                {
                    node.ActualSize = Size.Empty;
                    node.ChildrenSize = Size.Empty;

                    if (node.Nodes.Count > 0)
                    {
                        nodeLevels.Enqueue(node.Nodes);
                    }
                }
            }
        }

        protected internal void CacheLastFind(List<RadTreeNode> nodes)
        {
            this.matches = new WeakReference(nodes);
        }

        private List<RadTreeNode> FindAll(Predicate<RadTreeNode> match)
        {
            List<RadTreeNode> nodes = new List<RadTreeNode>();

            if (match(this))
            {
                nodes.Add(this);
            }

            for (int i = 0; i < this.Nodes.Count; i++)
            {
                List<RadTreeNode> childNodes = this.Nodes[i].FindAll(match);
                for (int j = 0; j < childNodes.Count; j++)
                {
                    nodes.Add(childNodes[j]);
                }
            }

            return nodes;
        }

        /// <summary>
        /// Gets a value if the node is root node
        /// </summary>
        [Description("Gets a value if the node is root node")]
        [Category("Behavior")]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        internal bool IsRootNode
        {
            get
            {
                if (this.parent == null || this.parent is RadTreeViewElement.RootTreeNode)
                {
                    return true;
                }
                return false;
            }
        }

        private void Update(RadTreeViewElement.UpdateActions updateAction)
        {
            if (this.state[SuspendNotificationsState])
            {
                return;
            }
            if (this.treeView != null)
            {
                this.treeView.Update(updateAction, this);
            }
        }

        private RadTreeNode GetLastVisibleNode(RadTreeNode parent)
        {
            int index = parent.Nodes.Count - 1;
            while (index >= 0)
            {
                RadTreeNode current = parent.Nodes[index];
                if (current.Visible || IsInDesignMode)
                {
                    if (current.Expanded && current.Nodes.Count > 0)
                    {
                        return GetLastVisibleNode(current);
                    }

                    return current;
                }
                index--;
            }

            return null;
        }

        private RadTreeViewElement FindTreeView()
        {
            RadTreeNode parent = this;

            while (parent.parent != null)
            {
                parent = parent.parent;
            }

            return parent.treeView;
        }

        internal int BoundIndex
        {
            get { return boundIndex; }
            set { boundIndex = value; }
        }

        internal bool NodesLoaded
        {
            get { return this.state[ChildNodesLoadedState]; }
            set { this.state[ChildNodesLoadedState] = value; }
        }

        internal virtual object ParentValue
        {
            get
            {
                if (this.boundIndex < this.TreeViewElement.BoundDescriptors.Count)
                {
                    TreeNodeDescriptor current = this.TreeViewElement.BoundDescriptors[this.boundIndex];
                    if (current.ParentDescriptor != null)
                    {
                        return current.GetParent(this); //.ParentDescriptor.GetValue(this.dataBoundItem);
                    }
                }

                return null;
            }
            set
            {
                if (this.boundIndex < this.TreeViewElement.BoundDescriptors.Count)
                {
                    TreeNodeDescriptor current = this.TreeViewElement.BoundDescriptors[this.boundIndex];
                    if (current.ParentDescriptor != null)
                    {
                        current.SetParent(this, value); //.ParentDescriptor.SetValue(this.dataBoundItem, value);
                    }
                }
            }
        }

        internal virtual object ChildValue
        {
            get
            {
                if (this.boundIndex < this.TreeViewElement.BoundDescriptors.Count)
                {
                    TreeNodeDescriptor current = this.TreeViewElement.BoundDescriptors[this.boundIndex];
                    if (current.ChildDescriptor != null)
                    {
                        return current.GetChild(this);//.ChildDescriptor.GetValue(this.dataBoundItem);
                    }
                }

                return null;
            }
            set
            {
                if (this.boundIndex < this.TreeViewElement.BoundDescriptors.Count)
                {
                    TreeNodeDescriptor current = this.TreeViewElement.BoundDescriptors[this.boundIndex];
                    if (current.ChildDescriptor != null)
                    {
                        current.SetChild(this, value);//.ChildDescriptor.SetValue(this.dataBoundItem, value);
                    }
                }
            }
        }

        private void GetFullPath(StringBuilder path, string pathSeparator)
        {
            if (this.parent != null)
            {
                this.parent.GetFullPath(path, pathSeparator);
                if (this.parent.parent != null)
                {
                    path.Append(pathSeparator);
                }

                path.Append(this.Text);
            }
        }

        protected virtual bool SetCheckStateCore(ToggleState value)
        {
            if (this.checkState == value)
            {
                return false;
            }

            if (this.TreeViewElement != null && !this.TreeViewElement.OnNodeCheckedChanging(this))
            {
                return false;
            }

            this.checkState = value;
            return true;
        }

        protected virtual void OnCheckStateChanged()
        {
            this.OnNotifyPropertyChanged("CheckState");

            if (this.TreeViewElement != null)
            {
                this.TreeViewElement.OnNodeCheckedChanged(this);
            }
        }

        protected virtual void UpdateParentCheckState()
        {
            if (this.parent == null || this.CheckType == UI.CheckType.None)
            {
                return;
            }

            if (this.TreeViewElement != null)
            {
                this.TreeViewElement.BeginUpdate();
            }

            int update = 0;
            if (this.CheckType == UI.CheckType.RadioButton)
            {
                for (int i = 0; i < this.parent.Nodes.Count; i++)
                {
                    RadTreeNode node = this.parent.Nodes[i];
                    if (node != this && node.CheckType == this.CheckType)
                    {
                        if (node.SetCheckStateCore(ToggleState.Off))
                        {
                            node.OnCheckStateChanged();
                            update++;
                        }
                    }
                }
            }
            else
            {
                update += UpdateParentCheckBoxes();
            }

            if (this.TreeViewElement != null)
            {
                this.TreeViewElement.EndUpdate(update > 0, RadTreeViewElement.UpdateActions.StateChanged);
            }
        }

        private int UpdateParentCheckBoxes()
        {
            if (this.TreeViewElement != null && !this.TreeViewElement.TriStateMode)
            {
                return 0;
            }

            int update = 0;
            Stack<RadTreeNode> parents = new Stack<RadTreeNode>();
            parents.Push(this.parent);
            while (parents.Count > 0)
            {
                RadTreeNode parentNode = parents.Pop();
                if (parentNode.CheckType == this.CheckType)
                {
                    bool equal = true;
                    foreach (RadTreeNode child in parentNode.Nodes)
                    {
                        if (child.CheckState != this.CheckState)
                        {
                            equal = false;
                            break;
                        }
                    }

                    ToggleState state = this.CheckState;
                    if (!equal)
                    {
                        state = ToggleState.Indeterminate;
                    }

                    if (parentNode.SetCheckStateCore(state))
                    {
                        parentNode.OnCheckStateChanged();
                        update++;
                    }

                    if (parentNode.Parent != null)
                    {
                        parents.Push(parentNode.Parent);
                    }
                }
            }

            return update;
        }

        protected virtual void UpdateChildrenCheckState()
        {
            if (this.CheckType != UI.CheckType.CheckBox || (this.TreeViewElement != null && !this.TreeViewElement.TriStateMode))
            {
                return;
            }

            if (this.TreeViewElement != null && !this.TreeViewElement.AutoCheckChildNodes)
            {
                return;
            }

            if (this.TreeViewElement != null)
            {
                this.TreeViewElement.BeginUpdate();
            }

            int update = 0;
            Stack<RadTreeNodeCollection> nodeStack = new Stack<RadTreeNodeCollection>();
            nodeStack.Push(this.Nodes);
            while (nodeStack.Count > 0)
            {
                RadTreeNodeCollection nodes = nodeStack.Pop();
                foreach (RadTreeNode child in nodes)
                {
                    if (child.SetCheckStateCore(this.CheckState))
                    {
                        child.OnCheckStateChanged();
                        update++;
                    }

                    if (child.Nodes.Count > 0)
                    {
                        nodeStack.Push(child.nodes);
                    }
                }
            }

            if (this.TreeViewElement != null)
            {
                this.TreeViewElement.EndUpdate(update > 0, RadTreeViewElement.UpdateActions.StateChanged);
            }
        }

        private void OnChildrenSizeChanged(Size newChildrenSize)
        {
            if (parent != null && !state[SuspendNotificationsState])
            {
                parent.ChildrenSize = UpdateChildrenSize(parent.childrenSize, newChildrenSize, childrenSize);
            }
            this.childrenSize = newChildrenSize;
        }

        private void OnActualSizeChanged(Size newSize)
        {
            if (parent != null && !state[SuspendNotificationsState])
            {
                Size temp = newSize;
                if (treeView != null)
                {
                    temp.Height += treeView.NodeSpacing;
                    if (actualSize.Height != 0)
                    {
                        actualSize.Height += treeView.NodeSpacing;
                    }
                }
                parent.ChildrenSize = UpdateChildrenSize(parent.childrenSize, temp, actualSize);
            }
            this.actualSize = newSize;
        }

        private void OnExpandedChanged(bool expanded)
        {
            if (!state[UpdateParentSizeOnExpandedChangedState])
            {
                state[UpdateParentSizeOnExpandedChangedState] = true;
                return;
            }

            if (expanded)
            {
                if (this.parent != null && this.childrenSize.Height != 0)
                {
                    Size childrenSize = this.parent.ChildrenSize;
                    if (childrenSize.Height != 0)
                    {
                        childrenSize.Height += this.childrenSize.Height;
                    }
                    childrenSize.Width = Math.Max(this.childrenSize.Width, childrenSize.Width);
                    this.parent.ChildrenSize = childrenSize;
                }
            }
            else
            {
                if (this.parent != null)
                {
                    Size childrenSize = this.parent.ChildrenSize;

                    if (this.childrenSize.Height != 0)
                    {
                        if (childrenSize.Height != 0)
                        {
                            childrenSize.Height -= this.childrenSize.Height;
                        }
                    }
                    if (this.childrenSize.Width != 0 && childrenSize.Width == this.childrenSize.Width)
                    {
                        int newWidth = 0;
                        foreach (RadTreeNode node in parent.Nodes)
                        {
                            if ((node.Visible || IsInDesignMode) && node.Expanded && node != this)
                            {
                                newWidth = Math.Max(newWidth, node.ChildrenSize.Width);
                            }
                            newWidth = Math.Max(newWidth, node.ActualSize.Width);
                        }
                        childrenSize.Width = newWidth;
                    }
                    this.parent.ChildrenSize = childrenSize;
                }
            }
        }

        private Size UpdateChildrenSize(Size childrenSize, Size newSize, Size oldSize)
        {
            if (newSize.Width > childrenSize.Width)
            {
                childrenSize.Width = newSize.Width;
            }
            else if (newSize.Width < childrenSize.Width && oldSize.Width == childrenSize.Width)
            {
                childrenSize.Width = newSize.Width;
                foreach (RadTreeNode node in parent.Nodes)
                {
                    if ((node.Visible || IsInDesignMode) && node.Expanded && node != this)
                    {
                        childrenSize.Width = Math.Max(childrenSize.Width, node.ChildrenSize.Width);
                    }
                    if (node != this)
                    {
                        childrenSize.Width = Math.Max(childrenSize.Width, node.ActualSize.Width);
                    }
                }
            }

            if (newSize.Height != oldSize.Height)
            {
                if (oldSize.Height != 0)
                {
                    childrenSize.Height += newSize.Height - oldSize.Height;
                }
                else
                {
                    childrenSize.Height += newSize.Height;
                }
            }

            childrenSize.Height = Math.Max(0, childrenSize.Height);

            return childrenSize;
        }

        #endregion

        #region ICloneable Members

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public object Clone()
        {
            Type type = base.GetType();
            RadTreeNode node = null;
            if (type == typeof(RadTreeNode))
            {
                node = new RadTreeNode(this.Text);
            }
            else
            {
                node = (RadTreeNode)Activator.CreateInstance(type);
            }

            node.Text = this.Text;
            node.Name = this.name;
            node.ToolTipText = this.toolTipText;
            node.ContextMenu = this.contextMenu;
            node.ItemHeight = this.ItemHeight;
            node.Visible = this.Visible;
            //node.Selected = this.Selected;
            node.Image = this.Image;
            node.ImageKey = this.ImageKey;
            node.ImageIndex = this.ImageIndex;
            node.Tag = this.Tag;
            node.ToolTipText = this.ToolTipText;
            node.Enabled = this.Enabled;

            if (this.style != null)
            {
                node.ForeColor = this.Style.ForeColor;
                node.BackColor = this.Style.BackColor;
                node.BackColor2 = this.Style.BackColor2;
                node.BackColor3 = this.Style.BackColor3;
                node.BackColor4 = this.Style.BackColor4;
                node.GradientAngle = this.Style.GradientAngle;
                node.GradientStyle = this.Style.GradientStyle;
                node.GradientPercentage = this.Style.GradientPercentage;
                node.GradientPercentage2 = this.Style.GradientPercentage2;
                node.NumberOfColors = this.Style.NumberOfColors;
                node.TextAlignment = this.Style.TextAlignment;
                node.Font = this.Style.Font;
                node.BorderColor = this.Style.BorderColor;
            }

            for (int i = 0; i < this.Nodes.Count; i++)
            {
                node.Nodes.Add((RadTreeNode)this.Nodes[i].Clone());
            }

            node.Expanded = this.Expanded;
            node.CheckState = this.CheckState;
            node.CheckType = this.CheckType;
            node.Tag = this.Tag;
            return node;
        }

        #endregion

        #region IDataItem Members

        /// <summary>
        /// Gets the data-bound object that populated the node.
        /// </summary>
        [Browsable(false)]
        public object DataBoundItem
        {
            get { return ((IDataItem)this).DataBoundItem; }
        }

        object IDataItem.DataBoundItem
        {
            get
            {
                return dataBoundItem;
            }
            set
            {
                if (value != dataBoundItem)
                {
                    dataBoundItem = value;
                }
            }
        }

        int IDataItem.FieldCount
        {
            get
            {
                return 1;
            }
        }

        object IDataItem.this[string name]
        {
            get { return this.Text; }
            set { this.Text = value.ToString(); }
        }

        object IDataItem.this[int index]
        {
            get { return this.Text; }
            set { this.Text = value.ToString(); }
        }

        int IDataItem.IndexOf(string name)
        {
            return 0;
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;
        public bool deleted;

        /// <summary>
        /// Allows PropertyChanged notifications to be temporary suspended.
        /// </summary>
        public void SuspendPropertyNotifications()
        {
            this.state[SuspendNotificationsState] = true;
        }

        /// <summary>
        /// Resumes property notifications after a previous <see cref="RadTreeNode.SuspendPropertyNotifications">SuspendPropertyNotifications</see> call.
        /// </summary>
        public void ResumePropertyNotifications()
        {
            this.state[SuspendNotificationsState] = false;
        }

        protected virtual bool SetBooleanProperty(string propertyName, int propertyKey, bool value)
        {
            bool oldValue = this.state[propertyKey];

            if (oldValue == value)
            {
                return false;
            }

            this.state[propertyKey] = value;

            if (!this.state[SuspendNotificationsState])
            {
                OnNotifyPropertyChanged(new PropertyChangedEventArgs(propertyName));
            }

            return true;
        }

        protected virtual void OnNotifyPropertyChanged(string name)
        {
            OnNotifyPropertyChanged(new PropertyChangedEventArgs(name));
        }

        protected virtual void OnNotifyPropertyChanged(PropertyChangedEventArgs args)
        {
            if (this.TreeViewElement != null)
            {
                if (args.PropertyName == "Expanded")
                {
                    this.NotifyExpandedChanged(this);
                }
                else if (args.PropertyName == "CheckType")
                {
                    this.TreeViewElement.Update(RadTreeViewElement.UpdateActions.StateChanged, this);
                }
                else if (args.PropertyName == "Visible")
                {
                    this.TreeViewElement.Update(RadTreeViewElement.UpdateActions.Resume);
                }
                else if (args.PropertyName == "Current")
                {
                    RadTreeNode newCurrent = this.Current ? this : null;

                    // If we have canceled the SelectedNodeChaning we should
                    // rollback the value property
                    if (!this.TreeViewElement.ProcessCurrentNode(newCurrent, true))
                    {
                        this.state[IsCurrentState] = !this.Current;
                        return;
                    }

                    if (this.TreeViewElement != null && !this.TreeViewElement.MultiSelect)
                    {
                        this.Selected = this.Current;
                    }
                }
                else if (args.PropertyName == "Selected")
                {
                    if (this.TreeViewElement != null && !this.TreeViewElement.MultiSelect)
                    {
                        this.Current = this.Selected;

                        // If we have canceled the SelectedNodeChaning we should
                        // rollback the value property
                        if (this.Current != this.Selected)
                        {
                            this.state[IsSelectedState] = !this.Selected;
                            return;
                        }
                    }

                    this.TreeViewElement.SelectedNodes.ProcessSelectedNode(this);
                }
            }

            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, args);
            }
        }

        protected virtual void NotifyExpandedChanged(RadTreeNode node)
        {
            RadTreeViewElement.UpdateActions action = RadTreeViewElement.UpdateActions.ExpandedChanged;

            //if (this.TreeViewElement != null && this.TreeViewElement.ExpandAnimation != ExpandAnimation.None)
            //{
            //    action = RadTreeViewElement.UpdateActions.ExpandedChangedUsingAnimation;
            //}

            if (node != null)
            {
                this.TreeViewElement.Update(action, node);
            }
            else
            {
                this.TreeViewElement.Update(action);
            }
        }

        private void Style_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.TreeViewElement != null)
            {
                this.TreeViewElement.Update(RadTreeViewElement.UpdateActions.StateChanged, this);
            }
        }

        #endregion

        #region Legacy

        /// <summary>
        /// Gets the next sibling tree node. 
        /// </summary>
        /// <remarks>
        /// This property gets the next node disregarding of its visibility. This could be
        /// used for traversing nodes through the RadTreeView.
        /// </remarks>
        [Description("Gets the next sibling tree node.")]
        [Category("Behavior")]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Obsolete("This property will be removed in the next version. Please use the NextNode instead.")]
        public RadTreeNode NextSiblingNode
        {
            get
            {
                return this.NextNode;
            }
        }

        /// <summary>
        /// Gets the previous sibling tree node. 
        /// </summary>
        /// <remarks>
        /// This property gets the previous node disregarding of its visibility. This could
        /// be used for traversing nodes through the RadTreeView.
        /// </remarks>
        [Description("Gets the previous sibling tree node.")]
        [Category("Behavior")]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Obsolete("This property will be removed in the next version. Please use the PrevNode instead.")]
        public RadTreeNode PrevSiblingNode
        {
            get
            {
                return PrevNode;
            }
        }

        /// <summary>
        /// Gets or sets the font of the node text.
        /// </summary>
        /// <value>The default value is null.</value>
        [NotifyParentProperty(true)]
        [Category("Appearance"), DefaultValue(null)]
        [Description("Gets or sets the font of the node text.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Font Font
        {
            get
            {
                if (HasStyle)
                {
                    return this.Style.Font;
                }
                return null;
            }
            set
            {
                if (Font != value)
                {
                    this.Style.Font = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the foreground color of the tree node. This color is applied to the text label.
        /// </summary>
        [NotifyParentProperty(true)]
        [Category("Appearance"), DefaultValue(typeof(Color), "")]
        [Description("Gets or sets the foreground color of the tree node. This color is applied to the text label.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public virtual Color ForeColor
        {
            get
            {
                if (HasStyle)
                {
                    return this.Style.ForeColor;
                }
                return Color.Empty;
            }
            set
            {
                if (ForeColor != value)
                {
                    this.Style.ForeColor = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the backcolor of the tree node. Color type represents an ARGB color.
        /// </summary>
        /// <seealso cref="GradientStyle">GradientStyle Property</seealso>
        /// <seealso cref="GradientStyle">NumberOfColors Property</seealso>
        [NotifyParentProperty(true)]
        [Category("Appearance"), DefaultValue(typeof(Color), "")]
        [Description("Gets or sets the first back color.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color BackColor
        {
            get
            {
                if (HasStyle)
                {
                    return this.Style.BackColor;
                }
                return Color.Empty;
            }
            set
            {
                if (BackColor != value)
                {
                    this.Style.BackColor = value;
                    OnNotifyPropertyChanged("Style");
                }
            }
        }

        /// <summary>
        /// Gets or sets the backcolor of the tree node. This property is applicable to radial, glass,
        /// office glass, gel, and vista gradients.
        /// </summary>
        /// <seealso cref="GradientStyle">GradientStyle Property</seealso>
        /// <seealso cref="GradientStyle">NumberOfColors Property</seealso>
        [NotifyParentProperty(true)]
        [Category("Appearance"), DefaultValue(typeof(Color), "")]
        [Description("Gets or sets the backcolor of the tree node.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color BackColor2
        {
            get
            {
                if (HasStyle)
                {
                    return this.Style.BackColor2;
                }
                return Color.Empty;
            }
            set
            {
                if (BackColor2 != value)
                {
                    this.Style.BackColor2 = value;
                    OnNotifyPropertyChanged("Style");
                }
            }
        }

        /// <summary>
        /// Gets or sets the backcolor of the tree node. This property is applicable to radial, glass,
        /// office glass, and vista gradients.
        /// </summary>
        /// <seealso cref="GradientStyle">GradientStyle Property</seealso>
        /// <seealso cref="GradientStyle">NumberOfColors Property</seealso>
        [NotifyParentProperty(true)]
        [Category("Appearance"), DefaultValue(typeof(Color), "")]
        [Description("Gets or sets the backcolor of the tree node.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color BackColor3
        {
            get
            {
                if (HasStyle)
                {
                    return this.Style.BackColor3;
                }
                return Color.Empty;
            }
            set
            {
                if (BackColor3 != value)
                {
                    this.Style.BackColor3 = value;
                    OnNotifyPropertyChanged("Style");
                }
            }
        }

        /// <summary>
        /// Gets or sets the backcolor of the tree node. This property is applicable to radial, glass,
        /// office glass, and vista gradients.
        /// </summary>
        /// <seealso cref="GradientStyle">GradientStyle Property</seealso>
        /// <seealso cref="GradientStyle">NumberOfColors Property</seealso>
        [NotifyParentProperty(true)]
        [Category("Appearance"), DefaultValue(typeof(Color), "")]
        [Description("Gets or sets the backcolor of the tree node.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color BackColor4
        {
            get
            {
                if (HasStyle)
                {
                    return this.Style.BackColor4;
                }
                return Color.Empty;
            }
            set
            {
                if (BackColor4 != value)
                {
                    this.Style.BackColor4 = value;
                    OnNotifyPropertyChanged("Style");
                }
            }
        }

        /// <summary>
        /// Gets or sets the border color of the tree node.
        /// </summary>		
        [NotifyParentProperty(true)]
        [Category("Appearance"), DefaultValue(typeof(Color), "")]
        [Description("Gets or sets the border color of the tree node.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color BorderColor
        {
            get
            {
                if (HasStyle)
                {
                    return this.Style.BorderColor;
                }
                return Color.Empty;
            }
            set
            {
                if (BorderColor != value)
                {
                    this.Style.BorderColor = value;
                    OnNotifyPropertyChanged("Style");
                }
            }
        }

        /// <summary>
        /// Gets or sets gradient angle for linear gradient.
        /// </summary>
        /// <seealso cref="GradientStyle">GradientStyle Property</seealso>
        /// <seealso cref="GradientPercentage">GradientPercentage Property</seealso>
        /// <seealso cref="GradientPercentage2">GradientPercentage2 Property</seealso>
        /// <seealso cref="NumberOfColors">NumberOfColors Property</seealso>
        /// <value>The default value is 90.0.</value>
        [NotifyParentProperty(true)]
        [Category("Appearance"), DefaultValue(90.0f)]
        [Description("Gets or sets gradient angle for linear gradient.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public float GradientAngle
        {
            get
            {
                if (HasStyle)
                {
                    return this.Style.GradientAngle;
                }
                return 90.0f;
            }
            set
            {
                if (GradientAngle != value)
                {
                    this.Style.GradientAngle = value;
                    OnNotifyPropertyChanged("Style");
                }
            }
        }

        /// <summary>
        /// Gets or sets GradientPercentage for linear, glass, office glass, gel, vista, and
        /// radial gradients.
        /// </summary>
        /// <seealso cref="GradientStyle">GradientStyle Property</seealso>
        /// <seealso cref="GradientPercentage2">GradientPercentage2 Property</seealso>
        /// <seealso cref="GradientAngle">GradientAngle Property</seealso>
        /// <seealso cref="NumberOfColors">NumberOfColors Property</seealso>
        /// <value>The default value is 0.5.</value>
        [NotifyParentProperty(true)]
        [Category("Appearance"), DefaultValue(0.5f)]
        [Description("Gets or sets GradientPercentage for linear, glass, office glass, gel, vista and radial gradients.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public float GradientPercentage
        {
            get
            {
                if (HasStyle)
                {
                    return this.Style.GradientPercentage;
                }
                return 0.5f;
            }
            set
            {
                if (GradientPercentage != value)
                {
                    this.Style.GradientPercentage = value;
                    OnNotifyPropertyChanged("Style");
                }
            }
        }

        /// <summary>
        /// Gets or sets GradientPercentage for office glass, vista, and radial
        /// gradients.
        /// </summary>
        /// <seealso cref="GradientStyle">GradientStyle Property</seealso>
        /// <seealso cref="GradientPercentage">GradientPercentage Property</seealso>
        /// <seealso cref="GradientAngle">GradientAngle Property</seealso>
        /// <seealso cref="NumberOfColors">NumberOfColors Property</seealso>
        /// <value>The default value is 0.5.</value>
        [NotifyParentProperty(true)]
        [Category("Appearance"), DefaultValue(0.5f)]
        [Description("Gets or sets GradientPercentage for office glass, vista, and radial gradients.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public float GradientPercentage2
        {
            get
            {
                if (HasStyle)
                {
                    return this.Style.GradientPercentage2;
                }
                return 0.5f;
            }
            set
            {
                if (GradientPercentage2 != value)
                {
                    this.Style.GradientPercentage2 = value;
                    OnNotifyPropertyChanged("Style");
                }
            }
        }

        /// <summary>
        /// Gets and sets the gradient style. The possible values are defined in the gradient
        /// style enumeration: solid, linear, radial, glass, office glass, gel, and vista.
        /// </summary>
        /// <value>
        ///     The default value is
        ///     <see cref="Telerik.WinControls.GradientStyles">GradientStyles.Linear</see>.
        /// </value>
        /// <seealso cref="GradientStyles">GradientStyles Enumeration</seealso>
        /// <seealso cref="GradientPercentage">GradientPercentage Property</seealso>
        /// <seealso cref="GradientPercentage2">GradientPercentage2 Property</seealso>
        /// <seealso cref="GradientAngle">GradientAngle Property</seealso>
        /// <seealso cref="NumberOfColors">NumberOfColors Property</seealso>
        [NotifyParentProperty(true)]
        [Category("Appearance"), DefaultValue(GradientStyles.Linear)]
        [Description("Gets or sets the gradient angle.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public GradientStyles GradientStyle
        {
            get
            {
                if (HasStyle)
                {
                    return this.Style.GradientStyle;
                }
                return GradientStyles.Linear;
            }
            set
            {
                if (GradientStyle != value)
                {
                    this.Style.GradientStyle = value;
                    OnNotifyPropertyChanged("Style");
                }
            }
        }

        /// <summary>
        /// Gets or sets the number of used colors in the gradient effect.
        /// </summary>
        /// <seealso cref="BackColor">BackColor Property</seealso>
        /// <seealso cref="BackColor2">BackColor2 Property</seealso>
        /// <seealso cref="BackColor3">BackColor3 Property</seealso>
        /// <seealso cref="BackColor4">BackColor4 Property</seealso>
        /// <seealso cref="GradientStyle">GradientStyle Property</seealso>
        /// <value>The default value is 4.</value>
        [NotifyParentProperty(true)]
        [Category("Appearance"), DefaultValue(4)]
        [Description("Gets or sets the number of used colors in the gradient effect.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int NumberOfColors
        {
            get
            {
                if (HasStyle)
                {
                    return this.Style.NumberOfColors;
                }
                return 4;
            }
            set
            {
                if (NumberOfColors != value)
                {
                    this.Style.NumberOfColors = value;
                    OnNotifyPropertyChanged("Style");
                }
            }
        }

        /// <summary>
        /// Gets or sets the text alignment.
        /// </summary>
        /// <value>
        ///     The default value is <a href="ms-help://MS.VSCC.v80/MS.MSDN.v80/MS.NETDEVFX.v20.en/cpref2/html/T_System_String.htm">ContentAlignment.MiddleLeft</a>.
        /// </value>		
        [NotifyParentProperty(true)]
        [Category("Appearance"), DefaultValue(ContentAlignment.MiddleLeft)]
        [Description("Gets or sets the text alignment.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public ContentAlignment TextAlignment
        {
            get
            {
                if (HasStyle)
                {
                    return this.Style.TextAlignment;
                }
                return ContentAlignment.MiddleLeft;
            }
            set
            {
                if (TextAlignment != value)
                {
                    this.Style.TextAlignment = value;
                    OnNotifyPropertyChanged("Style");
                }
            }
        }

        /// <summary>
        /// Gets or sets whether the tree node will display a radio button.
        /// </summary>
        /// <seealso cref="ChildListType">ChildListType Property</seealso>
        /// <remarks>
        /// If there is parent node, it is required parent's <see cref="ChildListType">ChildListType property</see> to be set
        /// to OptionList in order to set this to true.
        /// </remarks>
        /// <value>The default value is false.</value>
        [Description("Gets or sets whether the tree node will display a radio button.")]
        [Category("Behavior")]
        [DefaultValue(false)]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Obsolete("This property will be removed in the next version. Please use the CheckType instead.")]
        public bool ShowRadioButton
        {
            get
            {
                return this.CheckType == UI.CheckType.RadioButton;
            }
            set
            {
                if (value)
                {
                    this.CheckType = UI.CheckType.RadioButton;
                }
                else if (this.CheckType == UI.CheckType.RadioButton)
                {
                    this.CheckType = UI.CheckType.None;
                }
            }
        }

        /// <summary>
        /// Gets or sets whether the tree node will display a check box.
        /// </summary>
        /// <seealso cref="ChildListType">ChildListType Property</seealso>
        /// <remarks>
        /// If there is parent node, it is required parent's <see cref="ChildListType">ChildListType property</see> to be set
        /// to CheckList in order to set this to true.
        /// </remarks>
        /// <value>The default value is false.</value>
        [DefaultValue(true)]
        [Description("Gets or sets whether the tree node will display a check box.")]
        [Category("Behavior")]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Obsolete("This property will be removed in the next version. Please use the CheckType instead.")]
        public bool ShowCheckBox
        {
            get
            {
                return this.CheckType == UI.CheckType.CheckBox;
            }
            set
            {
                if (value)
                {
                    this.CheckType = UI.CheckType.CheckBox;
                }
                else if (this.CheckType == UI.CheckType.CheckBox)
                {
                    this.CheckType = UI.CheckType.None;
                }
            }
        }

        /// <summary>
        /// Gets or sets the type of option list formed by child nodes.
        /// </summary>
        /// <remarks>
        /// If this property is set to a CheckList, then ShowRadioButton property of the
        /// child nodes cannot be set to true. Respectively, if it is set to OptionList, then
        /// ShowCheckBox property of the child nodes cannot be set to true.
        /// </remarks>
        /// <seealso cref="ShowCheckBox">ShowCheckBox Property</seealso>
        /// <seealso cref="ShowRadioButton">ShowRadioButton Property</seealso>
        /// <value>The default value is <see cref="ChildListType">ChildListType.Custom</see>.</value>
        [DefaultValue(ChildListType.CheckList)]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Obsolete("This property will be removed in the next version. Please use the CheckType instead.")]
        public ChildListType ChildListType
        {
            get
            {
                switch (this.CheckType)
                {
                    case CheckType.CheckBox:
                        return UI.ChildListType.CheckList;
                    case CheckType.RadioButton:
                        return UI.ChildListType.OptionList;

                    default:
                        return UI.ChildListType.Custom;
                }
            }
            set
            {
                switch (value)
                {
                    case ChildListType.CheckList:
                        this.CheckType = UI.CheckType.CheckBox;
                        break;
                    case ChildListType.OptionList:
                        this.CheckType = UI.CheckType.RadioButton;
                        break;
                    default:
                        this.CheckType = UI.CheckType.None;
                        break;
                }
            }
        }

        /// <summary>
        /// Gets or sets if the node was loaded on demand.
        /// </summary>
        /// <value>The default value is false.</value>
        [DefaultValue(false)]
        [Description("Gets or sets if the node was loaded on demand.")]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Obsolete("This property will be removed in the next version. Please use TreeNodeCollection provider instead.")]
        public bool LoadedOnDemand
        {
            get
            {
                return false;
            }
            set
            {
            }
        }

        /// <summary>
        /// Checks if the node has a particular node in its collection
        /// </summary>
        /// <param name="node">the node to be searched</param>
        /// <returns>boolean value indicating the node to be searched</returns>
        [Description("Checks if the node has a particular node in its collection")]
        [Obsolete("This property will be removed in the next version. Please use TreeNodeCollection Contains instead.")]
        public bool Contains(RadTreeNode node)
        {
            if (null == node)
            {
                return false;
            }

            for (RadTreeNode node1 = node.Parent; node1 != null; node1 = node1.Parent)
            {
                if (node1 == this)
                {
                    return true;
                }
            }
            return false;
        }

        #endregion
    }
}

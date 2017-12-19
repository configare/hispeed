using System.ComponentModel;
using System.Collections.Specialized;
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Design;

namespace Telerik.WinControls.UI
{
    public abstract class PropertyGridItemBase : INotifyPropertyChanged
    {
        #region Fields

        protected const int SuspendNotificationsState = 1;
        protected const int IsExpandedState = SuspendNotificationsState << 1;
        protected const int IsVisibleState = IsExpandedState << 1;
        protected const int IsEnableState = IsVisibleState << 1;
        protected const int IsModifiedState = IsEnableState << 1;

        protected BitVector32 state = new BitVector32();

        private object tag;
        private int itemHeight;
        protected string text;
        protected string description;
        private string toolTipText;
        
        private string imageKey;
        private int imageIndex = -1;
        private Image image;

        private RadContextMenu contextMenu;

        private PropertyGridTableElement propertyGridTableElement;

        #endregion

        #region Initialize
        
        public PropertyGridItemBase(PropertyGridTableElement propertyGridElement)
        {
            this.propertyGridTableElement = propertyGridElement;
            this.Visible = true;
            this.state[IsEnableState] = true;
            this.state[IsVisibleState] = true;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the parent property grid that the item is assigned to. 
        /// </summary>
        public virtual PropertyGridTableElement PropertyGridTableElement
        {
            get
            {
                return this.propertyGridTableElement;
            }
        }

        #region Behavior

        /// <summary>
        /// Gets or sets a value indicating whether this instance is visible.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is visible; otherwise, <c>false</c>.
        /// </value>
        public virtual bool Visible
        {
            get
            {
                return state[IsVisibleState];
            }
            set
            {
                SetBooleanProperty("Visible", IsVisibleState, value);
                this.PropertyGridTableElement.Update(UI.PropertyGridTableElement.UpdateActions.StateChanged);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this item is selected.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this item is selected; otherwise, <c>false</c>.
        /// </value>
        public virtual bool Selected
        {
            get
            {
                if (this.propertyGridTableElement != null)
                {
                    return this.propertyGridTableElement.SelectedGridItem == this;
                }
                return false;
            }
            set
            {
                if (value)
                {
                    this.propertyGridTableElement.SelectedGridItem = this;
                }
                else if (this.propertyGridTableElement.SelectedGridItem == this)
                {
                    this.propertyGridTableElement.SelectedGridItem = null;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this item is expanded.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this item is expanded; otherwise, <c>false</c>.
        /// </value>
        public virtual bool Expanded
        {
            get
            {
                return state[IsExpandedState];
            }
            set
            {
                if (Expanded == value)
                {
                    return;
                }

                if (this.PropertyGridTableElement != null)
                {
                    RadPropertyGridCancelEventArgs args = new RadPropertyGridCancelEventArgs(this);
                    this.PropertyGridTableElement.OnItemExpandedChanging(args);
                    if (args.Cancel)
                    {
                        return;
                    }
                }

                this.SetBooleanProperty("Expanded", IsExpandedState, value);
                this.Update(PropertyGridTableElement.UpdateActions.ExpandedChanged);

                if (this.PropertyGridTableElement != null)
                {
                    this.PropertyGridTableElement.OnItemExpandedChanged(new RadPropertyGridEventArgs(this));
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the item can respond to user interaction.
        /// </summary>
        /// <value>The default value is true.</value>
        public virtual bool Enabled
        {
            get
            {
                return state[IsEnableState];
            }
            set
            {
                if (Enabled != value)
                {
                    SetBooleanProperty("Enabled", IsEnableState, value);
                    Update(UI.PropertyGridTableElement.UpdateActions.StateChanged);
                }
            }
        }

        #endregion

        #region Appearance

        /// <summary>
        /// Gets or sets the height of the item.
        /// </summary>
        /// <value>The default value is 20.</value>
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
                    this.PropertyGridTableElement.Update(PropertyGridTableElement.UpdateActions.Resume);
                    this.OnNotifyPropertyChanged("ItemHeight");
                }
            }
        }

        /// <summary>
        /// Gets or sets the image of the node.
        /// </summary>		
        /// <seealso cref="ImageIndex">ImageIndex Property</seealso>		
        /// <seealso cref="ImageKey">ImageKey Property</seealso>		
        /// <seealso cref="SelectedImage">SelectedImage Property</seealso>		
        /// <seealso cref="StateImage">StateImage Property</seealso>		
        [TypeConverter(typeof(Telerik.WinControls.Primitives.ImageTypeConverter))]
        public virtual Image Image
        {
            get
            {
                if (this.image != null)
                {
                    return this.image;
                }

                int index = (this.ImageIndex >= 0) ? this.ImageIndex : this.propertyGridTableElement.ImageIndex;
                if (index >= 0 && string.IsNullOrEmpty(this.ImageKey))
                {
                    RadControl control = this.propertyGridTableElement.ElementTree.Control as RadControl;
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
                if (this.propertyGridTableElement != null)
                {
                    treeKey = this.propertyGridTableElement.ImageKey;
                }

                string key = (string.IsNullOrEmpty(this.ImageKey)) ? treeKey : this.ImageKey;
                if (!string.IsNullOrEmpty(key))
                {
                    RadControl control = this.propertyGridTableElement.ElementTree.Control as RadControl;
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
                    Update(UI.PropertyGridTableElement.UpdateActions.StateChanged);
                }
            }
        }

        /// <summary>
        /// Gets or sets the left image list index value of the image displayed when the tree
        /// node is not selected.
        /// </summary>
        /// <seealso cref="Image">Image Property</seealso>
        /// <seealso cref="ImageKey">ImageKey Property</seealso>
        [RelatedImageList("PropertyGrid.ImageList"),
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
                    Update(UI.PropertyGridTableElement.UpdateActions.StateChanged);
                }
            }
        }

        /// <summary>
        /// 	<see cref="SelectedImageKey">SelectedImageKey Property</see>Gets or sets the key
        ///     for the left image associated with this tree node when the node is not selected.
        /// </summary>
        /// <seealso cref="Image">Image Property</seealso>
        /// <seealso cref="ImageIndex">ImageIndex Property</seealso>
        [RelatedImageList("PropertyGrid.ImageList"),
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
                    Update(UI.PropertyGridTableElement.UpdateActions.StateChanged);
                }
            }
        }
        
        /// <summary>
        /// Gets or sets the text associated with this item.
        /// </summary>
        public virtual string Label
        {
            get
            {
                return this.text;
            }
            set
            {
                if (this.text != value)
                {
                    this.text = value;
                    Update(UI.PropertyGridTableElement.UpdateActions.StateChanged);
                    OnNotifyPropertyChanged(new PropertyChangedEventArgs("Text"));
                }
            }
        }

        /// <summary>
        /// Gets or sets the description associated with this item.
        /// </summary>
        public virtual string Description
        {
            get
            {
                return this.description;
            }
            set
            {
                if (this.description != value)
                {
                    this.description = value;
                    Update(UI.PropertyGridTableElement.UpdateActions.StateChanged);
                    OnNotifyPropertyChanged(new PropertyChangedEventArgs("Description"));
                }
            }
        }

        /// <summary>
        /// Gets or sets the tool tip text associated with this item.
        /// </summary>
        public virtual string ToolTipText
        {
            get
            {
                return this.toolTipText;
            }
            set
            {
                if (this.toolTipText != value)
                {
                    this.toolTipText = value;
                    Update(UI.PropertyGridTableElement.UpdateActions.StateChanged);
                    OnNotifyPropertyChanged(new PropertyChangedEventArgs("ToolTipText"));
                }
            }
        }
        
        /// <summary>Gets or sets the context menu associated to the item.</summary>
        /// <value>Returns an instance of <see cref="RadDropDownMenu">RadDropDownMenu Class</see> that
        /// is associated with the item. The default value is null.</value>
        /// <remarks>
        /// This property could be used to associate a custom menu and replace the property grid's
        /// default. If the context menu is invoked by right-clicking an item, the property grid's menu
        /// will not be shown and the context menu assigned to this item will be shown instead.
        /// </remarks>
        public virtual RadContextMenu ContextMenu
        {
            get
            {
                return this.contextMenu;
            }
            set
            {
                if (this.contextMenu != value)
                {
                    this.contextMenu = value;
                    this.OnNotifyPropertyChanged("ContextMenu");
                }
            }
        }

        #endregion

        #region Data

        /// <summary>
        /// Gets or sets the tag object that can be used to store user data, corresponding to the item.
        /// </summary>
        /// <value>The tag.</value>
        public virtual object Tag
        {
            get
            {
                return this.tag;
            }
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
        /// Gets a value indicating how deep in the hierarchy this propety is.
        /// </summary>
        public int Level
        {
            get
            {
                int level = 0;
                PropertyGridItemBase parent = this.Parent;
                while (parent != null)
                {
                    parent = parent.Parent;
                    level++;
                }
                return level;
            }
        }

        /// <summary>
        /// Gets the child items list associated with this item.
        /// </summary>
        public abstract PropertyGridItemCollection GridItems
        {
            get;
        }

        /// <summary>
        /// Gets a value indicating whether this item is expandable.
        /// </summary>
        public abstract bool Expandable
        {
            get;
        }

        /// <summary>
        /// Gets the parent item for this item.
        /// </summary>
        public virtual PropertyGridItemBase Parent
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the property name
        /// </summary>
        public abstract string Name
        {
            get;
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Expandes the item.
        /// </summary>
        public virtual void Expand()
        {
            Expanded = true;
        }
        
        /// <summary>
        /// Collapses the item.
        /// </summary>
        public virtual void Collapse()
        {
            Expanded = false;
        }

        /// <summary>
        /// Ensures that this item is visible in the content of the RadPropertyGridElement.
        /// </summary>
        public virtual void EnsureVisible()
        {
            this.propertyGridTableElement.EnsureVisible(this);
        }

        /// <summary>
        /// Selects the grid tiem.
        /// </summary>
        public virtual void Select()
        {
            this.Selected = true;
        }

        #endregion

        #region Implementation
        
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

        protected virtual void Update(PropertyGridTableElement.UpdateActions updateAction)
        {
            if (this.state[SuspendNotificationsState])
            {
                return;
            }

            if (this.propertyGridTableElement != null)
            {
                this.propertyGridTableElement.Update(updateAction);
            }
        }

        /// <summary>
        /// Allows PropertyChanged notifications to be temporary suspended.
        /// </summary>
        public void SuspendPropertyNotifications()
        {
            this.state[SuspendNotificationsState] = true;
        }

        /// <summary>
        /// Resumes property notifications after a previous SuspendPropertyNotifications call.
        /// </summary>
        public void ResumePropertyNotifications()
        {
            this.state[SuspendNotificationsState] = false;
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void OnNotifyPropertyChanged(string name)
        {
            OnNotifyPropertyChanged(new PropertyChangedEventArgs(name));
        }

        public virtual void OnNotifyPropertyChanged(PropertyChangedEventArgs args)
        {
            if (!state[SuspendNotificationsState])
            {
                PropertyChangedEventHandler handler = PropertyChanged;
                if (handler != null)
                {
                    handler(this, args);
                }
            }
        }

        #endregion
    }
}

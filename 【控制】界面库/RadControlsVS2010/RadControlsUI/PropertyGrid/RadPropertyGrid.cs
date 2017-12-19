using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Design;
using Telerik.WinControls.Data;
using Telerik.WinControls.Design;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Displays the properties of an object in a grid with two columns with a property name in the first column and value in the second.
    /// </summary>
    [TelerikToolboxCategory(ToolboxGroupStrings.EditorsGroup)]
    [ToolboxItem(true)]
    [Description("Displays the properties of an object in a grid with two columns with a property name in the first column and value in the second.")]
    [DefaultProperty("SelectedObject"), DefaultEvent("SelectedItemChanged")]
    [Designer(DesignerConsts.RadPropertyGridDesignerString)]
    public class RadPropertyGrid : RadControl
    {
        #region Fields

        private PropertyGridElement propertyGridElement;

        #endregion

        #region Constructors & Initialization

        public RadPropertyGrid()
        {
            this.RadContextMenu = new PropertyGridDefaultContextMenu(this.PropertyGridElement.PropertyTableElement);
        }

        protected override void CreateChildItems(RadElement parent)
        {
            this.propertyGridElement = CreatePropertyGridElement();
            parent.Children.Add(this.propertyGridElement);
        }

        protected virtual PropertyGridElement CreatePropertyGridElement()
        {
            return new PropertyGridElement();
        }

        protected override void Dispose(bool disposing)
        {
            if (RadContextMenu != null && RadContextMenu.GetType() == typeof(PropertyGridDefaultContextMenu))
            {
                RadContextMenu.Dispose();
                RadContextMenu = null;
            }
            base.Dispose(disposing);
        }

        #endregion

        #region Properties

        #region Behavior
        
        /// <summary>
        /// Gets a value indicating whether there are currently open editors.
        /// </summary>
        [Description("Gets a value indicating whether there are currently open editors.")]
        [Category(RadDesignCategory.BehaviorCategory)]
        [Browsable(false)]
        public bool IsEditing
        {
            get
            {
                return this.PropertyGridElement.PropertyTableElement.IsEditing;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the user is allowed to edit the values of the properties.
        /// </summary>
        [Description("Gets or sets a value indicating whether the user is allowed to edit the values of the properties.")]
        [Category(RadDesignCategory.BehaviorCategory)]
        [DefaultValue(false)]
        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool ReadOnly
        {
            get
            {
                return this.PropertyGridElement.PropertyTableElement.ReadOnly;
            }
            set
            {
                this.PropertyGridElement.PropertyTableElement.ReadOnly = value;
            }
        }

        /// <summary>
        /// Gets the active editor.
        /// </summary>
        [Description("Gets the active editor.")]
        [Category(RadDesignCategory.BehaviorCategory)]
        [Browsable(false)]
        public IValueEditor ActiveEditor
        {
            get
            {
                return this.propertyGridElement.PropertyTableElement.ActiveEditor;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating how user begins editing a cell.
        /// </summary>
        [Description("Gets or sets a value indicating how user begins editing a cell.")]
        [Category(RadDesignCategory.BehaviorCategory)]
        [DefaultValue(RadPropertyGridBeginEditModes.BeginEditOnClick)]
        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public RadPropertyGridBeginEditModes BeginEditMode
        {
            get
            {
                return this.propertyGridElement.PropertyTableElement.BeginEditMode;
            }
            set
            {
                if (this.propertyGridElement.PropertyTableElement.BeginEditMode != value)
                {
                    this.propertyGridElement.PropertyTableElement.BeginEditMode = value;
                    OnNotifyPropertyChanged("BeginEditMode");
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the groups will be expanded or collapsed upon creation.
        /// </summary>
        [Description("Gets or sets a value indicating whether the groups will be expanded or collapsed upon creation.")]
        [Category(RadDesignCategory.BehaviorCategory)]
        [DefaultValue(true)]
        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool AutoExpandGroups
        {
            get
            {
                return this.PropertyGridElement.PropertyTableElement.AutoExpandGroups;
            }
            set
            {
                this.PropertyGridElement.PropertyTableElement.AutoExpandGroups = value;
            }
        }

        /// <summary>
        /// Gets or sets the shortcut menu associated with the control.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// A <see cref="Telerik.WinControls.UI.RadContextMenu"/> that represents the shortcut menu associated with the control.
        /// </returns>
        [Category("Behavior")]
        [DefaultValue(null)]
        [Description("Gets or sets the shortcut menu associated to the RadPropertyGrid.")]
        public virtual RadContextMenu RadContextMenu
        {
            get { return this.PropertyGridElement.PropertyTableElement.ContextMenu; }
            set
            {
                PropertyGridTableElement propertyTable = this.PropertyGridElement.PropertyTableElement;
                if (propertyTable.ContextMenu != value)
                {
                    if (propertyTable.ContextMenu is TreeViewDefaultContextMenu)
                    {
                        propertyTable.ContextMenu.Dispose();
                    }

                    propertyTable.ContextMenu = value;
                    if (propertyTable.ContextMenu != null)
                    {
                        propertyTable.ContextMenu.ThemeName = this.ThemeName;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the default context menu is enabled.
        /// </summary>
        /// <value>The default value is false.</value>
        [DefaultValue(true)]
        [Description("Gets or sets a value indicating whether the default context menu is enabled.")]
        public bool AllowDefaultContextMenu
        {
            get { return this.RadContextMenu is PropertyGridDefaultContextMenu; }
            set
            {
                if (value && this.RadContextMenu is PropertyGridDefaultContextMenu)
                {
                    return;
                }

                if (!value && this.RadContextMenu is PropertyGridDefaultContextMenu)
                {
                    this.RadContextMenu = null;
                    return;
                }

                this.RadContextMenu = new PropertyGridDefaultContextMenu(this.PropertyGridElement.PropertyTableElement);
                base.OnNotifyPropertyChanged("AllowDefaultContextMenu");
            }
        }

        #endregion

        #region Data

        /// <summary>
        /// Gets or sets the <see cref="PropertyGridTableElement"/> selected item.
        /// </summary>
        [Description("Gets or sets the PropertyGridViewElement selected item.")]
        [Category(RadDesignCategory.DataCategory)]
        [DefaultValue(null)]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public PropertyGridItemBase SelectedGridItem
        {
            get
            {
                return this.PropertyGridElement.PropertyTableElement.SelectedGridItem;
            }
            set
            {
                this.PropertyGridElement.PropertyTableElement.SelectedGridItem = value;
            }
        }

        /// <summary>
        /// Gets or sets the object which properties the <see cref="RadPropertyGrid"/> is displaying.
        /// </summary>
        [Description("Gets or sets the object which properties the RadPropertyGrid is displaying.")]
        [Category(RadDesignCategory.DataCategory)]
        [DefaultValue((string)null)]
        [TypeConverter(typeof(SelectedObjectConverter))]
        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public object SelectedObject
        {
            get
            {
                return this.PropertyGridElement.PropertyTableElement.SelectedObject;
            }
            set
            {
                if (value == "")
                {
                    value = null;
                }
                this.PropertyGridElement.PropertyTableElement.SelectedObject = value;
            }
        }

        /// <summary>
        /// Gets the Items collection.
        /// </summary>
        [Description("Gets the Items collection.")]
        [Category(RadDesignCategory.DataCategory)]
        [Browsable(false)]
        public PropertyGridItemCollection Items
        {
            get 
            {
                return new PropertyGridItemCollection(this.PropertyGridElement.PropertyTableElement.PropertyItems);
            }
        }

        /// <summary>
        /// Gets the Groups collection.
        /// </summary>
        [Description("Gets the Groups collection.")]
        [Category(RadDesignCategory.DataCategory)]
        [Browsable(false)]
        public PropertyGridGroupItemCollection Groups
        {
            get
            {
                return this.PropertyGridElement.PropertyTableElement.Groups;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether grouping is enabled.
        /// </summary>
        [Description("Gets or sets a value indicating whether grouping is enabled.")]
        [Category(RadDesignCategory.BehaviorCategory)]
        [DefaultValue(true)]
        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool EnableGrouping
        {
            get
            {
                return this.PropertyGridElement.EnableGrouping;
            }
            set
            {
                this.PropertyGridElement.EnableGrouping = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether sorting is enabled.
        /// </summary>
        [Description("Gets or sets a value indicating whether sorting is enabled.")]
        [Category(RadDesignCategory.BehaviorCategory)]
        [DefaultValue(true)]
        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool EnableSorting
        {
            get
            {
                return this.PropertyGridElement.EnableSorting;
            }
            set
            {
                this.PropertyGridElement.EnableSorting = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether filtering is enabled.
        /// </summary>
        [Description("Gets or sets a value indicating whether filtering is enabled.")]
        [Category(RadDesignCategory.BehaviorCategory)]
        [DefaultValue(true)]
        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool EnableFiltering
        {
            get
            {
                return this.PropertyGridElement.EnableFiltering;
            }
            set
            {
                this.PropertyGridElement.EnableFiltering = value;
            }
        }

        /// <summary>
        /// Gets the group descriptors.
        /// </summary>
        [Description("Gets the group descriptors.")]
        [Category(RadDesignCategory.DataCategory)]
        [DefaultValue(null)]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public GroupDescriptorCollection GroupDescriptors
        {
            get
            {
                return this.PropertyGridElement.PropertyTableElement.GroupDescriptors;
            }
        }

        /// <summary>
        /// Gets the filter descriptors.
        /// </summary>
        [Description("Gets the filter descriptors.")]
        [Category(RadDesignCategory.DataCategory)]
        [DefaultValue(null)]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public FilterDescriptorCollection FilterDescriptors
        {
            get
            {
                return this.PropertyGridElement.PropertyTableElement.FilterDescriptors;
            }
        }

        /// <summary>
        /// Gets the sort descriptors.
        /// </summary>
        [Description("Gets the sort descriptors.")]
        [Category(RadDesignCategory.DataCategory)]
        [DefaultValue(null)]
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SortDescriptorCollection SortDescriptors
        {
            get
            {
                return this.PropertyGridElement.PropertyTableElement.SortDescriptors;
            }
        }

        #endregion

        #region Appearance

        /// <summary>
        /// Gets or sets the sort order of items.
        /// </summary>
        [Description("Gets or sets the sort order of items.")]
        [Category(RadDesignCategory.AppearanceCategory)]
        [DefaultValue(SortOrder.None)]
        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public SortOrder SortOrder
        {
            get
            {
                return this.PropertyGridElement.PropertyTableElement.SortOrder;
            }
            set
            {
                this.PropertyGridElement.PropertyTableElement.SortOrder = value;
            }
        }

        /// <summary>
        /// Gets or sets the mode in which the properties will be displayed in the <see cref="RadPropertyGrid"/>.
        /// </summary>
        [Description("Gets or sets the mode in which the properties will be displayed in the RadPropertyGrid."),
        Category(RadDesignCategory.AppearanceCategory),
        Browsable(true), DefaultValue(PropertySort.NoSort)]
        public PropertySort PropertySort
        {
            get
            {
                return this.propertyGridElement.PropertyTableElement.PropertySort;
            }
            set
            {
                this.propertyGridElement.PropertyTableElement.PropertySort = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="PropertyGridHelpElement"/> is visible.
        /// </summary>
        [Description("Gets or sets a value indicating whether the PropertyGridHelpElement is visible."),
        Category(RadDesignCategory.AppearanceCategory),
        Browsable(true), DefaultValue(true)]
        public bool HelpVisible
        {
            get
            {
                return this.propertyGridElement.SplitElement.HelpVisible;
            }
            set
            {
                this.propertyGridElement.SplitElement.HelpVisible = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the search box of the <see cref="RadPropertyGrid"/> should be visible
        /// </summary>
        [Description("Gets or sets a value indicating whether the search box of the RadPropertyGrid should be visible"),
        Category(RadDesignCategory.AppearanceCategory),
        Browsable(true), DefaultValue(false)]
        public bool ToolbarVisible
        {
            get
            {
                return this.propertyGridElement.ToolbarVisible;
            }
            set
            {
                this.propertyGridElement.ToolbarVisible = value;
            }
        }

        /// <summary>
        /// Gets the <see cref="PropertyGridTableElement"/> of this control.
        /// </summary>
        [Description("Gets the RadPropertyGridElement of this control.")]
        [Category(RadDesignCategory.AppearanceCategory)]
        [Browsable(false)]
        public PropertyGridElement PropertyGridElement
        {
            get { return this.propertyGridElement; }
        }

        /// <summary>
        /// Gets or sets the height of the items.
        /// </summary>
        /// <value>The height of the item.</value>
        [Browsable(true), DefaultValue(24)]
        [Category(RadDesignCategory.AppearanceCategory)]
        [Description("Gets or sets a value indicating the height of the RadPropertyGrid items.")]
        public int ItemHeight
        {
            get
            {
                return this.PropertyGridElement.PropertyTableElement.ItemHeight;
            }
            set
            {
                this.PropertyGridElement.PropertyTableElement.ItemHeight = value;
            }
        }

        /// <summary>
        /// Gets or sets the distance between items of the RadPropertyGridElement.
        /// </summary>
        [Description("Gets or sets the distance between items of the RadPropertyGridElement.")]
        [Category(RadDesignCategory.AppearanceCategory)]
        [DefaultValue(-1)]
        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int ItemSpacing
        {
            get
            {
                return this.PropertyGridElement.PropertyTableElement.ItemSpacing;
            }
            set
            {
                this.PropertyGridElement.PropertyTableElement.ItemSpacing = value;
            }
        }

        /// <summary>
        /// Gets or sets the width of the indentation of subitems.
        /// </summary>
        [Description("Gets or sets the width of the indentation of subitems.")]
        [Category(RadDesignCategory.AppearanceCategory)]
        [DefaultValue(20)]
        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int ItemIndent
        {
            get
            {
                return this.PropertyGridElement.PropertyTableElement.ItemIndent;
            }
            set
            {
                this.PropertyGridElement.PropertyTableElement.ItemIndent = value;
            }
        }

        protected override Size DefaultSize
        {
            get
            {
                return new Size(200, 300);
            }
        }

        #endregion

        #endregion

        #region Events

        /// <summary>
        /// Occurs before the selected object is changed.
        /// </summary>
        [Category(RadDesignCategory.BehaviorCategory)]
        [Description("Occurs before the selected object is changed.")]
        public event PropertyGridSelectedObjectChangingEventHandler SelectedObjectChanging
        {
            add { this.PropertyGridElement.PropertyTableElement.SelectedObjectChanging += value; }
            remove { this.PropertyGridElement.PropertyTableElement.SelectedObjectChanging -= value; }
        }

        /// <summary>
        /// Occurs after the selected object is changed.
        /// </summary>
        [Category(RadDesignCategory.BehaviorCategory)]
        [Description("Occurs after the selected object is changed.")]
        public event PropertyGridSelectedObjectChangedEventHandler SelectedObjectChanged
        {
            add { this.PropertyGridElement.PropertyTableElement.SelectedObjectChanged += value; }
            remove { this.PropertyGridElement.PropertyTableElement.SelectedObjectChanged -= value; }
        }

        /// <summary>
        /// Occurs before a property grid item is selected.
        /// </summary>
        [Category(RadDesignCategory.BehaviorCategory)]
        [Description("Occurs before a property grid item is selected.")]
        public event RadPropertyGridCancelEventHandler SelectedGridItemChanging
        {
            add { this.PropertyGridElement.PropertyTableElement.SelectedGridItemChanging += value; }
            remove { this.PropertyGridElement.PropertyTableElement.SelectedGridItemChanging -= value; }
        }

        /// <summary>
        /// Occurs after the property item is selected.
        /// <remarks>
        /// For more information about handling events, see also SelectedItemChanging.
        /// </remarks>
        /// </summary>
        [Category(RadDesignCategory.BehaviorCategory)]
        [Description("Occurs after a node is selected.")]
        public event RadPropertyGridEventHandler SelectedGridItemChanged
        {
            add { this.PropertyGridElement.PropertyTableElement.SelectedGridItemChanged += value; }
            remove { this.PropertyGridElement.PropertyTableElement.SelectedGridItemChanged -= value; }
        }

        /// <summary>
        /// Occurs when opening the context menu.
        /// </summary>
        [Category(RadDesignCategory.ActionCategory)]
        [Description("Occurs when opening the context menu.")]
        public event PropertyGridContextMenuOpeningEventHandler ContextMenuOpening
        {
            add { this.PropertyGridElement.PropertyTableElement.ContextMenuOpening += value; }
            remove { this.PropertyGridElement.PropertyTableElement.ContextMenuOpening -= value; }
        }

        #region Item events

        /// <summary>
        /// Occurs when the user presses a mouse button over a property grid item.
        /// </summary>
        [Category(RadDesignCategory.BehaviorCategory)]
        [Description("Occurs when the user presses a mouse button over a property grid item.")]
        public event PropertyGridMouseEventHandler ItemMouseDown
        {
            add { this.PropertyGridElement.PropertyTableElement.ItemMouseDown += value; }
            remove { this.PropertyGridElement.PropertyTableElement.ItemMouseDown -= value; }
        }

        /// <summary>
        /// Occurs when the user moves the mouse in the area of a property grid item.
        /// </summary>
        [Category(RadDesignCategory.BehaviorCategory)]
        [Description("Occurs when the user moves the mouse in the area of a property grid item.")]
        public event PropertyGridMouseEventHandler ItemMouseMove
        {
            add { this.PropertyGridElement.PropertyTableElement.ItemMouseMove += value; }
            remove { this.PropertyGridElement.PropertyTableElement.ItemMouseMove -= value; }
        }

        /// <summary>
        ///  Occurs when a mouse button is clicked inside a <see cref="PropertyGridItemElementBase"/>
        /// </summary>
        [Category(RadDesignCategory.BehaviorCategory)]
        [Description("Occurs when a mouse button is clicked inside a PropertyGridItemElementBase")]
        public event RadPropertyGridEventHandler ItemMouseClick
        {
            add { this.PropertyGridElement.PropertyTableElement.ItemMouseClick += value; }
            remove { this.PropertyGridElement.PropertyTableElement.ItemMouseClick -= value; }
        }

        /// <summary>
        /// Occurs when a mouse button is double clicked inside a <see cref="PropertyGridItemElementBase"/>
        /// </summary>
        [Category(RadDesignCategory.BehaviorCategory)]
        [Description("Occurs when a mouse button is double clicked inside a PropertyGridItemElementBase")]
        public event RadPropertyGridEventHandler ItemMouseDoubleClick
        {
            add { this.PropertyGridElement.PropertyTableElement.ItemMouseDoubleClick += value; }
            remove { this.PropertyGridElement.PropertyTableElement.ItemMouseDoubleClick -= value; }
        }

        /// <summary>
        /// Occurs before the value of the Expanded property of a property grid item is changed.
        /// </summary>
        [Category(RadDesignCategory.ActionCategory)]
        [Description("Occurs before the value of the Expanded property of a property grid item is changed.")]
        public event RadPropertyGridCancelEventHandler ItemExpandedChanging
        {
            add { this.PropertyGridElement.PropertyTableElement.ItemExpandedChanging += value; }
            remove { this.PropertyGridElement.PropertyTableElement.ItemExpandedChanging -= value; }
        }

        /// <summary>
        /// Occurs after the value of the Expanded property of a property grid item is changed.
        /// </summary>
        [Category(RadDesignCategory.ActionCategory)]
        [Description("Occurs after the value of the Expanded property of a tproperty grid item is changed.")]
        public event RadPropertyGridEventHandler ItemExpandedChanged
        {
            add { this.PropertyGridElement.PropertyTableElement.ItemExpandedChanged += value; }
            remove { this.PropertyGridElement.PropertyTableElement.ItemExpandedChanged -= value; }
        }

        /// <summary>
        /// Occurs when the item changes its state and needs to be formatted.
        /// </summary>
        [Category(RadDesignCategory.ActionCategory)]
        [Description("Occurs when the item changes its state and needs to be formatted.")]
        public event PropertyGridItemFormattingEventHandler ItemFormatting
        {
            add { this.PropertyGridElement.PropertyTableElement.ItemFormatting += value; }
            remove { this.PropertyGridElement.PropertyTableElement.ItemFormatting -= value; }
        }

        /// <summary>
        /// Occurs when a new node element is going to be created.
        /// </summary>
        [Category(RadDesignCategory.ActionCategory)]
        [Description("Occurs when a new item is going to be created.")]
        public event CreatePropertyGridItemEventHandler CreateItem
        {
            add { this.propertyGridElement.PropertyTableElement.CreateItem += value; }
            remove { this.propertyGridElement.PropertyTableElement.CreateItem -= value; }
        }

        /// <summary>
        /// Occurs when a new node element is going to be created.
        /// </summary>
        [Category(RadDesignCategory.ActionCategory)]
        [Description("Occurs when a new item element is going to be created.")]
        public event CreatePropertyGridItemElementEventHandler CreateItemElement
        {
            add { this.propertyGridElement.PropertyTableElement.CreateItemElement += value; }
            remove { this.propertyGridElement.PropertyTableElement.CreateItemElement -= value; }
        }
        
        #endregion

        #region Editing

        /// <summary>
        /// Occurs when editor is required.
        /// </summary>
        [Category(RadDesignCategory.ActionCategory)]
        [Description("Occurs when editor is required.")]
        public event PropertyGridEditorRequiredEventHandler EditorRequired
        {
            add { this.propertyGridElement.PropertyTableElement.EditorRequired += value; }
            remove { this.propertyGridElement.PropertyTableElement.EditorRequired -= value; }
        }

        /// <summary>
        /// Occurs when editing is started.
        /// </summary>
        [Category(RadDesignCategory.ActionCategory)]
        [Description("Occurs when editing is started.")]
        public event PropertyGridItemEditingEventHandler Editing
        {
            add { this.propertyGridElement.PropertyTableElement.Editing += value; }
            remove { this.propertyGridElement.PropertyTableElement.Editing -= value; }
        }

        /// <summary>
        /// Occurs when editor is initialized.
        /// </summary>
        [Category(RadDesignCategory.ActionCategory)]
        [Description("Occurs when editor is initialized.")]
        public event PropertyGridItemEditorInitializedEventHandler EditorInitialized
        {
            add { this.propertyGridElement.PropertyTableElement.EditorInitialized += value; }
            remove { this.propertyGridElement.PropertyTableElement.EditorInitialized -= value; }
        }

        /// <summary>
        /// Occurs when editing has been finished.
        /// </summary>
        [Category(RadDesignCategory.ActionCategory)]
        [Description("Occurs when editing has been finished.")]
        public event PropertyGridItemEditedEventHandler Edited
        {
            add { this.propertyGridElement.PropertyTableElement.Edited += value; }
            remove { this.propertyGridElement.PropertyTableElement.Edited -= value; }
        }

        /// <summary>
        /// Occurs when item's value is changing.
        /// </summary>
        [Category(RadDesignCategory.ActionCategory)]
        [Description("Occurs when item's value is changing.")]
        public event PropertyGridItemValueChangingEventHandler PropertyValueChanging
        {
            add { this.propertyGridElement.PropertyTableElement.PropertyValueChanging += value; }
            remove { this.propertyGridElement.PropertyTableElement.PropertyValueChanging -= value; }
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        [Category(RadDesignCategory.ActionCategory)]
        [Description("Occurs when a property value changes.")]
        public event PropertyGridItemValueChangedEventHandler PropertyValueChanged
        {
            add { this.propertyGridElement.PropertyTableElement.PropertyValueChanged += value;  }
            remove { this.propertyGridElement.PropertyTableElement.PropertyValueChanged -= value; }
        }

        /// <summary>
        /// Fires when a property value is validating.
        /// </summary>
        [Category(RadDesignCategory.ActionCategory)]
        [Description("Fires when a property value is validating.")]
        public event PropertyValidatingEventHandler PropertyValidating
        {
            add { this.propertyGridElement.PropertyTableElement.PropertyValidating += value; }
            remove { this.propertyGridElement.PropertyTableElement.PropertyValidating -= value; }
        }

        /// <summary>
        /// Fires when a peoperty has finished validating.
        /// </summary>
        [Category(RadDesignCategory.ActionCategory)]
        [Description("Fires when a peoperty has finished validating.")]
        public event PropertyValidatedEventHandler PropertyValidated
        {
            add { this.propertyGridElement.PropertyTableElement.PropertyValidated += value; }
            remove { this.propertyGridElement.PropertyTableElement.PropertyValidated -= value; }
        }
      
        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Expands all the categories in the <see cref="Telerik.WinControls.UI.RadPropertyGrid"/>.
        /// </summary>
        public void ExpandAllGridItems()
        {
            this.propertyGridElement.ExpandAllGridItems();
        }

        /// <summary>
        /// Collapses all the categories in the <see cref="Telerik.WinControls.UI.RadPropertyGrid"/>.
        /// </summary>
        public void CollapseAllGridItems()
        {
            this.propertyGridElement.CollapseAllGridItems();
        }

        /// <summary>
        /// Resets the selected property to its default value.
        /// </summary>
        public void ResetSelectedProperty()
        {
            this.propertyGridElement.ResetSelectedProperty();
        }

        /// <summary>
        /// Puts the current item in edit mode.
        /// </summary>
        /// <returns>true if successfull.</returns>
        public void BeginEdit()
        {
            this.propertyGridElement.PropertyTableElement.BeginEdit();
        }

        /// <summary>
        ///  Commits any changes and ends the edit operation on the current item.
        /// </summary>
        /// <returns>true if successfull.</returns>
        public bool EndEdit()
        {
            return this.propertyGridElement.PropertyTableElement.EndEdit();
        }

        /// <summary>
        /// Close the currently active editor and discard changes.
        /// </summary>
        public void CancelEdit()
        {
            this.propertyGridElement.PropertyTableElement.CancelEdit();
        }

        #endregion

        #region Event handlers

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

            if (!this.PropertyGridElement.PropertyTableElement.ProcessContextMenu(point))
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

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            if (this.PropertyGridElement.PropertyTableElement.ProcessMouseWheel(e))
            {
                return;
            }
            base.OnMouseWheel(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (this.PropertyGridElement.PropertyTableElement.ProcessMouseDown(e))
            {
                return;
            }
            base.OnMouseDown(e);
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            if (this.PropertyGridElement.PropertyTableElement.ProcessMouseClick(e))
            {
                return;
            }
            base.OnMouseClick(e);
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            if (this.PropertyGridElement.PropertyTableElement.ProcessMouseDoubleClick(e))
            {
                return;
            }
            base.OnMouseDoubleClick(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (this.PropertyGridElement.PropertyTableElement.ProcessKeyDown(e))
            {
                return;
            }
            base.OnKeyDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (this.PropertyGridElement.PropertyTableElement.ProcessMouseMove(e))
            {
                return;
            }
            base.OnMouseMove(e);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            if (this.PropertyGridElement.PropertyTableElement.ProecessMouseEnter(e))
            {
                return;
            }
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            if (this.PropertyGridElement.PropertyTableElement.ProecessMouseLeave(e))
            {
                return;
            }
            base.OnMouseLeave(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (this.PropertyGridElement.PropertyTableElement.ProcessMouseUp(e))
            {
                return;
            }
            base.OnMouseUp(e);
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
                case Keys.Enter:
                    return true;
            }

            return base.IsInputKey(keyData);
        }

        #endregion
    }
}

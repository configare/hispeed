using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;
using Telerik.WinControls.Collections;
using System.Collections;
using Telerik.WinControls.Themes.Design;
using Telerik.WinControls.UI.Menu;
using Telerik.WinControls.Layouts;
using Telerik.WinControls.Design;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Collections.Specialized;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// 	<para>Represents a menu. RadMenu can be horizontal or vertical. You can add,
    ///     remove, and disable menu items at run-time. It offers full support for the
    ///     Telerik RadControls for WinForms theming
    ///     engine, allowing you to easily construct a variety of stunning visual effects. You
    ///     can nest any other RadControl within a RadMenu. For
    ///     example, you can create a menu with an embedded textbox or combobox.</para>
    /// 	<para>RadMenu is a simple wrapper for the RadMenuElement class.</para>
    /// </summary>
    [TelerikToolboxCategory(ToolboxGroupStrings.MenuAndToolbarsGroup)]
    [Description("Builds attractive navigation systems")]
    [DefaultBindingProperty("Items"), DefaultProperty("Items")]
    [ToolboxItem(true)]
    [Designer(DesignerConsts.RadMenuDesignerString)]
    public class RadMenu : RadItemsControl, IMessageListener, ITooltipOwner
    {

        #region Nested

        /// <summary>
        /// This enumerator describes the states <see cref="RadMenu"/>can
        /// jump into when processing mnemonics.
        /// </summary>
        public enum RadMenuState
        {
            /// <summary>
            /// When the menu is in this state, that means that Mnemonics are visible.
            /// </summary>
            CuesVisible,
            /// <summary>
            /// When the menu is in this state, that means it listens for keyboard input and can process mnemonics.
            /// </summary>
            CuesVisibleKeyboardActive,
            /// <summary>
            /// When the menu is in this state, that means it can process keyboard input not associated with mnemonics.
            /// This can be navigation input for instance.
            /// </summary>
            KeyboardActive,
            /// <summary>
            /// When the menu is in this state, that means it will not process mnemonics.
            /// </summary>
            NotActive
        }

        #endregion

        #region Events
        /// <commentsfrom cref="RadMenuElement.OrientationChanged" filter=""/>
        [Category(RadDesignCategory.PropertyChangedCategory)]
        [RadDescription("OrientationChanged", typeof(RadMenuElement))]
        public event EventHandler OrientationChanged;

        /// <commentsfrom cref="RadMenuElement.AllItemsEqualHeightChanged" filter=""/>
        [Category(RadDesignCategory.PropertyChangedCategory)]
        [RadDescription("AllItemsEqualHeightChanged", typeof(RadMenuElement))]
        public event EventHandler AllItemsEqualHeightChanged;

        /// <commentsfrom cref="RadItem.TextOrientationChanged" filter=""/>
        [Category(RadDesignCategory.PropertyChangedCategory)]
        public event EventHandler TextOrientationChanged;

        #endregion

        #region Fields

        private RadMenuState menuState = RadMenuState.NotActive;

        private RadMenuElement menuElement;
        internal bool menuMergeApplied = false;
        private List<RadMenuItemBase> itemsToBeRestoredAfterUnmerge = new List<RadMenuItemBase>();
        internal List<RadMenuItemBase> persistedItemsUponMenuMerge = new List<RadMenuItemBase>();
        //the source menu upon menu merge:
        private RadMenu sourceMenuUponMerge = null;
        //the form in which this menu is placed:
        private Form parentForm = null;
        //private static int VK_F10 = 0x79;
        private bool showKeyboardCues = false;
        private ApplicationMdiState applicationMdiState = new ApplicationMdiState();
        private bool systemKeyHighlight = true;

        private bool activated = false;
        private bool highLightCycleCompleted = false;

        #endregion

        #region Ctor

        /// <summary>
        /// Initializes a new instance of the RadMenu class. RadMenu can be horizontal or
        /// vertical. You can add, remove, and disable menu items at run-time. It offers full
        /// support for the Telerik RadControls for WinForms
        /// theming engine, allowing you to easily construct a variety of stunning visual effects.
        /// You can nest any other RadControl within a RadMenu. For
        /// example, you can create a menu with an embedded textbox or combobox.
        /// </summary>
        public RadMenu()
        {
            this.AutoSize = true;
            this.SetStyle(ControlStyles.Selectable, false);

            this.Items.ItemsChanged += new ItemChangedDelegate(OnRadMenu_ItemsChanged);
            this.Initialized += new EventHandler(RadMenu_Initialized);
        }


        #endregion

        #region Initialization

        protected override void InitializeRootElement(RootRadElement rootElement)
        {
            base.InitializeRootElement(rootElement);
            rootElement.SetDefaultValueOverride(RadElement.StretchHorizontallyProperty, true);
            rootElement.SetDefaultValueOverride(RadElement.StretchVerticallyProperty, false);
            //rootElement.StretchHorizontally = true;
            //rootElement.StretchVertically = false;
            this.Dock = DockStyle.Top;
        }

        protected override void CreateChildItems(RadElement parent)
        {
            base.CreateChildItems(parent);
            this.menuElement = new RadMenuElement();
            this.menuElement.OrientationChanged += delegate(object sender, EventArgs args) { OnOrientationChanged(args); };
            this.menuElement.AllItemsEqualHeightChanged += delegate(object sender, EventArgs args) { OnAllItemsEqualHeightChanged(args); };
            this.menuElement.TextOrientationChanged += delegate(object sender, EventArgs args) { OnTextOrientationChanged(args); };
            parent.Children.Add(this.menuElement);
        }

        #endregion

        #region Helper methods

        public void SetMenuState(RadMenuState state)
        {
            RadMenuState oldState = this.menuState;
            this.menuState = state;
            this.OnMenuStateChanged(oldState, state);
        }

        private void ToggleKeyboardCues(RadMenuItem currentItem, bool value)
        {
            foreach (RadMenuItemBase item in currentItem.Items)
            {
                RadMenuItem castedItem = item as RadMenuItem;

                if (castedItem != null)
                {
                    this.ToggleKeyboardCues(castedItem, value);
                }
            }

            if (currentItem != null)
            {
                currentItem.ShowKeyboardCue = value;
            }
        }

        private bool CheckMouseOverChildDropDown(IPopupControl popup)
        {
            if (popup.Bounds.Contains(MousePosition))
            {
                return true;
            }

            foreach (IPopupControl popupControl in popup.Children)
            {
                return this.CheckMouseOverChildDropDown(popupControl);
            }

            return false;
        }

        private void PerformItemClick(RadMenuItemBase item)
        {
            item.ShowChildItems();
            item.DropDown.SelectFirstVisibleItem();

            item.PerformClick();
        }

        #endregion

        #region MenuMerge

        protected bool isMainMenu = true;

        /// <summary>
        /// Gets or sets boolean value that determines whether
        /// RadMenu handles the MDI menu functionality.
        /// </summary>
        [Browsable(true)]
        [Category(RadDesignCategory.BehaviorCategory)]
        [Description("Defines whether RadMenu handles the MDI menu functionality.")]
        [DefaultValue(true)]
        public bool IsMainMenu
        {
            get
            {
                return this.isMainMenu;
            }
            set
            {
                this.isMainMenu = value;
            }
        }

        private void PrepareMdiMenuStrip(Form parentForm)
        {
            if (parentForm.MainMenuStrip == null
                && this.isMainMenu)
            {
                parentForm.MainMenuStrip = new RadRibbonFormMainMenuStrip();
            }
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            this.RollOverItemSelection = true;
            Form parentForm = this.FindForm();

            if (parentForm != null)
            {
                this.parentForm = parentForm;

                if (parentForm.IsMdiContainer)
                {
                    this.PrepareMdiMenuStrip(parentForm);

                    foreach (Control control in parentForm.Controls)
                    {
                        if (control is MdiClient)
                        {
                            MdiClient client = control as MdiClient;

                            foreach (Control controlAlreadyInTheCollection in client.Controls)
                            {
                                ControlEventArgs args = new ControlEventArgs(controlAlreadyInTheCollection);
                                MdiClient_ControlAdded(client, args);
                            }
                            client.ControlAdded += new ControlEventHandler(this.MdiClient_ControlAdded);
                            client.ControlRemoved += new ControlEventHandler(client_ControlRemoved);
                            break;
                        }
                    }

                    this.menuElement.MinimizeButton.Click += new EventHandler(MinimizeButton_Click);
                    this.menuElement.MaximizeButton.Click += new EventHandler(MaximizeButton_Click);
                    this.menuElement.CloseButton.Click += new EventHandler(CloseButton_Click);

                    applicationMdiState.MdiParentForm = parentForm;
                }

                if (parentForm.IsMdiChild)
                {
                    if (parentForm.WindowState == FormWindowState.Maximized)
                    {
                        applicationMdiState.MaximizedChildForm = parentForm;
                    }
                }
            }

            base.OnHandleCreated(e);
        }




        private void client_ControlRemoved(object sender, ControlEventArgs e)
        {
            e.Control.SizeChanged -= new EventHandler(MdiChild_SizeChanged);
            e.Control.TextChanged -= new EventHandler(MdiChildFormToActivate_TextChanged);

            Form castedForm = e.Control as Form;
            if (castedForm != null)
            {
                castedForm.Shown -= new EventHandler(castedForm_Shown);
            }

            foreach (RadMenuItemBase item in this.Items)
            {
                if (item.MdiList)
                {
                    item.Click -= new EventHandler(item_Click);
                    for (int i = item.Items.Count - 1; i >= 0; i--)
                    {
                        if (item.Items[i] is RadMenuItem && ((RadMenuItem)item.Items[i]).MdiChildFormToActivate == e.Control)
                        {
                            item.Items.RemoveAt(i);
                            break;
                        }
                    }
                }
            }

            if (parentForm.MdiChildren.Length == 0)
            {
                if (this.menuMergeApplied)
                {
                    this.UnmergeMenu(applicationMdiState.MaximizedChildFormMenu);
                    this.menuMergeApplied = false;

                }

                applicationMdiState.MaximizedChildForm = null;
                applicationMdiState.MaximizedChildFormMenu = null;
                this.menuElement.SystemButtons.Visibility = ElementVisibility.Collapsed;
            }
        }

        private void MdiClient_ControlAdded(object sender, ControlEventArgs e)
        {
            e.Control.SizeChanged += new EventHandler(MdiChild_SizeChanged);

            Form castedForm = e.Control as Form;
            if (castedForm != null)
            {
                castedForm.Shown += new EventHandler(castedForm_Shown);
            }

            foreach (RadMenuItemBase item in this.Items)
            {
                if (item.MdiList)
                {
                    RadMenuItem formItem = new RadMenuItem(e.Control.Text);
                    item.BitState[RadMenuItemBase.IsMdiListItemStateKey] = true;
                    formItem.MdiChildFormToActivate = (Form)e.Control;
                    formItem.Click += new EventHandler(formItem_Click);
                    item.Items.Add(formItem);
                    item.Click += new EventHandler(item_Click);
                    formItem.MdiChildFormToActivate.TextChanged += new EventHandler(MdiChildFormToActivate_TextChanged);
                }
            }
        }

        private void castedForm_Shown(object sender, EventArgs e)
        {
            if (((Form)sender).WindowState == FormWindowState.Maximized)
            {
                MdiChild_SizeChanged(sender, new EventArgs());
            }
        }

        private void MdiChildFormToActivate_TextChanged(object sender, EventArgs e)
        {
            Form form = sender as Form;
            foreach (RadMenuItemBase item in this.Items)
            {
                if (item.MdiList)
                {
                    foreach (RadMenuItemBase subItem in item.Items)
                    {
                        RadMenuItem formItem = subItem as RadMenuItem;
                        if (formItem != null &&
                            formItem.MdiChildFormToActivate != null && formItem.MdiChildFormToActivate == form)
                        {
                            formItem.Text = form.Text;
                        }
                    }
                }
            }
        }

        private void item_Click(object sender, EventArgs e)
        {
            if (!(sender is RadMenuItem))
            {
                return;
            }

            foreach (RadMenuItemBase itemBase in ((RadMenuItem)sender).Items)
            {
                RadMenuItem item = itemBase as RadMenuItem;
                if (item == null)
                    continue;

                if (item.MdiChildFormToActivate != null)
                {
                    if (item.MdiChildFormToActivate == applicationMdiState.MdiParentForm.ActiveMdiChild)
                    {
                        item.IsChecked = true;
                    }
                    else
                    {
                        item.IsChecked = false;
                    }
                }
            }
        }

        //"this reference" is the menu in the Parent Form for the following event handler:
        private void MdiChild_SizeChanged(object sender, EventArgs e)
        {
            Form form = sender as Form;

            if (form != null && form.WindowState == FormWindowState.Maximized && GetFirstRadMenuInForm(form) == null)
            {
                if (this.menuMergeApplied)
                {
                    this.UnmergeMenu(applicationMdiState.MaximizedChildFormMenu);
                    this.menuMergeApplied = false;
                }

                applicationMdiState.MaximizedChildForm = form;
                applicationMdiState.MaximizedChildFormMenu = null;

                if (this.isMainMenu)
                {
                    this.menuElement.SystemButtons.Visibility = ElementVisibility.Visible;
                }

                return;
            }

            if (form != null && form.WindowState != FormWindowState.Maximized && !this.menuMergeApplied &&
                applicationMdiState.MaximizedChildForm == null)
            {
                this.menuElement.SystemButtons.Visibility = ElementVisibility.Collapsed;
                return;
            }

            if (form != null && form.WindowState != FormWindowState.Maximized && !this.menuMergeApplied &&
                applicationMdiState.MaximizedChildForm != null)
            {
                bool allFormsAreNotMaximized = true;
                foreach (Form mdiChild in applicationMdiState.MdiParentForm.MdiChildren)
                {
                    if (mdiChild.WindowState == FormWindowState.Maximized)
                    {
                        allFormsAreNotMaximized = false;
                        break;
                    }
                }

                if (allFormsAreNotMaximized)
                {
                    this.menuElement.SystemButtons.Visibility = ElementVisibility.Collapsed;
                    return;
                }
            }

            if (form != null && form.WindowState != FormWindowState.Maximized && this.menuMergeApplied && form == applicationMdiState.MaximizedChildForm)
            {
                this.UnmergeMenu(applicationMdiState.MaximizedChildFormMenu);
                this.menuMergeApplied = false;
                applicationMdiState.MaximizedChildFormMenu.Visible = true;
                applicationMdiState.MaximizedChildForm = null;
                applicationMdiState.MaximizedChildFormMenu = null;
                this.MenuElement.SystemButtons.Visibility = ElementVisibility.Collapsed;
                return;
            }

            if (form != null && form.WindowState == FormWindowState.Maximized && applicationMdiState.MaximizedChildForm == null)
            {
                applicationMdiState.MaximizedChildForm = form;

                if (this.menuElement.SystemButtons.Visibility != ElementVisibility.Visible && this.isMainMenu)
                {
                    this.menuElement.SystemButtons.Visibility = ElementVisibility.Visible;
                }

                RadMenu srcMenu = GetFirstRadMenuInForm(applicationMdiState.MaximizedChildForm);
                applicationMdiState.MaximizedChildFormMenu = srcMenu;

                if (srcMenu != null && !this.menuMergeApplied)
                {
                    this.sourceMenuUponMerge = srcMenu;
                    this.MergeMenu(srcMenu);
                    srcMenu.Visible = false;
                    this.menuMergeApplied = true;
                }

                return;
            }

            if (form != null && form.WindowState == FormWindowState.Maximized &&
                applicationMdiState.MaximizedChildForm != null && applicationMdiState.MaximizedChildForm != form)
            {
                if (this.menuMergeApplied)
                {
                    this.UnmergeMenu(applicationMdiState.MaximizedChildFormMenu);
                    this.menuMergeApplied = false;
                    applicationMdiState.MaximizedChildFormMenu.Visible = true;
                }

                RadMenu srcMenu = GetFirstRadMenuInForm(form);

                if (srcMenu != null)
                {
                    this.sourceMenuUponMerge = srcMenu;
                    this.MergeMenu(srcMenu);
                    this.menuMergeApplied = true;
                    srcMenu.Visible = false;

                    applicationMdiState.MaximizedChildForm = form;
                    applicationMdiState.MaximizedChildFormMenu = srcMenu;
                }

                if (this.isMainMenu)
                {
                    this.MenuElement.SystemButtons.Visibility = ElementVisibility.Visible;
                }

                return;
            }
        }

        private RadMenu GetFirstRadMenuInForm(Form form)
        {
            RadMenu menu = null;

            foreach (Control control in form.Controls)
            {
                if (control is RadMenu)
                {
                    menu = (RadMenu)control;
                    break;
                }
            }

            return menu;
        }

        private void OnParentForm_Deactivate(object sender, EventArgs e)
        {
            this.SetMenuState(RadMenuState.NotActive);
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            foreach (RadMenuItemBase item in this.Items)
            {
                if (item.MdiList)
                {
                    for (int childItemIndex = 0; childItemIndex < item.Items.Count; childItemIndex++)
                    {
                        RadMenuItem currentMenuItem = item.Items[childItemIndex] as RadMenuItem;

                        if (currentMenuItem != null && currentMenuItem.MdiChildFormToActivate == applicationMdiState.MaximizedChildForm)
                        {
                            item.Items.Remove(currentMenuItem);
                            break;
                        }
                    }
                }
            }

            applicationMdiState.MaximizedChildForm.Close();
        }

        private void MaximizeButton_Click(object sender, EventArgs e)
        {
            applicationMdiState.MaximizedChildForm.WindowState = FormWindowState.Normal;
        }

        private void MinimizeButton_Click(object sender, EventArgs e)
        {
            applicationMdiState.MaximizedChildForm.WindowState = FormWindowState.Minimized;
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {

            foreach (RadMenuItemBase item in this.Items)
            {
                if (item.MdiList)
                {
                    for (int i = item.Items.Count - 1; i >= 0; i--)
                    {
                        if (((RadMenuItemBase)item.Items[i]).GetBitState(RadMenuItemBase.IsMdiListItemStateKey))
                        {
                            ((RadMenuItemBase)item.Items[i]).Click -= new EventHandler(formItem_Click);
                            item.Items.RemoveAt(i);
                        }
                    }
                }
            }

            if (parentForm != null && parentForm.IsMdiContainer)
            {
                this.menuElement.MinimizeButton.Click -= new EventHandler(MinimizeButton_Click);
                this.menuElement.MaximizeButton.Click -= new EventHandler(MaximizeButton_Click);
                this.menuElement.CloseButton.Click -= new EventHandler(CloseButton_Click);

                foreach (Control control in parentForm.Controls)
                {
                    if (control is MdiClient)
                    {
                        MdiClient client = control as MdiClient;
                        client.ControlAdded -= new ControlEventHandler(this.MdiClient_ControlAdded);
                        client.ControlRemoved -= new ControlEventHandler(client_ControlRemoved);
                        break;
                    }
                }
            }

            base.OnHandleDestroyed(e);
        }

        private void formItem_Click(object sender, EventArgs e)
        {

            if (((RadMenuItem)sender).MdiChildFormToActivate != null && ((RadMenuItem)sender).MdiChildFormToActivate.MdiParent != null)
            {
                foreach (Form form in ((RadMenuItem)sender).MdiChildFormToActivate.MdiParent.MdiChildren)
                {
                    if (form.WindowState == FormWindowState.Maximized && form != ((RadMenuItem)sender).MdiChildFormToActivate)
                    {
                        form.WindowState = FormWindowState.Normal;
                    }
                }

                ((RadMenuItem)sender).MdiChildFormToActivate.Activate();
            }
        }

        public virtual void MergeMenu(RadMenu sourceMenu)
        {
            if (this == sourceMenu)
            {
                throw new ArgumentException("Menu self merging");
            }

            menuMergeApplied = true;

            int index = this.Items.Count;
            int[] subMenuIndices = new int[index];
            for (int i = 0; i < index; i++)
            {
                subMenuIndices[i] = ((RadMenuItemBase)this.Items[i]).Items.Count;
            }

            PersistMenuItemsInChildMenuUponMenuMerge(sourceMenu);

            foreach (RadMenuItemBase item in sourceMenu.persistedItemsUponMenuMerge)
            {
                item.BitState[RadMenuItemBase.IsParticipatingInMergeStateKey] = true;

                switch (item.MergeType)
                {
                    case MenuMerge.Add:

                        int position = index;

                        for (int i = index; i < this.Items.Count; i++)
                        {
                            if (((RadMenuItemBase)this.Items[i]).MergeOrder > item.MergeOrder)
                            {
                                break;
                            }

                            position++;
                        }

                        this.Items.Insert(position, item);

                        break;

                    case MenuMerge.Replace:

                        int replacePosition = sourceMenu.persistedItemsUponMenuMerge.IndexOf(item);

                        if (replacePosition < this.Items.Count)
                        {
                            if (((RadMenuItemBase)this.Items[replacePosition]).MergeType != MenuMerge.Add)
                            {
                                ((RadMenuItemBase)this.Items[replacePosition]).PositionToBeRestoredAfterMerge = replacePosition;
                                this.itemsToBeRestoredAfterUnmerge.Add((RadMenuItemBase)this.Items[replacePosition]);
                                this.Items.RemoveAt(replacePosition);
                                this.Items.Insert(replacePosition, item);
                            }
                            else
                            {
                                this.Items.Add(item);
                            }
                        }
                        else
                        {
                            this.Items.Add(item);
                        }

                        break;
                    case MenuMerge.MergeItems:

                        int mergePosition = sourceMenu.persistedItemsUponMenuMerge.IndexOf(item);

                        if (mergePosition < this.Items.Count)
                        {
                            if (((RadMenuItemBase)this.Items[mergePosition]).MergeType != MenuMerge.Add)
                            {
                                foreach (RadMenuItemBase subItemToBeMerge in item.Items)
                                {
                                    int positionInSubmenu = subMenuIndices[mergePosition];

                                    for (int i = positionInSubmenu; i < ((RadMenuItemBase)this.Items[mergePosition]).Items.Count; i++)
                                    {
                                        if (((RadMenuItemBase)((RadMenuItemBase)this.Items[mergePosition]).Items[i]).MergeOrder > subItemToBeMerge.MergeOrder)
                                        {
                                            break;
                                        }
                                        positionInSubmenu++;
                                    }
                                    subItemToBeMerge.BitState[RadMenuItemBase.IsParticipatingInMergeStateKey] = true;
                                    ((RadMenuItemBase)this.Items[mergePosition]).Items.Insert(positionInSubmenu, subItemToBeMerge);
                                }
                            }
                            else
                            {
                                this.Items.Add(item);
                            }
                        }
                        else
                        {
                            this.Items.Add(item);
                        }

                        break;
                    case MenuMerge.Remove:
                        {
                            item.BitState[RadMenuItemBase.IsParticipatingInMergeStateKey] = false;
                            break;
                        }
                }
            }

            menuMergeApplied = true;

            this.sourceMenuUponMerge = sourceMenu;
        }

        public void UnmergeMenu(RadMenu src)
        {
            this.menuMergeApplied = false;

            for (int i = this.Items.Count - 1; i >= 0; i--)
            {
                RadMenuItemBase menuItemBase = this.Items[i] as RadMenuItemBase;

                for (int j = menuItemBase.Items.Count - 1; j >= 0; j--)
                {
                    RadMenuItemBase childMenuItem = menuItemBase.Items[j] as RadMenuItemBase;
                    if (childMenuItem.GetBitState(RadMenuItemBase.IsParticipatingInMergeStateKey))
                    {
                        childMenuItem.BitState[RadMenuItemBase.IsParticipatingInMergeStateKey] = false;
                        menuItemBase.Items.RemoveAt(j);
                    }
                }

                if (menuItemBase.GetBitState(RadMenuItemBase.IsParticipatingInMergeStateKey))
                {
                    menuItemBase.BitState[RadMenuItemBase.IsParticipatingInMergeStateKey] = false;
                    this.Items.RemoveAt(i);
                }
            }


            foreach (RadMenuItemBase itemToBeRestored in this.itemsToBeRestoredAfterUnmerge)
            {
                itemToBeRestored.BitState[RadMenuItemBase.IsParticipatingInMergeStateKey] = false;
                this.Items.Insert(itemToBeRestored.PositionToBeRestoredAfterMerge, itemToBeRestored);
            }

            itemsToBeRestoredAfterUnmerge.Clear();

            RestoreMenuItemsInChildMenuUponMenuUnmerge(src);

            this.sourceMenuUponMerge = null;
            this.menuMergeApplied = false;
        }

        internal void PersistMenuItemsInChildMenuUponMenuMerge(RadMenu src)
        {
            src.persistedItemsUponMenuMerge.Clear();

            foreach (RadMenuItemBase item in src.Items)
            {
                src.persistedItemsUponMenuMerge.Add(item);
            }

            for (int i = src.Items.Count - 1; i >= 0; i--)
            {
                src.Items.RemoveAt(i);
            }
        }

        internal void RestoreMenuItemsInChildMenuUponMenuUnmerge(RadMenu src)
        {
            for (int i = src.Items.Count - 1; i >= 0; i--)
            {
                src.Items.RemoveAt(i);
            }

            foreach (RadMenuItemBase item in src.persistedItemsUponMenuMerge)
            {
                src.Items.Add(item);
                this.UpdateItemsOwner(item);
            }
        }

        private void UpdateItemsOwner(RadMenuItemBase menuItem)
        {
            RadElement originalOwner = menuItem.Items.Owner;
            menuItem.Items.Owner = null;
            menuItem.Items.Owner = originalOwner;

            foreach (RadMenuItemBase childItem in menuItem.Items)
            {
                this.UpdateItemsOwner(childItem);
            }
        }

        #endregion

        #region IMessageListener Members



        public InstalledHook DesiredHook
        {
            get
            {
                return InstalledHook.GetMessage;
            }
        }

        private bool CanProcessMessage(ref Message msg)
        {
            Control fromHandle = Control.FromHandle(msg.HWnd);

            if (fromHandle == null)
            {
                return false;
            }

            Form parentForm = this.FindForm();

            if (parentForm != null && !object.ReferenceEquals(parentForm, Form.ActiveForm))
            {
                return false;
            }

            return true;
        }

        public MessagePreviewResult PreviewMessage(ref Message msg)
        {
            switch (msg.Msg)
            {
                case NativeMethods.WM_LBUTTONUP:
                case NativeMethods.WM_MBUTTONUP:
                case NativeMethods.WM_RBUTTONUP:
                case NativeMethods.WM_NCLBUTTONUP:
                case NativeMethods.WM_NCMBUTTONUP:
                case NativeMethods.WM_NCRBUTTONUP:
                    {
                        this.PreprocessMouseEvent(ref msg);
                        return MessagePreviewResult.Processed;
                    }
                case NativeMethods.WM_SYSKEYDOWN:
                    {
                        if (!this.CanProcessMessage(ref msg))
                        {
                            return MessagePreviewResult.Processed;
                        }

                        Keys keyData = (Keys)msg.WParam;

                        char parsedChar;
                        Char.TryParse(keyData.ToString(), out parsedChar);

                        if ((this.menuState == RadMenuState.CuesVisible
                            || this.menuState == RadMenuState.CuesVisibleKeyboardActive) && this.CanProcessMnemonic(parsedChar))
                        {
                            this.SetMenuState(RadMenuState.CuesVisibleKeyboardActive);
                            this.ProcessMnemonic(parsedChar);
                            return MessagePreviewResult.ProcessedNoDispatch | MessagePreviewResult.NoContinue;
                        }
                        bool isMenuKey = (keyData == Keys.Menu || keyData == Keys.Alt);
                        if (isMenuKey && this.ProcessFirstStageMnemonicActivation(ref msg, keyData))
                        {
                            return MessagePreviewResult.ProcessedNoDispatch | MessagePreviewResult.NoContinue;
                        }

                        return MessagePreviewResult.Processed;
                    }
                case NativeMethods.WM_SYSKEYUP:
                    {
                        if (!this.CanProcessMessage(ref msg))
                        {
                            return MessagePreviewResult.Processed;
                        }

                        Keys keyData = (Keys)msg.WParam;
                        bool isMenuKey = (keyData == Keys.Menu || keyData == Keys.F10 || keyData == Keys.Alt);
                        if (isMenuKey &&
                            this.ProcessSecondStageMnemonicActivation(ref msg, (Keys)msg.WParam))
                        {
                            return MessagePreviewResult.ProcessedNoDispatch | MessagePreviewResult.NoContinue;
                        }

                        return MessagePreviewResult.Processed;
                    }

                case NativeMethods.WM_CHAR:
                    {
                        if (!this.CanProcessMessage(ref msg))
                        {
                            return MessagePreviewResult.Processed;
                        }

                        uint charCode = (uint)msg.WParam;

                        if (charCode < char.MinValue || charCode > char.MaxValue)
                        {
                            return MessagePreviewResult.Processed;
                        }

                        char parsedChar = (char)charCode;
                        if (this.ProcessMnemonic(parsedChar))
                        {
                            return MessagePreviewResult.ProcessedNoDispatch | MessagePreviewResult.NoContinue;
                        }
                        return MessagePreviewResult.Processed;
                    }
                case NativeMethods.WM_KEYDOWN:
                    {
                        if (!this.CanProcessMessage(ref msg))
                        {
                            return MessagePreviewResult.Processed;
                        }

                        Keys keyData = (Keys)(((int)((long)msg.WParam))) | ModifierKeys;
                        bool canProcessKey = (this.menuState == RadMenuState.KeyboardActive || this.menuState == RadMenuState.CuesVisibleKeyboardActive);
                        if (canProcessKey && this.ProcessCmdKey(ref msg, keyData))
                        {
                            return MessagePreviewResult.ProcessedNoDispatch | MessagePreviewResult.NoContinue;
                        }

                        if (canProcessKey && this.IsInputKey(keyData))
                        {
                            return MessagePreviewResult.ProcessedNoDispatch;
                        }

                        if (canProcessKey && this.ProcessDialogKey(keyData))
                        {
                            return MessagePreviewResult.ProcessedNoDispatch | MessagePreviewResult.NoContinue;
                        }

                        if (canProcessKey && this.OnHandleKeyDown(msg))
                        {
                            return MessagePreviewResult.ProcessedNoDispatch;
                        }

                        return MessagePreviewResult.Processed;
                    }
                default:
                    {
                        return MessagePreviewResult.Processed;
                    }
            }
        }


        public void PreviewWndProc(Message msg)
        {

        }

        public void PreviewSystemMessage(SystemMessage message, Message msg)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Event handling

        protected virtual void OnMenuStateChanged(RadMenuState oldState, RadMenuState newState)
        {
            switch (newState)
            {
                case RadMenuState.NotActive:
                    {
                        this.ShowMenuKeyboardCues = false;
                        this.highLightCycleCompleted = false;
                        this.ProcessKeyboard = false;
                        RadMenuItemBase selectedItem = this.GetSelectedItem() as RadMenuItemBase;
                        if (selectedItem != null)
                        {
                            selectedItem.Deselect();
                        }
                        //Should show children should be set after the Deselect call
                        //since if there is a selected item with opened drop-down
                        //the shouldShowChildren flag will be reset in the OnItemDeselected event.
                        this.shouldShowChildren = false;
                        break;
                    }
                case RadMenuState.CuesVisible:
                    {
                        this.ProcessKeyboard = false;
                        this.ShowMenuKeyboardCues = true;
                        this.highLightCycleCompleted = false;
                        break;
                    }
                case RadMenuState.CuesVisibleKeyboardActive:
                    {
                        this.ProcessKeyboard = true;
                        this.highLightCycleCompleted = true;
                        this.ShowMenuKeyboardCues = true;
                        break;
                    }
                case RadMenuState.KeyboardActive:
                    {
                        this.ProcessKeyboard = true;
                        this.ShowMenuKeyboardCues = false;
                        this.highLightCycleCompleted = false;
                        break;
                    }
            }
        }

        protected override void OnLoad(System.Drawing.Size desiredSize)
        {
            base.OnLoad(desiredSize);


            Form parentForm = this.FindForm();
            if (parentForm != null)
            {
                parentForm.Deactivate += OnParentForm_Deactivate;
            }

            if (!this.IsDesignMode)
            {
                RadMessageFilter.Instance.AddListener(this);
            }
        }

        protected override bool ProcessFocusRequested(RadElement element)
        {
            return false;
        }

        protected override void OnLeave(EventArgs e)
        {
            base.OnLeave(e);

            this.SetMenuState(RadMenuState.NotActive);

            foreach (RadMenuItemBase item in this.Items)
            {
                if (item.HasChildren && item.IsPopupShown)
                {
                    item.HideChildItems();
                }
            }
        }

        protected virtual void PreprocessMouseEvent(ref Message msg)
        {
            Point location = new Point(msg.LParam.ToInt32());

            RadMenuItemBase selectedItem = this.GetCurrentProcessedItem(this) as RadMenuItemBase;

            if (selectedItem == null)
            {
                return;
            }

            if (this.Bounds.Contains(location)
                && this.menuState == RadMenuState.CuesVisible)
            {
                if (selectedItem.IsPopupShown && !selectedItem.ControlBoundingRectangle.Contains(location))
                {
                    this.SetMenuState(RadMenuState.NotActive);
                    return;
                }

                if (!selectedItem.ControlBoundingRectangle.Contains(location))
                {
                    this.SetMenuState(RadMenuState.NotActive);
                    selectedItem.Deselect();
                    return;
                }
            }

            if (!selectedItem.HasChildren || !selectedItem.IsOnDropDown)
            {
                this.SetMenuState(RadMenuState.NotActive);
                return;
            }

        }

        protected override bool ProcessUpDownArrowKey(bool down)
        {
            RadMenuItemBase currentItem = this.GetSelectedItem() as RadMenuItemBase;

            if (currentItem == null)
                return false;

            if (currentItem.IsPopupShown)
                return false;

            if (currentItem.HasChildren && currentItem.Enabled)
            {
                currentItem.ShowChildItems();
                currentItem.DropDown.SelectFirstVisibleItem();
                return true;
            }

            return base.ProcessUpDownArrowKey(down);
        }

        protected virtual bool CanProcessItem(RadMenuItemBase menuItem)
        {
            return menuItem != null && menuItem.Enabled;
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (keyData == Keys.Enter)
            {
                RadMenuItemBase localSelectedItem = this.GetSelectedItem() as RadMenuItemBase;

                if (!this.CanProcessItem(localSelectedItem))
                {
                    return false;
                }

                if (!localSelectedItem.IsPopupShown)
                {
                    this.BeginInvoke(new Telerik.WinControls.UI.RadDropDownMenu.PerformClickInvoker(this.PerformItemClick), localSelectedItem);
                    return true;
                }

                RadMenuItemBase menuItem = this.GetCurrentProcessedItem(this);

                if (!this.CanProcessItem(menuItem))
                {
                    return false;
                }

                if (!menuItem.HasChildren)
                {
                    localSelectedItem.Deselect();
                    this.SetMenuState(RadMenuState.NotActive);
                }

                return false;

            }

            return base.ProcessDialogKey(keyData);
        }

        protected override bool CallBaseProcessDialogKey(Keys keyData)
        {
            return false;
        }

        protected virtual RadMenuItemBase GetCurrentProcessedItem(IItemsControl itemsControl)
        {
            foreach (RadItem item in itemsControl.Items)
            {
                RadMenuItemBase menuItem = item as RadMenuItemBase;

                if (menuItem != null && menuItem.HasChildren && menuItem.IsPopupShown)
                {
                    return this.GetCurrentProcessedItem(menuItem.DropDown);
                }
            }

            return itemsControl.GetSelectedItem() as RadMenuItemBase;
        }

        protected override bool ProcessArrowKey(Keys keyCode)
        {
            bool processResult = false;

            switch (keyCode)
            {
                case Keys.Left:
                case Keys.Right:
                    {
                        if (this.Orientation == Orientation.Horizontal)
                        {
                            processResult = this.ProcessLeftRightArrowKey(keyCode == Keys.Right);
                        }
                        else
                        {
                            RadMenuItemBase currentProcessedItem = this.GetCurrentProcessedItem(this);
                            RadMenuItemBase selectedItem = this.GetSelectedItem() as RadMenuItemBase;

                            if (selectedItem == null || currentProcessedItem == null)
                            {
                                return false;
                            }

                            if (selectedItem.IsPopupShown
                                && (object.ReferenceEquals(currentProcessedItem.HierarchyParent, selectedItem) || !currentProcessedItem.HasChildren))
                            {
                                return this.ProcessLeftRightArrowKey(keyCode == Keys.Right);
                            }
                            else
                            {
                                processResult = this.ProcessUpDownArrowKey(keyCode == Keys.Left);
                            }
                        }

                        return processResult;
                    }
                case Keys.Up:
                case Keys.Down:
                    {
                        if (this.Orientation == Orientation.Horizontal)
                        {
                            processResult = this.ProcessUpDownArrowKey(keyCode == Keys.Down);
                        }
                        else
                        {
                            RadMenuItemBase selectedItem = this.GetSelectedItem() as RadMenuItemBase;

                            if (selectedItem != null && selectedItem.IsPopupShown)
                            {
                                processResult = false;
                            }
                            else
                            {
                                processResult = this.ProcessLeftRightArrowKey(keyCode == Keys.Down);
                            }
                        }

                        return processResult;
                    }
            }
            return processResult;
        }

        protected override bool ProcessLeftRightArrowKey(bool right)
        {
            RadMenuItemBase currentMenuItem = this.GetSelectedItem() as RadMenuItemBase;

            if (currentMenuItem == null)
            {
                return base.ProcessLeftRightArrowKey(right);
            }

            RadMenuItemBase currentSubMenuItem = this.GetCurrentProcessedItem(this);

            if (currentSubMenuItem != null
                && currentSubMenuItem.ElementTree.Control is RadDropDownMenu
                && (currentSubMenuItem.ElementTree.Control as RadDropDownMenu).CanNavigate(right ? Keys.Right : Keys.Left))
            {
                return false;
            }

            bool result = base.ProcessLeftRightArrowKey(right);

            currentMenuItem.HideChildItems();

            currentMenuItem = this.GetSelectedItem() as RadMenuItemBase;

            if (this.CanProcessItem(currentMenuItem)
                && this.shouldShowChildren
                && currentMenuItem.HasChildren)
            {
                currentMenuItem.ShowChildItems();
                currentMenuItem.DropDown.SelectFirstVisibleItem();
            }

            return result;
        }

        protected virtual void PerformMouseDown(RadMenuItemBase menuItem)
        {
            if (this.menuState == RadMenuState.NotActive || this.menuState == RadMenuState.CuesVisible)
            {
                if (this.menuState == RadMenuState.CuesVisible)
                {
                    this.SetMenuState(RadMenuState.CuesVisibleKeyboardActive);
                }
                else
                {
                    this.SetMenuState(RadMenuState.KeyboardActive);
                }
            }

            if (!this.CanProcessItem(menuItem))
            {
                return;
            }

            if (menuItem != this.GetSelectedItem())
            {
                menuItem.Select();
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            RadMenuItemBase menuItem = this.elementTree.GetElementAtPoint(e.Location) as RadMenuItemBase;

            this.PerformMouseDown(menuItem);

            base.OnMouseDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            RadMenuItemBase menuItem = this.elementTree.GetElementAtPoint(e.Location) as RadMenuItemBase;
            RadMenuItemBase currentSelectedItem = this.GetSelectedItem() as RadMenuItemBase;

            if (!this.CanProcessItem(menuItem))
            {
                base.OnMouseMove(e);
                return;
            }

            if (currentSelectedItem != null &&
                !object.ReferenceEquals(currentSelectedItem, menuItem))
            {
                currentSelectedItem.Deselect();
                currentSelectedItem.HideChildItems();

            }

            if (this.shouldShowChildren
                && !object.ReferenceEquals(currentSelectedItem, menuItem))
            {
                menuItem.ShowChildItems();
            }

            base.OnMouseMove(e);
        }


        private void OnRadMenu_ItemsChanged(RadItemCollection changed, RadItem target, ItemsChangeOperation operation)
        {
            if (operation == ItemsChangeOperation.Inserted)
            {
                RadMenuItemBase menuItem = target as RadMenuItemBase;

                if (menuItem != null && !this.IsDesignMode)
                {
                    menuItem.SetDefaultValueOverride(RadMenuItemBase.PopupDirectionProperty, RadDirection.Down);

                    if (menuItem is RadMenuItem)
                    {
                        (menuItem as RadMenuItem).ShowKeyboardCue = false;
                    }
                }
            }
        }

        private bool shouldShowChildren = false;


        protected override void OnItemSelected(ItemSelectedEventArgs args)
        {
            base.OnItemSelected(args);

            if (args.Item is RadMenuItemBase)
            {
                RadMenuItemBase menuItem = args.Item as RadMenuItemBase;
                menuItem.Selected = true;
            }
        }

        protected override void OnItemDeselected(ItemSelectedEventArgs args)
        {
            base.OnItemDeselected(args);

            if (args.Item is RadMenuItemBase)
            {
                RadMenuItemBase item = args.Item as RadMenuItemBase;
                item.Selected = false;
                if (item.Enabled && item.HasChildren)
                {
                    this.shouldShowChildren = item.IsPopupShown;
                }
            }
        }



        /// <commentsfrom cref="RadMenuElement.OnOrientationChanged" filter=""/>
        protected virtual void OnOrientationChanged(EventArgs args)
        {
            if (this.OrientationChanged != null)
            {
                this.OrientationChanged(this, args);
            }
        }

        /// <commentsfrom cref="RadMenuElement.OnAllItemsEqualHeightChanged" filter=""/>
        protected virtual void OnAllItemsEqualHeightChanged(EventArgs args)
        {
            if (this.AllItemsEqualHeightChanged != null)
            {
                this.AllItemsEqualHeightChanged(this, args);
            }
        }

        /// <commentsfrom cref="RadItem.OnTextOrientationChanged" filter=""/>
        protected virtual void OnTextOrientationChanged(EventArgs args)
        {
            if (this.TextOrientationChanged != null)
            {
                this.TextOrientationChanged(this, args);
            }
        }


        private void RadMenu_Initialized(object sender, EventArgs e)
        {
            if (!IsDesignMode)
            {
                this.SetMenuState(RadMenuState.NotActive);
            }
        }

        internal bool Activated
        {
            get
            {
                return activated;
            }
            set
            {
                if (activated != value)
                {
                    activated = value;
                }
            }
        }

        private bool FindAndSelectItem(char keyData)
        {
            RadMenuItemBase selectedItem = this.GetSelectedItem() as RadMenuItemBase;

            if (selectedItem != null && selectedItem.IsPopupShown)
            {
                if (selectedItem.DropDown.CanProcessMnemonic(keyData))
                {
                    this.SetMenuState(RadMenuState.NotActive);
                }
                return false;
            }

            foreach (RadMenuItemBase item in this.Items)
            {
                char pressed = keyData;
                if (item.Enabled && IsMnemonic(pressed, item.Text))
                {

                    RadMenuItemBase currentSelectedItem = this.GetSelectedItem() as RadMenuItemBase;

                    if (currentSelectedItem != null && currentSelectedItem.IsPopupShown)
                    {
                        currentSelectedItem.HideChildItems();
                    }

                    item.Select();

                    currentSelectedItem = this.GetSelectedItem() as RadMenuItemBase;

                    if (currentSelectedItem != null)
                    {
                        this.BeginInvoke(new RadDropDownMenu.PerformClickInvoker(this.PerformItemClick), currentSelectedItem);
                    }

                    return true;
                }
            }

            return false;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether the menu items should be stretched to fill the available space.
        /// </summary>
        [Category("Behavior")]
        [Description("Indicates whether the menu items should be stretched to fill the available space.")]
        [Browsable(true)]
        [DefaultValue(false)]
        public bool StretchItems
        {
            get
            {
                if (this.menuElement != null && this.menuElement.ItemsLayout != null)
                {
                    return this.menuElement.ItemsLayout.StretchItems;
                }
                else
                {
                    return false;
                }
            }
            set
            {
                if (this.menuElement != null && this.menuElement.ItemsLayout != null)
                {
                    this.menuElement.ItemsLayout.StretchItems = value;
                }
            }
        }

        internal bool ShowMenuKeyboardCues
        {
            get
            {
                return showKeyboardCues;
            }

            set
            {
                if (!this.IsDesignMode && value != this.showKeyboardCues)
                {
                    this.showKeyboardCues = value;
                    foreach (RadMenuItemBase item in this.Items)
                    {
                        RadMenuItem castedItem = item as RadMenuItem;

                        if (castedItem != null)
                        {
                            this.ToggleKeyboardCues(castedItem, value);
                        }
                    }
                }
            }
        }

        internal bool HighLightCycleCompleted
        {
            get
            {
                return this.highLightCycleCompleted;
            }

            set
            {
                this.highLightCycleCompleted = value;
            }
        }

        /// <summary>
        /// Gets or sets whether the Alt or F10 keys can be used to highlight the menu.
        /// </summary>
        [Category("Behavior")]
        [Description("Gets or sets whether the Alt or F10 can be used to highlight the menu.")]
        [Browsable(true)]
        [DefaultValue(true)]
        public bool SystemKeyHighlight
        {
            get
            {
                return this.systemKeyHighlight;
            }

            set
            {
                if (this.systemKeyHighlight != value)
                {
                    this.systemKeyHighlight = value;
                    OnNotifyPropertyChanged("SystemKeyHighlight");
                }
            }
        }

        protected override Size DefaultSize
        {
            get
            {
                return new Size(100, 24);
            }
        }

        [DefaultValue(true)]
        public override bool AutoSize
        {
            get
            {
                return base.AutoSize;
            }
            set
            {
                base.AutoSize = value;
            }
        }

        /// <summary>
        /// Gets the instance of RadMenuElement wrapped by this control. RadMenuElement
        /// is the main element in the hierarchy tree and encapsulates the actual functionality of RadMenu.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RadMenuElement MenuElement
        {
            get
            {
                return this.menuElement;
            }
        }

        /// <commentsfrom cref="RadMenuElement.Items" filter=""/>
        [RadEditItemsAction]
        [RadNewItem("Type here", true, true, false)]
        [Browsable(true), Category(RadDesignCategory.DataCategory)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public override RadItemOwnerCollection Items
        {
            get
            {
                return this.menuElement.Items;
            }
        }

        [Category(RadDesignCategory.BehaviorCategory),
        DefaultValue(true),
        Description("ToolStripAllowMergeDescr")]
        public bool AllowMerge
        {
            get
            {
                return this.menuElement.AllowMerge;
            }
            set
            {
                this.menuElement.AllowMerge = value;
            }
        }

        /// <commentsfrom cref="RadMenuElement.Orientation" filter=""/>
        [Browsable(true), Category(RadDesignCategory.BehaviorCategory)]
        [RadDescription("Orientation", typeof(RadMenuElement))]
        [RadDefaultValue("Orientation", typeof(RadMenuElement))]
        public Orientation Orientation
        {
            get
            {
                return this.menuElement.Orientation;
            }
            set
            {
                if (value != this.menuElement.Orientation)
                {
                    this.menuElement.Orientation = value;
                    if (value == Orientation.Horizontal)
                    {
                        if (this.RootElement != null)
                        {
                            this.RootElement.ResetValue(RadElement.StretchVerticallyProperty, ValueResetFlags.Local);
                            this.RootElement.ResetValue(RadElement.StretchHorizontallyProperty, ValueResetFlags.Local);
                        }
                        if (this.Dock == DockStyle.Left)
                            this.Dock = DockStyle.Top;
                        if (this.Dock == DockStyle.Right)
                            this.Dock = DockStyle.Bottom;
                    }
                    else
                    {
                        if (this.RootElement != null)
                        {
                            this.RootElement.StretchHorizontally = false;
                            this.RootElement.StretchVertically = true;
                        }
                        if (this.Dock == DockStyle.Top)
                            this.Dock = DockStyle.Left;
                        if (this.Dock == DockStyle.Bottom)
                            this.Dock = DockStyle.Right;
                    }
                }
            }
        }

        [DefaultValue(DockStyle.Top)]
        public override DockStyle Dock
        {
            get
            {
                return base.Dock;
            }
            set
            {
                base.Dock = value;
            }
        }

        /// <commentsfrom cref="RadMenuElement.AllItemsEqualHeight" filter=""/>
        [Browsable(true), Category(RadDesignCategory.BehaviorCategory)]
        [RadDescription("AllItemsEqualHeight", typeof(RadMenuElement))]
        [RadDefaultValue("AllItemsEqualHeight", typeof(RadMenuElement))]
        public bool AllItemsEqualHeight
        {
            get
            {
                return this.menuElement.AllItemsEqualHeight;
            }
            set
            {
                this.menuElement.AllItemsEqualHeight = value;
            }
        }

        /// <commentsfrom cref="RadMenuElement.DropDownAnimationEnabled" filter=""/>
        [Browsable(true), Category(RadDesignCategory.AppearanceCategory)]
        [RadDefaultValue("DropDownAnimationEnabled", typeof(RadMenuElement))]
        public bool DropDownAnimationEnabled
        {
            get
            {
                return this.menuElement.DropDownAnimationEnabled;
            }
            set
            {
                this.menuElement.DropDownAnimationEnabled = value;
            }
        }

        /// <commentsfrom cref="RadMenuElement.DropDownAnimationEasing" filter=""/>
        [Browsable(true), Category(RadDesignCategory.AppearanceCategory)]
        [RadDefaultValue("DropDownAnimationEasing", typeof(RadMenuElement))]
        public RadEasingType DropDownAnimationEasing
        {
            get
            {
                return this.menuElement.DropDownAnimationEasing;
            }
            set
            {
                this.menuElement.DropDownAnimationEasing = value;
            }
        }

        /// <commentsfrom cref="RadMenuElement.DropDownAnimationFrames" filter=""/>
        [Browsable(true), Category(RadDesignCategory.AppearanceCategory)]
        [RadDefaultValue("DropDownAnimationFrames", typeof(RadMenuElement))]
        public int DropDownAnimationFrames
        {
            get
            {
                return this.menuElement.DropDownAnimationFrames;
            }
            set
            {
                this.menuElement.DropDownAnimationFrames = value;
            }
        }

        #endregion

        #region Message Handlers

        protected virtual RadMenuItemBase GetSysCharItem(IItemsControl itemsControl, char searchKey)
        {
            foreach (RadMenuItemBase menuItem in itemsControl.Items)
            {
                if (menuItem.Enabled && IsMnemonic(searchKey, menuItem.Text))
                {
                    return menuItem;
                }
                else if (menuItem.HasChildren && menuItem.DropDown != null)
                {
                    RadMenuItemBase foundItem = this.GetSysCharItem(menuItem.DropDown, searchKey);

                    if (foundItem != null)
                    {
                        return foundItem;
                    }

                }
            }

            return null;
        }

        protected override bool ProcessCmdKey(ref Message m, Keys keyData)
        {
            if (keyData == Keys.Escape && PopupManager.Default.PopupCount == 0)
            {
                this.SetMenuState(RadMenuState.NotActive);

                RadMenuItemBase menuItem = this.GetSelectedItem() as RadMenuItemBase;
                if (menuItem != null)
                {
                    menuItem.Deselect();
                }
                return true;
            }

            if (m.WParam == (IntPtr)NativeMethods.VK_RETURN)
                return false;

            return base.ProcessCmdKey(ref m, keyData);
        }


        protected virtual bool ProcessFirstStageMnemonicActivation(ref Message m, Keys keyData)
        {
            bool firstTimeKeyDown = !Convert.ToBoolean((int)m.LParam & (1 << 30));

            if (!firstTimeKeyDown || !this.Visible || !this.systemKeyHighlight)
            {
                return false;
            }

            if (this.menuState == RadMenuState.CuesVisibleKeyboardActive)
            {
                RadMenuItemBase menuItemBase = this.GetSelectedItem() as RadMenuItemBase;
                if (menuItemBase != null)
                {
                    menuItemBase.HideChildItems();
                    menuItemBase.Deselect();
                }

                this.SetMenuState(RadMenuState.NotActive);
            }
            else
            {
                //this.ShowMenuKeyboardCues = true;
                //this.menuState = RadMenuState.CuesVisible;
                this.SetMenuState(RadMenuState.CuesVisible);
            }



            return true;
        }

        protected virtual bool ProcessSecondStageMnemonicActivation(ref Message m, Keys keyData)
        {
            if (!this.Visible || !this.systemKeyHighlight)
            {
                return false;
            }

            if (keyData == Keys.F10)
            {
                if (this.menuState == RadMenuState.NotActive)
                {
                    this.SetMenuState(RadMenuState.CuesVisibleKeyboardActive);
                    this.SelectFirstVisibleItem();
                    return true;
                }
                else
                {
                    RadMenuItemBase selectedItem = this.GetSelectedItem() as RadMenuItemBase;

                    if (selectedItem != null)
                    {
                        selectedItem.HideChildItems();
                    }
                    this.SetMenuState(RadMenuState.NotActive);

                    return true;
                }
            }

            if (this.menuState == RadMenuState.CuesVisible)
            {
                this.SetMenuState(RadMenuState.CuesVisibleKeyboardActive);
                this.SelectFirstVisibleItem();
                return true;
            }

            return false;
        }

        public override bool CanProcessMnemonic(char keyData)
        {
            RadMenuItemBase activeItem = this.GetSelectedItem() as RadMenuItemBase;
            if (activeItem != null && activeItem.IsPopupShown)
            {
                return false;
            }
            foreach (RadMenuItemBase menuItem in this.Items)
            {
                if (IsMnemonic(keyData, menuItem.Text))
                {
                    return true;
                }
            }

            return false;
        }

        protected override bool ProcessMnemonic(char charCode)
        {
            //Since the ProcessMnemonic can be also called by the base class,
            //we make sure that we process mnemonics only when the
            //menu is in the correct state, i.e. the alt key was pressed
            //and the mnemonics have been highlighted.
            if (this.menuState != RadMenuState.CuesVisibleKeyboardActive &&
                this.menuState != RadMenuState.CuesVisible)
            {
                return false;
            }

            if (FindAndSelectItem(charCode))
            {
                return true;
            }

            return false;
        }

        #endregion


        #region Dispose

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                RadMessageFilter.Instance.RemoveListener(this);
                this.Initialized -= new EventHandler(RadMenu_Initialized);
                this.Items.ItemsChanged -= new ItemChangedDelegate(this.OnRadMenu_ItemsChanged);
                Form parentForm = this.FindForm();
                if (parentForm != null)
                {
                    parentForm.Deactivate -= this.OnParentForm_Deactivate;
                }
            }


            base.Dispose(disposing);
        }

        #endregion

        protected override RootRadElement CreateRootElement()
        {
            return new RadMenuRootElement();
        }

        public class RadMenuRootElement : RootRadElement
        {
            protected internal override bool? ShouldSerializeProperty(PropertyDescriptor property)
            {
                if (property.Name == "StretchHorizontally" ||
                    property.Name == "StretchVertically")
                    return false;
                bool? res = base.ShouldSerializeProperty(property);
                return res;
            }

            protected override Type ThemeEffectiveType
            {
                get
                {
                    return typeof(RootRadElement);
                }
            }
        }

        protected override AccessibleObject CreateAccessibilityInstance()
        {
            return new RadMenuAccessibleObject(this);
        }

        #region ITooltipOwner
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public object Owner
        {
            get
            {
                return null;
            }
            set
            {
                
            }
        }
        #endregion
    }


}

internal struct ApplicationMdiState
{
    public Form MdiParentForm;
    public Form MaximizedChildForm;
    public Telerik.WinControls.UI.RadMenu MaximizedChildFormMenu;
}

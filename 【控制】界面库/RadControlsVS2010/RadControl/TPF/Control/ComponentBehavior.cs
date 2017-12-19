using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls.Assistance;
using Telerik.WinControls.Keyboard;
using System.Diagnostics;

namespace Telerik.WinControls
{
    public class ComponentBehavior : IDisposable
    {
        #region Fields

        private IComponentTreeHandler owner;
        private Control ownerControl;
        private FillRepository bitmapRepository = new FillRepository();
        private bool mouseOver;

        internal RadItem itemCapture;
        internal protected bool ItemCaptureState = true;
        internal RadElement selectedElement;

        #endregion

        #region Constructors

        public ComponentBehavior(IComponentTreeHandler owner)
        {
            this.owner = owner;
            this.ownerControl = owner as Control;
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.lastFocusedElement = null;
                this.selectedElement = null;
                this.itemCapture = null;

                if (this.keyMapItems != null)
                {
                    this.keyMapItems.Clear();
                }

                if (this.tooltip != null)
                {
                    this.tooltip.Dispose();
                }

                if (this.tipPresenter != null)
                {
                    this.tipPresenter.Dispose();
                }

                if (this.shortcuts != null)
                {
                    this.shortcuts.Dispose();
                }

                if (this.bitmapRepository != null)
                {
                    this.bitmapRepository.Dispose();
                }

                if (oldOwnerControl != null)
                {
                    oldOwnerControl = null;
                }

                this.DisposeKeyTips();
            }
        }

        #endregion

        #region Mnemonics

        internal protected RadItem GetActivatedItem(Control control, char charCode)
        {
            IComponentTreeHandler owner = control as IComponentTreeHandler;
            if (owner == null)
            {
                return null;
            }

            RootRadElement root = owner.RootElement;
            for (int i = 0; i < owner.RootElement.Children.Count; i++)
            {
                RadItem item = GetActivatedItem(owner.RootElement.Children[i], charCode);
                if (item != null)
                {
                    return item;
                }
            }
            return null;
        }

        internal protected RadItem GetActivatedItem(RadElement element, char charCode)
        {
            RadItem testItem = element as RadItem;
            if (testItem != null)
            {
                if ((!string.IsNullOrEmpty(testItem.Text) && testItem.Enabled))
                {
                    if (testItem.GetBitState(RadItem.ContainsMnemonicStateKey) && Control.IsMnemonic(charCode, testItem.Text))
                    {
                        return testItem;
                    }
                }

                if ((!string.IsNullOrEmpty(testItem.Text) && testItem.Enabled))
                {
                    if (testItem.GetBitState(RadItem.ContainsMnemonicStateKey) && TelerikHelper.IsPseudoMnemonic(charCode, testItem.Text))
                    {
                        return testItem;
                    }
                }
            }
            return null;
        }

        internal protected bool ProcessMnemonic(char charCode)
        {
            if (!TelerikHelper.CanProcessMnemonic(this.ownerControl))
            {
                return false;
            }
            List<Control> mnemonicControlsList = new List<Control>();
            this.GetThemedChildControlsList(this.ownerControl, mnemonicControlsList);
            if (mnemonicControlsList.Count > 0)
            {
                Control validControl = GetValidChildControlByMnemonic(mnemonicControlsList, charCode);
                if (validControl != null)
                {
                    return false;
                }
                for (int i = 0; i < mnemonicControlsList.Count; i++)
                {
                    RadItem item = GetActivatedItem(mnemonicControlsList[i], charCode);
                    if (item != null)
                    {
                        item.Focus();
                        return item.ProcessMnemonic(charCode);
                    }
                }
            }
            return false;
        }

        protected virtual void GetThemedChildControlsList(Control control, List<Control> mnemonicList)
        {
            if ((control is IComponentTreeHandler) &&
                TelerikHelper.CanProcessMnemonicNoRecursive(control))
            {
                mnemonicList.Add(control);
            }
            foreach (Control control1 in control.Controls)
            {
                if (control1 != null &&
                    (control1 is IComponentTreeHandler))
                {
                    this.GetThemedChildControlsList(control1, mnemonicList);
                }
            }
        }

        protected virtual Control GetValidChildControlByMnemonic(List<Control> mnemonicList, char charCode)
        {
            if (mnemonicList.Count > 0)
            {
                return null;
            }
            for (int i = 0; i < mnemonicList.Count; i++)
            {
                char ch1 = WindowsFormsUtils.GetMnemonic(mnemonicList[i].Text, true);
                if (ch1 != '\0' &&
                    charCode.Equals(ch1))
                {
                    return mnemonicList[i];
                }
            }
            return null;
        }

        internal virtual void ProcessItemMnemonic(RadItem item, char charCode)
        {
            if (TelerikHelper.CanProcessMnemonic(this.ownerControl) && (item != null))
            {
                item.Focus();
                //item.Select();
            }
        }

        //internal protected bool ProcessMnemonic(char charCode)
        //{
        //    if (!TelerikHelper.CanProcessMnemonic(this.ownerControl))
        //    {
        //        return false;
        //    }
        //    ArrayList mnemonicList = new ArrayList();
        //    if (this.ownerControl is IComponentTreeHandler)
        //    {

        //    }
        //    this.GetChildControlsMnemonicList(this.ownerControl, mnemonicList);

        //    RadItem selectedItem = null;// this.GetSelectedItem();
        //    int index = 0;
        //    if (selectedItem != null)
        //    {
        //        index = mnemonicList.IndexOf(selectedItem);
        //    }
        //    index = Math.Max(0, index);
        //    RadItem item = null;
        //    bool flag = false;
        //    int num2 = index;

        //    for (int i = 0; i < mnemonicList.Count; i++)
        //    {
        //        RadItem item3 = mnemonicList[num2] as RadItem;
        //        num2 = (num2 + 1) % mnemonicList.Count;
        //        if ((!string.IsNullOrEmpty(item3.Text) && item3.Enabled))
        //        {
        //            //flag = flag || (item3 is ToolStripMenuItem);
        //            if (item3.ContainsMnemonic && Control.IsMnemonic(charCode, item3.Text))
        //            {
        //                if (item == null)
        //                {
        //                    item = item3;
        //                }
        //                else
        //                {
        //                    if (item == selectedItem)
        //                    {
        //                        this.ProcessItemMnemonic(item3, charCode);
        //                    }
        //                    else
        //                    {
        //                        this.ProcessItemMnemonic(item, charCode);
        //                    }
        //                    return true;
        //                }
        //            }
        //        }
        //    }
        //    if (item != null)
        //    {
        //        return item.ProcessMnemonic(charCode);
        //    }
        //    if (!flag)
        //    {
        //        return false;
        //    }
        //    num2 = index;
        //    for (int j = 0; j < mnemonicList.Count; j++)
        //    {
        //        RadItem item4 = mnemonicList[num2] as RadItem;
        //        num2 = (num2 + 1) % mnemonicList.Count;
        //        if (((!string.IsNullOrEmpty(item4.Text)) &&
        //            (item4.Enabled)) && TelerikHelper.IsPseudoMnemonic(charCode, item4.Text))
        //        {
        //            if (item != null)
        //            {
        //                if (item == selectedItem)
        //                {
        //                    this.ProcessItemMnemonic(item4, charCode);
        //                }
        //                else
        //                {
        //                    this.ProcessItemMnemonic(item, charCode);
        //                }
        //                return true;
        //            }
        //            item = item4;
        //        }
        //    }
        //    return ((item != null) && item.ProcessMnemonic(charCode));
        //}
        //protected virtual void GetChildControlsMnemonicList(Control control, ArrayList mnemonicList)
        //{
        //    char ch1 = WindowsFormsUtils.GetMnemonic(control.Text, true);
        //    if (ch1 != '\0')
        //    {
        //        mnemonicList.Add(ch1);
        //    }
        //    foreach (Control control1 in control.Controls)
        //    {
        //        if (control1 != null)
        //        {
        //            this.GetChildControlsMnemonicList(control1, mnemonicList);
        //        }
        //    }
        //}

        protected virtual string GetMnemonicText(string text)
        {
            char ch1 = WindowsFormsUtils.GetMnemonic(text, false);
            if (ch1 != '\0')
            {
                return ("Alt+" + ch1);
            }
            return null;
        }

        protected virtual bool ProcessMnemonicChar(char charCode)
        {
            return true;
        }
        #endregion

        #region Focus Support
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RadElement FocusedElement
        {
            get
            {
                return this.currentFocusedElement;
            }
            set
            {
                if (value != null)
                {
                    value.Focus();
                }
                else if (this.currentFocusedElement != null)
                {
                    this.currentFocusedElement.SetElementFocused(false);
                    this.currentFocusedElement = null;
                }
            }
        }

        public virtual bool OnGotFocus(EventArgs e)
        {
            //base.OnGotFocus(e);
            if (this.lastFocusedElement != null)
            {
                if (!this.lastFocusedElement.IsDisposed)
                {
                    this.currentFocusedElement = lastFocusedElement;
                    this.currentFocusedElement.SetElementFocused(true);
                }
                this.lastFocusedElement = null;
            }
            return false;
        }

        internal RadElement lastFocusedElement = null;
        internal RadElement currentFocusedElement = null;

        public virtual bool OnLostFocus(EventArgs e)
        {
            //base.OnLostFocus(e);

            if (this.currentFocusedElement != null)
            {
                this.currentFocusedElement.SetElementFocused(false);
                lastFocusedElement = currentFocusedElement;
            }
            // Key Tips Logic
            if (this.isKeyMapActive)
            {
                this.ResetKeyMapInternal();
            }
            return false;
        }

        internal void SettingElementFocused(RadElement toCangeFocus, bool focusValue)
        {
            if (focusValue)
            {
                if (this.currentFocusedElement != null)
                {
                    this.currentFocusedElement.SetElementFocused(false);
                }
                this.currentFocusedElement = toCangeFocus;
                this.lastFocusedElement = toCangeFocus;
            }
            else if (this.currentFocusedElement == toCangeFocus)
            {
                this.currentFocusedElement.SetElementFocused(false);
                this.currentFocusedElement = null;
            }
        }

        private bool allowShowFocusCues = false;

        /// <summary>
        /// Indicates focus cues display, when available, based on the corresponding control type and the current UI state.
        /// </summary>
        [DefaultValue(false), Category("Accessibility")]
        public virtual bool AllowShowFocusCues
        {
            get
            {
                return allowShowFocusCues;
            }
            set
            {
                if (this.allowShowFocusCues != value)
                {
                    this.allowShowFocusCues = value;
                    this.owner.InvalidateIfNotSuspended();
                }
            }
        }

        internal bool ShouldShowFocusCues
        {
            get
            {
                return this.AllowShowFocusCues && this.owner.GetShowFocusCues();
            }
        }

        #endregion

        #region Data Validation
        internal static BindingFlags flags =
            (BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
        private PropertyInfo cancelProperty = null;
        internal bool ValidationCancelled = false;

        internal bool ValidationCanceledAction()
        {
            this.cancelProperty = GetValidationCanceledProperty();
            return this.ValidationCancelled = (bool)this.cancelProperty.GetValue(this.owner, null);
        }

        internal static PropertyInfo GetValidationCanceledProperty()
        {
            //if (control == null)
            //{
            //    return false;
            //}
            return typeof(Control).GetProperty("ValidationCancelled", flags);
        }

        public void SetElementValue(RadElement element, RadProperty dependencyProperty, object value)
        {
            if (this.ownerControl.InvokeRequired)
            {
                this.ownerControl.BeginInvoke(new SetValueDelegate(element.SetValue), dependencyProperty, value);
            }
            else
            {
                element.SetValue(dependencyProperty, value);
            }
        }

        internal Form FindFormInternal(Control control)
        {
            Control control1 = control;
            while ((control1.Parent != null))
            {
                control1 = control1.Parent;
            }
            return control1 as Form;
        }

        #endregion

        #region ToolTip Management

        private ToolTip tooltip = null;
        private bool showItemToolTips;
        private static Size onePixel = new Size(1, 1);
        private RadItem currentlyActiveTooltipItem;
        private RadItem currentlyActiveScreentipItem;
        private ScreenTipPresenter tipPresenter;

        internal ToolTipTextNeededEventArgs OnToolTipTextNeeded(RadItem sender)
        {
            Size offset = Size.Empty;
            Cursor mouseCursor = Cursor.Current;

            if (mouseCursor != null)
            {
                offset.Width = 1;
                offset.Height += mouseCursor.Size.Height - mouseCursor.HotSpot.Y;
            }

            ToolTipTextNeededEventArgs e = new ToolTipTextNeededEventArgs(sender.ToolTipText, offset);
            this.owner.CallOnToolTipTextNeeded(sender, e);
            return e;
        }

        internal ScreenTipNeededEventArgs CallOnScreenTipNeeded(RadItem item)
        {
            Size offset = Size.Empty;

            if (this.ShowScreenTipsBellowControl)
            {
                offset.Height = this.ownerControl.ClientRectangle.Height;
                offset.Width = item.ControlBoundingRectangle.X - this.ownerControl.ClientRectangle.X;
            }
            else
            {
                offset.Height = Math.Max(item.Size.Height, 15);
            }

            ScreenTipNeededEventArgs e = new ScreenTipNeededEventArgs(item, offset);
            this.owner.CallOnScreenTipNeeded(this, e);
            return e;
        }

        /// <summary>
        /// Gets the tool tip
        /// </summary>
        /// <value>The tool tip.</value>
        public ToolTip ToolTip
        {
            get
            {
                if (this.tooltip == null)
                {
                    this.tooltip = new ToolTip();
                }
                return this.tooltip;
            }
        }

        internal ScreenTipPresenter ScreenPresenter
        {
            get
            {
                if (this.tipPresenter == null)
                {
                    this.tipPresenter = new ScreenTipPresenter(this.OwnerControl);
                    TelerikHelper.SetDropShadow(this.tipPresenter.Handle);
                }
                return this.tipPresenter;
            }
        }

        protected virtual bool DefaultShowItemToolTips
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether ToolTips are shown for the RadItem objects contained in 
        /// the RadControl.
        /// </summary>
        [Description("Gets or sets a value indicating whether ToolTips are shown for the RadItem objects contained in the RadControl."),
        DefaultValue(true), Category("Behavior")]
        public bool ShowItemToolTips
        {
            get
            {
                return this.showItemToolTips;
            }
            set
            {
                if (this.showItemToolTips != value)
                {
                    this.showItemToolTips = value;

                    if (!this.showItemToolTips)
                    {
                        this.UpdateToolTip(null);
                    }
                }
            }
        }

        internal void UpdateScreenTip(ScreenTipNeededEventArgs args)
        {
            RadItem item = null;

            if (args != null)
            {
                item = args.Item;
            }

            if (this.ShowItemToolTips && item != this.currentlyActiveScreentipItem)
            {
                this.HideScreenTip();
                this.currentlyActiveScreentipItem = item;

                if (this.currentlyActiveScreentipItem != null)
                {
                    this.ShowScreenTip(args);
                }
            }
        }

        internal void HideScreenTip()
        {
            this.ScreenPresenter.Hide();
        }

        internal void ShowScreenTip(ScreenTipNeededEventArgs args)
        {
            RadItem item = args.Item;
            Size offset = args.Offset;
            int delay = args.Delay;

            IntPtr activeHwnd = NativeMethods.GetActiveWindow();

            NativeMethods.SetWindowLong(
                new HandleRef(this, this.ScreenPresenter.Handle),
                NativeMethods.GWL_HWNDPARENT, new HandleRef(this, activeHwnd));

            Cursor mouseCursor = Cursor.Current;

            if (mouseCursor != null)
            {
                Point screenTipLocation = Point.Empty;

                if (this.ShowScreenTipsBellowControl)
                {
                    screenTipLocation = this.ownerControl.PointToScreen(this.ownerControl.Location);
                }
                else
                {
                    screenTipLocation = item.PointToScreen(item.Location);
                }

                screenTipLocation.X += offset.Width;
                screenTipLocation.Y += offset.Height;

                this.ScreenPresenter.Show(item.ScreenTip, screenTipLocation, delay);
            }
        }

        /// <summary>
        /// Gets or sets value indicating whether the control should show all screen tips under the control client rectangle, as required for the RibbonBar control, for example
        /// </summary>
        public bool ShowScreenTipsBellowControl
        {
            get { return showScreenTipsBellowControl; }
            set { showScreenTipsBellowControl = value; }
        }

        internal void UpdateToolTip(RadItem item)
        {

            this.UpdateToolTip(item, Size.Empty);
        }

        static Control oldOwnerControl = null;
        internal void UpdateToolTip(RadItem item, Size offset)
        {
            if (this.ShowItemToolTips && item != this.currentlyActiveTooltipItem && this.ToolTip != null)
            {
                if (item == null)
                {
                    this.ToolTip.Hide(this.ownerControl);
                }

                this.ToolTip.Active = false;
                this.currentlyActiveTooltipItem = item;
                Cursor mouseCursor = Cursor.Current;

                if (this.currentlyActiveTooltipItem != null && mouseCursor != null)
                {
                    this.ToolTip.Active = true;
                    Point mousePosition = Cursor.Position;
                    mousePosition.X += offset.Width;
                    mousePosition.Y += offset.Height;
                    Rectangle rectangle = WindowsFormsUtils.ConstrainToScreenBounds(new Rectangle(mousePosition, onePixel));
                    mousePosition = this.ownerControl.PointToClient(rectangle.Location);
                    string toolTipText = this.currentlyActiveTooltipItem.ToolTipText;
                    

                    Form ownerForm = this.ownerControl.FindForm();
                    if (ownerForm==null)
                    {
                        ITooltipOwner owner = this.ownerControl as ITooltipOwner;
                        while (owner!=null)
                        {
                            Control control = owner as Control;
                            if (control != null && owner.Owner==null)
                            {
                                mousePosition = control.PointToClient(rectangle.Location);
                                this.ToolTip.SetToolTip(control, toolTipText);                                         
                                this.ToolTip.Show(toolTipText, control, mousePosition, this.ToolTip.AutoPopDelay);
                                break;
                            }

                            if (owner.Owner == null)
                            {
                                break;
                            }

                            owner = owner.Owner as ITooltipOwner;
                        }                        
                    }
                    else
                    {                        
                        this.ToolTip.Show(toolTipText, this.ownerControl, mousePosition, this.ToolTip.AutoPopDelay);
                    }
                }
            }
        }

        #endregion

        #region Chords

        private bool showCommandBindingsHints;
        private Shortcuts shortcuts = null;

        [Category(RadDesignCategory.BehaviorCategory),
            //DefaultValue(null),
        TypeConverter(typeof(ExpandableObjectConverter)),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public InputBindingsCollection CommandBindings
        {
            get
            {
                return this.Shortcuts.InputBindings;
            }
        }

        internal Shortcuts Shortcuts
        {
            get
            {
                if (this.shortcuts == null)
                {
                    this.shortcuts = new Shortcuts(this.ownerControl);
                }
                return this.shortcuts;
            }
        }

        internal bool ShowCommandBindingsHints
        {
            get
            {
                return this.showCommandBindingsHints;
            }
            set
            {
                if (this.showCommandBindingsHints != value)
                {
                    this.showCommandBindingsHints = value;
                }
            }
        }

        #endregion

        #region KeyTip Management

        private bool isKeyMapActive = false;
        private bool enableKeyTips = false;
        private IntPtr focusedBeforeKeyMap = IntPtr.Zero;
        private Cursor previousCursor = null;
        //private bool paintKeyTips = false;
        //private RadItem activeKeyMapItem = null;
        private AdornerLayer adornerLayer = null;

        private Stack<RadItem> keyMapItems = new Stack<RadItem>();
        private Stack<Keys> keyMaps = new Stack<Keys>();

        private StringBuilder keyMapBuilder = new StringBuilder();
        private bool showScreenTipsBellowControl;

        protected internal RadItem ActiveKeyMapItem
        {
            get
            {
                if (keyMapItems.Count > 0)
                {
                    return keyMapItems.Peek();
                }
                return null;
            }
            set
            {
                keyMapItems.Push(value);
            }
        }

        internal protected bool IsKeyMapActive
        {
            get
            {
                return isKeyMapActive;
            }
        }

        /// <summary>
        /// Gets whether this instance of RadControl is on a active form
        /// </summary>
        internal protected bool IsParentFormActive
        {
            get
            {
                Form form1 = this.FindFormInternal(this.ownerControl);
                if (form1 == null)
                {
                    return false;
                }
                if (form1.IsMdiChild)
                {
                    if (form1.MdiParent == null)
                    {
                        return false;
                    }
                    if (form1.MdiParent.ActiveMdiChild != form1)
                    {
                        return false;
                    }
                }
                else if (form1 != Form.ActiveForm)
                {
                    return false;
                }
                return true;
            }
        }

        /// <summary>
        /// Gets or sets whether Key Map (Office 2007 like accelerator keys map)
        /// is used for this speciffic control. Currently this option is implemented for 
        /// the RadRibbonBar control only.
        /// </summary>
        [Browsable(true), DefaultValue(false)]
        public bool EnableKeyMap
        {
            get
            {
                return this.EnableKeyTips;
            }
            set
            {
                this.EnableKeyTips = value;
            }
        }

        /// <summary>
        /// Gets or sets whether Key Tips (Office 2007 like accelerator keys map)
        /// are used for this speciffic control.
        /// </summary>
        [Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        internal protected virtual bool EnableKeyTips
        {
            get
            {
                return this.enableKeyTips;
            }
            set
            {
                if (this.enableKeyTips != value)
                {
                    this.enableKeyTips = value;
                    if (value)
                    {
                        ChordMessageFilter.RegisterKeyTipsConsumer(this.owner);
                    }
                    else
                    {
                        ChordMessageFilter.UnregisterKeyTipsConsumer(this.owner);
                    }
                    this.OnEnableKeyTipsChanged();
                }
            }
        }

        /// <summary>
        /// Clears all resources reserved for the KeyTips functionallity
        /// </summary>
        internal protected virtual void DisposeKeyTips()
        {
            this.DisposeAdornerLayer();
            ChordMessageFilter.UnregisterKeyTipsConsumer(this.owner);
            this.focusedBeforeKeyMap = IntPtr.Zero;
        }

        internal protected virtual bool SetKeyMap()
        {
            //if (this.isKeyMapActive)
            {
                this.ResetKeyMap();
            }
            //else
            {
                this.InitializeKeyMap();
            }
            return true;
        }

        internal protected virtual void InitializeKeyMap()
        {
            previousCursor = Cursor.Current;
            Cursor.Current = Cursors.Default;
            //Debug.WriteLine("Ribbon Initialized");
            this.GetKeyMapFocus();
            this.isKeyMapActive = true;
            this.SetInternalKeyMapFocus();
            keyMapItems.Clear();
            this.keyMaps.Clear();
            this.ownerControl.Invalidate();
        }

        internal protected virtual void ResetKeyMap()
        {
            Cursor.Current = previousCursor;
            previousCursor = null;
            //Debug.WriteLine("Ribbon Reset");
            keyMapItems.Clear();
            keyMaps.Clear();
            this.isKeyMapActive = false;
            this.ownerControl.Capture = false;
            this.ReturnKeyMapFocus();
            this.ownerControl.Invalidate();
        }

        internal protected virtual void ResetKeyMapInternal()
        {
            Cursor.Current = previousCursor;
            previousCursor = null;
            keyMapItems.Clear();
            keyMaps.Clear();
            this.isKeyMapActive = false;
            this.ownerControl.Capture = false;
            this.ownerControl.Invalidate();
        }

        internal protected bool GetKeyMapFocus()
        {
            this.focusedBeforeKeyMap = NativeMethods.GetFocus();
            return true;
        }

        internal protected virtual bool SetInternalKeyMapFocus()
        {
            return true;
        }

        protected internal bool ReturnKeyMapFocus()
        {
            if (this.focusedBeforeKeyMap != IntPtr.Zero)
            {
                NativeMethods.SetFocus(new HandleRef(null, this.focusedBeforeKeyMap));
            }
            return true;
        }

        protected virtual void OnEnableKeyTipsChanged()
        {
            if (this.EnableKeyTips)
            {
                //this.InitializeAdornerLayer();
            }
            else
            {
                //this.DisposeAdornerLayer();
            }
        }

        protected virtual void InitializeAdornerLayer()
        {
            if (this.adornerLayer != null)
            {
                this.adornerLayer.BringToFront();
            }
            else
            {
                this.adornerLayer = new AdornerLayer(this.ownerControl);
                this.adornerLayer.Bounds = new Rectangle(0, 0, this.ownerControl.Width, this.ownerControl.Height);
                this.adornerLayer.Visible = true;
                this.ownerControl.Controls.Add(this.adornerLayer);
                this.adornerLayer.BringToFront();
            }
        }

        protected virtual void DisposeAdornerLayer()
        {
            if (this.adornerLayer != null)
            {
                this.adornerLayer.Visible = false;
                this.ownerControl.Controls.Remove(this.adornerLayer);
                this.adornerLayer.Dispose();
                this.adornerLayer = null;
            }
        }

        internal protected virtual List<RadItem> GetRootItems()
        {
            return null;
        }

        internal protected virtual bool ProccessKeyMap(Keys input)
        {
            if (input == Keys.Enter ||
                input == Keys.Space)
            {
                return false;
            }
            string inputString = this.GetKeyStringRepresentation(input);
            if (input == Keys.Escape)
            {
                if (this.keyMapItems.Count > 0)
                {
                    this.keyMapItems.Pop();
                }
                else
                {
                    this.ResetKeyMap();
                }
                //this.keyMaps.Pop();
                this.ownerControl.Invalidate();
                return true;
            }
            if (char.IsLetterOrDigit(input.ToString(), 0))
            {
                if (this.IsPartOfKeyTip(input, inputString))
                {
                    //if (this.ActivateSelectedItem(this.ActiveKeyMapItem)) 
                    //{
                    //    this.ResetKeyMap();
                    //}
                    this.ActivateSelectedItem(this.ActiveKeyMapItem);
                    if (this.GetKeyFocusChildren(this.ActiveKeyMapItem).Count == 0)
                    {
                        this.ResetKeyMap();
                    }
                }
            }
            return true;
        }

        internal protected virtual bool ActivateSelectedItem(RadItem currentKeyMapItem)
        {
            return true;
        }

        internal protected virtual int ProcessUnmappedItems(List<RadItem> childrenToBeMapped)
        {
            int firstFreeAutoNumber = 0;
            List<RadItem> unmapped = new List<RadItem>();
            //here we search for auto-numbered items
            for (int i = 0; i < childrenToBeMapped.Count; i++)
            {
                if (childrenToBeMapped[i].AutoNumberKeyTip > firstFreeAutoNumber)
                {
                    firstFreeAutoNumber = childrenToBeMapped[i].AutoNumberKeyTip;
                }
                if (string.IsNullOrEmpty(childrenToBeMapped[i].KeyTip) &&
                    (childrenToBeMapped[i].AutoNumberKeyTip == 0))
                {
                    unmapped.Add(childrenToBeMapped[i]);
                }
            }
            firstFreeAutoNumber++;
            for (int i = 0; i < unmapped.Count; i++)
            {
                unmapped[i].AutoNumberKeyTip = firstFreeAutoNumber;
                firstFreeAutoNumber++;
            }
            return firstFreeAutoNumber;
        }

        internal protected virtual int IsIndexOfItemKeyTip(RadItem item, string keyTipFragment)
        {
            string keyTip = item.KeyTip;
            if (item.KeyTip == string.Empty && item.AutoNumberKeyTip > 0)
            {
                keyTip = item.AutoNumberKeyTip.ToString();
            }
            if (keyTipFragment.Length > keyTip.Length)
            {
                return -1;
            }
            return keyTip.LastIndexOf(keyTipFragment, StringComparison.InvariantCulture);
        }

        internal protected virtual bool IsExactMatch(RadItem item, string keyTipFragment)
        {
            string keyTip = item.KeyTip;
            if (item.KeyTip == string.Empty && item.AutoNumberKeyTip > 0)
            {
                keyTip = item.AutoNumberKeyTip.ToString();
            }
            return keyTip.Equals(keyTipFragment, StringComparison.InvariantCulture);
        }

        internal protected virtual bool IsPartOfKeyTip(Keys input, string representation)
        {
            representation = representation.Replace("NUMPAD", "");
            bool shouldKeepSimple = false;
            string keyPartialSignature = keyMapBuilder.ToString() + representation;
            List<RadItem> childrenToBeMapped = GetCurrentKeyMap(this.ActiveKeyMapItem);
            this.ProcessUnmappedItems(childrenToBeMapped);

            for (int i = 0; i < childrenToBeMapped.Count; i++)
            {
                int match = IsIndexOfItemKeyTip(childrenToBeMapped[i], keyPartialSignature);
                if (match == 0)
                {
                    shouldKeepSimple = true;
                    if (this.IsExactMatch(childrenToBeMapped[i], keyPartialSignature))
                    {
                        this.ActiveKeyMapItem = childrenToBeMapped[i];
                        keyMapBuilder.Length = 0;
                        keyMaps.Clear();
                        this.ownerControl.Invalidate();
                        return true;
                    }
                }
            }

            if (shouldKeepSimple)
            {
                if (representation != string.Empty)
                {
                    keyMapBuilder.Append(representation);
                }
                else
                {
                    keyMapBuilder.Append(input.ToString());
                }
                return true;
            }
            return false;
        }

        //internal protected virtual bool IsPartOfKeyTip(Keys input, string representation)
        //{
        //    bool shouldKeepSample = false;

        //    if (representation != string.Empty)
        //    {
        //        builder.Append(representation);
        //    }
        //    else
        //    {
        //        builder.Append(input.ToString());
        //    }
        //    //if (input == Keys.O)
        //    //{
        //    //    int i = 0;
        //    //}
        //    string keyPartialSignature = builder.ToString();
        //    List<RadItem> childrenToBeMapped = GetCurrentKeyMap(this.ActiveKeyMapItem);
        //    this.ProcessUnmappedItems(childrenToBeMapped);

        //    //if (keyPartialSignature.Length < testString.Length)
        //    //{
        //    //    shouldKeepSample = true;
        //    //}

        //    for (int i = 0; i < childrenToBeMapped.Count; i++)
        //    {
        //        int match = -1;
        //        if (childrenToBeMapped[i].KeyTip != string.Empty ||
        //            childrenToBeMapped[i].AutoNumberKeyTip > 0)
        //        {
        //            string testString = childrenToBeMapped[i].KeyTip;
        //            if (childrenToBeMapped[i].AutoNumberKeyTip > 0)
        //            {
        //                testString = childrenToBeMapped[i].AutoNumberKeyTip.ToString();
        //            }
        //            //if (keyPartialSignature == "SM1")
        //            //{
        //            //    int k = 0;
        //            //} 

        //            //match =
        //            //    testString.LastIndexOf(keyPartialSignature, StringComparison.InvariantCulture);
        //            Debug.WriteLine("testString:" + testString+", keyPartialSignature:" + keyPartialSignature);
        //            match =
        //                keyPartialSignature.LastIndexOf(testString, StringComparison.InvariantCulture);


        //            if (match > -1)
        //            {
        //                if ((match + testString.Length) == keyPartialSignature.Length)
        //                {
        //                    this.ActiveKeyMapItem = childrenToBeMapped[i];
        //                    builder.Length = 0;
        //                    keyMaps.Clear();
        //                    this.ownerControl.Invalidate();
        //                    return true;
        //                }
        //                else
        //                {
        //                    shouldKeepSample = true;
        //                }
        //            }
        //            //else if (keyPartialSignature.Length < testString.Length)
        //            //{
        //            //    shouldKeepSample = true;
        //            //}
        //            //else
        //            //{
        //            //    shouldKeepSample = false;
        //            //}
        //        } 
        //    }
        //    if (!shouldKeepSample)
        //    {
        //        builder.Remove((builder.Length - 1), 1);
        //        return false;
        //    }
        //    return false;
        //}

        internal protected virtual List<RadItem> GetCurrentKeyMap(RadItem currentKeyMapItem)
        {
            return this.GetKeyFocusChildren(currentKeyMapItem);
        }

        internal protected virtual List<RadItem> GetKeyFocusChildren(RadItem currentKeyMapItem)
        {
            //if (currentKeyMapItem == null)
            //{
            //    return GetRootItems();
            //}
            //List<RadItem> children = new List<RadItem>();
            //foreach (RadItem item in currentKeyMapItem.Children)
            //{
            //    children.Add(item);
            //}
            //return children;
            return null;
        }

        protected virtual string GetKeyStringRepresentation(Keys input)
        {
            string output = string.Empty;
            switch (input)
            {
                case Keys.D0:
                    output = "0";
                    break;
                case Keys.D1:
                    output = "1";
                    break;
                case Keys.D2:
                    output = "2";
                    break;
                case Keys.D3:
                    output = "3";
                    break;
                case Keys.D4:
                    output = "4";
                    break;
                case Keys.D5:
                    output = "5";
                    break;
                case Keys.D6:
                    output = "6";
                    break;
                case Keys.D7:
                    output = "7";
                    break;
                case Keys.D8:
                    output = "8";
                    break;
                case Keys.D9:
                    output = "9";
                    break;
                default:
                    output = input.ToString();
                    break;
            }
            return output.ToUpper();
        }

        #endregion

        /// <summary>
        /// Determines whether the mouse over the owning IComponentTreeHandler instance.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool MouseOver
        {
            get
            {
                return this.mouseOver;
            }
            internal set
            {
                this.mouseOver = value;
            }
        }

        internal protected IComponentTreeHandler Owner
        {
            get
            {
                return this.owner;
            }
        }

        [Browsable(false)]
        [Description("Gets the Control instance that hosts the TPF graph.")]
        internal protected Control OwnerControl
        {
            get
            {
                return this.ownerControl;
            }
        }

        // Only RadItem should manipulate this property
        internal RadItem ItemCapture
        {
            get
            {
                return this.itemCapture;
            }
            set
            {
                this.itemCapture = value;
                ItemCaptureState = true;
            }
        }

        /// <summary>
        /// Gets the current selected element (hovered by the mouse).
        /// </summary>
        [Browsable(false)]
        public RadElement SelectedElement
        {
            get
            {
                return this.selectedElement;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public FillRepository BitmapRepository
        {
            get
            {
                return this.bitmapRepository;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Telerik.WinControls.Themes.Design;
using System.ComponentModel;
using System.Windows.Forms;
using Telerik.WinControls.Elements;
using System.Drawing.Design;
using System.Diagnostics;
using Telerik.WinControls.Design;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// 	<para>
    /// 	    Represents a RadCommandBar control - a flexible component for implementation of tool and 
    /// 	    button bars featuring docking behavior, toggling buttons, shrinkable toolbars. The RadCommandBar is responsible for managing
    ///         <see cref="RadCommandBarBaseItem">RadCommandBarBaseItem items</see> which are positioned on some
    ///         of the <see cref="CommandBarStripElement">CommandBarStripElement elements</see>    ///        
    ///     </para>
    /// 	<para>
    ///         Only items that inherit the <see cref="RadCommandBarBaseItem">RadCommandBarBaseItem</see> class 
    ///         can be placed inside the strip elements. You han use the special <see cref="CommandBarHostItem">CommandBarHostItem</see>
    ///         to host any other <see cref="RadElement">RadElement</see>.
    ///     </para>
    /// </summary>
    [TelerikToolboxCategory(ToolboxGroupStrings.MenuAndToolbarsGroup)]
    [ToolboxItem(true)]
    [Description("A flexible component for implementation of tool and button bars featuring docking behavior, toggling buttons, shrinkable toolbars")]
    [DefaultProperty("Rows")]
    [Docking(DockingBehavior.Ask)]
    [Designer(DesignerConsts.RadCommandBarDesignerString)]
    public class RadCommandBar : RadControl
    {
        #region Event keys
        public static readonly object OrientationChangingEventKey = new object();
        public static readonly object OrientationChangedEventKey = new object();
        #endregion
         
        #region Fields

        protected RadCommandBarElement commandBarElement;
        protected Form parentForm; 
        protected RadDropDownMenu contextMenu;
        protected RadMenuItem customizeMenuItem;

        #endregion

        #region Overrides

        protected override RootRadElement CreateRootElement()
        {
            return new RadCommandBarRootElement();
        }

        public override bool ControlDefinesThemeForElement(RadElement element)
        {
            return (
                element is RadTextBoxElement ||
                element is RadDropDownListArrowButtonElement ||
                element is RadDropDownListEditableAreaElement ||
                element is RadDropDownListElement ||
                element is RadArrowButtonElement ||
                element is RadCommandBarArrowButton);
        }

        protected override Size DefaultSize
        {
            get
            {
                return new Size(25, 25);
            }
        }

        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);

            if (this.parentForm != null)
            {
                this.parentForm.SizeChanged -= new EventHandler(parentForm_SizeChanged);
            }

            this.parentForm = this.FindForm();

            if (this.parentForm != null)
            {
                this.parentForm.SizeChanged += new EventHandler(parentForm_SizeChanged);
            }
        }

        void parentForm_SizeChanged(object sender, EventArgs e)
        {
            if (System.Windows.Forms.FormWindowState.Minimized == this.parentForm.WindowState)
            {
                foreach (CommandBarStripElement strip in this.CommandBarElement.StripInfoHolder.StripInfoList)
                {
                    if (strip.FloatingForm != null)
                    {
                        strip.FloatingForm.Visible = false;
                    }
                }
            }
            else
            {
                foreach (CommandBarStripElement strip in this.CommandBarElement.StripInfoHolder.StripInfoList)
                {
                    if (strip.FloatingForm != null)
                    {
                        strip.FloatingForm.Visible = strip.VisibleInCommandBar;
                    }
                }
            }
        }

        protected override void CreateChildItems(RadElement parent)
        {
            base.CreateChildItems(parent);
            this.commandBarElement = new RadCommandBarElement();
            this.ApplyOrientation(commandBarElement.Orientation);
            this.RootElement.Children.Add(this.commandBarElement);
            this.AutoSize = true;
            this.contextMenu = new RadDropDownMenu();
            this.customizeMenuItem = new RadMenuItem(CommandBarLocalizationProvider.CurrentProvider.GetLocalizedString(CommandBarStringId.ContextMenuCustomizeText));
            customizeMenuItem.Click += new EventHandler(customizeMenuItem_Click);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                RadElement elementAtPoint = this.ElementTree.GetElementAtPoint(e.Location);
                if( (!(elementAtPoint is CommandBarRowElement)
                    && !(elementAtPoint is RadCommandBarElement))
                    && (elementAtPoint != null))
                {
                    return;
                }

                contextMenu.Items.Clear();
                contextMenu.RightToLeft = this.RightToLeft;
                contextMenu.ThemeName = this.ThemeName;
                foreach (CommandBarStripElement stripInfo in this.CommandBarElement.StripInfoHolder.StripInfoList)
                {
                    CommandBarDropDownMenu item = new CommandBarDropDownMenu(stripInfo);
                    item.Text = stripInfo.DisplayName;
                    item.IsChecked = stripInfo.VisibleInCommandBar;
                    contextMenu.Items.Add(item);
                }

                contextMenu.Items.Add(new RadMenuSeparatorItem());
                customizeMenuItem.Text = CommandBarLocalizationProvider.CurrentProvider.GetLocalizedString(CommandBarStringId.ContextMenuCustomizeText);
                contextMenu.Items.Add(customizeMenuItem);
                contextMenu.Show(this.PointToScreen(e.Location));
            }
            
        }

        protected override void OnThemeChanged()
        {
            base.OnThemeChanged();
            this.SetThemeCore();
        }

        /// <summary>
        /// Propagete ThemeName to child bar's menu
        /// </summary>
        protected virtual void SetThemeCore()
        {
            foreach (CommandBarRowElement line in this.Rows)
            {
                foreach (CommandBarStripElement strip in line.Strips)
                {
                    strip.OverflowButton.HostControlThemeName = this.ThemeName;
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        #endregion

        #region IItemsOwner Members

        /// <summary>
        /// Gets the rows of the commandbar.
        /// </summary>
        [RadNewItem("", false, true, true)]
        [RadEditItemsAction]
        [Browsable(true), Category(RadDesignCategory.DataCategory)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [RefreshProperties(RefreshProperties.All)]
        public RadCommandBarLinesElementCollection Rows
        {
            get { return this.commandBarElement.Rows; }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the <c ref="RadDropDownMenu"/> menu opened upon rightclick on the control.
        /// </summary>
        public RadDropDownMenu CustomizeContextMenu
        {
            get
            {
                return contextMenu;
            }
        }

        /// <summary>
        /// Gets or sets the size in pixels when current strip is being Drag and Drop in next or previous row.
        /// </summary>
        [Description("Gets or sets the size in pixels when current strip is being Drag and Drop in next or previous row")]
        [DefaultValue(typeof(Size), "4,4")]
        [Category(RadDesignCategory.BehaviorCategory)]
        public Size DragSize
        {
            get
            {
                return this.CommandBarElement.DragSize;
            }
            set
            {
                this.CommandBarElement.DragSize = value;
            }
        }

        /// <summary>
        /// Gets or sets the RadCommandBarElement of the RadCommandBar control.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public RadCommandBarElement CommandBarElement
        {
            get
            {
                return commandBarElement;
            }
            set
            {
                commandBarElement = value;
            }
        }

        /// <summary>
        /// Gets or sets the orientation of the commandbar - could be horizontal or vertical.
        /// This property is controlled by the Dock property of the RadCommandBar control.
        /// </summary>
        [Category(RadDesignCategory.BehaviorCategory)]
        [Description("Orientation of the CommandBar - could be horizontal or vertical")]
        [DefaultValue(Orientation.Horizontal)]
        [Browsable(false)]
        public Orientation Orientation
        {
            get
            {
                return this.commandBarElement.Orientation;
            }
            set
            {
                if (this.commandBarElement == null || this.commandBarElement.Orientation == value)
                {
                    return;
                }

                this.SetOrientationCore(value, true);
            }
        }


        /// <summary>
        /// Gets or sets which RadCommandBar borders are docked to its parent control and determines
        /// how a control is resized with its parent.
        /// </summary>
        /// <returns>
        /// One of the <see cref="T:System.Windows.Forms.DockStyle"/> values. The default
        /// is <see cref="F:System.Windows.Forms.DockStyle.None"/>.
        /// </returns>
        /// <exception cref="T:System.ComponentModel.InvalidEnumArgumentException">
        /// The value assigned is not one of the <see cref="T:System.Windows.Forms.DockStyle"/>
        /// values. 
        /// </exception>
        /// <filterpriority>1</filterpriority>        
        [System.ComponentModel.RefreshPropertiesAttribute(RefreshProperties.All)]
        [CategoryAttribute("Layout")]
        [System.ComponentModel.LocalizableAttribute(true)]
        [System.ComponentModel.DefaultValueAttribute(0)]
        public override DockStyle Dock
        {
            get
            {
                return base.Dock;
            }
            set
            {
                switch (value)
                {
                    case DockStyle.Left:
                    case DockStyle.Right:
                        this.Orientation = System.Windows.Forms.Orientation.Vertical;
                        break;
                    case DockStyle.None:
                    case DockStyle.Top:
                    case DockStyle.Fill:
                    case DockStyle.Bottom:
                        this.Orientation = System.Windows.Forms.Orientation.Horizontal;
                        break;
                }

                base.Dock = value;
            }
        }

        /// <summary>
        /// Apllies the orientation to the control and its child elements.
        /// </summary>
        /// <param name="value">The orientation to apply</param>
        /// <param name="fireEvents">Indicates whether events should be fired</param>
        protected virtual void SetOrientationCore(System.Windows.Forms.Orientation value, bool fireEvents)
        {
            CancelEventArgs args = new CancelEventArgs();
            bool cancel = (fireEvents) ? this.OnOrientationChanging(args) : false;
            if (cancel)
            {
                return;
            }

            this.RootElement.SuspendLayout(true);
            this.ApplyOrientation(value);
            this.commandBarElement.SetOrientationCore(value);
            this.RootElement.ResumeLayout(true, true);
            if (fireEvents)
            {
                this.OnOrientationChanged(new EventArgs());
            }
        }



        #endregion

        #region Events

        /// <summary> 
        /// Occurs before the orientation is changed.
        /// </summary>
        public event CancelEventHandler OrientationChanging
        {
            add
            {
                this.Events.AddHandler(OrientationChangingEventKey, value);
            }

            remove
            {
                this.Events.RemoveHandler(OrientationChangingEventKey, value);
            }
        }

        /// <summary>
        /// Occurs after the orientation is changed.
        /// </summary>
        public event EventHandler OrientationChanged
        {
            add
            {
                this.Events.AddHandler(OrientationChangedEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(OrientationChangedEventKey, value);
            }
        }

        /// <summary>
        /// Occurs before a floating form is created.
        /// </summary>
        public event CancelEventHandler FloatingStripCreating
        {
            add
            {
                this.CommandBarElement.FloatingStripCreating += value;
            }
            remove
            {
                this.CommandBarElement.FloatingStripCreating -= value;
            }
        }

        /// <summary>
        /// Occurs before a floating strip is docked.
        /// </summary>
        public event CancelEventHandler FloatingStripDocking
        {
            add
            {
                this.CommandBarElement.FloatingStripDocking += value;
            }
            remove
            {
                this.CommandBarElement.FloatingStripDocking -= value;
            }
        }

        /// <summary>
        /// Occurs when a floating strip is created.
        /// </summary>
        public event EventHandler FloatingStripCreated
        {
            add
            {
                this.CommandBarElement.FloatingStripCreated += value;
            }
            remove
            {
                this.CommandBarElement.FloatingStripCreated -= value;
            }
        }

        /// <summary>
        /// Occurs when a floating strip is docked.
        /// </summary>
        public event EventHandler FloatingStripDocked
        {
            add
            {
                this.CommandBarElement.FloatingStripDocked += value;
            }
            remove
            {
                this.CommandBarElement.FloatingStripDocked -= value;
            }
        }

        #endregion

        #region Event Managers

        /// <summary>
        /// Raises the <see cref="E:RadCommandBar.OrientationChanging"/> event.
        /// </summary>
        /// <param name="args">A <see cref="T:System.ComponentModel.CancelEventArgs"/> that contains the
        /// event data.</param>
        /// <returns>True if the change of orientation should be canceled, false otherwise.</returns>
        protected virtual bool OnOrientationChanging(CancelEventArgs args)
        {
            CancelEventHandler handler = (CancelEventHandler)this.Events[OrientationChangingEventKey];
            if (handler != null)
            {
                handler(this, args);
            }

            return args.Cancel;
        }

        /// <summary>
        /// Raises the <see cref="E:RadCommandBar.OrientationChanged"/> event.
        /// </summary>
        /// <param name="args">A <see cref="T:System.EventArgs"/> that contains the
        /// event data.</param>
        protected virtual void OnOrientationChanged(EventArgs args)
        {
            EventHandler handler = (EventHandler)this.Events[OrientationChangedEventKey];
            if (handler != null)
            {
                handler(this, args);
            }
        }

        #endregion

        #region Helpers
         
        private void customizeMenuItem_Click(object sender, EventArgs e)
        {
            CommandBarCustomizeDialogProvider.CurrentProvider.ShowCustomizeDialog(this, this.CommandBarElement.StripInfoHolder);
        }
        
        private void ApplyOrientation(Orientation orientation)
        {
            this.commandBarElement.SuspendLayout();
            if (orientation == Orientation.Horizontal)
            {
                this.RootElement.StretchVertically = false;
                this.RootElement.StretchHorizontally = true;
            }
            else
            {
                this.RootElement.StretchVertically = true;
                this.RootElement.StretchHorizontally = false;
            }
            this.commandBarElement.ResumeLayout(true);
        }

        #endregion

        #region RootElement

        /// <summary>
        /// Represents the <see cref="RootRadElement">RootElement</see> of the <see cref="RadCommandBar">RadCommandBar</see> control.
        /// </summary>
        public class RadCommandBarRootElement : RootRadElement
        {
            protected internal override bool? ShouldSerializeProperty(PropertyDescriptor property)
            {
                if (property.Name == "StretchHorizontally" ||
                    property.Name == "StretchVertically")
                {
                    return false;
                }

                bool? res = base.ShouldSerializeProperty(property);
                return res;
            }
        }

        #endregion

        protected override AccessibleObject CreateAccessibilityInstance()
        {
            return new RadCommandBarAccessibleObject(this);
        }
    }
}

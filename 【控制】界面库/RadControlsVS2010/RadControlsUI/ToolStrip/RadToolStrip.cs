using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.ComponentModel;
using Telerik.WinControls.Primitives;
using System.Drawing.Design;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Telerik.WinControls.Layouts;
using Telerik.WinControls.UI.TabStrip;
using Telerik.WinControls;
using Telerik.WinControls.Design;
using Telerik.WinControls.Themes.Design;
using Telerik.WinControls.Elements;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// 	<para>
    ///         Represents a RadToolStrip control. The ToolStrip is responsible for managing
    ///         <see cref="RadToolStripItem">ToolStrip items</see> which are positioned on some
    ///         of the <see cref="RadToolStripElement">ToolStrip elements</see> (bars in
    ///         ToolStrip where a ToolItem can be placed).
    ///         A ToolStrip manager makes sure that the items are
    ///         relocated appropriately after each drag and drop or resize operation.
    ///     </para>
    /// 	<para>
    ///         Any item/element can be placed inside a <see cref="RadToolStripItem">ToolStrip
    ///         item</see>.
    ///     </para>
    /// </summary>
    [RadThemeDesignerData(typeof(RadToolStripDesignTimeData))]
	[Designer(DesignerConsts.RadToolStripDesignerString)]
	[ToolboxItem(false)]
	[Description("A flexible component for implementation of tool and button strips featuring docking behavior, toggling buttons, shrinkable toolbars for limited window space and more")]
    [DefaultProperty("Items")] 
    [Docking(DockingBehavior.Ask)]
    [Obsolete("This control is obsolete. Use RadCommandBar instead.")]    
	public class RadToolStrip : RadControl, IItemsOwner
    {
        private RadToolStripManager toolStripManager;
        /// <summary>
        /// Gets or sets whether ToolStripItems can change their location.
        /// </summary>
        public RadToolStrip()
        {
            this.MinimumSize = new Size(5, 5);

            this.AutoSize = true;
			this.CausesValidation = false;
        }

		[DefaultValue(false), Browsable(false)]
		public new bool CausesValidation
		{
			get
			{
				return base.CausesValidation;
			}
			set
			{
				base.CausesValidation = value;
			}
		}

        protected override Size DefaultSize
        {
            get
            {
                return new Size(200, 25);
            }
        }

        protected override bool GetUseNewLayout()
        {
            return false;
        }

        public bool AllowDragging
        {
            get
            {
                return this.toolStripManager.AllowDragging;
            }
            set
            {
                this.toolStripManager.AllowDragging = value;
            }
        }

        public bool AllowFloating
        {
            get
            {
                return this.toolStripManager.AllowFloating;
            }
            set
            {
                this.toolStripManager.AllowFloating = value;
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
                this.toolStripManager.parentAutoSize = value;
                base.AutoSize = value;
            }
        }

        /// <summary>
        /// Gets or sets the tool strip orientation.
        /// </summary>
        public Orientation Orientation
        {
            get
            {
                return this.toolStripManager.Orientation;
            }
            set
            {
                CancelEventArgs cancelEventArgs = new CancelEventArgs();
                OnOrientationChanging(cancelEventArgs);
                if (cancelEventArgs.Cancel == true)
                    return;

                if (value != this.toolStripManager.Orientation)
                {
                    this.SuspendLayout();

                    this.toolStripManager.Orientation = value;
                    if (value == Orientation.Horizontal)
                    {
                        if (this.RootElement != null)
                        {
                            this.RootElement.StretchVertically = false;
                            this.RootElement.StretchHorizontally = true;
                        }
                    }
                    else
                    {
                        if (this.RootElement != null)
                        {
                            this.RootElement.StretchHorizontally = false;
                            this.RootElement.StretchVertically = true;
                        }
                    }

                    this.ResumeLayout(false);

                    if (value == Orientation.Horizontal)
                    {
                        if (this.Dock == DockStyle.Left)
                            this.Dock = DockStyle.Top;
                        if (this.Dock == DockStyle.Right)
                            this.Dock = DockStyle.Bottom;
                    }
                    else
                    {
                        if (this.Dock == DockStyle.Top)
                            this.Dock = DockStyle.Left;
                        if (this.Dock == DockStyle.Bottom)
                            this.Dock = DockStyle.Right;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the instance of RadToolStripManager wrapped by this control. RadToolStripManager
        /// is the main element in the hierarchy tree and encapsulates the actual functionality of RadToolStrip.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RadToolStripManager ToolStripManager
        {
            get
            {
                return this.toolStripManager;
            }

        }

        /// <summary>
        /// Gets or sets whether OverFlowButton should be visible.
        /// </summary>
        public bool ShowOverFlowButton
        {
            get
            {
                return this.toolStripManager.ShowOverFlowButton;
            }
            set
            {
                this.toolStripManager.ShowOverFlowButton = value;
            }
        }

        /// <commentsfrom cref="RadToolStripElement.Items" filter=""/>
        [RadEditItemsAction]
        [Browsable(true), Category(RadDesignCategory.DataCategory)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Editor(DesignerConsts.RadItemCollectionEditorString, typeof(UITypeEditor))]
        [RadNewItem("", false, false, false)]
        public RadItemOwnerCollection Items
        {
            get
            {
                return this.toolStripManager.Items;
            }
        }

        /// <summary>
        /// Fires the OrientationChanged event
        /// </summary>
        protected virtual void OnOrientationChanged(ToolStripOrientationEventArgs args)
        {
            if (this.OrientationChanged != null)
            {
                this.OrientationChanged(this, args);
            }
        }

        protected virtual void OnOrientationChanging(CancelEventArgs args)
        {
            if (this.OrientationChanging != null)
            {
                this.OrientationChanging(this, args);
            }
        }

        /// <summary>
        /// Fires the RowChanged event
        /// </summary>
        protected virtual void OnRowChanged(ToolStripChangedEventArgs args)
        {
            if (this.RowChanged != null)
            {
                this.RowChanged(this, args);
            }
        }

        /// <summary>
        /// Fires the DragStarting event
        /// </summary>
        protected virtual void OnDragStarting(ToolStripDragEventArgs args)
        {
            if (this.DragStarting != null)
            {
                this.DragStarting(this, args);
            }
        }

        internal protected virtual void OnFloatingFormCreating(CancelEventArgs args)
        {
            if (this.FloatingFormCreating != null)
            {
                this.FloatingFormCreating(this, args);
            }
        }

        internal protected virtual void OnFloatingFormCreated(EventArgs args)
        {
            if (this.FloatingFormCreated != null)
            {
                this.FloatingFormCreated(this, args);
            }
        }

        private Dictionary<RadToolStripItem, RadToolStripElement> itemsPositionsBeforeDrag = new Dictionary<RadToolStripItem, RadToolStripElement>();
        /// <summary>
        /// Fires the DragStarted event
        /// </summary>
        protected virtual void OnDragStarted(ToolStripDragEventArgs args)
        {
            if (this.DragStarted != null)
            {
                this.DragStarted(this, args);
            }
        }

        /// <summary>
        /// Fires the DragEnding event
        /// </summary>
        protected virtual void OnDragEnding(ToolStripDragEventArgs args)
        {
            if (this.DragEnding != null)
            {
                this.DragEnding(this, args);
            }
        }

        /// <summary>
        /// Fires the DragEnded event
        /// </summary>
        protected virtual void OnDragEnded(ToolStripDragEventArgs args)
        {          
            if (this.DragEnded != null)
            {
                this.DragEnded(this, args);
            }
        }

        /// <commentsfrom cref="RadToolStripManager.OrientationChanged" filter=""/>
        [Category(RadDesignCategory.PropertyChangedCategory)]
        [RadDescription("OrientationChanged", typeof(RadToolStripManager))]
        public event ToolStripOrientationEventHandler OrientationChanged;

        [Category(RadDesignCategory.PropertyChangedCategory)]
        [Description("Occurs when the orientation of the ToolStrip is about to change.")]
        public event CancelEventHandler OrientationChanging;

        /// <commentsfrom cref="RadToolStripManager.RowChanged" filter=""/>
        [Category(RadDesignCategory.PropertyChangedCategory)]
        [RadDescription("RowChanged", typeof(RadToolStripManager))]
        public event ToolStripChangeEventHandler RowChanged;

        /// <commentsfrom cref="RadToolStripManager.DragStarting" filter=""/>
        [Category(RadDesignCategory.DragDropCategory)]
        [RadDescription("DragStarting", typeof(RadToolStripManager))]
        public event ToolStripDragEventHandler DragStarting;

        /// <commentsfrom cref="RadToolStripManager.DragStarted" filter=""/>
        [Category(RadDesignCategory.DragDropCategory)]
        [RadDescription("DragStarted", typeof(RadToolStripManager))]
        public event ToolStripDragEventHandler DragStarted;

        /// <commentsfrom cref="RadToolStripManager.DragEnding" filter=""/>
        [Category(RadDesignCategory.DragDropCategory)]
        [RadDescription("DragEnding", typeof(RadToolStripManager))]
        public event ToolStripDragEventHandler DragEnding;

        /// <commentsfrom cref="RadToolStripManager.DragEnded" filter=""/>
        [Category(RadDesignCategory.DragDropCategory)]
        [RadDescription("DragEnded", typeof(RadToolStripManager))]
        public event ToolStripDragEventHandler DragEnded;

        [Category(RadDesignCategory.DragDropCategory)]
        [Description("FloatingFormCreating")]
        public event CancelEventHandler FloatingFormCreating;

        [Category(RadDesignCategory.DragDropCategory)]
        [Description("FloatingFormCreated")]
        public event EventHandler FloatingFormCreated;
        
        protected override void InitializeRootElement(RootRadElement rootElement)
        {
            base.InitializeRootElement(rootElement);
            rootElement.StretchVertically = false;
        }

        protected override void OnLoad(Size desiredSize)
        {
            base.OnLoad(desiredSize);

            //additional call to ensure initial desired size
            this.Size = this.GetPreferredSize(new Size(Int32.MaxValue, Int32.MaxValue));
        }

        protected override void CreateChildItems(RadElement parent)
        {
            this.toolStripManager = new RadToolStripManager();
            this.toolStripManager.AutoSizeMode = RadAutoSizeMode.WrapAroundChildren;
            this.toolStripManager.Items.ItemsChanged += new ItemChangedDelegate(Items_ItemsChanged);
          
            parent.Children.Add(this.toolStripManager);

            this.toolStripManager.RootElement = this.RootElement;

            this.toolStripManager.OrientationChanged +=
                delegate(object sender, ToolStripOrientationEventArgs args) { OnOrientationChanged(args); };
            this.toolStripManager.RowChanged +=
                delegate(object sender, ToolStripChangedEventArgs args) { OnRowChanged(args); };

            this.toolStripManager.DragEnded +=
                delegate(object sender, ToolStripDragEventArgs args) { OnDragEnded(args); };
            this.toolStripManager.DragEnding +=
                delegate(object sender, ToolStripDragEventArgs args) { OnDragEnding(args); };
            this.toolStripManager.DragStarted +=
                delegate(object sender, ToolStripDragEventArgs args) { OnDragStarted(args); };
            this.toolStripManager.DragStarting +=
                delegate(object sender, ToolStripDragEventArgs args) { OnDragStarting(args); };

        }

        private void Items_ItemsChanged(RadItemCollection changed, RadItem target, ItemsChangeOperation operation)
        {
            this.PerformLayout();
        }

        public override bool ControlDefinesThemeForElement(RadElement element)
        {
            Type elementType = element.GetType();

            if (elementType == typeof(RadButtonElement))
                return true;

            if (elementType == typeof(RadMenuElement))
                return true;

            if (elementType == typeof(RadToolStripManager))
                return true;

            if (elementType.Equals(typeof(RadTextBoxElement)))
            {
                if (element.FindAncestorByThemeEffectiveType(typeof(RadComboBoxElement)) != null)
                {
                    return true;
                }
            }
            else if (elementType.Equals(typeof(RadMaskedEditBoxElement)))
            {
                if (element.FindAncestor<RadDateTimePickerElement>() != null)
                {
                    return true;
                }
            }

            return false;
        }
    }
}

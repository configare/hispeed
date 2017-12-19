using System;
using System.ComponentModel;
using System.Drawing;
using System.Security.Permissions;
using System.Windows.Forms;
using Telerik.WinControls.UI.Docking;
using Telerik.WinControls.UI.Design;
using Telerik.WinControls.Keyboard;

namespace Telerik.WinControls.UI
{
    [ToolboxItem(false)]
    [Designer("Telerik.WinControls.UI.Design.SplitPanelDesigner, Telerik.WinControls.UI.Design, Version=" + VersionNumber.Number + ", Culture=neutral, PublicKeyToken=5bb2a467cbec794e")]
    public class SplitPanel : RadControl
    {
        protected SplitPanelElement splitPanelElement;
        private Size desiredSize; 
        private bool collapsed = false;
        private BorderStyle borderStyle;
        private SplitPanelSizeInfo sizeInfo;
        private static readonly object ControlTreeChangedEventKey;

        [Browsable(false)]
        public override ComponentThemableElementTree ElementTree
        {
            get
            {
                return base.ElementTree;

            }
        }

        /// <summary>
        /// Gets or sets whether Key Map (Office 2007 like accelerator keys map)
        /// is used for this speciffic control. Currently this option is implemented for 
        /// the RadRibbonBar control only.
        /// </summary>
        [Browsable(false), DefaultValue(false)]
        public new bool EnableKeyMap
        {
            get
            {
                return base.EnableKeyMap;
            }
            set
            {
                base.EnableKeyMap = value;
            }
        }

        /// <summary>Gets or sets StyleSheet for the control. Generally the stylesheet is assigned automatically when control's ThemeName is assigned.</summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [DefaultValue(null)]
        [Browsable(false)]
        public new StyleSheet Style
        {
            get
            {
                return base.Style;
            }
            set
            {
                base.Style = value;
            }
        }

        [Browsable(false)]
        [Category(RadDesignCategory.BehaviorCategory), TypeConverter(typeof(ExpandableObjectConverter)), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public new InputBindingsCollection CommandBindings
        {
            get
            {
                return base.CommandBindings;
            }
        }

        [Browsable(false)]
        public override string ThemeClassName
        {
            get
            {
                return base.ThemeClassName;
            }
            set
            {
                base.ThemeClassName = value;
            }
        }

        [Browsable(false)]
        public new Size ImageScalingSize
        {
            get
            {
                return base.ImageScalingSize;
            }
            set
            {
                base.ImageScalingSize = value;
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override ImageList SmallImageList
        {
            get
            {
                return base.SmallImageList;
            }
            set
            {
                base.SmallImageList = value;
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new ImageList ImageList
        {
            get
            {
                return base.ImageList;
            }
            set
            {
                base.ImageList = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the ThemeClassName property was set to value different from null (Nothing in VB.NET).
        /// </summary>
        [Browsable(false)]
        public new bool IsThemeClassNameSet
        {
            get
            {
                return base.IsThemeClassNameSet;

            }
        }

        [Browsable(false)]
        public new bool IsDesignMode
        {
            get
            {
                return base.DesignMode;
            }
        }

        public SplitPanel()  //SplitContainer owner
        {
            this.sizeInfo = new SplitPanelSizeInfo();
            this.sizeInfo.PropertyChanged += new PropertyChangedEventHandler(OnSizeInfo_PropertyChanged);
            
            //we are a layout-only control, no need to obtain focus
            this.TabStop = false;

            this.desiredSize = this.DefaultSize;
            base.MinimumSize = new Size(25, 25);
        }

        static SplitPanel()
        {
            ControlTreeChangedEventKey = new object();
        }

        public event ControlTreeChangedEventHandler ControlTreeChanged
        {
            add
            {
                this.Events.AddHandler(ControlTreeChangedEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(ControlTreeChangedEventKey, value);
            }
        }

        private void OnSizeInfo_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (this.Parent != null)
            {
                this.Parent.PerformLayout();
            }
        }

        private bool ShouldSerializeLocation()
        {
            return false;
        }

        [DefaultValue(typeof(BorderStyle), "None")]
        public BorderStyle BorderStyle
        {
            get
            {
                return this.borderStyle;
            }
            set
            {
                if (this.borderStyle != value)
                {
                    if (!ClientUtils.IsEnumValid(value, (int)value, 0, 2))
                    {
                        throw new InvalidEnumArgumentException("value", (int)value, typeof(BorderStyle));
                    }

                    this.borderStyle = value;
                    base.UpdateStyles();
                }
            }
        }

        /// <summary>
        /// Gets the object that encapsulates sizing information for this panel.
        /// </summary>
        [Description("Gets the object that encapsulates sizing information for this panel.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public SplitPanelSizeInfo SizeInfo
        {
            get
            {
                return this.sizeInfo;
            }
        }

        protected override CreateParams CreateParams
        {
            [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
            get
            {
                CreateParams createParams = base.CreateParams;
                createParams.ExStyle |= 0x10000;
                createParams.ExStyle &= -513;
                createParams.Style &= -8388609;
                switch (this.borderStyle)
                {
                    case BorderStyle.FixedSingle:
                        createParams.Style |= 0x800000;
                        return createParams;

                    case BorderStyle.Fixed3D:
                        createParams.ExStyle |= 0x200;
                        return createParams;
                }
                return createParams;
            }
        }

        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            base.SetBoundsCore(x, y, width, height, specified);
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override Size MinimumSize
        {
            get
            {
                if (this.sizeInfo != null)
                {
                    return this.sizeInfo.MinimumSize;
                }

                return new Size(25, 25);
            }
            set
            {
                base.MinimumSize = value;
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override Size MaximumSize
        {
            get
            {
                if (this.sizeInfo != null)
                {
                    return this.sizeInfo.MaximumSize;
                }

                return base.MaximumSize;
            }
            set
            {
                base.MaximumSize = value;
            }
        }

        protected override Size DefaultSize
        {
            get
            {
                return new Size(200, 200);
            }
        }

        [Browsable(false)]
        public RadSplitContainer SplitContainer
        {
            get
            {
                return this.Parent as RadSplitContainer;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new Control Parent
        {
            get
            {
                return base.Parent;
            }
            set
            {
                base.Parent = value;
            }
        }

        /// <summary>
        /// Gets the instance of RadPanelElement wrapped by this control. RadPanelElement
        /// is the main element in the hierarchy tree and encapsulates the actual functionality of RadPanel.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SplitPanelElement SplitPanelElement
        {
            get
            {
                return this.splitPanelElement;
            }
        }

        /// <summary>Creates the main panel element and adds it in the root element.</summary>
        protected override void CreateChildItems(RadElement parent)
        {
            if (this.splitPanelElement == null)
            {
                this.splitPanelElement = new SplitPanelElement();
            }

            this.splitPanelElement.AutoSizeMode = this.AutoSize ? RadAutoSizeMode.WrapAroundChildren : RadAutoSizeMode.FitToAvailableSize;

            this.RootElement.Children.Add(splitPanelElement);
            base.CreateChildItems(parent);
        }

        [DefaultValue(false)]
        public virtual bool Collapsed
        {
            get
            {
                return this.collapsed;
            }
            set
            {
                if (this.collapsed != value)
                {
                    this.collapsed = value;
                    this.Visible = !this.collapsed;

                    RadSplitContainer container = this.SplitContainer;
                    if (container != null)
                    {
                        container.OnChildPanelCollapsedChanged(this);
                    }
                }
            }
        }

        protected override Padding DefaultMargin
        {
            get
            {
                return new Padding(0, 0, 0, 0);
            }
        }

        protected override void OnControlAdded(ControlEventArgs e)
        {
            base.OnControlAdded(e);

            this.OnControlTreeChanged(new ControlTreeChangedEventArgs(this, e.Control, ControlTreeChangeAction.Add));
        }

        protected internal virtual void OnControlTreeChanged(ControlTreeChangedEventArgs args)
        {
            SplitPanel parent = this.Parent as SplitPanel;
            if (parent != null)
            {
                parent.OnControlTreeChanged(args);
            }

            ControlTreeChangedEventHandler eh = this.Events[ControlTreeChangedEventKey] as ControlTreeChangedEventHandler;
            if (eh != null)
            {
                eh(this, args);
            }
        }

        protected override void OnControlRemoved(ControlEventArgs e)
        {
            base.OnControlRemoved(e);

            this.OnControlTreeChanged(new ControlTreeChangedEventArgs(this, e.Control, ControlTreeChangeAction.Remove));
        }
    }
}

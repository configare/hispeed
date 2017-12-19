using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Telerik.WinControls.Themes.Design;
using System.Windows.Forms;
using Telerik.WinControls.Design;
using System.Security.Permissions;

namespace Telerik.WinControls.UI
{
    /// <summary>Implements the basic functionality for the scrolling.</summary>
    /// <remarks>
    /// 	<para>
    ///         This class can be used both for horizontal and for vertical scrolling through its
    ///         property <see cref="ScrollType"/>. Only the
    ///         specialized children are put in the Toolbox:
    ///         <see cref="RadHScrollBar"/> and <see cref="RadVScrollBar"/>.
    ///     </para>
    /// 	<para>
    ///         To adjust the value range of the scroll bar control set the
    ///         <see cref="Minimum"/> and
    ///         <see cref="Maximum"/> properties. To adjust the
    ///         distance the scroll thumb moves, set the <see cref="SmallChange"/>
    ///         and <see cref="LargeChange"/>
    ///         properties. To adjust the starting point of the scroll thumb, set the
    ///         <see cref="Value"/> property when the control is
    ///         initially displayed.
    ///     </para>
    /// </remarks>
    [DefaultProperty("Value")]
    [DefaultEvent("Scroll")]
	[ToolboxItem(false), ComVisible(true)]
    [RadThemeDesignerData(typeof(RadScrollBarDesignTimeData))]
	public class RadScrollBar : RadControl
	{
		private RadScrollBarElement scrollBar;
        private ScrollType scrollType = RadScrollBarElement.DefaultScrollType;

        #region Constructor, Initialization & Disposal

        /// <summary>
        /// Initialize new scroll bar
        /// </summary>
        public RadScrollBar()
        {
            this.AutoSize = true;
        }

        protected override void CreateChildItems(RadElement parent)
        {
            base.CreateChildItems(this.scrollBar);

            this.scrollBar = new RadScrollBarElement();
            this.scrollBar.ScrollType = this.ScrollType;
            this.scrollBar.Scroll += new ScrollEventHandler(scrollBar_Scroll);
            this.scrollBar.ValueChanged += new EventHandler(scrollBar_ValueChanged);
            this.scrollBar.ScrollParameterChanged += new EventHandler(scrollBar_ScrollParameterChanged);
            this.RootElement.Children.Add(this.scrollBar);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.scrollBar != null)
                {
                    this.scrollBar.Scroll -= new ScrollEventHandler(scrollBar_Scroll);
                    this.scrollBar.ValueChanged -= new EventHandler(scrollBar_ValueChanged);
                    this.scrollBar.ScrollParameterChanged -= new EventHandler(scrollBar_ScrollParameterChanged);
                }
            }
            base.Dispose(disposing);
        }

        #endregion

        #region Events

        /// <commentsfrom cref="RadScrollBarElement.ValueChanged" filter=""/>
        [Category(RadDesignCategory.ActionCategory)]
        [RadDescription("ValueChanged", typeof(RadScrollBarElement))]
        public event EventHandler ValueChanged;

        /// <commentsfrom cref="RadScrollBarElement.ScrollParameterChanged" filter=""/>
        [Category(RadDesignCategory.PropertyChangedCategory)]
        [RadDescription("ScrollParameterChanged", typeof(RadScrollBarElement))]
        public event EventHandler ScrollParameterChanged;

        #endregion

        #region Properties

        public override string ThemeClassName
        {
            get { return typeof(RadScrollBar).FullName; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the control is automatically resized
        /// to display its entire contents.
        /// </summary>
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
        /// Gets the instance of RadScrollBarElement wrapped by this control. RadScrollBarElement
        /// is the main element in the hierarchy tree and encapsulates the actual functionality of both
        /// RadHScrollBar and RadVScrollBar.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RadScrollBarElement ScrollBarElement
        {
            get
            {
                return this.scrollBar;
            }
        }

        /// <commentsfrom cref="RadScrollBarElement.ThumbLengthProportion" filter=""/>
        [Category(RadDesignCategory.BehaviorCategory)]
        [RadDefaultValue("ThumbLengthProportion", typeof(RadScrollBarElement))]
        [RadDescription("ThumbLengthProportion", typeof(RadScrollBarElement))]
        public double ThumbLengthProportion
        {
            get
            {
                return this.scrollBar.ThumbLengthProportion;
            }
            set
            {
                this.scrollBar.ThumbLengthProportion = value;
            }
        }

        /// <commentsfrom cref="RadScrollBarElement.MinThumbLength" filter=""/>
        [Category(RadDesignCategory.BehaviorCategory)]
        [RadDefaultValue("MinThumbLength", typeof(RadScrollBarElement))]
        [RadDescription("MinThumbLength", typeof(RadScrollBarElement))]
        public int MinThumbLength
        {
            get
            {
                return this.scrollBar.MinThumbLength;
            }
            set
            {
                this.scrollBar.MinThumbLength = value;
            }
        }

        /// <commentsfrom cref="RadScrollBarElement.ThumbLength" filter=""/>
        [Category(RadDesignCategory.BehaviorCategory)]
        [RadDescription("ThumbLength", typeof(RadScrollBarElement))]
        public int ThumbLength
        {
            get
            {
                return this.scrollBar.ThumbLength;
            }
        }

        /// <commentsfrom cref="RadScrollBarElement.Minimum" filter=""/>
        [Category(RadDesignCategory.BehaviorCategory)]
        [RadDefaultValue("Minimum", typeof(RadScrollBarElement))]
        [RadDescription("Minimum", typeof(RadScrollBarElement))]
        public int Minimum
        {
            get
            {
                return this.scrollBar.Minimum;
            }
            set
            {
                this.scrollBar.Minimum = value;
            }
        }

        /// <commentsfrom cref="RadScrollBarElement.Maximum" filter=""/>
        [Category(RadDesignCategory.BehaviorCategory)]
        [RadDefaultValue("Maximum", typeof(RadScrollBarElement))]
        [RadDescription("Maximum", typeof(RadScrollBarElement))]
        public int Maximum
        {
            get
            {
                return this.scrollBar.Maximum;
            }
            set
            {
                this.scrollBar.Maximum = value;
            }
        }

        /// <commentsfrom cref="RadScrollBarElement.Value" filter=""/>
        [Category(RadDesignCategory.BehaviorCategory)]
        [RadDefaultValue("Value", typeof(RadScrollBarElement))]
        [RadDescription("Value", typeof(RadScrollBarElement))]
        public int Value
        {
            get
            {
                return this.scrollBar.Value;
            }
            set
            {
                this.scrollBar.Value = value;
            }
        }

        /// <commentsfrom cref="RadScrollBarElement.SmallChange" filter=""/>
        [Category(RadDesignCategory.BehaviorCategory)]
        [RadDefaultValue("SmallChange", typeof(RadScrollBarElement))]
        [RadDescription("SmallChange", typeof(RadScrollBarElement))]
        public int SmallChange
        {
            get
            {
                return this.scrollBar.SmallChange;
            }
            set
            {
                this.scrollBar.SmallChange = value;
            }
        }

        /// <commentsfrom cref="RadScrollBarElement.LargeChange" filter=""/>
        [Category(RadDesignCategory.BehaviorCategory)]
        [RadDefaultValue("LargeChange", typeof(RadScrollBarElement))]
        [RadDescription("LargeChange", typeof(RadScrollBarElement))]
        public int LargeChange
        {
            get
            {
                return this.scrollBar.LargeChange;
            }
            set
            {
                this.scrollBar.LargeChange = value;
            }
        }
       
        /// <commentsfrom cref="RadScrollBarElement.ScrollType" filter=""/>
        [Category(RadDesignCategory.BehaviorCategory)]
        [RadDefaultValue("ScrollType", typeof(RadScrollBarElement))]
        [RadDescription("ScrollType", typeof(RadScrollBarElement))]
        public virtual ScrollType ScrollType
        {
            get
            {
                if (this.scrollBar != null)
                {
                    this.scrollType = this.scrollBar.ScrollType;
                }
                return this.scrollType;
            }
            set
            {
                this.scrollType = value;
                if (this.scrollBar != null)
                {
                    this.scrollBar.ScrollType = value;
                }
            }
        }
        
        #endregion

        #region Public Methods

        /// <summary>
        ///     Decrements the thumb position by the number of small steps given as a parameter.
        ///     The distance of a small step is determined by the
        ///     <see cref="SmallChange">SmallChange</see> property.
        /// </summary>
        public void PerformSmallDecrement(int numSteps)
        {
            if (scrollBar != null)
            {
                scrollBar.PerformSmallDecrement(numSteps);
            }
        }

        /// <summary>
        ///     Increments the thumb position by the number of small steps given as a parameter.
        ///     The distance of a small step is determined by the
        ///     <see cref="SmallChange">SmallChange</see> property.
        /// </summary>
        public void PerformSmallIncrement(int numSteps)
        {
            if (scrollBar != null)
            {
                scrollBar.PerformSmallIncrement(numSteps);
            }
        }

        /// <summary>
        ///     Decrements the thumb position by the number of large steps given as a parameter.
        ///     The distance of a large step is determined by the
        ///     <see cref="LargeChange">LargeChange</see> property.
        /// </summary>
        public void PerformLargeDecrement(int numSteps)
        {
            if (scrollBar != null)
            {
                scrollBar.PerformLargeDecrement(numSteps);
            }
        }

        /// <summary>
        ///     Increments the thumb position by the number of large steps given as a parameter.
        ///     The distance of a large step is determined by the
        ///     <see cref="LargeChange">LargeChange</see> property.
        /// </summary>
        public void PerformLargeIncrement(int numSteps)
        {
            if (scrollBar != null)
            {
                scrollBar.PerformLargeIncrement(numSteps);
            }
        }

        /// <summary>
        ///     Scrolls to the first position specified by the <see cref="Minimum">Minimum</see>
        ///     property.
        /// </summary>
        public void PerformFirst()
        {
            if (scrollBar != null)
            {
                scrollBar.PerformFirst();
            }
        }

        /// <summary>
        ///     Scrolls to the last position specified by the <see cref="Maximum">Maximum</see>
        ///     property.
        /// </summary>
        public void PerformLast()
        {
            if (scrollBar != null)
            {
                scrollBar.PerformLast();
            }
        }

        /// <summary>
        /// Scrolls to the specified position.
        /// </summary>
        public void PerformScrollTo(Point position)
        {
            if (scrollBar != null)
            {
                scrollBar.PerformScrollTo(position);
            }
        }

        public override bool ControlDefinesThemeForElement(RadElement element)
        {
            if (element is RadScrollBarElement)
            {
                return true;
            }

            return false;
        }

        #endregion

        #region Event handlers

        protected virtual void OnValueChanged(EventArgs e)
        {
            if (ValueChanged != null)
            {
                ValueChanged(this, e);
            }
        }

        protected virtual void OnScrollParameterChanged(EventArgs args)
        {
            if (ScrollParameterChanged != null)
            {
                ScrollParameterChanged(this, args);
            }
        }

        private void scrollBar_ValueChanged(object sender, EventArgs e)
        {
            this.OnValueChanged(e);
        }

        private void scrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            this.OnScroll(e);
        }

        private void scrollBar_ScrollParameterChanged(object sender, EventArgs e)
        {
            this.OnScrollParameterChanged(e);
        }

        #endregion

        protected override AccessibleObject CreateAccessibilityInstance()
        {
            return new RadScrollBarAccessibleObject(this);
        }

        [ComVisible(true)]
        public class RadScrollBarAccessibleObject : Control.ControlAccessibleObject
        {
            public RadScrollBarAccessibleObject(RadScrollBar owner) : base(owner)
            {
            	
            }

            public override AccessibleRole Role
            {
                [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
                get
                {
                    return AccessibleRole.ScrollBar;
                }
            }

            public override string Value
            {
                [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
                get
                {
                    return (this.Owner as RadScrollBar).Value.ToString();
                }
                set
                {
                    base.Value = value;
                }
            }

            public override string Name
            {
                get
                {
                    return this.Value;
                }
                set
                {
                    base.Name = value;
                }
            }

            public override string Description
            {
                get
                {
                    return (this.Owner as RadScrollBar).Value.ToString();
                }
            }            
        }
    }
}
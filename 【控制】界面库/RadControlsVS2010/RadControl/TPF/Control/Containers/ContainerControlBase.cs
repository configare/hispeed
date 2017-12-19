using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Security.Permissions;
using System.Drawing;
using System.Collections;
using System.Runtime.InteropServices;
using Telerik.WinControls.Collections;
using Telerik.WinControls.Design;
using Telerik.WinControls.Enumerations;
using System.Globalization;

namespace Telerik.WinControls.Containers
{
    /// <summary>
    /// Represents a base class for all container controls - 
    /// controls that contain other controls. 
    /// </summary>
	[ToolboxItem(false), ComVisible(false)]
    public class ContainerControlBase : ContainerControl
    {
		#region Constructors
		static ContainerControlBase()
		{
			ContainerControlBase.RequestResizeEventKey = new object();
			ContainerControlBase.BorderStyleChangedEventKey = new object();
			ContainerControlBase.BorderColorPropertyKey = new object();
			ContainerControlBase.BorderColorChangedEventKey = new object();
		}
        /// <summary>
        /// Initializes a new instance of the ContainerControlBase class.
        /// </summary>
		public ContainerControlBase()
		{
			//base.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
			//this.SuspendLayout();
			//// set forbidden child types
			this.SetStyle(
			ControlStyles.OptimizedDoubleBuffer |
			ControlStyles.DoubleBuffer |
			ControlStyles.AllPaintingInWmPaint |
			ControlStyles.UserPaint
			, true);
			UpdateStyles();

			this.forbiddenTypes = new List<System.Type>(3);
			this.allowedTypes = new List<System.Type>(3);
			//this.ResumeLayout();
		}
		#endregion

        //Fields
        private System.Windows.Forms.BorderStyle borderStyle = BorderStyle.None;
        protected int borderSize = 0;
        protected Hashtable properties;
        private EventsCollection events;
		private double sizeWeight = 0.5;

        protected readonly List<System.Type> forbiddenTypes;
        protected readonly List<System.Type> allowedTypes;
        protected TypeRestriction validationShema = TypeRestriction.ValidateForbiddenTypes;

        private static readonly object BorderStyleChangedEventKey;
        private static readonly object RequestResizeEventKey;
        private static readonly object BorderColorChangedEventKey;
        private static readonly object BorderColorPropertyKey;

        #region Events
		///<summary> Adds a delegate to the list. </summary>
		///<param name="eventKey">The object that owns the event.</param>
		///<param name="handler">The delegate to add to the list.</param>
		protected void AddEventHandler(object eventKey, Delegate handler)
		{
			if (this.events == null)
			{
				this.events = new EventsCollection();
			}
			this.events[eventKey] = Delegate.Combine((Delegate)this.events[eventKey], handler);
		}

		///<summary> Removes a delegate from the list. </summary>
		///<param name="eventKey">The object that owns the event.</param>
		///<param name="handler">The delegate to remove from the list.</param>
		protected void RemoveEventHandler(object eventKey, Delegate handler)
		{
			if (this.events != null)
			{
				this.events[eventKey] = Delegate.Remove((Delegate)this.events[eventKey], handler);
			}
		}

		///<summary> Raises the specified event. </summary>
		///<param name="eventKey">The object that owns the event.</param>
		///<param name="e">An <see cref="System.EventArgs"></see> that contains the event data.</param>
		protected virtual void RaiseEvent(object eventKey, EventArgs e)
		{
			if (this.events != null)
			{
				Delegate delegate1 = (Delegate)this.events[eventKey];
				if (delegate1 != null)
				{
					object[] objArray1 = new object[] { this, e };
					delegate1.DynamicInvoke(objArray1);
				}
			}
		}

		/// <summary>
		/// Occurs when the value of the BorderStyle property has changed. 
		/// </summary>
        public event EventHandler BorderStyleChanged
        {
            add
            {
                base.Events.AddHandler(ContainerControlBase.BorderStyleChangedEventKey, value);
            }
            remove
            {
                base.Events.RemoveHandler(ContainerControlBase.BorderStyleChangedEventKey, value);
            }
        }

		/// <summary>
		/// Raises the BorderStyleChanged event. 
		/// </summary>
		/// <param name="e">An EventArgs that contains the event data.</param>
        protected virtual void OnBorderStyleChanged(EventArgs e)
        {
            EventHandler handler1 = base.Events[ContainerControlBase.BorderStyleChangedEventKey] as EventHandler;
            if (handler1 != null)
            {
                handler1(this, e);
            }
        }
        #endregion

		#region Properties
		/// <summary>
		/// Encapsulates the information needed when creating a control.
		/// </summary>
		protected override System.Windows.Forms.CreateParams CreateParams
		{
			[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
			get
			{
				System.Windows.Forms.CreateParams params1 = base.CreateParams;
				params1.Style &= -8388609;
				if ((this.borderStyle != BorderStyle.None))
				{
					switch (this.borderStyle)
					{
						case System.Windows.Forms.BorderStyle.FixedSingle:
							{
								params1.Style |= 0x800000;
								break;
							}
						case System.Windows.Forms.BorderStyle.Fixed3D:
							{
								params1.ExStyle |= 0x200;
								break;
							}
					}
				}
				return params1;
			}
		}

		/// <summary>
		/// Specifies the border style for a control. 
		/// </summary>
		[DefaultValue(0), Category(RadDesignCategory.AppearanceCategory),
		DispId(-504), Description("Specifies the border style for a control.")]
		public virtual System.Windows.Forms.BorderStyle BorderStyle
		{
			get
			{
				return this.borderStyle;
			}
			set
			{
				if (this.borderStyle != value)
				{
					this.borderStyle = value;
					switch (this.borderStyle)
					{
						case System.Windows.Forms.BorderStyle.None:
							{
								this.borderSize = 0;
								break;
							}
						case System.Windows.Forms.BorderStyle.FixedSingle:
							{
								this.borderSize = 1;
								break;
							}
						case System.Windows.Forms.BorderStyle.Fixed3D:
							{
								this.borderSize = 4;
								break;
							}
					}
					base.UpdateStyles();
					base.RecreateHandle();
					this.OnBorderStyleChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Gets the space, in pixels, that is specified by default between controls.
		/// </summary>
		protected override Padding DefaultMargin
		{
			get
			{
				return new Padding(0, 0, 0, 0);
			}
		}

		/// <summary>
		/// Gets the internal spacing, in pixels, of the contents of a control. 
		/// </summary>
		protected override Padding DefaultPadding
		{
			get
			{
				return new Padding(0, 0, 0, 0);
			}
		}

		/// <summary>
		/// this is the statistical weight of the container which is taken into account
		/// when the contaner participates in a layout chain.
		/// </summary>
		public virtual double SizeWeight
		{
			get
			{
				return this.sizeWeight;
			}
			set
			{
				this.sizeWeight = value;
			}
		} 
		#endregion

		/// <summary>
		/// Overrides Control.CreateControlsInstance. 
		/// </summary>
		/// <returns>A new instance of ContainerControlBase.ContainerTypedControlCollection assigned to the control.</returns>
        
		[EditorBrowsable(EditorBrowsableState.Advanced)]
        protected override Control.ControlCollection CreateControlsInstance()
        {
            return new ContainerTypedControlCollection(this, false);
        }

		protected IntPtr SendMessage(int msg, bool wparam, int lparam)
		{
			return NativeMethods.SendMessage(new HandleRef(this, this.Handle), msg, wparam, lparam);
		}

		protected static bool IsControlNullOrEmpty(Control control)
		{
			return ((control == null)||
					(control.Controls.Count == 0));
		}

        #region Layout Management
        ///<summary> Raises the <see cref="System.Windows.Forms.Control.Layout"></see> event. </summary>
        ///<param name="e">A <see cref="System.Windows.Forms.LayoutEventArgs"></see> containing the event data.</param>
        protected override void OnLayout(LayoutEventArgs e)
        {
            base.OnLayout(e);
            this.ContainerLayout(e);
        }

        protected virtual Rectangle CalculateContainerClientArea()
        {
            return Rectangle.Empty;
        }
        
		protected virtual void LayoutContentCore()
        {
        }

        protected virtual void ContainerLayout(System.Windows.Forms.LayoutEventArgs e)
        {
            if ((e.AffectedControl != null) &&
				!string.IsNullOrEmpty(e.AffectedProperty))
            {
				this.LayoutContentCore();
            }
        }
        #endregion

		#region Dynamic Properties
		///<summary> Sets the value of the specified property. </summary>
		///<param name="key">The property whose value to set.</param>
		///<param name="value">An object representing the value to assign to the property.</param>
		protected void SetPropertyValue(object key, object value)
		{
			if (this.properties == null)
			{
				this.properties = new Hashtable();
			}
			this.properties[key] = value;
		}

		///<summary> Retrieves the value of the specified property. </summary>
		///<param name="key">The property whose value to retrieve.</param>
		protected object GetPropertyValue(object key)
		{
			if (this.properties == null)
			{
				return null;
			}
			return this.properties[key];
		}

		///<summary> Removes the specified property from the properties collection. </summary>
		///<param name="key">The property to remove.</param>
		protected void RemovePropertyValue(object key)
		{
			if (this.properties != null)
			{
				this.properties.Remove(key);
				if (this.properties.Count == 0)
				{
					this.properties = null;
				}
			}
		}

		///<summary> Retrieves a boolean value indicating if the specified property has been explicitly set. </summary>
		///<param name="key">The property to evaluate.</param>
		protected bool IsPropertyDefined(object key)
		{
			if (this.properties == null)
			{
				return false;
			}
			return this.properties.Contains(key);
		}
		#endregion

		#region Type Management Methods
		protected virtual void RegisterForbiddenType(System.Type type)
		{
			this.forbiddenTypes.Add(type);
		}
		protected virtual void RegisterAllowedType(System.Type type)
		{
			this.allowedTypes.Add(type);
		}
		protected virtual List<System.Type> GetForbiddenTypes()
		{
			return this.forbiddenTypes;
		}
		protected virtual List<System.Type> GetAllowedTypes()
		{
			return this.allowedTypes;
		}
		protected virtual List<System.Type> GetValidationTypes(TypeRestriction validationShema)
		{
			switch (validationShema)
			{
				case TypeRestriction.ValidateForbiddenTypes:
					return GetForbiddenTypes();
				case TypeRestriction.ValidateAllowedTypes:
					return GetAllowedTypes();
			}
			return null;
		}
		protected virtual List<System.Type> GetValidationTypes()
		{
			return GetValidationTypes(this.validationShema);
		}
		#endregion

        #region Nested Types

        public class ContainerTypedControlCollection : ReadOnlyControlCollection
        {
            public ContainerTypedControlCollection(Control control, bool isReadOnly)
                : base(control, isReadOnly)
            {
                this.owner = control as ContainerControlBase ;
            }

			// Fields
			private ContainerControlBase owner;
			private const string readOnlyMessage = "The Controls collection is Read Only. You are not allowed to add controls.";
			private const string nullNotAllowedMessage = "Null refferences not allowed";
			private const string allowedOnlyMessage = "The ContainerTypedControlCollection of the current instance allows types: ";
			private const string forbiddenOnlyMessage = "The ContainerTypedControlCollection of the current instance forbids types: ";

            public override void Add(Control value)
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nullNotAllowedMessage);
                }
                if (this.IsReadOnly)
                {
                    throw new NotSupportedException(readOnlyMessage);
                }
                this.ValidateType(value.GetType());
                base.Add(value);
            }

            public override void Remove(Control value)
            {
                this.ValidateType(value.GetType());

                if ((this.owner != null) && (!this.owner.DesignMode) && this.IsReadOnly)
                {
                    throw new NotSupportedException(readOnlyMessage);
                }
                base.Remove(value);
            }

            public override void SetChildIndex(Control child, int newIndex)
            {
                if ((this.owner != null) && (!this.owner.DesignMode))
                {
                    if (this.IsReadOnly)
                    {
                        throw new NotSupportedException(readOnlyMessage);
                    }
                }
                else
                {
                    return;
                }
                base.SetChildIndex(child, newIndex);
            }

            public void ValidateType(System.Type type)
            {
                if (this.owner == null)
                {
                    return;
                }

                List<System.Type> validatedTypes = owner.GetValidationTypes(owner.validationShema);
                for (int i = 0; i < validatedTypes.Count; i++)
                {

                    if (validatedTypes[i].IsAssignableFrom(type)) //if (!validatedTypes[i].IsAssignableFrom(type))
                    {
                        object[] objArray = new object[validatedTypes.Count];
                        string message = String.Empty;

                        for (int j = 0; j < validatedTypes.Count; j++)
                        {
                            objArray[j] = validatedTypes[j].Name;
                        }
                        switch (this.owner.validationShema)
                        {
                            case TypeRestriction.ValidateForbiddenTypes:
                                message = forbiddenOnlyMessage;
                                break;
                            case TypeRestriction.ValidateAllowedTypes:
                                message = allowedOnlyMessage;
                                break;
                        }
                        throw new ArgumentException( string.Format(CultureInfo.CurrentCulture, message, objArray));
                    }
                }
            }
        }

        #endregion
    }
}
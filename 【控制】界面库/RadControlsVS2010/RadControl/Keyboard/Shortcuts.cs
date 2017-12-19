using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Drawing;
using Telerik.WinControls.Keyboard;
using Telerik.WinControls.Commands;
using System.Reflection;
using Telerik.WinControls.Elements;
using Telerik.WinControls;
using System.Drawing.Design;

namespace Telerik.WinControls.Keyboard
{

	/// <summary>
	/// Represents keyboard shortcuts. 
	/// </summary>
	[ProvideProperty("CommandBinding", typeof(IComponent)),
	Description("Chords Provider"),
	DefaultEvent("Activate"),
	Designer(DesignerConsts.ShortcutsDesignerString),
    EditorAttribute(DesignerConsts.InputBindingEditorString, typeof(UITypeEditor)),
	ToolboxItemFilter("System.Windows.Forms"),
	ToolboxBitmap(typeof(ResFinder), "Resources.Shortcuts.bmp"),
    ToolboxItem(false)]
    [Browsable(false)]
	public partial class Shortcuts : Component, IExtenderProvider
	{
		#region Constructors
		/// <summary>Initializes a new instance of the Shortcuts class.</summary>
		public Shortcuts()
		{
			InitializeComponent();
		}

		public Shortcuts(Control owner)
		{
			this.owner = owner;
			InitializeComponent();
			if (!this.DesignMode)
			{
				this.AddShortcutsSupport();
			}
		}

		/// <summary>Initializes a new instance of the Shortcuts class.</summary>
		public Shortcuts(IContainer container)
		{
			container.Add(this);
			InitializeComponent();
			if (this.DesignMode)
			{
				return;
			}
			this.AddShortcutsSupport();
		}
		#endregion

		/// <summary>
		/// Fires when a shortcut is activated.
		/// </summary>
		public event ChordsEventHandler Activate;

		protected virtual void OnActivate(ChordEventArgs e)
		{
			ChordsEventHandler handler = this.Activate;
			if (handler != null)
			{
				handler(this, e);
			}
		}

		private InputBindingsCollection inputBindings = new InputBindingsCollection();
		private ChordMessageFilter chordMessageFilter;
		private Control owner;

		public Control Owner
		{
			get 
			{ 
				return this.owner; 
			}
			set 
			{ 
				this.owner = value; 
			}
		}

		internal Form OwnerForm
		{
			get
			{
				if (this.owner != null)
					return this.owner.FindForm();
				return null;
			}
		}


		/// <summary>
		/// Gets the input bindings.
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content) /*, Browsable(false)*/]
		public InputBindingsCollection InputBindings
		{
			get
			{
				return this.inputBindings;
			}
		}

		/// <summary>
		/// Adds the command bindings.
		/// </summary>
		/// <param name="binding"></param>
		public void AddCommandBindings(InputBinding binding)
		{
			this.inputBindings.Add(binding);
		}
		/// <summary>
		/// Adds command bindings.
		/// </summary>
		/// <param name="bindings"></param>
		public void AddCommandBindings(List<InputBinding> bindings)
		{
			//this.commandBindings.AddRange(bindings);
			this.inputBindings.AddRange(bindings.ToArray());
		}
		/// <summary>
		/// Adds commands bindings.
		/// </summary>
		/// <param name="bindings"></param>
		public void AddCommandBindings(InputBindingsCollection bindings)
		{
			//this.commandBindings.AddRange(bindings.ToArray());
			this.inputBindings.AddRange(bindings);
		}

		#region Initialization
		internal virtual void AddShortcutsSupport()
		{
			try
			{
				this.chordMessageFilter = ChordMessageFilter.CreateInstance();
				ChordMessageFilter.RegisterChordsConsumer(this);
			}
			finally
			{
			}
		}

		internal virtual void RemoveShortcutsSupport()
		{
			try
			{
				if (this.chordMessageFilter != null)
				{
					ChordMessageFilter.UnregisterChordsConsumer(this);
					this.chordMessageFilter = null;
				}
			}
			finally
			{
			}
		}
		#endregion

		#region IExtenderProvider Members

		public bool CanExtend(object extendee)
		{
			if (typeof(RadControl).IsAssignableFrom(extendee.GetType()) ||
				typeof(RadItem).IsAssignableFrom(extendee.GetType()))
			{
				return true;
			}
			return false;
		}

		[Category(RadDesignCategory.BehaviorCategory),
		DefaultValue(null),
		TypeConverter(typeof(ExpandableObjectConverter)),
		//Editor(typeof(InputBindingEditor), typeof(UITypeEditor)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public InputBinding GetCommandBinding(IComponent component)
		{
			InputBindingsCollection collection = this.inputBindings.GetBindingByComponent(component);
			if (collection == null ||
				(collection != null && collection.Count == 0))
			{
				//return new InputBinding();
			}
			if (collection != null && collection.Count > 0)
			{
				return collection[0];
			}
			return null;
		}

		public void SetCommandBinding(IComponent component, InputBinding value)
		{
			if (value == null || (value != null && value.IsEmpty))
			{
				this.inputBindings.RemoveBindingByComponent(component);
			}
			else
			{
				this.inputBindings.Add(value);
			}
		}

		private bool ShouldSerializeCommandBinding(InputBinding value)
		{
			if (!value.IsEmpty)
				return true;
			else
				return false;
		}

		public void ResetCommandBinding(InputBinding value)
		{
			value.Clear();
		}
		#endregion
	}
}
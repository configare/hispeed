using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Design;
using System.Windows.Forms;
using System.ComponentModel;
using System.Diagnostics;

namespace Telerik.WinControls.UI
{
    [RadToolboxItem(false)]
    public abstract class EditorBaseElement : RadEditorElement, IInputEditor, ISupportInitialize
    {
        #region BitState Keys

        internal const ulong InitializingStateKey = RadItemLastStateKey << 1;
        internal const ulong CallEditorHandlerStateKey = InitializingStateKey << 1;

        internal const ulong EditorBaseElementLastStateKey = CallEditorHandlerStateKey;

        #endregion

        #region Fields

        private object defaultValue = null;
        protected IEditorHandler EditorHandler = null;
        protected object originalValue;
        private object nullValue = null;
        private IEditorManager editorManager = null;
        private RadItem editorElement = null;

        #endregion

        #region Event Keys

        private static readonly object ValidatingEventKey;
        private static readonly object ValueChangedEventKey;
        private static readonly object ValueChangingEventKey;
        private static readonly object ValidationErrorEventKey;

        #endregion

        #region Initialization

        static EditorBaseElement()
        {
            EditorBaseElement.ValidatingEventKey = new object();
            EditorBaseElement.ValueChangedEventKey = new object();
            EditorBaseElement.ValueChangingEventKey = new object();
            EditorBaseElement.ValidationErrorEventKey = new object();
        }

        protected EditorBaseElement()
        {
        }

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.editorElement = this;
        }

        #endregion

        #region Properties

        public virtual RadItem EditorElement
        {
            get
            {
                return this.editorElement;
            }
            set
            {
                this.editorElement = value;
            }
        }

        #endregion

        #region Events

        protected override void OnTextChanging(TextChangingEventArgs e)
		{
			base.OnTextChanging(e);
			if (e.Cancel)
			{
				return;
			}
			ValueChangingEventArgs changingArgs = new ValueChangingEventArgs(e.NewValue);
			this.OnValueChanging(changingArgs);
			e.Cancel = changingArgs.Cancel;
		}

		protected override void OnTextChanged(EventArgs e)
		{
			base.OnTextChanged(e);
			this.OnValueChanged(new EventArgs());
		}

        protected virtual void OnValidating(ValueChangingEventArgs e)
        {
            if (this.Validating != null)
            {
                this.Validating(this, e);
            }
        }

        protected virtual void OnValidated(EventArgs e)
        {
            if (this.Validated != null)
            {
                this.Validated(this, e);
            }
        }

		//Note: This event is not consistent with the API of combobox so it never fires!
		[Category(RadDesignCategory.ActionCategory)]
		public event ValueChangingEventHandler ValueChanging
		{
			add
			{
				base.Events.AddHandler(ValueChangingEventKey, value);
			}
			remove
			{
				base.Events.RemoveHandler(ValueChangingEventKey, value);
			}
		}
		  
		protected virtual void OnValueChanging(ValueChangingEventArgs args)
        {
            if (!this.GetBitState(InitializingStateKey))
            {
                if (EditorHandler != null)
                {
                    this.EditorHandler.HandleEditorValueChanging(args);
                    if (args.Cancel)
                    {
                        return;
                    }
                }

                ValueChangingEventHandler handler1 =
                    (ValueChangingEventHandler)base.Events[ValueChangingEventKey];
                if (handler1 != null)
                {
                    handler1(this, args);
                }
            }
        }
		
        [Browsable(true),
        Category(RadDesignCategory.ActionCategory),
        Description("Occurs when the editor finished the value editing.")]
        public event EventHandler ValueChanged
        {
            add
            {
                base.Events.AddHandler(ValueChangedEventKey, value);
            }
            remove
            {
                base.Events.RemoveHandler(ValueChangedEventKey, value);
            }
        }       

        protected virtual void OnValueChanged(EventArgs args)
        {
            if (!this.GetBitState(InitializingStateKey))
            {
                if (EditorHandler != null)
                {
                    this.EditorHandler.HandleEditorValueChanged(args);
                }

                EventHandler handler1 =
                    (EventHandler)base.Events[ValueChangedEventKey];
                if (handler1 != null)
                {
                    handler1(this, args);
                }
            }
        }

        protected virtual void OnQueryValue(CancelEventArgs e)
        {
            if (QueryValue != null)
            {
                QueryValue(this, e);
            }
        }

        public event CancelEventHandler QueryValue;

		/// <summary>
		/// Occurs when internally the editor detects an error or when the Validating event fails.
		/// </summary>
		public event ValidationErrorEventHandler ValidationError
		{
			add
			{
				base.Events.AddHandler(ValidationErrorEventKey, value);
			}
			remove
			{
				base.Events.RemoveHandler(ValidationErrorEventKey, value);
			}
		}

		protected virtual void OnValidationError(ValidationErrorEventArgs args)
		{
			ValidationErrorEventHandler handler1 =
			   (ValidationErrorEventHandler)base.Events[ValidationErrorEventKey]; ;
			if (handler1 != null)
			{
				handler1(this, args);
			}
		}

		protected virtual void OnValidationError(string message)
		{
			Exception ex = new Exception(message);
			this.OnValidationError(new ValidationErrorEventArgs(null, ex));
		}

        public event CancelEventHandler Validating;

        public event EventHandler Validated;

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (this.EditorHandler != null && this.GetBitState(CallEditorHandlerStateKey))
            {
                this.EditorHandler.HandleEditorKeyDown(e);
                this.BitState[CallEditorHandlerStateKey] = false;
            }
            else
            {
                base.OnKeyDown(e);
            }
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (this.GetBitState(CallEditorHandlerStateKey) &&
                this.EditorHandler != null)
            {
                this.EditorHandler.HandleEditorKeyUp(e);
            }
            this.BitState[CallEditorHandlerStateKey] = false;
            base.OnKeyUp(e);
        }

        #endregion

        #region ISupportInitialize Members

        public virtual void BeginInit()
        {
            this.BitState[InitializingStateKey] = true;
        }

        public virtual void EndInit()
        {
            this.BitState[InitializingStateKey] = false;
        }

        #endregion

        #region IInputEditor Members

        public void ProcessKeyPress(KeyPressEventArgs e)
        {
            this.OnKeyPress(e);
        }

        public void ProcessKeyDown(KeyEventArgs e)
        {
            this.OnKeyDown(e);
        }

        public void ProcessKeyUp(KeyEventArgs e)
        {
            this.OnKeyUp(e);
        }

        public void ProcessMouseEnter(EventArgs e)
        {
            this.OnMouseEnter(e);
        }

        public void ProcessMouseLeave(EventArgs e)
        {
            this.OnMouseLeave(e);
        }

        public void ProcessMouseUp(MouseEventArgs e)
        {
            this.OnMouseUp(e);
        }

        public void ProcessMouseDown(MouseEventArgs e)
        {
            this.OnMouseDown(e);
        }

        public void ProcessMouseMove(MouseEventArgs e)
        {
            this.OnMouseMove(e);
        }

        public void ProcessMouseWheel(MouseEventArgs e)
        {
            this.OnMouseWheel(e);
        }

        public object NullValue
        {
            get 
            { 
                return this.nullValue; 
            }
            set
            {
                if (this.nullValue != value)
                {
                    this.nullValue = value;
                    this.OnNotifyPropertyChanged("NullValue");
                }
            }
        }

        public virtual string EditorType
        {
            get 
            {
                return this.GetType().ToString();
            }
        }

        public virtual bool IsModified
        {
            get 
            { 
                return !object.Equals(this.Value, this.originalValue); 
            }
        }

        protected virtual void OnFormat(ConvertEventArgs e)
        {
            if (this.Format != null)
            {
                this.Format(this, e);
            }
        }

        public event ConvertEventHandler Format;

        protected virtual void OnParse(ConvertEventArgs e)
        {
            if (this.Parse != null)
            {
                this.Parse(this, e);
            }
        }

        public event ConvertEventHandler Parse;

        /// <summary>
        /// Gets whether the editor is instantiated on demand or is always availabele. 
        /// Example: GridBooleanCellElement and GridViewBooleanColumn.
        /// </summary>
        public virtual bool IsNestedEditor
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the VisualElement that must receive the focus, when the editor is invoked.
        /// </summary>
        /// <returns></returns>
        public RadElement FocusableElement()
        {
            return this;
        }

        /// <summary>
        /// Initializes the provider.
        /// </summary>
        public virtual void Initialize()
        {
        }

        /// <summary>
        /// Initializes the provider.
        /// </summary>
        /// <param name="value">value to be pre-loaded inside the initialized editor.</param>
        public virtual void Initialize(object value)
        {
            this.Initialize();
            this.originalValue = value;
            this.Value = value;
        }

		public void Initialize(IEditorHandler owner, params object[] values)
		{
			this.EditorHandler = owner;
		}

        /// <summary>
        /// Initializes the provider.
        /// </summary>
        /// <param name="owner">the owner</param>
        /// <param name="value">value to be pre-loaded inside the initialized editor.</param>
		[Obsolete("This method is obsolete and will be removed.")]
		public void Initialize(object owner, object value)
        {
            this.EditorHandler = owner as IEditorHandler;
            this.Initialize(value);
        }

        public virtual object Value
        {
            get 
            {
                return null;
            }
            set 
            { 
            }
        }

        public object DefaultValue
        {
            get 
            { 
                return this.defaultValue; 
            }
            set
            {
                if (this.defaultValue != value)
                {
                    this.defaultValue = value;
                    this.OnNotifyPropertyChanged("DefaultValue");
                }
            }
        }

        public virtual object MinValue
        {
            get
            {
                return null;
            }
            set
            {
            }
        }

        public virtual object MaxValue
        {
            get
            {
                return null;
            }
            set
            {
            }
        }

        public virtual EditorVisualMode VisualMode
        {
            get
            {
                return EditorVisualMode.Default;
            }
        }

        public virtual EditorSupportedType SupportedType
        {
            get
            {
                return EditorSupportedType.AlphaNumeric;
            }
        }

        public virtual void BeginEdit()
        {
        }

        public virtual bool EndEdit()
        {
            return true;
        }

        public virtual bool Validate()
        {
            this.ValidateCore();
            return true;
        }

        protected virtual void ValidateCore()
        {
            ValueChangingEventArgs args = new ValueChangingEventArgs(Value);
            this.OnValidating(args);
            if (!args.Cancel)
            {
                this.OnValidated(new EventArgs());
            }
            else
            {
                this.OnValidationError("On Validating canceled");
                this.Value = this.originalValue;
            }
        }


        public IEditorManager EditorManager
        {
            get
            {
                return this.editorManager;
            }
            set
            {
                this.editorManager = value;
            }
        }

        #endregion

        #region IInputElement Members

        public bool CaptureMouse()
        {
            this.Capture = true;
            return this.Capture;
        }

        public void ReleaseMouseCapture()
        {
            this.Capture = false;
        }

        public virtual bool Focusable
        {
            get 
            { 
                return true; 
            }
            set {}
        }

        public bool IsEnabled
        {
            get 
            { 
                return this.Enabled; 
            }
        }

        public bool IsMouseCaptured
        {
            get 
            { 
                return this.Capture; 
            }
        }

        #endregion

        internal protected virtual Form FindForm()
        {
            if (this.ElementTree != null)
            {
                return this.ElementTree.Control.FindForm();
            }
            return null;
        }
    }
}

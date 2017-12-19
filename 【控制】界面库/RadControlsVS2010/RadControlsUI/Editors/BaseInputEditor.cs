using System;
using System.ComponentModel;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Abstract class that represents basic logic for editor
    /// </summary>
    public abstract class BaseInputEditor : IInputEditor, ISupportInitialize
    {
        #region Fields

        protected bool isActive;
        protected bool isInitializing;
        protected bool isInBeginEditMode;
        protected object originalValue;
        protected RadElement editorElement;
        protected IEditorManager editorManager;
        protected RadElement ownerElement;

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether this is the active editor in grid.
        /// </summary>
        public bool IsActive
        {
            get
            {
                return this.isActive;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the editor is initializing.
        /// </summary>
        public bool IsInitalizing
        {
            get
            {
                return this.isInitializing;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the editor is in BeginMode mode.
        /// </summary>
        public bool IsInBeginEditMode
        {
            get
            {
                return this.isInBeginEditMode;
            }
            internal set
            {
                this.isInBeginEditMode = value;
            }
        }

        /// <summary>
        /// Gets the element that owns this editor.
        /// </summary>
        public RadElement OwnerElement
        {
            get
            {
                return this.ownerElement;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the editor is in RightToLeft mode.
        /// </summary>
        public bool RightToLeft
        {
            get
            {
                if (this.EditorElement.IsInValidState(true))
                {
                    return this.EditorElement.ElementTree.Control.RightToLeft == System.Windows.Forms.RightToLeft.Yes;
                }
                return false;
            }
        }

        #endregion

        #region IInputEditor Members

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

        /// <summary>
        /// Gets the type of the editor value
        /// </summary>
        public abstract Type DataType { get; }

        /// <summary>
        /// Gets or sets the editor value.
        /// </summary>
        public virtual object Value
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets a value indicating whether the editor value is modified.
        /// </summary>
        public virtual bool IsModified
        {
            get
            {
                if (this.originalValue == null)
                {
                    return this.originalValue != this.Value;
                }

                return !this.originalValue.Equals(this.Value);
            }
        }

        /// <summary>
        /// Gets the <see cref="RadElement"/> associated with this editor.
        /// </summary>
        public virtual RadElement EditorElement
        {
            get
            {
                if (editorElement == null)
                {
                    editorElement = CreateEditorElement();
                }

                return editorElement;
            }
        }

        /// <summary>
        /// Initializes the editor. Used internally in RadGridView.
        /// </summary>
        /// <param name="owner">The owner of this editor.</param>
        /// <param name="value">The value of the editor.</param>
        public virtual void Initialize(object owner, object value)
        {
            this.ownerElement = owner as RadElement;
            this.originalValue = null;

            this.Value = value;
        }

        /// <summary>
        /// Starts the editing process. Used internally in RadGridView.
        /// </summary>
        public virtual void BeginEdit()
        {
            this.isActive = true;
            this.originalValue = this.Value;
            if (this.OwnerElement != null)
            {
                this.OwnerElement.BitState[RadItem.IsFocusableStateKey] = false;
            }
        }

        /// <summary>
        /// Finishes the editing process. Used internally in RadGridView.
        /// </summary>
        /// <returns></returns>
        public virtual bool EndEdit()
        {
            if (this.OwnerElement != null)
            {
                this.OwnerElement.BitState[RadItem.IsFocusableStateKey] = true;
            }
            this.isActive = false;
            this.originalValue = null;
            return true;
        }

        /// <summary>
        /// Validates the value currently entered in the editor.
        /// </summary>
        /// <returns></returns>
        public virtual bool Validate()
        {
            ValueChangingEventArgs changingArgs = new ValueChangingEventArgs(Value);
            this.OnValidating(changingArgs);

            if (changingArgs.Cancel)
            {
                this.Value = this.originalValue;

                return false;
            }

            this.OnValidated();

            return true;
        }

        /// <summary>
        /// Fires when changing the value of the editor.
        /// </summary>
        public event ValueChangingEventHandler ValueChanging;

        /// <summary>
        /// Fires when the editor value has been changed.
        /// </summary>
        public event EventHandler ValueChanged;

        /// <summary>
        /// Fires when the editor is validating.
        /// </summary>
        public event CancelEventHandler Validating;

        /// <summary>
        /// Fires when the editor has finished validating.
        /// </summary>
        public event EventHandler Validated;

        /// <summary>
        /// Fires when a validation error is occured.
        /// </summary>
        public event ValidationErrorEventHandler ValidationError;

        #endregion

        #region ISupportInitialize Members

        /// <summary>
        /// Begins the editor initialization process.
        /// </summary>
        public void BeginInit()
        {
            this.isInitializing = true;
        }

        /// <summary>
        /// Finishes the editor initialization process.
        /// </summary>
        public void EndInit()
        {
            this.isInitializing = false;
        }

        #endregion

        #region Events

        /// <summary>
        /// Fires the <see cref="ValueChanging"/> event.
        /// </summary>
        /// <param name="e">A <see cref="ValueChangingEventArgs"/> that contains the event data.</param>
        public virtual void OnValueChanging(ValueChangingEventArgs e)
        {
            if (ValueChanging != null)
            {
                ValueChanging(this, e);
            }
        }

        /// <summary>
        /// Fires the <see cref="ValueChanged"/> event.
        /// </summary>
        public virtual void OnValueChanged()
        {
            if (ValueChanged != null)
            {
                ValueChanged(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Fires the <see cref="Validating"/> event.
        /// </summary>
        /// <param name="e">A <see cref="CancelEventArgs"/> that contains the event data.</param>
        public virtual void OnValidating(CancelEventArgs e)
        {
            if (this.Validating != null)
            {
                this.Validating(this, e);
            }
        }

        /// <summary>
        /// Fires the <see cref="Validated"/> event.
        /// </summary>
        public virtual void OnValidated()
        {
            if (this.Validated != null)
            {
                this.Validated(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Fires the <see cref="ValidationError"/> event.
        /// </summary>
        /// <param name="args"></param>
        public virtual void OnValidationError(ValidationErrorEventArgs args)
        {
            if (this.ValidationError != null)
            {
                this.ValidationError(this, args);
            }
        }

        #endregion

        #region Abstract Methods

        /// <summary>
        /// Creates a new editor element. 
        /// </summary>
        /// <returns>a <see cref="RadElement"/> if successfull</returns>
        protected abstract RadElement CreateEditorElement();
        #endregion
    }
}

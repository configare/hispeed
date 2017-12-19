using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Provides information about the validation process.
    /// </summary>
    public class ValidationErrorEventArgs : EventArgs
    {
        // Fields
        private Exception exception;
        private object value;

        #region Constructors
        public ValidationErrorEventArgs(object value, Exception exception)
        {
            this.value = value;
            this.exception = exception;
        } 
        #endregion

        #region Properties
        /// <summary>
        /// Gets the exception that is caused by the validation of the edited value. Generally 
        /// the exception is populated by the validation logic and is available for rising by the editor.
        /// </summary>
        public Exception Exception
        {
            get
            {
                return this.exception;
            }
        }

        /// <summary>
        /// Gets the edited value that fails to be validated
        /// </summary>
        public object Value
        {
            get
            {
                return this.value;
            }
        } 
        #endregion
    }
}

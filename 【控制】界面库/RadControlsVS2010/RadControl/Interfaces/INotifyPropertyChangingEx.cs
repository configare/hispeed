using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.Interfaces
{
    /// <summary>
    /// Notifies clients that a property value is changing.
    /// </summary>
    public interface INotifyPropertyChangingEx
    {
        /// <summary>
        /// Occurs when a property value is changing.
        /// </summary>
        event PropertyChangingEventHandlerEx PropertyChanging;
    }
}

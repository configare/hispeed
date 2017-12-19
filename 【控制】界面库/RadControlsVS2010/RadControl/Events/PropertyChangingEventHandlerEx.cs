using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.Interfaces
{
    /// <summary>
    /// Represents the method that will handle the Telerik.WinControls.Interfaces.INotifyPropertyChanging.PropertyChanging
    /// event of an Telerik.WinControls.Interfaces.INotifyPropertyChanging interface.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">A System.ComponentModel.PropertyChangingEventArgs that contains the event data.</param>
    public delegate void PropertyChangingEventHandlerEx(object sender, PropertyChangingEventArgsEx e);
}

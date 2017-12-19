using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing.Design;
using Telerik.WinControls.Keyboard;

namespace Telerik.WinControls.Commands
{
    // make this Interface Generic
    //[Editor(typeof(CommandEditor), typeof(UITypeEditor)), TypeConverter(typeof(CommandConverter))]
    public interface ICommand
    {
        string Name { get; }
        string Type { get; }
        Type OwnerType { get;set; }
        Type ContextType { get;set; }

        object Execute();
        object Execute(params object[] settings);
        
        bool CanExecute(object target);

        string ToString();

        event EventHandler CanExecuteChanged;
        event CommandEventHandler HandleExecute;
        event CommandEventHandler Executed;
    }
}

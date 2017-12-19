using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.Commands
{
    /// <exclude/>
    interface ICommandSource
    {        
        bool AddCommand(ICommand command);
        void AddCommands(List<ICommand> list);
        void AddCommands(CommandList list);
        bool RemoveCommand(ICommand command);
        bool Contains(ICommand command);
        ICommand this[object id] { get; }
        //ICommand Command { get; }
        List<ICommand> Commands { get; }

        //bool RemoveCommand(string id);
        //bool Contains(string id);
        //ICommand GetCommand(object id);
    }
}

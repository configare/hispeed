using System;
using System.Collections.Generic;
using System.Text;

namespace CodeCell.Bricks.AppFramework
{
    public interface ICommandHelper
    {
        ICommand FindCommand(string name);
        ICommand FindCommand(Guid id);
        ICommand[] FindCommand(Type type);
        void SetCurrentTool(ITool tool);
        bool EablededRefreshUI { get; set; }
        ICommand CurrentCommand { get; }
        IToolbar FindToolbar(Type type);
        object GetControlByCommandType(Type commandType);
    }
}

using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Collections;
using System.Runtime.InteropServices;

namespace Telerik.WinControls.Commands
{
	[ToolboxItem(false), ComVisible(false)]
    public partial class CommandListComponent : Component, ICommandSource, IEnumerable
    {
        private Hashtable commands = new Hashtable();

        public CommandListComponent()
        {
            InitializeComponent();
        }

        public CommandListComponent(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        #region ICommandSource Members

        public bool AddCommand(ICommand command)
        {
            if (commands.Contains(command.Name))
                return false;

            commands.Add(command.Name, command);

            return true;
        }

        public void AddCommands(List<ICommand> list)
        {
            foreach (ICommand cmd in list)
                AddCommand(cmd);
        }

        public void AddCommands(CommandList list)
        {
            foreach (ICommand cmd in list)
                AddCommand(cmd);
        }

        public bool RemoveCommand(string id)
        {
            if (commands.Contains(id))
            {
                commands.Remove(id);
                return true;
            }

            return false;
        }

        public bool Contains(string id)
        {
            return commands.Contains(id);
        }

        public ICommand GetCommand(string id)
        {
            if (commands.Contains(id))
                return (ICommand)commands[id];
            
            return null;
        }

        public ICommand this[string id]
        {
            get 
            { 
                return GetCommand(id); 
            }
            set
            {
                if (commands.Contains(id))
                    commands[id] = value;
            }
        }

        #endregion

        #region IEnumerable Members

        public IEnumerator GetEnumerator()
        {
            return commands.Values.GetEnumerator();
        }

        #endregion

        #region ICommandSource Members


        public bool RemoveCommand(ICommand command)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public bool Contains(ICommand command)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public ICommand this[object id]
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public List<ICommand> Commands
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        #endregion
    }
}

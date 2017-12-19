using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Telerik.WinControls.Commands
{
    
    public class CommandEventArgs : CancelEventArgs
    {
        // Fields
        private object target;
        private object[] settings;
        private ICommand command;

        public CommandEventArgs(object command, params object[] settings)
        {
            this.command = command as ICommand;
            this.settings = settings;
        }

        public CommandEventArgs(object command, object target, params object[] settings) : this(command, settings)
        {
            this.target = target;
        }

        public object Target
        {
            get { return target; }
            set { target = value; }
        }

        public object[] Settings
        {
            get { return settings; }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Security;
using Telerik.WinControls.Commands;
using Telerik.WinControls.Containers;
using System.ComponentModel;
using Telerik.WinControls.Elements;
using System.Drawing.Design;

namespace Telerik.WinControls.Keyboard
{
    [Editor(DesignerConsts.InputBindingEditorString, typeof(UITypeEditor))]
    public class InputBinding
    {
        // Methods
        static InputBinding() 
        {
            InputBinding.instanceLock = new object();
        }

        public InputBinding()
        {
        }
        public InputBinding(ICommand command, Chord chord, object commandContext)
        {
            this.command = command;
            this.chord = chord;
            this.commandContext = commandContext;
        }

        // Properties
        [DefaultValue(null),
		TypeConverter(typeof(CommandInstanceConverter))]
        public ICommand Command 
        {
            get
            {
                return this.command;
            }
            set
            {
                this.command = value;
            }  
        }

		[DefaultValue(null),
        TypeConverter(typeof(CommandContextConverter))]//CommandSourceConverter
        public object CommandContext
        {
            get
            {
                return this.commandContext;
            }
            set
            {
                this.commandContext = value;
            }
        }

        public virtual Chord Chord 
        {
            get 
            {
                return this.chord;
            }
            set
            {
                this.chord = value;
            }  
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Browsable(false)]
        public bool IsEmpty
        {
            get
            {
                return (Chord == null || 
                    CommandContext == null ||
                    Command == null);
            }
        }

        public void Clear() 
        { 
            this.Chord = null;
            this.CommandContext = null;
            this.Command = null;
        }

        // Fields
        private ICommand command;
        private object commandContext;
        internal static object instanceLock;
        private Chord chord;

    }
}

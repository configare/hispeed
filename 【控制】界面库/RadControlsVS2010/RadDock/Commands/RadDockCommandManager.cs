using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Windows.Forms;

namespace Telerik.WinControls.UI.Docking
{
    /// <summary>
    /// Manages all the commands registered with a RadDock instance.
    /// </summary>
    public class RadDockCommandManager : RadDockObject, IMessageListener
    {
        #region Fields

        private Dictionary<string, RadDockCommand> commands;
        private List<Keys> currKeys;
        private RadDock dockManager;
        private bool enabled;
        private bool loaded;

        #endregion

        #region Construct/Dispose

        /// <summary>
        /// Constructs a new instance of the <see cref="RadDockCommandManager">CommandManager</see> class.
        /// </summary>
        /// <param name="dockManager">The RadDock instance this manager is associated with.</param>
        public RadDockCommandManager(RadDock dockManager)
        {
            this.enabled = true;
            this.dockManager = dockManager;
            this.commands = new Dictionary<string, RadDockCommand>();
            this.currKeys = new List<Keys>();
            this.InitPredefinedCommands();
        }

        private void InitPredefinedCommands()
        {
            this.commands.Add(PredefinedCommandNames.DisplayQuickNavigator, new QuickNavigatorDisplayCommand());
            this.commands.Add(PredefinedCommandNames.CloseActiveDocument, new ActiveDocumentCloseCommand());
            this.commands.Add(PredefinedCommandNames.NextDocument, new NextDocumentCommand());
            this.commands.Add(PredefinedCommandNames.PreviousDocument, new PreviousDocumentCommand());
        }

        /// <summary>
        /// The manager gets notified that the associated
        /// </summary>
        protected internal virtual void OnDockManagerLoaded()
        {
            if (this.loaded)
            {
                return;
            }

            this.EnsureHook();
            this.loaded = true;
        }

        private void EnsureHook()
        {
            if (this.enabled)
            {
                RadMessageFilter.Instance.AddListener(this);
            }
            else
            {
                RadMessageFilter.Instance.RemoveListener(this);
            }
        }

        /// <summary>
        /// Unregisters this instance from the global RadMessageFilter instance.
        /// </summary>
        protected override void DisposeManagedResources()
        {
            base.DisposeManagedResources();

            RadMessageFilter.Instance.RemoveListener(this);
        }

        #endregion

        #region IMessageListener

        InstalledHook IMessageListener.DesiredHook
        {
            get 
            {
                return InstalledHook.GetMessage;
            }
        }

        MessagePreviewResult IMessageListener.PreviewMessage(ref Message msg)
        {
            if (msg.Msg == NativeMethods.WM_KEYUP || msg.Msg == NativeMethods.WM_SYSKEYUP)
            {
                Keys key = this.GetKeyFromMessage(msg);
                if (this.IsModifierKey(key))
                {
                    //we have received a WM_KEYUP for a modifier key, clear currently collected key mappings
                    this.currKeys.Clear();
                }
            }
            else if (msg.Msg == NativeMethods.WM_KEYDOWN || msg.Msg == NativeMethods.WM_SYSKEYDOWN)
            {
                if (this.CanProcessMessage(msg))
                {
                    return this.ProcessKeyDown(msg);
                }
            }

            return MessagePreviewResult.NotProcessed;
        }

        /// <summary>
        /// Determines whether the manager should process keyboard messages
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        protected virtual bool CanProcessMessage(Message msg)
        {
            if (!this.dockManager.Enabled || !this.dockManager.IsHandleCreated)
            {
                return false;
            }

            Form dockParentForm = this.dockManager.FindForm();
            Form activeForm = Form.ActiveForm;

            return activeForm == dockParentForm || activeForm is FloatingWindow || activeForm is AutoHidePopup;
        }

        /// <summary>
        /// Processes a WM_KEYDOWN message that comes into the UI thread.
        /// Default implementation will attempt to execute a command that matches completely or partially the keys combination.
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        protected virtual MessagePreviewResult ProcessKeyDown(Message msg)
        {
            Keys currKey = this.GetKeyFromMessage(msg);
            //do not process modifier keys
            if (this.IsModifierKey(currKey))
            {
                return MessagePreviewResult.NotProcessed;
            }

            if (this.currKeys.Contains(currKey))
            {
                return MessagePreviewResult.ProcessedNoDispatch;
            }

            this.currKeys.Add(currKey);

            Keys modifiers = Control.ModifierKeys;
            Keys[] mappings = this.currKeys.ToArray();

            //check for shortcut combination first
            foreach (RadDockCommand command in this.commands.Values)
            {
                if (command.IsShortcut(modifiers, mappings))
                {
                    this.currKeys.Clear();

                    if (command.CanExecute(this.dockManager))
                    {
                        command.Execute(this.dockManager);
                        return MessagePreviewResult.ProcessedNoDispatch;
                    }

                    return MessagePreviewResult.NotProcessed;
                }
            }

            //check for partial shortcut combination
            foreach (RadDockCommand command in this.commands.Values)
            {
                if (command.IsPartialShortcut(modifiers, mappings))
                {
                    return MessagePreviewResult.ProcessedNoDispatch;
                }
            }

            //the key is not processed, clear all currently collected keys
            this.currKeys.Clear();

            //the message is not processed
            return MessagePreviewResult.NotProcessed;
        }

        /// <summary>
        /// Determines whether the specified Key is a modifier.
        /// </summary>
        /// <param name="currKey"></param>
        /// <returns></returns>
        protected virtual bool IsModifierKey(Keys currKey)
        {
            return currKey == Keys.ControlKey ||
                   currKey == Keys.ShiftKey ||
                   currKey == Keys.Menu;
        }

        void IMessageListener.PreviewWndProc(Message msg)
        {
        }

        void IMessageListener.PreviewSystemMessage(SystemMessage message, Message msg)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Registers the specified command, using the command's Name as a key.
        /// </summary>
        /// <param name="command"></param>
        public void RegisterCommand(RadDockCommand command)
        {
            if (command == null)
            {
                throw new ArgumentNullException("Command");
            }
            if (this.commands.ContainsKey(command.Name))
            {
                throw new ArgumentException("Command with the specified name is already registered.");
            }

            this.commands.Add(command.Name, command);
        }

        /// <summary>
        /// Removes an already registered command with the specified name.
        /// </summary>
        /// <param name="name"></param>
        public void UnregisterCommand(string name)
        {
            if (this.commands.ContainsKey(name))
            {
                this.commands.Remove(name);
            }
        }

        /// <summary>
        /// Finds the RadDockCommand instance which name equals the specified one. May be null if no such command is registered.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public RadDockCommand FindCommandByName(string name)
        {
            RadDockCommand command;
            this.commands.TryGetValue(name, out command);
            return command;
        }

        /// <summary>
        /// Attempts to execute the RadDockCommand which matches the specified name.
        /// </summary>
        /// <param name="name"></param>
        public void ExecuteCommand(string name)
        {
            if (!this.enabled)
            {
                return;
            }

            RadDockCommand command = this.FindCommandByName(name);
            if (command != null && command.CanExecute(this.dockManager))
            {
                command.Execute(this.dockManager);
            }
        }

        /// <summary>
        /// Notifies for a change in the Enabled state.
        /// </summary>
        protected virtual void OnEnabledChanged()
        {
            if (this.loaded)
            {
                this.EnsureHook();
            }
        }

        private Keys GetKeyFromMessage(Message m)
        {
            return (Keys)((int)m.WParam.ToInt64()) & Keys.KeyCode;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets all the registered commands with this manager.
        /// </summary>
        public IEnumerable<RadDockCommand> Commands
        {
            get
            {
                return this.commands.Values;
            }
        }

        /// <summary>
        /// Determines whether the command manager is currently enabled.
        /// </summary>
        public bool Enabled
        {
            get
            {
                return this.enabled;
            }
            set
            {
                if (this.enabled == value)
                {
                    return;
                }

                this.enabled = value;
                this.OnEnabledChanged();
            }
        }

        #endregion
    }
}

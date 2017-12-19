using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Runtime.InteropServices;

namespace Telerik.WinControls.Commands
{
	[ToolboxItem(false), ComVisible(false)]
    public partial class CommandComponentBase : Component, ICommand, ICommandPresentation 
    {
        /// <summary>
        /// Represents the method that will handle HandleExecute, and Execucted events.
        /// </summary>
        public event CommandEventHandler HandleExecute;
        public event CommandEventHandler Executed;
        private Type context;
        private string name = String.Empty;
        private string type = String.Empty;
        private string text = String.Empty;
        private System.Windows.Forms.ImageList imageList = null;

        public CommandComponentBase()
        {
            InitializeComponent();
        }

        public CommandComponentBase(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        #region ICommand Members

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string Type 
        {
            get { return ""; }
            set { type = value; }
        }

        public virtual object Execute()
        {
            return Execute(null, null);
        }

        public virtual object Execute(params object[] settings)
        {
            return Execute(null, settings);
        }

        public virtual object Execute(object target, params object[] settings)
        {
            if (HandleExecute != null)
            {
                HandleExecute(this, new CommandEventArgs(target, settings));
                if (Executed != null)
                {
                    Executed(this, new CommandEventArgs(target, settings));
                }
                return true;
            }
            return false;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        #endregion

        #region ICommandPresentation Members

        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        #endregion

        #region IImageListProvider Members

        public System.Windows.Forms.ImageList ImageList
        {
            get { return imageList; }
            set { imageList = value; } 
        }

        public System.Drawing.Image GetImageAt(int index)
        {
            if (imageList != null && imageList.Images.Count > index)
                return imageList.Images[index];
            return null;
        }

        public System.Drawing.Image GetImageAt(string index)
        {
            if (imageList != null && imageList.Images.ContainsKey(index))
                return imageList.Images[index];
            return null;
        }

        #endregion

        #region ICommand Members

        Type ICommand.OwnerType
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        public Type ContextType
        {
            get
            {
                return context;
            }
            set
            {
                this.context = value;
            }
        }

        public void Execute(object parameter, object target)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public event EventHandler CanExecuteChanged;

		protected virtual void OnCanExecuteChanged()
		{
			EventHandler handler = this.CanExecuteChanged;
			if (handler != null)
			{
				handler(this, EventArgs.Empty);
			}
		}

        #endregion
    }
}

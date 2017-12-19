using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls;
using System.Windows.Forms;

namespace Telerik.WinControls.Commands
{
	public class CommandBase : ICommand, ICommandPresentation
	{
		/// <summary>
		/// 
		/// </summary>
		public event CommandEventHandler HandleExecute;
		/// <summary>
		/// 
		/// </summary>
		public event CommandEventHandler Executed;

		private string name = String.Empty;
		private string text = String.Empty;
		private string type = String.Empty;
		private ImageList imageList = null;
		private Type owner;
		private Type context;
		/// <summary>
		/// Initializes a new instance of the CommandBase class.
		/// </summary>
		public CommandBase()
		{
		}
		/// <summary>
		/// Initializes a new instance of the CommandBase class using command name.
		/// </summary>
		/// <param name="name"></param>
		public CommandBase(string name)
			: this(name, String.Empty, String.Empty)
		{
		}
		/// <summary>
		/// Initializes a new instance of the CommandBase class.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="text"></param>
		public CommandBase(string name, string text)
			: this(name, String.Empty, text)
		{
		}
		public CommandBase(string name, string text, string type)
		{
			this.name = name;
			this.text = text;
			this.type = type;
		}
		/// <summary>
		/// Retrieves a text representation of the instance.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			if (!string.IsNullOrEmpty(this.Name))
			{
				return this.Name;
			}
			else
			{
				return base.ToString();
			}
		}
		#region ICommand Members

		/// <summary>
		/// Gets or sets the command name/
		/// </summary>
		public string Name
		{
			get { return name; }
			set { name = value; }
		}
		/// <summary>
		/// Gets or sets the command type.
		/// </summary>
		public string Type
		{
			get { return ""; }
			set { type = value; }
		}

		/// <summary>
		/// Executes the command.
		/// </summary>
        public virtual object Execute()
		{
			return Execute(null, null);
		}

		/// <summary>
		/// Executes the command with the given settings.
		/// </summary>
		/// <param name="settings">
		///  
		///</param>
        public virtual object Execute(params object[] settings)
		{
			if (settings.Length > 0)
			{
                RaiseHandleExecute(settings);
                RaiseExecuted(settings);
			}
            return null;
		}

        protected virtual bool RaiseHandleExecute(params object[] settings)
        {
            if (HandleExecute != null)
            {   
                CommandEventArgs e = new CommandEventArgs(this, settings);
                HandleExecute(this, e);
                return e.Cancel;
            }
            return false;
        }

        protected virtual void RaiseExecuted(params object[] settings)
        {
            if (Executed != null)
            {
                Executed(this, new CommandEventArgs(this, settings));
            }
        }

		public virtual bool CanExecute(object parameter)
		{
			if (typeof(RadControl).IsAssignableFrom(parameter.GetType()) ||
				typeof(RadItem).IsAssignableFrom(parameter.GetType()))
			{
				return true;
			}
			return false;
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


		public Type OwnerType
		{
			get
			{
				return owner;
			}
			set
			{
				this.owner = value;
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

		//public void Execute(object parameter, object target)
		//{
		//    throw new Exception("The method or operation is not implemented.");
		//}

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

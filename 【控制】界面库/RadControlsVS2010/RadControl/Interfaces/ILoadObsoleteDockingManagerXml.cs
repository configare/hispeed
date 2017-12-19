using System.IO;
using System;
using System.Windows.Forms;

namespace Telerik.WinControls.Interfaces
{
	public interface ILoadObsoleteDockingManagerXml
	{
		void LoadDockingManagerXml(TextReader input);
		event EventHandler<DockableDeserializedEventArgs> DockableDeserialized;
	}

	public class DockableDeserializedEventArgs : EventArgs
	{
		private Control dockable = null;
		private Guid id = Guid.Empty;

		public DockableDeserializedEventArgs(Control dockable, Guid id)
		{
			this.dockable = dockable;
			this.id = id;
		}

		public Control Dockable
		{
			get
			{
				return this.dockable;
			}
		}

		public Guid Id
		{
			get
			{
				return this.id;
			}
		}
	}
}

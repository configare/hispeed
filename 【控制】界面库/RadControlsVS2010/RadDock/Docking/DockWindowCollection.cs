using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Windows.Forms;

namespace Telerik.WinControls.UI.Docking
{
    /// <summary>
    /// A collection that stores all DockWindow instances available per RadDock basis.
    /// </summary>
    public class DockWindowCollection : ICollection, IEnumerable<DockWindow>
    {
        #region Fields

        private RadDock owner;

        #endregion

        #region Constructor

        /// <summary>
        /// 
        /// </summary>
        /// <param name="owner"></param>
        internal DockWindowCollection(RadDock owner)
        {
            this.owner = owner;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets all DockWindow instances that has the specified DockState.
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public DockWindow[] GetWindows(DockState state)
        {
			return this.GetWindows(delegate(DockWindow dockWindow)
			{
				return dockWindow.DockState == state;
			});
        }

        /// <summary>
        /// Gets all the DockWindow instances that match the specified predicate.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
		public DockWindow[] GetWindows(Predicate<DockWindow> predicate)
		{
			if (this.Count == 0)
			{
				return new DockWindow[0];
			}
			if (predicate == null)
			{
				return new List<DockWindow>(this).ToArray();
			}

			List<DockWindow> list = new List<DockWindow>();
			int count = this.Count;
			DockWindow dockWindow = null;
			for (int i = 0; i < count; i++)
			{
				dockWindow = this[i];
				if (predicate(dockWindow))
				{
					list.Add(dockWindow);
				}
			}

			return list.ToArray();
		}

        /// <summary>
        /// Gets all the ToolWindow instances available.
        /// </summary>
        public DockWindow[] ToolWindows
        {
            get
            {
                int count = this.Count;
                if (count == 0)
                {
                    return new DockWindow[0];
                }

                List<DockWindow> toolWindows = new List<DockWindow>(count / 2);
                foreach (DockWindow window in this.owner.InnerList.Values)
                {
                    if (window.DockType == DockType.ToolWindow)
                    {
                        toolWindows.Add(window);
                    }
                }

                return toolWindows.ToArray();
            }
        }

        /// <summary>
        /// Gets all the DocumentWindow instances available.
        /// This will not include ToolWindow instances that are currently TabbedDocuments.
        /// </summary>
        public DockWindow[] DocumentWindows
        {
            get
            {
                int count = this.Count;
                if (count == 0)
                {
                    return new DockWindow[0];
                }

                List<DockWindow> docWindows = new List<DockWindow>(count / 2);
                foreach (DockWindow window in this.owner.InnerList.Values)
                {
                    if (window.DockType == DockType.Document)
                    {
                        docWindows.Add(window);
                    }
                }

                return docWindows.ToArray();
            }
        }

        /// <summary>
        /// Gets the DockWindow instance at the specified index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public DockWindow this[int index]
        {
            get
            {
                return this.owner.InnerList.Values[index];
            }
        }

        /// <summary>
        /// Gets the DockWindow instances that matches the specified name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public DockWindow this[string name]
        {
            get
            {
                return this.owner.InnerList[name];
            }
        }

        #endregion

        #region ICollection Members

        /// <summary>
        /// Copies the collection to the destination Array, starting from the specified index.
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index"></param>
        public void CopyTo(Array array, int index)
        {
            if (array is DockWindow[])
            {
                this.owner.InnerList.Values.CopyTo((DockWindow[])array, index);
            }
        }

        /// <summary>
        /// Gest the number of DockWindow instances registered with the owning RadDock.
        /// </summary>
        public int Count
        {
            get 
            { 
                return this.owner.InnerList.Count; 
            }
        }

        bool ICollection.IsSynchronized
        {
            get
            {
                return false;
            }
        }

        object ICollection.SyncRoot
        {
            get
            {
                return this.owner;
            }
        }

        #endregion

        #region IEnumerable Members

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator()
        {
            return this.owner.InnerList.Values.GetEnumerator();
        }

        #endregion

		#region IEnumerable<DockWindow> Members

		IEnumerator<DockWindow> IEnumerable<DockWindow>.GetEnumerator()
		{
			return this.owner.InnerList.Values.GetEnumerator();
		}

		#endregion
	}
}

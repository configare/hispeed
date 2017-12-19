using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Telerik.WinControls.UI.Docking
{
    /// <summary>
    /// A logical structure that treats a number of <see cref="DockWindow">DockWindow</see> instances as group that is auto-hidden/restored at once.
    /// </summary>
    public class AutoHideGroup
    {
        #region Constructor

        /// <summary>
        /// Constructor generally used by serializer
        /// </summary>
        public AutoHideGroup()
        {
            this.windows = new DockWindowSerializableCollection(new List<DockWindow>());
        }

        /// <summary>
        /// Initializes a new group, associated with the specified windows.
        /// </summary>
        /// <param name="windows"></param>
        public AutoHideGroup(IEnumerable<DockWindow> windows)
        {
            this.windows = new DockWindowSerializableCollection( new List<DockWindow>(windows));
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public DockWindowSerializableCollection Windows
        {
            get
            {
                return this.windows;
            }
        }

        #endregion

        #region Fields

        private DockWindowSerializableCollection windows;

        #endregion
    }
}
